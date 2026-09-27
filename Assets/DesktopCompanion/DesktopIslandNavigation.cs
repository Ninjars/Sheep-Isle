using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[DefaultExecutionOrder(-100)]
public sealed class DesktopIslandNavigation : MonoBehaviour
{
    [SerializeField] private NavMeshData islandData;
    private NavMeshDataInstance instance;

    public void SetData(NavMeshData data) { islandData = data; }

    private void OnEnable()
    {
        if (islandData == null)
        {
            Debug.LogError("The desktop island has no baked navigation data.");
            return;
        }
        instance = NavMesh.AddNavMeshData(islandData);
    }

    private IEnumerator Start()
    {
        // Agents are stored disabled in the prefab so they do not attempt to
        // bind before navigation data and static obstacle carving are ready.
        yield return new WaitForSeconds(0.75f);
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

    private void OnDisable()
    {
        if (instance.valid) instance.Remove();
    }
}
