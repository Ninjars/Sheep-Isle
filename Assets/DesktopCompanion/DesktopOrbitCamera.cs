using UnityEngine;

[RequireComponent(typeof(Camera))]
public sealed class DesktopOrbitCamera : MonoBehaviour
{
    [SerializeField, Range(30f, 65f)] private float fieldOfView = 42f;
    [SerializeField] private float minimumPitch = 5f;
    [SerializeField] private float maximumPitch = 70f;
    [SerializeField] private float minimumDistance = 80f;
    [SerializeField] private float maximumDistance = 220f;
    [SerializeField] private float yawDegreesPerPixel = 0.35f;
    [SerializeField] private float pitchDegreesPerPixel = 0.25f;
    [SerializeField] private float zoomPerWheelStep = 0.12f;
    [SerializeField] private float smoothing = 9f;

    private Vector3 pivot;
    private float yaw;
    private float pitch;
    private float distance;
    private float targetYaw;
    private float targetPitch;
    private float targetDistance;
    private Vector3 lastMousePosition;
    private bool orbiting;

    public float Yaw => targetYaw;
    public float Pitch => targetPitch;
    public float Distance => targetDistance;

    private void Awake()
    {
        var camera = GetComponent<Camera>();

        // The scene camera already points at the placeholder island. Preserve
        // that composition by using its ground-plane aim as the orbit pivot.
        var ground = new Plane(Vector3.up, Vector3.zero);
        pivot = ground.Raycast(new Ray(transform.position, transform.forward), out var hit)
            ? transform.position + transform.forward * hit
            : transform.position + transform.forward * 140f;

        var offset = transform.position - pivot;
        distance = Mathf.Clamp(offset.magnitude, minimumDistance, maximumDistance);
        yaw = Mathf.Atan2(offset.x, offset.z) * Mathf.Rad2Deg;
        pitch = Mathf.Clamp(Mathf.Asin(offset.y / offset.magnitude) * Mathf.Rad2Deg,
            minimumPitch, maximumPitch);
        targetYaw = yaw;
        targetPitch = pitch;
        targetDistance = distance;

        camera.orthographic = false;
        camera.fieldOfView = fieldOfView;
        ApplyView();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            orbiting = true;
            lastMousePosition = Input.mousePosition;
        }

        if (orbiting)
        {
            if (!Input.GetMouseButton(1))
                orbiting = false;
            else
            {
                var mousePosition = Input.mousePosition;
                var delta = mousePosition - lastMousePosition;
                lastMousePosition = mousePosition;
                var distanceScale = Mathf.Sqrt(targetDistance / 140f);
                targetYaw += delta.x * yawDegreesPerPixel * distanceScale;
                targetPitch = Mathf.Clamp(targetPitch - delta.y * pitchDegreesPerPixel,
                    minimumPitch, maximumPitch);
            }
        }

        var wheel = Input.mouseScrollDelta.y;
        if (wheel != 0f)
            targetDistance = Mathf.Clamp(targetDistance * Mathf.Exp(-wheel * zoomPerWheelStep),
                minimumDistance, maximumDistance);

        var blend = 1f - Mathf.Exp(-smoothing * Time.unscaledDeltaTime);
        yaw = Mathf.LerpAngle(yaw, targetYaw, blend);
        pitch = Mathf.Lerp(pitch, targetPitch, blend);
        distance = Mathf.Lerp(distance, targetDistance, blend);
        ApplyView();
    }

    private void OnApplicationFocus(bool focused)
    {
        if (!focused) orbiting = false;
    }

    public void RestoreView(float savedYaw, float savedPitch, float savedDistance)
    {
        targetYaw = yaw = savedYaw;
        targetPitch = pitch = Mathf.Clamp(savedPitch, minimumPitch, maximumPitch);
        targetDistance = distance = Mathf.Clamp(savedDistance, minimumDistance, maximumDistance);
        ApplyView();
    }

    private void ApplyView()
    {
        var rotation = Quaternion.Euler(pitch, yaw, 0f);
        transform.SetPositionAndRotation(pivot + rotation * Vector3.back * distance, rotation);
    }
}
