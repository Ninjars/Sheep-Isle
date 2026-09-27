using UnityEngine;
using UnityEngine.AI;

// Uses the original square Foot prefab and its hop animation. The four feet
// live in world space, separate from the sheep body, as in the old project.
public sealed class CompanionFloatingFeet : MonoBehaviour
{
    [SerializeField] private Game.Foot footPrefab;
    [SerializeField] private LayerMask groundMask = 513;
    [SerializeField] private float footScale = 1.8f;
    [SerializeField] private float stepDistance = 0.55f;

    private static readonly Vector3[] LocalAnchors =
    {
        new Vector3(-0.9f, -0.15f, 0.55f),
        new Vector3(-0.9f, -0.15f, -0.55f),
        new Vector3(0.9f, -0.15f, 0.55f),
        new Vector3(0.9f, -0.15f, -0.55f)
    };

    private readonly Game.Foot[] feet = new Game.Foot[4];
    private float nextStepAt;
    private int nextFoot;

    public void Configure(Game.Foot prefab) { footPrefab = prefab; }

    private void Start()
    {
        if (footPrefab == null)
        {
            Debug.LogError(name + " has no square foot prefab.");
            return;
        }

        for (var i = 0; i < feet.Length; i++)
        {
            var position = SurfacePoint(i, out var normal);
            var rotation = Quaternion.LookRotation(transform.forward, normal);
            feet[i] = Instantiate(footPrefab, position, rotation);
            feet[i].name = name + " - Foot " + (i + 1);
            feet[i].transform.localScale = Vector3.one * footScale;
        }
    }

    private void Update()
    {
        if (footPrefab == null || Time.time < nextStepAt) return;
        nextStepAt = Time.time + 0.12f;
        var foot = feet[nextFoot];
        var index = nextFoot;
        nextFoot = (nextFoot + 1) % feet.Length;
        if (foot == null) return;

        var position = SurfacePoint(index, out var normal);
        if ((foot.targetPosition - position).sqrMagnitude < stepDistance * stepDistance) return;
        foot.setTarget(position, Quaternion.LookRotation(transform.forward, normal));
    }

    private Vector3 SurfacePoint(int index, out Vector3 normal)
    {
        var anchor = transform.TransformPoint(LocalAnchors[index]);
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
