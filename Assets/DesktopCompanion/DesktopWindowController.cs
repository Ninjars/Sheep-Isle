using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public sealed class DesktopWindowController : MonoBehaviour
{
    private const int WindowStyle = -16;
    private const int ExtendedWindowStyle = -20;
    private const int Popup = unchecked((int)0x80000000);
    private const int Visible = 0x10000000;
    private const int Layered = 0x00080000;
    private const int TopmostStyle = 0x00000008;
    private const uint ColorKey = 0x00000001;
    private const uint Magenta = 0x00FF00FF;
    private const uint NoSize = 0x0001;
    private const uint NoMove = 0x0002;
    private const uint NoZOrder = 0x0004;
    private const uint NoActivate = 0x0010;
    private const uint FrameChanged = 0x0020;
    private const int RightMouseButton = 0x02;

    private static readonly IntPtr Topmost = new IntPtr(-1);
    private static readonly IntPtr Normal = new IntPtr(-2);

    [SerializeField, Range(320, 800)] private int windowSize = 480;
    private IntPtr window;
    private bool pinned;
    private bool dragging;
    private Point dragCursorStart;
    private Rect dragWindowStart;

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
    [StructLayout(LayoutKind.Sequential)]
    private struct Point { public int X; public int Y; }

    [StructLayout(LayoutKind.Sequential)]
    private struct Rect { public int Left; public int Top; public int Right; public int Bottom; }

    [DllImport("user32.dll")] private static extern IntPtr GetActiveWindow();
    [DllImport("user32.dll", SetLastError = true)] private static extern int GetWindowLong(IntPtr hwnd, int index);
    [DllImport("user32.dll", SetLastError = true)] private static extern int SetWindowLong(IntPtr hwnd, int index, int value);
    [DllImport("user32.dll", SetLastError = true)] private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint colorKey, byte alpha, uint flags);
    [DllImport("user32.dll", SetLastError = true)] private static extern bool SetWindowPos(IntPtr hwnd, IntPtr after, int x, int y, int width, int height, uint flags);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern bool SetWindowText(IntPtr hwnd, string title);
    [DllImport("user32.dll")] private static extern short GetAsyncKeyState(int virtualKey);
    [DllImport("user32.dll")] private static extern bool GetCursorPos(out Point point);
    [DllImport("user32.dll")] private static extern bool GetWindowRect(IntPtr hwnd, out Rect rect);
#else
    private struct Point { public int X; public int Y; }
    private struct Rect { public int Left; public int Top; public int Right; public int Bottom; }
#endif

    private IEnumerator Start()
    {
        var camera = GetComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color32(255, 0, 255, 255);

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        Application.targetFrameRate = 30;
        QualitySettings.antiAliasing = 0;
        Screen.fullScreenMode = FullScreenMode.Windowed;
        Screen.SetResolution(windowSize, windowSize, false);
        // Unity applies the resolution change at the end of this frame. Styling
        // the native window before then lets Unity restore its caption and frame.
        yield return null;

        for (var attempt = 0; attempt < 100 && window == IntPtr.Zero; attempt++)
        {
            window = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
            if (window == IntPtr.Zero) window = GetActiveWindow();
            if (window == IntPtr.Zero) yield return new WaitForSecondsRealtime(0.05f);
        }

        if (window == IntPtr.Zero)
        {
            Debug.LogError("Desktop window handle was not found.");
            yield break;
        }

        SetWindowLong(window, WindowStyle, Popup | Visible);
        SetWindowPos(window, IntPtr.Zero, 0, 0, 0, 0,
            NoMove | NoSize | NoZOrder | FrameChanged);
        SetWindowLong(window, ExtendedWindowStyle,
            GetWindowLong(window, ExtendedWindowStyle) | Layered);
        if (!SetLayeredWindowAttributes(window, Magenta, 255, ColorKey))
            Debug.LogError("Could not enable desktop window transparency: " + Marshal.GetLastWin32Error());
        UpdateTitle();
#endif
        yield break;
    }

    private void Update()
    {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        if (window == IntPtr.Zero) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
            return;
        }

        if (Input.GetKeyDown(KeyCode.P))
            TogglePin();

        if (Input.GetMouseButtonDown(1) &&
            GetCursorPos(out dragCursorStart) &&
            GetWindowRect(window, out dragWindowStart))
            dragging = true;

        if (!dragging) return;
        if ((GetAsyncKeyState(RightMouseButton) & 0x8000) == 0)
        {
            dragging = false;
            return;
        }

        if (GetCursorPos(out var cursor))
            SetWindowPos(window, IntPtr.Zero,
                dragWindowStart.Left + cursor.X - dragCursorStart.X,
                dragWindowStart.Top + cursor.Y - dragCursorStart.Y,
                0, 0, NoSize | NoZOrder | NoActivate);
#endif
    }

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
    private void TogglePin()
    {
        var requested = !pinned;
        var applied = SetWindowPos(window, requested ? Topmost : Normal, 0, 0, 0, 0,
            NoMove | NoSize | NoActivate);
        pinned = (GetWindowLong(window, ExtendedWindowStyle) & TopmostStyle) != 0;
        if (!applied || pinned != requested)
            Debug.LogWarning("Could not change pin state: " + Marshal.GetLastWin32Error());

        UpdateTitle();
    }

    private void UpdateTitle()
    {
        SetWindowText(window, pinned
            ? "Sheep Isle (pinned) - P unpin, right-drag move, Esc exit"
            : "Sheep Isle - P pin, right-drag move, Esc exit");
    }
#endif
}
