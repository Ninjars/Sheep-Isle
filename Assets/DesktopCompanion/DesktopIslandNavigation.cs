using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

[DefaultExecutionOrder(-100)]
public sealed class DesktopIslandNavigation : MonoBehaviour
{
    [SerializeField] private NavMeshSurface islandSurface;

    public void SetSurface(NavMeshSurface surface) { islandSurface = surface; }

    private IEnumerator Start()
    {
        // Agents are stored disabled in the prefab so they do not attempt to
        // bind before the surface and static obstacle carving are ready.
        yield return new WaitForSeconds(0.75f);
        if (islandSurface == null || islandSurface.navMeshData == null ||
            !islandSurface.isActiveAndEnabled)
        {
            Debug.LogError("The desktop island has no active baked NavMeshSurface.");
            yield break;
        }
        foreach (var sheep in FindObjectsByType<CompanionSheep>(FindObjectsSortMode.None))
        {
            var agent = sheep.GetComponent<NavMeshAgent>();
            if (!NavMesh.SamplePosition(sheep.transform.position, out var hit, 5f, NavMesh.AllAreas))
            {
                Debug.LogError(sheep.name + " has no walkable position near its starting point.");
                continue;
            }
            sheep.transform.position = hit.position;
            agent.enabled = true;
            if (!agent.isOnNavMesh)
                Debug.LogError(sheep.name + " could not bind to the island NavMesh.");
        }
    }
}
