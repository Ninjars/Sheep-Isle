using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public static class DesktopCompanionBuild
{
    private const string ScenePath = "Assets/Scenes/Desktop Companion.unity";
    private const string SheepSourcePath = "Assets/Game/Agents/Sheep.prefab";
    private const string SheepPrefabPath = "Assets/DesktopCompanion/Companion Sheep.prefab";
    private const string FootPrefabPath = "Assets/Game/Agents/Foot.prefab";
    private const string NavigationPath = "Assets/Scenes/Desktop Companion/NavMesh.asset";

    [MenuItem("Sheep Isle/Desktop Companion/Prepare Scene")]
    public static void PrepareScene()
    {
        var sheepPrefab = PrepareSheepPrefab();
        var scene = SceneManager.GetSceneByPath(ScenePath);
        var openedHere = !scene.isLoaded;
        if (openedHere)
            scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Additive);

        try
        {
            var cameraObject = scene.GetRootGameObjects()
                .Single(root => root.name == "Desktop Camera");
            if (cameraObject.GetComponent<DesktopWindowController>() == null)
            {
                cameraObject.AddComponent<DesktopWindowController>();
                EditorSceneManager.SaveScene(scene);
            }
            if (cameraObject.GetComponent<DesktopOrbitCamera>() == null)
            {
                cameraObject.AddComponent<DesktopOrbitCamera>();
                EditorSceneManager.SaveScene(scene);
            }
            if (cameraObject.GetComponent<CompanionSheepClick>() == null)
                cameraObject.AddComponent<CompanionSheepClick>();
            if (cameraObject.GetComponent<CompanionSoundSettings>() == null)
                cameraObject.AddComponent<CompanionSoundSettings>();

            PrepareSheepScene(scene, sheepPrefab);
            EditorSceneManager.SaveScene(scene);
        }
        finally
        {
            if (openedHere) EditorSceneManager.CloseScene(scene, true);
        }
    }

    private static GameObject PrepareSheepPrefab()
    {
        var existing = AssetDatabase.LoadAssetAtPath<GameObject>(SheepPrefabPath);
        if (existing != null)
        {
            var contents = PrefabUtility.LoadPrefabContents(SheepPrefabPath);
            try
            {
                var visuals = contents.transform.Find("Visuals");
                var agent = contents.GetComponent<NavMeshAgent>();
                var changed = visuals.localScale != Vector3.one * 1.8f || agent.enabled;
                for (var i = visuals.childCount - 1; i >= 0; i--)
                {
                    var child = visuals.GetChild(i);
                    if (!child.name.StartsWith("Leg ")) continue;
                    UnityEngine.Object.DestroyImmediate(child.gameObject);
                    changed = true;
                }
                var floatingFeet = contents.GetComponent<CompanionFloatingFeet>();
                if (floatingFeet == null)
                {
                    floatingFeet = contents.AddComponent<CompanionFloatingFeet>();
                    floatingFeet.Configure(LoadFootPrefab());
                    changed = true;
                }
                if (changed)
                {
                    contents.GetComponent<CompanionSheep>().Configure(visuals,
                        visuals.Find("Head"),
                        AssetDatabase.LoadAssetAtPath<SoundBank>("Assets/Sounds/SheepSounds.asset"));
                    TuneSheepPrefab(contents);
                    PrefabUtility.SaveAsPrefabAsset(contents, SheepPrefabPath);
                }
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(contents);
            }
            return AssetDatabase.LoadAssetAtPath<GameObject>(SheepPrefabPath);
        }

        var source = AssetDatabase.LoadAssetAtPath<GameObject>(SheepSourcePath);
        if (source == null) throw new Exception("Original sheep prefab is missing.");
        var sheep = (GameObject)PrefabUtility.InstantiatePrefab(source);
        try
        {
            PrefabUtility.UnpackPrefabInstance(sheep, PrefabUnpackMode.Completely,
                InteractionMode.AutomatedAction);
            UnityEngine.Object.DestroyImmediate(sheep.GetComponent<Game.SheepAgent>());
            UnityEngine.Object.DestroyImmediate(sheep.GetComponent<Game.AutonomousLegomatic>());
            UnityEngine.Object.DestroyImmediate(sheep.GetComponent<Rigidbody>());
            sheep.name = "Companion Sheep";

            var body = sheep.transform.Find("Mesh");
            var head = sheep.transform.Find("Head");
            if (body == null || head == null)
                throw new Exception("Original sheep body or head is missing.");
            var visuals = new GameObject("Visuals").transform;
            visuals.SetParent(sheep.transform, false);
            body.SetParent(visuals, false);
            head.SetParent(visuals, false);

            TuneSheepPrefab(sheep);

            var audio = sheep.GetComponent<AudioSource>();
            audio.playOnAwake = false;
            // The desktop camera sits far from a small island; 2D playback
            // keeps baas audible without depending on that camera distance.
            audio.spatialBlend = 0f;
            audio.volume = 0.35f;
            var sounds = AssetDatabase.LoadAssetAtPath<SoundBank>("Assets/Sounds/SheepSounds.asset");
            if (sounds == null) throw new Exception("Sheep sound bank is missing.");
            sheep.AddComponent<CompanionFloatingFeet>().Configure(LoadFootPrefab());
            sheep.AddComponent<CompanionSheep>().Configure(visuals, head, sounds);

            var saved = PrefabUtility.SaveAsPrefabAsset(sheep, SheepPrefabPath);
            if (saved == null) throw new Exception("Could not save companion sheep prefab.");
            AssetDatabase.SaveAssets();
            return saved;
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(sheep);
        }
    }

    private static Game.Foot LoadFootPrefab()
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(FootPrefabPath);
        if (prefab == null || prefab.GetComponent<Game.Foot>() == null)
            throw new Exception("Original square foot prefab is missing.");
        return prefab.GetComponent<Game.Foot>();
    }

    private static void TuneSheepPrefab(GameObject sheep)
    {
        sheep.transform.Find("Visuals").localScale = Vector3.one * 1.8f;
        var collider = sheep.GetComponent<BoxCollider>();
        collider.center = new Vector3(0f, 0f, 0.2f);
        collider.size = new Vector3(2.1f, 2.2f, 3f);
        var agent = sheep.GetComponent<NavMeshAgent>();
        agent.enabled = false;
        agent.radius = 0.75f;
        agent.height = 2.7f;
        agent.baseOffset = 1.35f;
        agent.speed = 1.8f;
        agent.acceleration = 3.5f;
        agent.angularSpeed = 180f;
        agent.stoppingDistance = 0.1f;
    }

    private static void PrepareSheepScene(Scene scene, GameObject sheepPrefab)
    {
        var data = AssetDatabase.LoadAssetAtPath<NavMeshData>(NavigationPath);
        if (data == null) throw new Exception("Desktop island navigation data is missing.");

        var navigation = scene.GetRootGameObjects()
            .FirstOrDefault(root => root.name == "Desktop Island Navigation");
        if (navigation == null)
        {
            navigation = new GameObject("Desktop Island Navigation");
            SceneManager.MoveGameObjectToScene(navigation, scene);
        }
        var nav = navigation.GetComponent<DesktopIslandNavigation>();
        if (nav == null) nav = navigation.AddComponent<DesktopIslandNavigation>();
        nav.SetData(data);
        var transforms = scene.GetRootGameObjects()
            .SelectMany(root => root.GetComponentsInChildren<Transform>(true)).ToArray();
        var trees = transforms.Single(t => t.name == "Tree Group");
        var rocks = transforms.Single(t => t.name == "Rock Group");
        var obstacles = navigation.GetComponent<DesktopSheepObstacles>();
        if (obstacles == null) obstacles = navigation.AddComponent<DesktopSheepObstacles>();
        obstacles.Configure(trees, rocks);

        var flock = scene.GetRootGameObjects()
            .FirstOrDefault(root => root.name == "Desktop Flock");
        if (flock != null) return;

        flock = new GameObject("Desktop Flock");
        SceneManager.MoveGameObjectToScene(flock, scene);
        var temporaryNavigation = NavMesh.AddNavMeshData(data);
        try
        {
            var triangles = NavMesh.CalculateTriangulation();
            if (triangles.indices.Length < 9)
                throw new Exception("Desktop island navigation has too few triangles for the flock.");
            var candidates = new List<Vector3>();
            for (var i = 0; i < triangles.indices.Length; i += 3)
                candidates.Add((triangles.vertices[triangles.indices[i]] +
                    triangles.vertices[triangles.indices[i + 1]] +
                    triangles.vertices[triangles.indices[i + 2]]) / 3f);
            var center = Vector3.zero;
            foreach (var candidate in candidates) center += candidate;
            center /= candidates.Count;
            candidates.Sort((a, b) =>
                (a - center).sqrMagnitude.CompareTo((b - center).sqrMagnitude));

            var chosen = new List<Vector3>();
            foreach (var candidate in candidates)
            {
                if (chosen.Any(other => (other - candidate).sqrMagnitude < 64f)) continue;
                if (!NavMesh.SamplePosition(candidate, out var hit, 1f, NavMesh.AllAreas)) continue;
                chosen.Add(hit.position);
                if (chosen.Count == 3) break;
            }
            if (chosen.Count < 3)
                throw new Exception("Could not find three separated sheep positions on the island.");
            for (var i = 0; i < chosen.Count; i++)
            {
                var sheep = (GameObject)PrefabUtility.InstantiatePrefab(sheepPrefab, scene);
                sheep.name = "Companion Sheep " + (i + 1);
                sheep.transform.SetParent(flock.transform, true);
                sheep.transform.position = chosen[i];
                sheep.transform.rotation = Quaternion.Euler(0f, i * 113f, 0f);
            }
        }
        finally
        {
            if (temporaryNavigation.valid) temporaryNavigation.Remove();
        }
    }

    [MenuItem("Sheep Isle/Desktop Companion/Build Windows Player")]
    public static void BuildWindowsPlayer()
    {
        PrepareScene();
        PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
        PlayerSettings.defaultScreenWidth = 480;
        PlayerSettings.defaultScreenHeight = 480;
        PlayerSettings.resizableWindow = false;
        PlayerSettings.runInBackground = true;
        PlayerSettings.useFlipModelSwapchain = false;
        PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows64,
            new[] { GraphicsDeviceType.Direct3D11 });

        var output = Environment.GetEnvironmentVariable("SHEEP_ISLE_BUILD_DIR");
        if (string.IsNullOrEmpty(output))
            output = Path.Combine(Directory.GetParent(Application.dataPath).FullName,
                "Builds", "Desktop Companion");
        Directory.CreateDirectory(output);

        var result = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = new[] { ScenePath },
            locationPathName = Path.Combine(output, "Sheep Isle.exe"),
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        });
        if (result.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
            throw new Exception("Desktop Companion build failed: " + result.summary.result);
        Debug.Log("Desktop Companion Windows player: " + output);
    }
}
