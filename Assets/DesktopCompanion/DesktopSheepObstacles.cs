using UnityEngine;
using UnityEngine.AI;

// The old island has visible trees and rocks but almost no colliders. Carve
// modest clearance around their trunks/bodies without using foliage bounds.
[DefaultExecutionOrder(-200)]
public sealed class DesktopSheepObstacles : MonoBehaviour
{
    [SerializeField] private Transform treeGroup;
    [SerializeField] private Transform rockGroup;

    public void Configure(Transform trees, Transform rocks)
    {
        treeGroup = trees;
        rockGroup = rocks;
    }

    private void Awake()
    {
        if (treeGroup == null || rockGroup == null)
        {
            Debug.LogError("Desktop sheep scenery groups are missing.");
            return;
        }

        foreach (Transform tree in treeGroup)
        {
            if (tree.name.Contains("Root")) continue;
            var renderer = tree.GetComponentInChildren<Renderer>();
            if (renderer == null || renderer.bounds.size.y < 3f) continue;
            var bounds = renderer.bounds;
            var width = Mathf.Min(bounds.size.x, bounds.size.z);
            AddCapsule(tree.name, tree.position, bounds.min.y,
                Mathf.Clamp(width * 0.22f, 0.65f, 1.35f), 3f);
        }

        foreach (Transform rock in rockGroup)
        {
            if (!rock.name.StartsWith("ZLPP_Rock_")) continue;
            var renderer = rock.GetComponentInChildren<Renderer>();
            if (renderer == null || renderer.bounds.size.y < 0.8f) continue;
            var bounds = renderer.bounds;
            var obstacleObject = new GameObject("Sheep clearance: " + rock.name);
            obstacleObject.transform.SetParent(transform, false);
            obstacleObject.transform.position = new Vector3(bounds.center.x,
                bounds.min.y + Mathf.Max(1.5f, bounds.size.y * 0.5f), bounds.center.z);
            var obstacle = obstacleObject.AddComponent<NavMeshObstacle>();
            obstacle.shape = NavMeshObstacleShape.Box;
            obstacle.size = new Vector3(bounds.size.x * 0.8f,
                Mathf.Max(3f, bounds.size.y), bounds.size.z * 0.8f);
            obstacle.carving = true;
        }
    }

    private void AddCapsule(string label, Vector3 pivot, float bottom, float radius, float height)
    {
        var obstacleObject = new GameObject("Sheep clearance: " + label);
        obstacleObject.transform.SetParent(transform, false);
        obstacleObject.transform.position = new Vector3(pivot.x, bottom + height * 0.5f, pivot.z);
        var obstacle = obstacleObject.AddComponent<NavMeshObstacle>();
        obstacle.shape = NavMeshObstacleShape.Capsule;
        obstacle.radius = radius;
        obstacle.height = height;
        obstacle.carving = true;
    }
}
