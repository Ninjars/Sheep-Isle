using System.Collections;
using UnityEngine;

namespace SheepIsle.DesktopWindowing
{
    public sealed class MacOSDesktopWindowBackend : IDesktopWindowBackend
    {
        private const float OpacityThreshold = 0.05f;

        private IMacOSWindowFacade window;
        private IMacOSMenuBridge menu;
        private bool visible = true;
        private bool pinned;
        private bool disposed;
        private Vector2 windowPosition;
        private Vector2 cursorPosition;

        public MacOSDesktopWindowBackend()
        {
        }

        public MacOSDesktopWindowBackend(IMacOSWindowFacade window, IMacOSMenuBridge menu)
        {
            this.window = window;
            this.menu = menu;
        }

        public bool IsReady { get; private set; }
        public bool HasRecoverySurface { get; private set; }
        public bool IsVisible => visible;
        public bool IsPinned => pinned;

        public Vector2 WindowPosition
        {
            get
            {
                if (window != null)
                {
                    windowPosition = window.WindowPosition;
                }

                return windowPosition;
            }
        }

        public Vector2 CursorPosition
        {
            get
            {
                if (window != null)
                {
                    cursorPosition = window.CursorPosition;
                }

                return cursorPosition;
            }
        }

        public IEnumerator Initialize(GameObject host, Camera camera, int windowSize)
        {
            if (disposed)
            {
                yield break;
            }

#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
            Application.runInBackground = true;
            Application.targetFrameRate = 30;
            QualitySettings.antiAliasing = 0;
            Screen.fullScreenMode = FullScreenMode.Windowed;
            Screen.SetResolution(windowSize, windowSize, false);

            window ??= new UniWindowFacade(host);
            menu ??= new MacOSMenuBridge();
#endif

            // Give the player window and UniWindow component one frame to attach.
            yield return null;

            bool transparent = window != null
                && window.Configure(camera, OpacityThreshold)
                && window.IsTransparent;
            IsReady = transparent;
            if (!transparent)
            {
                Debug.LogError(
                    "Could not enable macOS desktop-window transparency. " +
                    "Verify the UniWindow ARM64 plug-in is present and the player is windowed.");
            }

            HasRecoverySurface = menu != null && menu.Initialize();
            if (!HasRecoverySurface)
            {
                Debug.LogError(
                    "Could not create the macOS Sheep Isle menu-bar item. " +
                    "The window will remain visible so it cannot become stranded.");
            }

            visible = HasRecoverySurface ? menu.IsWindowVisible() : true;
            pinned = window != null && window.IsPinned;
            _ = WindowPosition;
            _ = CursorPosition;
            SetRecoverySurfaceState(visible, pinned);
        }

        public DesktopWindowAction ConsumeActions()
        {
            return HasRecoverySurface
                ? menu.ConsumeActions()
                : DesktopWindowAction.None;
        }

        public bool TrySetVisible(bool requested)
        {
            if (!HasRecoverySurface)
            {
                return false;
            }

            bool applied = menu.TrySetWindowVisible(requested);
            visible = menu.IsWindowVisible();
            SetRecoverySurfaceState(visible, pinned);
            return applied && visible == requested;
        }

        public bool TrySetPinned(bool requested)
        {
            if (window == null)
            {
                return false;
            }

            bool applied = window.TrySetPinned(requested);
            pinned = window.IsPinned;
            SetRecoverySurfaceState(visible, pinned);
            return applied && pinned == requested;
        }

        public bool TrySetWindowPosition(Vector2 requested, bool clampToVisibleArea)
        {
            if (window == null)
            {
                return false;
            }

            bool moved = window.TrySetWindowPosition(requested);
            bool clamped = !clampToVisibleArea
                || (HasRecoverySurface && menu.ClampWindowToVisibleScreen());
            windowPosition = window.WindowPosition;
            return moved && clamped;
        }

        public void SetRecoverySurfaceState(bool isVisible, bool isPinned)
        {
            visible = isVisible;
            pinned = isPinned;
            if (HasRecoverySurface)
            {
                menu.SetMenuState(visible, pinned);
            }
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            HasRecoverySurface = false;
            IsReady = false;
            menu?.Dispose();
            window?.Dispose();
        }
    }
}
