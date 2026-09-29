using System;

namespace SheepIsle.DesktopWindowing
{
    [Flags]
    public enum DesktopWindowAction
    {
        None = 0,
        Show = 1,
        Hide = 2,
        TogglePin = 4,
        Quit = 8
    }
}
