using NUnit.Framework;
using SheepIsle.DesktopWindowing;

namespace SheepIsle.DesktopWindowing.Tests
{
    public sealed class DesktopWindowInputPolicyTests
    {
        [TestCase(false)]
        [TestCase(true)]
        public void SelectDragInput_MiddlePress_ReturnsMiddleOnBothPlatforms(bool isMacOS)
        {
            Assert.That(DesktopWindowInputPolicy.SelectDragInput(
                isMacOS, true, false, false), Is.EqualTo(DesktopWindowDragInput.MiddleMouse));
        }

        [TestCase(false, DesktopWindowDragInput.None)]
        [TestCase(true, DesktopWindowDragInput.OptionPrimary)]
        public void SelectDragInput_OptionPrimary_ReturnsOptionPrimaryOnlyOnMacOS(
            bool isMacOS, DesktopWindowDragInput expected)
        {
            Assert.That(DesktopWindowInputPolicy.SelectDragInput(
                isMacOS, false, true, true), Is.EqualTo(expected));
        }

        [Test]
        public void IsDragHeld_OptionReleased_ReturnsFalseWhilePrimaryRemainsHeld()
        {
            Assert.That(DesktopWindowInputPolicy.IsDragHeld(
                DesktopWindowDragInput.OptionPrimary, false, true, false), Is.False);
        }

        [TestCase(false, false)]
        [TestCase(true, true)]
        public void ReservesPrimaryClick_OptionPrimary_ReturnsTrueOnlyOnMacOS(
            bool isMacOS, bool expected)
        {
            Assert.That(DesktopWindowInputPolicy.ReservesPrimaryClick(
                isMacOS, true, true), Is.EqualTo(expected));
        }
    }
}
