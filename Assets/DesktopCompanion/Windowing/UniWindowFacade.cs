using Kirurobo;
using UnityEngine;

namespace SheepIsle.DesktopWindowing
{
    public sealed class UniWindowFacade : IMacOSWindowFacade
    {
        private UniWindowController controller;
        private readonly bool ownsController;
        private bool disposed;

        public UniWindowFacade(GameObject host)
        {
            controller = host.GetComponent<UniWindowController>();
            if (controller == null)
            {
                controller = host.AddComponent<UniWindowController>();
                ownsController = true;
            }
        }

        public bool IsTransparent => controller != null
            && controller.isTransparent
            && controller.isFreePositioningEnabled;
        public bool IsPinned => controller != null && controller.isTopmost;
        public Vector2 WindowPosition => controller != null ? controller.windowPosition : Vector2.zero;
        public Vector2 CursorPosition => controller != null ? controller.cursorPosition : Vector2.zero;

        public bool Configure(Camera camera, float opacityThreshold)
        {
            if (controller == null)
            {
                return false;
            }

            controller.currentCamera = camera;
            controller.transparentType = UniWindowController.TransparentType.Alpha;
            controller.hitTestType = UniWindowController.HitTestType.Opacity;
            controller.opacityThreshold = opacityThreshold;
            controller.autoSwitchCameraBackground = true;
            controller.forceWindowed = true;
            controller.isHitTestEnabled = true;
            controller.isFreePositioningEnabled = true;
            controller.isTransparent = true;
            return IsTransparent;
        }

        public bool TrySetPinned(bool requested)
        {
            if (controller == null)
            {
                return false;
            }

            controller.isTopmost = requested;
            return controller.isTopmost == requested;
        }

        public bool TrySetWindowPosition(Vector2 requested)
        {
            if (controller == null)
            {
                return false;
            }

            controller.windowPosition = requested;
            return Vector2.SqrMagnitude(controller.windowPosition - requested) <= 1f;
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            if (ownsController && controller != null)
            {
                Object.Destroy(controller);
            }

            controller = null;
        }
    }
}
