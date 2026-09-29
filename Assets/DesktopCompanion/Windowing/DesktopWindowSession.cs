using System;
using UnityEngine;

namespace SheepIsle.DesktopWindowing
{
    public sealed class DesktopWindowSession : IDisposable
    {
        private readonly IDesktopWindowBackend backend;
        private readonly IDesktopWindowSettings settings;
        private readonly Action<bool> visibilityChanged;
        private Vector2 dragCursorStart;
        private Vector2 dragWindowStart;
        private bool disposed;

        public bool IsVisible { get; private set; }
        public bool IsPinned { get; private set; }
        public bool IsMoving { get; private set; }

        public DesktopWindowSession(
            IDesktopWindowBackend backend,
            IDesktopWindowSettings settings,
            Action<bool> visibilityChanged)
        {
            this.backend = backend ?? throw new ArgumentNullException(nameof(backend));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.visibilityChanged = visibilityChanged;
            IsVisible = backend.IsVisible;
            IsPinned = backend.IsPinned;
        }

        public void Restore()
        {
            if (settings.TryLoadPosition(out var savedPosition) &&
                backend.TrySetWindowPosition(savedPosition, true))
            {
                settings.SavePosition(backend.WindowPosition);
            }

            bool requestedPin = settings.LoadPinned();
            bool pinApplied = backend.TrySetPinned(requestedPin);
            IsPinned = backend.IsPinned;
            if (pinApplied && IsPinned == requestedPin)
            {
                settings.SavePinned(IsPinned);
            }

            IsVisible = backend.IsVisible;
            UpdateRecoverySurface();
        }

        public bool ProcessActions(DesktopWindowAction actions)
        {
            if ((actions & DesktopWindowAction.Quit) != 0)
            {
                SavePosition();
                return true;
            }

            if ((actions & DesktopWindowAction.Show) != 0)
            {
                SetVisible(true);
            }
            else if ((actions & DesktopWindowAction.Hide) != 0)
            {
                SetVisible(false);
            }

            if ((actions & DesktopWindowAction.TogglePin) != 0)
            {
                TogglePin();
            }

            return false;
        }

        public bool SetVisible(bool requested)
        {
            if (!requested && !backend.HasRecoverySurface)
            {
                return false;
            }

            bool previous = IsVisible;
            bool operationSucceeded = backend.TrySetVisible(requested);
            IsVisible = backend.IsVisible;

            if (IsVisible != previous)
            {
                if (!IsVisible)
                {
                    SavePosition();
                    IsMoving = false;
                }

                visibilityChanged?.Invoke(IsVisible);
            }

            UpdateRecoverySurface();
            return operationSucceeded && IsVisible == requested;
        }

        public bool TogglePin()
        {
            bool requested = !IsPinned;
            bool operationSucceeded = backend.TrySetPinned(requested);
            IsPinned = backend.IsPinned;
            bool applied = operationSucceeded && IsPinned == requested;
            if (applied)
            {
                settings.SavePinned(IsPinned);
            }

            UpdateRecoverySurface();
            return applied;
        }

        public void BeginMove(DesktopWindowDragInput input)
        {
            if (input == DesktopWindowDragInput.None)
            {
                return;
            }

            dragCursorStart = backend.CursorPosition;
            dragWindowStart = backend.WindowPosition;
            IsMoving = true;
        }

        public void UpdateMove(bool held)
        {
            if (!IsMoving)
            {
                return;
            }

            if (!held)
            {
                IsMoving = false;
                SavePosition();
                return;
            }

            Vector2 requested = dragWindowStart + backend.CursorPosition - dragCursorStart;
            backend.TrySetWindowPosition(requested, false);
        }

        public void SavePosition()
        {
            settings.SavePosition(backend.WindowPosition);
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            SavePosition();
            backend.Dispose();
        }

        private void UpdateRecoverySurface()
        {
            backend.SetRecoverySurfaceState(IsVisible, IsPinned);
        }
    }
}
