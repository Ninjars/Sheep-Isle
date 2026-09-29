using System;
using System.Collections;
using UnityEngine;

namespace SheepIsle.DesktopWindowing
{
    public interface IDesktopWindowBackend : IDisposable
    {
        bool IsReady { get; }
        bool HasRecoverySurface { get; }
        bool IsVisible { get; }
        bool IsPinned { get; }
        Vector2 WindowPosition { get; }
        Vector2 CursorPosition { get; }

        IEnumerator Initialize(GameObject host, Camera camera, int windowSize);
        DesktopWindowAction ConsumeActions();
        bool TrySetVisible(bool requested);
        bool TrySetPinned(bool requested);
        bool TrySetWindowPosition(Vector2 requested, bool clampToVisibleArea);
        void SetRecoverySurfaceState(bool visible, bool pinned);
    }
}
