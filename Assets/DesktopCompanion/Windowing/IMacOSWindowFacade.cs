using System;
using UnityEngine;

namespace SheepIsle.DesktopWindowing
{
    public interface IMacOSWindowFacade : IDisposable
    {
        bool IsTransparent { get; }
        bool IsPinned { get; }
        Vector2 WindowPosition { get; }
        Vector2 CursorPosition { get; }

        bool Configure(Camera camera, float opacityThreshold);
        bool TrySetPinned(bool requested);
        bool TrySetWindowPosition(Vector2 requested);
    }
}
