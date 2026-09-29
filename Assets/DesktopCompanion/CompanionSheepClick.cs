using UnityEngine;
using SheepIsle.DesktopWindowing;

[RequireComponent(typeof(Camera))]
public sealed class CompanionSheepClick : MonoBehaviour
{
    private Camera sceneCamera;

    private void Awake() { sceneCamera = GetComponent<Camera>(); }

    private void Update()
    {
        bool primaryPressed = Input.GetMouseButtonDown(0);
        bool optionPressed = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
        bool isMacOS = Application.platform == RuntimePlatform.OSXPlayer;
        if (!DesktopWindowInputPolicy.AllowsGameplayPrimaryClick(
                isMacOS, primaryPressed, optionPressed)) return;
        var ray = sceneCamera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hit, 500f)) return;
        var sheep = hit.collider.GetComponentInParent<CompanionSheep>();
        if (sheep != null) sheep.Pet(transform);
    }
}
