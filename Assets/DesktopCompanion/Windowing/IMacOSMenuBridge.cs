using System;

namespace SheepIsle.DesktopWindowing
{
    public interface IMacOSMenuBridge : IDisposable
    {
        bool Initialize();
        DesktopWindowAction ConsumeActions();
        void SetMenuState(bool visible, bool pinned);
        bool TrySetWindowVisible(bool requested);
        bool IsWindowVisible();
        bool ClampWindowToVisibleScreen();
    }
}
