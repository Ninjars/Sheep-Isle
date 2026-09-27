using UnityEngine;

[RequireComponent(typeof(Camera))]
public sealed class CompanionSheepClick : MonoBehaviour
{
    private Camera sceneCamera;

    private void Awake() { sceneCamera = GetComponent<Camera>(); }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        var ray = sceneCamera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hit, 500f)) return;
        var sheep = hit.collider.GetComponentInParent<CompanionSheep>();
        if (sheep != null) sheep.Pet(transform);
    }
}
