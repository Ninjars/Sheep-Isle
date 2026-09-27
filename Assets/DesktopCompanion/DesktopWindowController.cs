using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public sealed class DesktopWindowController : MonoBehaviour
{
    public static event Action<bool> VisibilityChanged;
    public static bool IsIslandVisible { get; private set; } = true;

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
    private const int MiddleMouseButton = 0x04;
    private const int HideWindowCommand = 0;
    private const int ShowWindowCommand = 5;
    private const uint NearestMonitor = 0x00000002;
    private const string PositionSavedKey = "DesktopCompanion.WindowPositionSaved.v1";
    private const string PositionXKey = "DesktopCompanion.WindowX.v1";
    private const string PositionYKey = "DesktopCompanion.WindowY.v1";
    private const string PinnedKey = "DesktopCompanion.Pinned.v1";

    private static readonly IntPtr Topmost = new IntPtr(-1);
    private static readonly IntPtr Normal = new IntPtr(-2);

    [SerializeField, Range(320, 800)] private int windowSize = 480;
    private IntPtr window;
    private bool pinned;
    private bool dragging;
    private Point dragCursorStart;
    private Rect dragWindowStart;
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
    private DesktopTrayIcon tray;
#endif

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
    [StructLayout(LayoutKind.Sequential)]
    private struct Point { public int X; public int Y; }

    [StructLayout(LayoutKind.Sequential)]
    private struct Rect { public int Left; public int Top; public int Right; public int Bottom; }

    [StructLayout(LayoutKind.Sequential)]
    private struct MonitorInfo
    {
        public int Size;
        public Rect Monitor;
        public Rect Work;
        public uint Flags;
    }

    [DllImport("user32.dll")] private static extern IntPtr GetActiveWindow();
    [DllImport("user32.dll", SetLastError = true)] private static extern int GetWindowLong(IntPtr hwnd, int index);
    [DllImport("user32.dll", SetLastError = true)] private static extern int SetWindowLong(IntPtr hwnd, int index, int value);
    [DllImport("user32.dll", SetLastError = true)] private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint colorKey, byte alpha, uint flags);
    [DllImport("user32.dll", SetLastError = true)] private static extern bool SetWindowPos(IntPtr hwnd, IntPtr after, int x, int y, int width, int height, uint flags);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern bool SetWindowText(IntPtr hwnd, string title);
    [DllImport("user32.dll")] private static extern short GetAsyncKeyState(int virtualKey);
    [DllImport("user32.dll")] private static extern bool GetCursorPos(out Point point);
    [DllImport("user32.dll")] private static extern bool GetWindowRect(IntPtr hwnd, out Rect rect);
    [DllImport("user32.dll")] private static extern IntPtr MonitorFromRect(ref Rect rect, uint flags);
    [DllImport("user32.dll", CharSet = CharSet.Auto)] private static extern bool GetMonitorInfo(IntPtr monitor, ref MonitorInfo info);
    [DllImport("user32.dll")] private static extern bool ShowWindow(IntPtr hwnd, int command);
    [DllImport("user32.dll")] private static extern bool IsWindowVisible(IntPtr hwnd);
    [DllImport("user32.dll")] private static extern bool SetForegroundWindow(IntPtr hwnd);
#else
    private struct Point { public int X; public int Y; }
    private struct Rect { public int Left; public int Top; public int Right; public int Bottom; }
#endif

    private IEnumerator Start()
    {
        IsIslandVisible = true;
        var camera = GetComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color32(255, 0, 255, 255);

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        Application.runInBackground = true;
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
        RestoreWindowState();
        UpdateTitle();
        tray = new DesktopTrayIcon(window);
#endif
        yield break;
    }

    private void Update()
    {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        if (window == IntPtr.Zero) return;
        if (tray != null)
        {
            if (tray.ExitRequested)
            {
                Application.Quit();
                return;
            }
            if (tray.ShowRequested) ShowIsland();
            if (tray.HideRequested) HideIsland();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
            return;
        }

        if (Input.GetKeyDown(KeyCode.P))
            TogglePin();

        if (Input.GetKeyDown(KeyCode.H))
        {
            HideIsland();
            return;
        }

        if (Input.GetMouseButtonDown(2) &&
            GetCursorPos(out dragCursorStart) &&
            GetWindowRect(window, out dragWindowStart))
            dragging = true;

        if (!dragging) return;
        if ((GetAsyncKeyState(MiddleMouseButton) & 0x8000) == 0)
        {
            dragging = false;
            SaveWindowPosition();
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
    private void RestoreWindowState()
    {
        if (PlayerPrefs.GetInt(PositionSavedKey, 0) != 0 &&
            GetWindowRect(window, out var current))
        {
            var x = PlayerPrefs.GetInt(PositionXKey, current.Left);
            var y = PlayerPrefs.GetInt(PositionYKey, current.Top);
            var width = current.Right - current.Left;
            var height = current.Bottom - current.Top;
            var saved = new Rect
            {
                Left = x, Top = y, Right = x + width, Bottom = y + height
            };
            var monitor = MonitorFromRect(ref saved, NearestMonitor);
            var info = new MonitorInfo { Size = Marshal.SizeOf(typeof(MonitorInfo)) };
            if (monitor != IntPtr.Zero && GetMonitorInfo(monitor, ref info))
            {
                x = Math.Max(info.Work.Left, Math.Min(x, info.Work.Right - width));
                y = Math.Max(info.Work.Top, Math.Min(y, info.Work.Bottom - height));
            }

            SetWindowPos(window, IntPtr.Zero, x, y, 0, 0,
                NoSize | NoZOrder | NoActivate);
            SaveWindowPosition();
        }

        ApplyPin(PlayerPrefs.GetInt(PinnedKey, 0) != 0, false);
    }

    private void SaveWindowPosition()
    {
        if (window == IntPtr.Zero || !GetWindowRect(window, out var rect)) return;
        PlayerPrefs.SetInt(PositionXKey, rect.Left);
        PlayerPrefs.SetInt(PositionYKey, rect.Top);
        PlayerPrefs.SetInt(PositionSavedKey, 1);
        PlayerPrefs.Save();
    }

    private void OnApplicationQuit()
    {
        SaveWindowPosition();
        tray?.Dispose();
        tray = null;
    }

    private void OnDestroy()
    {
        tray?.Dispose();
        tray = null;
    }

    private void HideIsland()
    {
        if (tray == null || !tray.IsReady) return;
        SaveWindowPosition();
        dragging = false;
        ShowWindow(window, HideWindowCommand);
        IsIslandVisible = IsWindowVisible(window);
        VisibilityChanged?.Invoke(IsIslandVisible);
        tray.SetVisible(IsIslandVisible);
    }

    private void ShowIsland()
    {
        ShowWindow(window, ShowWindowCommand);
        IsIslandVisible = IsWindowVisible(window);
        VisibilityChanged?.Invoke(IsIslandVisible);
        SetForegroundWindow(window);
        tray?.SetVisible(IsIslandVisible);
    }

    private void TogglePin()
    {
        ApplyPin(!pinned, true);
    }

    private void ApplyPin(bool requested, bool persist)
    {
        var applied = SetWindowPos(window, requested ? Topmost : Normal, 0, 0, 0, 0,
            NoMove | NoSize | NoActivate);
        pinned = (GetWindowLong(window, ExtendedWindowStyle) & TopmostStyle) != 0;
        if (!applied || pinned != requested)
            Debug.LogWarning("Could not change pin state: " + Marshal.GetLastWin32Error());

        if (persist)
        {
            PlayerPrefs.SetInt(PinnedKey, pinned ? 1 : 0);
            PlayerPrefs.Save();
        }
        UpdateTitle();
    }

    private void UpdateTitle()
    {
        SetWindowText(window, pinned
            ? "Sheep Isle (pinned) - P unpin, H hide, middle-drag move, Esc exit"
            : "Sheep Isle - P pin, H hide, middle-drag move, Esc exit");
    }
#endif
}
