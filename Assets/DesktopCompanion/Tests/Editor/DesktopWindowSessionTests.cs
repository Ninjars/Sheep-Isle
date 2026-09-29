using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace SheepIsle.DesktopWindowing.Tests
{
    public sealed class DesktopWindowSessionTests
    {
        [Test]
        public void Restore_ClampsSavedPosition_AndPersistsObservedPosition()
        {
            var backend = new FakeBackend
            {
                WindowPosition = new Vector2(10f, 20f),
                ClampedPosition = new Vector2(300f, 400f)
            };
            var settings = new FakeSettings
            {
                HasSavedPosition = true,
                SavedPosition = new Vector2(5000f, -2000f)
            };
            var session = new DesktopWindowSession(backend, settings, null);

            session.Restore();

            Assert.That(backend.WindowPosition, Is.EqualTo(new Vector2(300f, 400f)));
            Assert.That(settings.SavedPosition, Is.EqualTo(new Vector2(300f, 400f)));
        }

        [Test]
        public void Restore_FailedPin_DoesNotOverwriteSavedPin()
        {
            var backend = new FakeBackend { IsPinned = false, PinSucceeds = false };
            var settings = new FakeSettings { SavedPinned = true };
            var session = new DesktopWindowSession(backend, settings, null);

            session.Restore();

            Assert.That(session.IsPinned, Is.False);
            Assert.That(settings.SavedPinned, Is.True);
            Assert.That(settings.PinSaveCount, Is.Zero);
        }

        [Test]
        public void SetVisible_HideWithoutRecoverySurface_KeepsWindowVisible()
        {
            var backend = new FakeBackend { HasRecoverySurface = false, IsVisible = true };
            var session = new DesktopWindowSession(backend, new FakeSettings(), null);

            Assert.That(session.SetVisible(false), Is.False);
            Assert.That(session.IsVisible, Is.True);
            Assert.That(backend.IsVisible, Is.True);
        }

        [Test]
        public void SetVisible_NativeFailure_KeepsObservedStateAndPreference()
        {
            var backend = new FakeBackend { IsVisible = true, VisibilitySucceeds = false };
            var observed = new List<bool>();
            var session = new DesktopWindowSession(backend, new FakeSettings(), observed.Add);

            Assert.That(session.SetVisible(false), Is.False);
            Assert.That(session.IsVisible, Is.True);
            Assert.That(observed, Is.Empty);
        }

        [Test]
        public void TogglePin_NativeFailure_KeepsObservedStateAndPreference()
        {
            var backend = new FakeBackend { IsPinned = false, PinSucceeds = false };
            var settings = new FakeSettings { SavedPinned = false };
            var session = new DesktopWindowSession(backend, settings, null);

            Assert.That(session.TogglePin(), Is.False);
            Assert.That(session.IsPinned, Is.False);
            Assert.That(settings.SavedPinned, Is.False);
            Assert.That(settings.PinSaveCount, Is.Zero);
        }

        [Test]
        public void SetVisible_UnchangedObservedState_DoesNotNotify()
        {
            var backend = new FakeBackend { IsVisible = true };
            var observed = new List<bool>();
            var session = new DesktopWindowSession(backend, new FakeSettings(), observed.Add);

            Assert.That(session.SetVisible(true), Is.True);
            Assert.That(observed, Is.Empty);
        }

        [Test]
        public void UpdateMove_AppliesCursorDelta_AndSavesObservedEndPosition()
        {
            var backend = new FakeBackend
            {
                CursorPosition = new Vector2(10f, 10f),
                WindowPosition = new Vector2(100f, 200f)
            };
            var settings = new FakeSettings();
            var session = new DesktopWindowSession(backend, settings, null);

            session.BeginMove(DesktopWindowDragInput.OptionPrimary);
            backend.CursorPosition = new Vector2(15f, 7f);
            session.UpdateMove(true);
            session.UpdateMove(false);

            Assert.That(backend.WindowPosition, Is.EqualTo(new Vector2(105f, 197f)));
            Assert.That(settings.SavedPosition, Is.EqualTo(new Vector2(105f, 197f)));
            Assert.That(session.IsMoving, Is.False);
        }

        [Test]
        public void ProcessActions_QuitCombinedWithOtherFlags_DoesNotHideOrToggle()
        {
            var backend = new FakeBackend { IsVisible = true, IsPinned = false };
            var session = new DesktopWindowSession(backend, new FakeSettings(), null);

            bool shouldQuit = session.ProcessActions(
                DesktopWindowAction.Quit | DesktopWindowAction.Hide | DesktopWindowAction.TogglePin);

            Assert.That(shouldQuit, Is.True);
            Assert.That(session.IsVisible, Is.True);
            Assert.That(session.IsPinned, Is.False);
        }

        [Test]
        public void ProcessActions_ShowAndHide_PrefersShow()
        {
            var backend = new FakeBackend { IsVisible = false };
            var session = new DesktopWindowSession(backend, new FakeSettings(), null);

            bool shouldQuit = session.ProcessActions(DesktopWindowAction.Show | DesktopWindowAction.Hide);

            Assert.That(shouldQuit, Is.False);
            Assert.That(session.IsVisible, Is.True);
            Assert.That(backend.IsVisible, Is.True);
        }

        private sealed class FakeBackend : IDesktopWindowBackend
        {
            public bool IsReady { get; set; } = true;
            public bool HasRecoverySurface { get; set; } = true;
            public bool IsVisible { get; set; } = true;
            public bool IsPinned { get; set; }
            public Vector2 WindowPosition { get; set; }
            public Vector2 CursorPosition { get; set; }
            public Vector2 ClampedPosition { get; set; }
            public bool VisibilitySucceeds { get; set; } = true;
            public bool PinSucceeds { get; set; } = true;
            public bool PositionSucceeds { get; set; } = true;

            public IEnumerator Initialize(GameObject host, Camera camera, int windowSize)
            {
                yield break;
            }

            public DesktopWindowAction ConsumeActions()
            {
                return DesktopWindowAction.None;
            }

            public bool TrySetVisible(bool requested)
            {
                if (VisibilitySucceeds)
                {
                    IsVisible = requested;
                }

                return VisibilitySucceeds;
            }

            public bool TrySetPinned(bool requested)
            {
                if (PinSucceeds)
                {
                    IsPinned = requested;
                }

                return PinSucceeds;
            }

            public bool TrySetWindowPosition(Vector2 requested, bool clampToVisibleArea)
            {
                if (!PositionSucceeds)
                {
                    return false;
                }

                WindowPosition = clampToVisibleArea ? ClampedPosition : requested;
                return true;
            }

            public void SetRecoverySurfaceState(bool visible, bool pinned)
            {
            }

            public void Dispose()
            {
            }
        }

        private sealed class FakeSettings : IDesktopWindowSettings
        {
            public bool HasSavedPosition { get; set; }
            public Vector2 SavedPosition { get; set; }
            public bool SavedPinned { get; set; }
            public int PinSaveCount { get; private set; }

            public bool TryLoadPosition(out Vector2 position)
            {
                position = SavedPosition;
                return HasSavedPosition;
            }

            public void SavePosition(Vector2 position)
            {
                HasSavedPosition = true;
                SavedPosition = position;
            }

            public bool LoadPinned()
            {
                return SavedPinned;
            }

            public void SavePinned(bool pinned)
            {
                SavedPinned = pinned;
                PinSaveCount++;
            }
        }
    }
}
