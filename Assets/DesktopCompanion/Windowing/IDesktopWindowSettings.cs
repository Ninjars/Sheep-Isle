using UnityEngine;

namespace SheepIsle.DesktopWindowing
{
    public interface IDesktopWindowSettings
    {
        bool TryLoadPosition(out Vector2 position);
        void SavePosition(Vector2 position);
        bool LoadPinned();
        void SavePinned(bool pinned);
    }
}
