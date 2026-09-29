using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace SheepIsle.DesktopWindowing.Tests
{
    public sealed class MacOSDesktopWindowBackendTests
    {
        [Test]
        public void Initialize_AllBoundariesSucceed_SetsReadyAndRecoveryAvailable()
        {
            var window = new FakeWindowFacade();
            var menu = new FakeMenuBridge();
            var backend = new MacOSDesktopWindowBackend(window, menu);

            Drain(backend.Initialize(null, null, 480));

            Assert.That(backend.IsReady, Is.True);
            Assert.That(backend.HasRecoverySurface, Is.True);
            Assert.That(backend.IsVisible, Is.True);
        }

        [Test]
        public void Initialize_TransparencyFails_LeavesBackendNotReady()
        {
            var window = new FakeWindowFacade { ConfigureSucceeds = false };
            var backend = new MacOSDesktopWindowBackend(window, new FakeMenuBridge());

            LogAssert.Expect(
                LogType.Error,
                "Could not enable macOS desktop-window transparency. Verify the UniWindow ARM64 plug-in is present and the player is windowed.");

            Drain(backend.Initialize(null, null, 480));

            Assert.That(backend.IsReady, Is.False);
        }

        [Test]
        public void Initialize_MenuFails_LeavesRecoveryUnavailable()
        {
            var menu = new FakeMenuBridge { InitializeSucceeds = false };
            var backend = new MacOSDesktopWindowBackend(new FakeWindowFacade(), menu);

            LogAssert.Expect(
                LogType.Error,
                "Could not create the macOS Sheep Isle menu-bar item. The window will remain visible so it cannot become stranded.");

            Drain(backend.Initialize(null, null, 480));

            Assert.That(backend.IsReady, Is.True);
            Assert.That(backend.HasRecoverySurface, Is.False);
        }

        [Test]
        public void ConsumeActions_CombinedNativeFlags_ReturnsEveryFlag()
        {
            const DesktopWindowAction expected = DesktopWindowAction.Show
                | DesktopWindowAction.Hide
                | DesktopWindowAction.TogglePin
                | DesktopWindowAction.Quit;
            var menu = new FakeMenuBridge { Actions = expected };
            var backend = new MacOSDesktopWindowBackend(new FakeWindowFacade(), menu);
            Drain(backend.Initialize(null, null, 480));

            Assert.That(backend.ConsumeActions(), Is.EqualTo(expected));
        }

        [Test]
        public void TrySetVisible_ReturnsObservedNativeVisibility()
        {
            var menu = new FakeMenuBridge { VisibilityAfterRequest = false };
            var backend = new MacOSDesktopWindowBackend(new FakeWindowFacade(), menu);
            Drain(backend.Initialize(null, null, 480));

            Assert.That(backend.TrySetVisible(true), Is.False);
            Assert.That(backend.IsVisible, Is.False);
        }

        [Test]
        public void TrySetPinned_ReturnsObservedUniWindowState()
        {
            var window = new FakeWindowFacade { PinnedAfterRequest = false };
            var backend = new MacOSDesktopWindowBackend(window, new FakeMenuBridge());
            Drain(backend.Initialize(null, null, 480));

            Assert.That(backend.TrySetPinned(true), Is.False);
            Assert.That(backend.IsPinned, Is.False);
        }

        [Test]
        public void TrySetWindowPosition_ClampEnabled_ReturnsPostClampPosition()
        {
            var window = new FakeWindowFacade();
            var menu = new FakeMenuBridge();
            menu.ClampAction = () => window.WindowPosition = new Vector2(300f, 400f);
            var backend = new MacOSDesktopWindowBackend(window, menu);
            Drain(backend.Initialize(null, null, 480));

            Assert.That(
                backend.TrySetWindowPosition(new Vector2(5000f, -2000f), true),
                Is.True);
            Assert.That(backend.WindowPosition, Is.EqualTo(new Vector2(300f, 400f)));
        }

        [Test]
        public void Dispose_CalledTwice_DisposesEachBoundaryOnce()
        {
            var window = new FakeWindowFacade();
            var menu = new FakeMenuBridge();
            var backend = new MacOSDesktopWindowBackend(window, menu);

            backend.Dispose();
            backend.Dispose();

            Assert.That(window.DisposeCount, Is.EqualTo(1));
            Assert.That(menu.DisposeCount, Is.EqualTo(1));
        }

        private static void Drain(IEnumerator routine)
        {
            while (routine.MoveNext())
            {
            }
        }

        private sealed class FakeWindowFacade : IMacOSWindowFacade
        {
            public bool ConfigureSucceeds { get; set; } = true;
            public bool PositionSucceeds { get; set; } = true;
            public bool PinSucceeds { get; set; } = true;
            public bool PinnedAfterRequest { get; set; } = true;
            public int DisposeCount { get; private set; }

            public bool IsTransparent { get; private set; }
            public bool IsPinned { get; private set; }
            public Vector2 WindowPosition { get; set; }
            public Vector2 CursorPosition { get; set; }

            public bool Configure(Camera camera, float opacityThreshold)
            {
                IsTransparent = ConfigureSucceeds;
                return ConfigureSucceeds;
            }

            public bool TrySetPinned(bool requested)
            {
                IsPinned = PinnedAfterRequest;
                return PinSucceeds;
            }

            public bool TrySetWindowPosition(Vector2 requested)
            {
                if (PositionSucceeds)
                {
                    WindowPosition = requested;
                }

                return PositionSucceeds;
            }

            public void Dispose()
            {
                DisposeCount++;
            }
        }

        private sealed class FakeMenuBridge : IMacOSMenuBridge
        {
            public bool InitializeSucceeds { get; set; } = true;
            public bool VisibilityRequestSucceeds { get; set; } = true;
            public bool VisibilityAfterRequest { get; set; } = true;
            public bool ClampSucceeds { get; set; } = true;
            public DesktopWindowAction Actions { get; set; }
            public Action ClampAction { get; set; }
            public int DisposeCount { get; private set; }

            public bool Initialize()
            {
                return InitializeSucceeds;
            }

            public DesktopWindowAction ConsumeActions()
            {
                DesktopWindowAction actions = Actions;
                Actions = DesktopWindowAction.None;
                return actions;
            }

            public void SetMenuState(bool visible, bool pinned)
            {
            }

            public bool TrySetWindowVisible(bool requested)
            {
                return VisibilityRequestSucceeds;
            }

            public bool IsWindowVisible()
            {
                return VisibilityAfterRequest;
            }

            public bool ClampWindowToVisibleScreen()
            {
                ClampAction?.Invoke();
                return ClampSucceeds;
            }

            public void Dispose()
            {
                DisposeCount++;
            }
        }
    }
}
