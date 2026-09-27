using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(AudioSource), typeof(BoxCollider))]
public sealed class CompanionSheep : MonoBehaviour
{
    [SerializeField] private Transform visuals;
    [SerializeField] private Transform head;
    [SerializeField] private SoundBank voices;
    [SerializeField] private float wanderRadius = 12f;
    [SerializeField] private Vector2 idleSeconds = new Vector2(3f, 8f);
    [SerializeField] private Vector2 baaSeconds = new Vector2(18f, 75f);
    [SerializeField] private Vector2 voicePitchRange = new Vector2(0.82f, 1.25f);

    private NavMeshAgent agent;
    private AudioSource audioSource;
    private AudioClip voice;
    private float voicePitch;
    private NavMeshPath path;
    private Vector3 visualRestPosition;
    private Quaternion headRestRotation;
    private float nextWanderAt;
    private float nextBaaAt;
    private float petStartedAt;
    private float petEndsAt;
    private bool hasDestination;
    private bool wasBeingPetted;
    private Transform petViewer;

    public void Configure(Transform visualRoot, Transform headTransform,
        SoundBank soundBank)
    {
        visuals = visualRoot;
        head = headTransform;
        voices = soundBank;
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        path = new NavMeshPath();
        visualRestPosition = visuals.localPosition;
        headRestRotation = head.localRotation;
        if (voices != null && voices.sounds != null && voices.sounds.Count > 0)
            voice = voices.sounds[Random.Range(0, voices.sounds.Count)];
        voicePitch = Random.Range(voicePitchRange.x, voicePitchRange.y);
        nextBaaAt = Time.time + Random.Range(6f, 25f);
    }

    private void OnEnable()
    {
        CompanionSoundSettings.Changed += OnSoundChanged;
    }

    private void OnDisable()
    {
        CompanionSoundSettings.Changed -= OnSoundChanged;
    }

    private void Start()
    {
        nextWanderAt = Time.time + Random.Range(0f, 3f);
    }

    private void Update()
    {
        var petting = Time.time < petEndsAt;
        if (petting)
        {
            TurnTowardCamera();
        }
        else
        {
            if (wasBeingPetted)
            {
                wasBeingPetted = false;
                if (agent.isOnNavMesh)
                {
                    agent.updateRotation = true;
                    agent.isStopped = false;
                    agent.ResetPath();
                }
                hasDestination = false;
                nextWanderAt = Time.time + Random.Range(idleSeconds.x, idleSeconds.y);
            }
            UpdateWandering();
        }

        if (Time.time >= nextBaaAt)
        {
            Baa();
            nextBaaAt = Time.time + Random.Range(baaSeconds.x, baaSeconds.y);
        }

        UpdateVisuals(petting);
    }

    public void Pet(Transform viewer)
    {
        petViewer = viewer;
        petStartedAt = Time.time;
        petEndsAt = Time.time + 1.45f;
        wasBeingPetted = true;
        if (agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.updateRotation = false;
        }
        Baa();
    }

    private void UpdateWandering()
    {
        if (!agent.isOnNavMesh) return;
        if (hasDestination && !agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance + 0.25f)
        {
            hasDestination = false;
            agent.ResetPath();
            nextWanderAt = Time.time + Random.Range(idleSeconds.x, idleSeconds.y);
        }
        if (hasDestination || Time.time < nextWanderAt) return;

        for (var attempt = 0; attempt < 8; attempt++)
        {
            var offset = Random.insideUnitCircle * wanderRadius;
            var candidate = transform.position + new Vector3(offset.x, 0f, offset.y);
            if (!NavMesh.SamplePosition(candidate, out var hit, 3f, NavMesh.AllAreas)) continue;
            if (Vector3.Distance(transform.position, hit.position) < 2f) continue;
            if (!agent.CalculatePath(hit.position, path) ||
                path.status != NavMeshPathStatus.PathComplete) continue;
            agent.isStopped = false;
            hasDestination = agent.SetDestination(hit.position);
            if (hasDestination) return;
        }
        nextWanderAt = Time.time + 3f;
    }

    private void TurnTowardCamera()
    {
        if (petViewer == null) return;
        var direction = petViewer.position - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.RotateTowards(transform.rotation,
                Quaternion.LookRotation(direction), 360f * Time.deltaTime);
    }

    private void UpdateVisuals(bool petting)
    {
        var breath = Mathf.Sin(Time.time * 3f) * 0.018f;
        var petFraction = petting ? Mathf.Clamp01((Time.time - petStartedAt) / 1.45f) : 0f;
        var hop = petting ? Mathf.Sin(petFraction * Mathf.PI) * 0.25f : 0f;
        var tilt = petting ? Mathf.Sin(petFraction * Mathf.PI * 2f) * 10f : 0f;
        visuals.localPosition = visualRestPosition + Vector3.up * (breath + hop);
        visuals.localRotation = Quaternion.Euler(0f, 0f, tilt);
        head.localRotation = headRestRotation * Quaternion.Euler(petting ? -12f * Mathf.Sin(petFraction * Mathf.PI) : 0f, 0f, 0f);
    }

    private void Baa()
    {
        if (!CompanionSoundSettings.Enabled || voice == null || audioSource.isPlaying) return;
        audioSource.pitch = voicePitch * Random.Range(0.96f, 1.04f);
        audioSource.PlayOneShot(voice);
    }

    private void OnSoundChanged(bool enabled)
    {
        if (!enabled) audioSource.Stop();
    }
}
