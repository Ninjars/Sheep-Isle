namespace SheepIsle.DesktopWindowing
{
    public static class DesktopWindowInputPolicy
    {
        public static DesktopWindowDragInput SelectDragInput(
            bool isMacOS,
            bool middlePressed,
            bool primaryPressed,
            bool optionPressed)
        {
            if (middlePressed)
            {
                return DesktopWindowDragInput.MiddleMouse;
            }

            if (isMacOS && primaryPressed && optionPressed)
            {
                return DesktopWindowDragInput.OptionPrimary;
            }

            return DesktopWindowDragInput.None;
        }

        public static bool IsDragHeld(
            DesktopWindowDragInput input,
            bool middleHeld,
            bool primaryHeld,
            bool optionHeld)
        {
            switch (input)
            {
                case DesktopWindowDragInput.MiddleMouse:
                    return middleHeld;
                case DesktopWindowDragInput.OptionPrimary:
                    return primaryHeld && optionHeld;
                default:
                    return false;
            }
        }

        public static bool ReservesPrimaryClick(bool isMacOS, bool primaryHeld, bool optionHeld)
        {
            return isMacOS && primaryHeld && optionHeld;
        }

        public static bool AllowsGameplayPrimaryClick(
            bool isMacOS,
            bool primaryPressed,
            bool optionPressed)
        {
            return primaryPressed
                && !ReservesPrimaryClick(isMacOS, primaryPressed, optionPressed);
        }
    }
}
