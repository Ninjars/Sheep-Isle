using UnityEngine;

namespace SheepIsle.DesktopWindowing
{
    public sealed class PlayerPrefsDesktopWindowSettings : IDesktopWindowSettings
    {
        private const string PositionSavedKey = "DesktopCompanion.WindowPositionSaved.v1";
        private const string PositionXKey = "DesktopCompanion.WindowX.v1";
        private const string PositionYKey = "DesktopCompanion.WindowY.v1";
        private const string PinnedKey = "DesktopCompanion.Pinned.v1";

        public bool TryLoadPosition(out Vector2 position)
        {
            position = default;
            if (PlayerPrefs.GetInt(PositionSavedKey, 0) == 0)
            {
                return false;
            }

            position = new Vector2(
                PlayerPrefs.GetInt(PositionXKey, 0),
                PlayerPrefs.GetInt(PositionYKey, 0));
            return true;
        }

        public void SavePosition(Vector2 position)
        {
            PlayerPrefs.SetInt(PositionXKey, Mathf.RoundToInt(position.x));
            PlayerPrefs.SetInt(PositionYKey, Mathf.RoundToInt(position.y));
            PlayerPrefs.SetInt(PositionSavedKey, 1);
            PlayerPrefs.Save();
        }

        public bool LoadPinned()
        {
            return PlayerPrefs.GetInt(PinnedKey, 0) != 0;
        }

        public void SavePinned(bool pinned)
        {
            PlayerPrefs.SetInt(PinnedKey, pinned ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
