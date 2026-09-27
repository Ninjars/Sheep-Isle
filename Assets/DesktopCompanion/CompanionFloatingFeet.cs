using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// Uses the original square Foot prefab and its hop animation. The four feet
// live in world space, separate from the sheep body, as in the old project.
[RequireComponent(typeof(NavMeshAgent))]
public sealed class CompanionFloatingFeet : MonoBehaviour
{
    [SerializeField] private Game.Foot footPrefab;
    [SerializeField] private LayerMask groundMask = 513;
    [SerializeField] private float footScale = 1.8f;

    private const float LookAheadSeconds = 0.35f;
    private const float MaximumLead = 0.65f;
    private const float MovingStepDistance = 0.25f;
    private const float RestStepDistance = 0.1f;
    private const float StepInterval = 0.1f;

    private static readonly Vector3[] LocalAnchors =
    {
        new Vector3(-0.9f, -0.15f, 0.85f),
        new Vector3(-0.9f, -0.15f, -0.85f),
        new Vector3(0.9f, -0.15f, 0.85f),
        new Vector3(0.9f, -0.15f, -0.85f)
    };

    private readonly Game.Foot[] feet = new Game.Foot[4];
    private readonly float[] lastStepAt = new float[4];
    private NavMeshAgent agent;
    private Vector3 lead;
    private float nextStepAt;
    private int nextFoot;

    public void Configure(Game.Foot prefab) { footPrefab = prefab; }

    private IEnumerator Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (footPrefab == null)
        {
            Debug.LogError(name + " has no square foot prefab.");
            yield break;
        }
        if (agent == null)
        {
            Debug.LogError(name + " has no NavMeshAgent for foot placement.");
            yield break;
        }

        // The scene registers the baked NavMesh and may move the sheep clear of
        // scenery before enabling its agent. Plant feet only after that move.
        while (!agent.enabled || !agent.isOnNavMesh)
            yield return null;

        for (var i = 0; i < feet.Length; i++)
        {
            var position = SurfacePoint(i, Vector3.zero, out var normal);
            var rotation = Quaternion.LookRotation(transform.forward, normal);
            feet[i] = Instantiate(footPrefab, position, rotation);
            feet[i].name = name + " - Foot " + (i + 1);
            feet[i].transform.localScale = Vector3.one * footScale;
            lastStepAt[i] = Time.time - feet[i].movementDurationSeconds;
        }
    }

    private void LateUpdate()
    {
        if (feet[0] == null || Time.time < nextStepAt) return;

        var velocity = agent.isOnNavMesh && !agent.isStopped ? agent.velocity : Vector3.zero;
        velocity.y = 0f;
        var moving = velocity.sqrMagnitude > 0.01f;
        var direction = agent.isOnNavMesh ? agent.desiredVelocity : velocity;
        direction.y = 0f;
        var desiredLead = moving && direction.sqrMagnitude > 0.01f
            ? direction.normalized * Mathf.Min(MaximumLead, velocity.magnitude * LookAheadSeconds)
            : Vector3.zero;
        lead = Vector3.MoveTowards(lead, desiredLead, 5f * Time.deltaTime);

        var threshold = moving ? MovingStepDistance : RestStepDistance;
        var bestGap = threshold * threshold;
        var bestIndex = -1;
        var bestPosition = Vector3.zero;
        var bestNormal = Vector3.up;
        for (var offset = 0; offset < feet.Length; offset++)
        {
            var index = (nextFoot + offset) % feet.Length;
            var foot = feet[index];
            if (foot == null || Time.time - lastStepAt[index] < foot.movementDurationSeconds)
                continue;
            var position = SurfacePoint(index, lead, out var normal);
            var gap = (foot.targetPosition - position).sqrMagnitude;
            if (gap <= bestGap) continue;
            bestGap = gap;
            bestIndex = index;
            bestPosition = position;
            bestNormal = normal;
        }

        if (bestIndex < 0) return;
        feet[bestIndex].setTarget(bestPosition,
            Quaternion.LookRotation(transform.forward, bestNormal));
        lastStepAt[bestIndex] = Time.time;
        nextFoot = (bestIndex + 1) % feet.Length;
        nextStepAt = Time.time + StepInterval;
    }

    private Vector3 SurfacePoint(int index, Vector3 ahead, out Vector3 normal)
    {
        var anchor = transform.TransformPoint(LocalAnchors[index]) + ahead;
        if (Physics.Raycast(anchor + Vector3.up * 2f, Vector3.down,
            out var hit, 8f, groundMask, QueryTriggerInteraction.Ignore))
        {
            normal = hit.normal;
            return hit.point;
        }
        if (NavMesh.SamplePosition(anchor, out var navHit, 2f, NavMesh.AllAreas))
        {
            normal = Vector3.up;
            return navHit.position;
        }
        normal = Vector3.up;
        return anchor - Vector3.up * 1.2f;
    }

    private void OnDestroy()
    {
        foreach (var foot in feet)
            if (foot != null) Destroy(foot.gameObject);
    }
}
