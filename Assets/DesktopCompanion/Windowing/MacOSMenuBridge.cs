using System;
using System.Runtime.InteropServices;

namespace SheepIsle.DesktopWindowing
{
    public sealed class MacOSMenuBridge : IMacOSMenuBridge
    {
        private bool disposed;

#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
        private bool initialized;

        [DllImport("SheepIsleMacBridge")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool SheepIsleMac_Initialize();

        [DllImport("SheepIsleMacBridge")]
        private static extern void SheepIsleMac_Dispose();

        [DllImport("SheepIsleMacBridge")]
        private static extern int SheepIsleMac_ConsumeActions();

        [DllImport("SheepIsleMacBridge")]
        private static extern void SheepIsleMac_SetMenuState(
            [MarshalAs(UnmanagedType.I1)] bool visible,
            [MarshalAs(UnmanagedType.I1)] bool pinned);

        [DllImport("SheepIsleMacBridge")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool SheepIsleMac_SetWindowVisible(
            [MarshalAs(UnmanagedType.I1)] bool visible);

        [DllImport("SheepIsleMacBridge")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool SheepIsleMac_IsWindowVisible();

        [DllImport("SheepIsleMacBridge")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool SheepIsleMac_ClampWindowToVisibleScreen();
#endif

        public bool Initialize()
        {
#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
            if (disposed)
            {
                return false;
            }

            try
            {
                initialized = SheepIsleMac_Initialize();
            }
            catch (DllNotFoundException)
            {
                initialized = false;
            }
            catch (EntryPointNotFoundException)
            {
                initialized = false;
            }

            return initialized;
#else
            return false;
#endif
        }

        public DesktopWindowAction ConsumeActions()
        {
#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
            return initialized
                ? (DesktopWindowAction)SheepIsleMac_ConsumeActions()
                : DesktopWindowAction.None;
#else
            return DesktopWindowAction.None;
#endif
        }

        public void SetMenuState(bool visible, bool pinned)
        {
#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
            if (initialized)
            {
                SheepIsleMac_SetMenuState(visible, pinned);
            }
#endif
        }

        public bool TrySetWindowVisible(bool requested)
        {
#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
            return initialized && SheepIsleMac_SetWindowVisible(requested);
#else
            return false;
#endif
        }

        public bool IsWindowVisible()
        {
#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
            return initialized && SheepIsleMac_IsWindowVisible();
#else
            return false;
#endif
        }

        public bool ClampWindowToVisibleScreen()
        {
#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
            return initialized && SheepIsleMac_ClampWindowToVisibleScreen();
#else
            return false;
#endif
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
            if (initialized)
            {
                SheepIsleMac_Dispose();
            }
            initialized = false;
#endif
        }
    }
}
