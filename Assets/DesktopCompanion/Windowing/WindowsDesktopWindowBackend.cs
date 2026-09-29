using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

namespace SheepIsle.DesktopWindowing
{
    public sealed class WindowsDesktopWindowBackend : IDesktopWindowBackend
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
        private const int MiddleMouseButton = 0x04;
        private const int HideWindowCommand = 0;
        private const int ShowWindowCommand = 5;
        private const uint NearestMonitor = 0x00000002;

        private static readonly IntPtr Topmost = new IntPtr(-1);
        private static readonly IntPtr Normal = new IntPtr(-2);

        private IntPtr window;
        private bool pinned;
        private bool visible = true;
        private bool disposed;
        private Vector2 windowPosition;
        private Vector2 cursorPosition;

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        private WindowsDesktopTray tray;

        [StructLayout(LayoutKind.Sequential)]
        private struct Point
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct NativeRect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MonitorInfo
        {
            public int Size;
            public NativeRect Monitor;
            public NativeRect Work;
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
        [DllImport("user32.dll")] private static extern bool GetWindowRect(IntPtr hwnd, out NativeRect rect);
        [DllImport("user32.dll")] private static extern IntPtr MonitorFromRect(ref NativeRect rect, uint flags);
        [DllImport("user32.dll", CharSet = CharSet.Auto)] private static extern bool GetMonitorInfo(IntPtr monitor, ref MonitorInfo info);
        [DllImport("user32.dll")] private static extern bool ShowWindow(IntPtr hwnd, int command);
        [DllImport("user32.dll")] private static extern bool IsWindowVisible(IntPtr hwnd);
        [DllImport("user32.dll")] private static extern bool SetForegroundWindow(IntPtr hwnd);
#endif

        public bool IsReady { get; private set; }

        public bool HasRecoverySurface
        {
            get
            {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
                return tray != null && tray.IsReady;
#else
                return false;
#endif
            }
        }

        public bool IsVisible => visible;
        public bool IsPinned => pinned;

        public Vector2 WindowPosition
        {
            get
            {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
                if (window != IntPtr.Zero && GetWindowRect(window, out var rect))
                {
                    windowPosition = new Vector2(rect.Left, rect.Top);
                }
#endif
                return windowPosition;
            }
        }

        public Vector2 CursorPosition
        {
            get
            {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
                if (GetCursorPos(out var point))
                {
                    cursorPosition = new Vector2(point.X, point.Y);
                }
#endif
                return cursorPosition;
            }
        }

        public bool IsMiddleMouseHeld
        {
            get
            {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
                return (GetAsyncKeyState(MiddleMouseButton) & 0x8000) != 0;
#else
                return false;
#endif
            }
        }

        public IEnumerator Initialize(GameObject host, Camera camera, int windowSize)
        {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color32(255, 0, 255, 255);
            Application.runInBackground = true;
            Application.targetFrameRate = 30;
            QualitySettings.antiAliasing = 0;
            Screen.fullScreenMode = FullScreenMode.Windowed;
            Screen.SetResolution(windowSize, windowSize, false);
            yield return null;

            for (var attempt = 0; attempt < 100 && window == IntPtr.Zero; attempt++)
            {
                window = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
                if (window == IntPtr.Zero)
                {
                    window = GetActiveWindow();
                }

                if (window == IntPtr.Zero)
                {
                    yield return new WaitForSecondsRealtime(0.05f);
                }
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
            {
                Debug.LogError("Could not enable desktop window transparency: " +
                    Marshal.GetLastWin32Error());
            }

            visible = IsWindowVisible(window);
            pinned = ReadPinned();
            _ = WindowPosition;
            _ = CursorPosition;
            tray = new WindowsDesktopTray(window);
            IsReady = true;
            UpdateTitle();
#else
            yield break;
#endif
        }

        public DesktopWindowAction ConsumeActions()
        {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            return tray?.ConsumeActions() ?? DesktopWindowAction.None;
#else
            return DesktopWindowAction.None;
#endif
        }

        public bool TrySetVisible(bool requested)
        {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            if (window == IntPtr.Zero)
            {
                return false;
            }

            ShowWindow(window, requested ? ShowWindowCommand : HideWindowCommand);
            visible = IsWindowVisible(window);
            if (visible && requested)
            {
                SetForegroundWindow(window);
            }

            return visible == requested;
#else
            return false;
#endif
        }

        public bool TrySetPinned(bool requested)
        {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            if (window == IntPtr.Zero)
            {
                return false;
            }

            bool applied = SetWindowPos(
                window,
                requested ? Topmost : Normal,
                0,
                0,
                0,
                0,
                NoMove | NoSize | NoActivate);
            pinned = ReadPinned();
            if (!applied || pinned != requested)
            {
                Debug.LogWarning("Could not change pin state: " + Marshal.GetLastWin32Error());
            }

            UpdateTitle();
            return applied && pinned == requested;
#else
            return false;
#endif
        }

        public bool TrySetWindowPosition(Vector2 requested, bool clampToVisibleArea)
        {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            if (window == IntPtr.Zero || !GetWindowRect(window, out var current))
            {
                return false;
            }

            int x = Mathf.RoundToInt(requested.x);
            int y = Mathf.RoundToInt(requested.y);
            if (clampToVisibleArea)
            {
                int width = current.Right - current.Left;
                int height = current.Bottom - current.Top;
                var requestedRect = new NativeRect
                {
                    Left = x,
                    Top = y,
                    Right = x + width,
                    Bottom = y + height
                };
                IntPtr monitor = MonitorFromRect(ref requestedRect, NearestMonitor);
                var info = new MonitorInfo { Size = Marshal.SizeOf(typeof(MonitorInfo)) };
                if (monitor != IntPtr.Zero && GetMonitorInfo(monitor, ref info))
                {
                    x = Math.Max(info.Work.Left, Math.Min(x, info.Work.Right - width));
                    y = Math.Max(info.Work.Top, Math.Min(y, info.Work.Bottom - height));
                }
            }

            return SetWindowPos(window, IntPtr.Zero, x, y, 0, 0,
                NoSize | NoZOrder | NoActivate);
#else
            return false;
#endif
        }

        public void SetRecoverySurfaceState(bool isVisible, bool isPinned)
        {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            tray?.SetVisible(isVisible);
            visible = isVisible;
            pinned = isPinned;
            UpdateTitle();
#endif
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            tray?.Dispose();
            tray = null;
#endif
            IsReady = false;
        }

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        private bool ReadPinned()
        {
            return window != IntPtr.Zero &&
                (GetWindowLong(window, ExtendedWindowStyle) & TopmostStyle) != 0;
        }

        private void UpdateTitle()
        {
            if (window == IntPtr.Zero)
            {
                return;
            }

            SetWindowText(window, pinned
                ? "Sheep Isle (pinned) - P unpin, H hide, middle-drag move, Esc exit"
                : "Sheep Isle - P pin, H hide, middle-drag move, Esc exit");
        }
#endif
    }
}
