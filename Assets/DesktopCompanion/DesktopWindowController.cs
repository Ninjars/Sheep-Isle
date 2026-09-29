using System;
using System.Collections;
using SheepIsle.DesktopWindowing;
using UnityEngine;

[DefaultExecutionOrder(-200)]
[RequireComponent(typeof(Camera))]
public sealed class DesktopWindowController : MonoBehaviour
{
    public static event Action<bool> VisibilityChanged;
    public static bool IsIslandVisible { get; private set; } = true;

    [SerializeField, Range(320, 800)] private int windowSize = 480;

    private IDesktopWindowBackend backend;
    private DesktopWindowSession session;
    private DesktopWindowDragInput activeDragInput;
    private bool disposed;

    private IEnumerator Start()
    {
        IsIslandVisible = true;

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        backend = new WindowsDesktopWindowBackend();
#else
        yield break;
#endif

        yield return backend.Initialize(gameObject, GetComponent<Camera>(), windowSize);
        if (!backend.IsReady)
        {
            backend.Dispose();
            backend = null;
            yield break;
        }

        session = new DesktopWindowSession(
            backend,
            new PlayerPrefsDesktopWindowSettings(),
            PublishVisibility);
        session.Restore();
        IsIslandVisible = session.IsVisible;
    }

    private void Update()
    {
        if (session == null)
        {
            return;
        }

        if (session.ProcessActions(backend.ConsumeActions()))
        {
            Application.Quit();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            session.SavePosition();
            Application.Quit();
            return;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            session.TogglePin();
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            session.SetVisible(false);
            return;
        }

        bool isMacOS = Application.platform == RuntimePlatform.OSXPlayer;
        bool optionHeld = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
        DesktopWindowDragInput requestedDrag = DesktopWindowInputPolicy.SelectDragInput(
            isMacOS,
            Input.GetMouseButtonDown(2),
            Input.GetMouseButtonDown(0),
            optionHeld);

        if (!session.IsMoving && requestedDrag != DesktopWindowDragInput.None)
        {
            activeDragInput = requestedDrag;
            session.BeginMove(activeDragInput);
        }

        if (!session.IsMoving)
        {
            return;
        }

        bool middleHeld = Input.GetMouseButton(2);
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        if (backend is WindowsDesktopWindowBackend windowsBackend)
        {
            middleHeld = windowsBackend.IsMiddleMouseHeld;
        }
#endif

        bool dragHeld = DesktopWindowInputPolicy.IsDragHeld(
            activeDragInput,
            middleHeld,
            Input.GetMouseButton(0),
            optionHeld);
        session.UpdateMove(dragHeld);
        if (!session.IsMoving)
        {
            activeDragInput = DesktopWindowDragInput.None;
        }
    }

    private static void PublishVisibility(bool visible)
    {
        IsIslandVisible = visible;
        VisibilityChanged?.Invoke(visible);
    }

    private void OnApplicationQuit()
    {
        DisposeSession();
    }

    private void OnDestroy()
    {
        DisposeSession();
    }

    private void DisposeSession()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        session?.Dispose();
        session = null;
        backend?.Dispose();
        backend = null;
    }
}
