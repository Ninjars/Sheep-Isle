using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public static class DesktopCompanionBuild
{
    private const string ScenePath = "Assets/Scenes/Desktop Companion.unity";

    [MenuItem("Sheep Isle/Desktop Companion/Prepare Scene")]
    public static void PrepareScene()
    {
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
        }
        finally
        {
            if (openedHere) EditorSceneManager.CloseScene(scene, true);
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
