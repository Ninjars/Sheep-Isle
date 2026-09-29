using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace SheepIsle.DesktopWindowing
{
    public sealed class WindowsDesktopTray : IDisposable
    {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        private const int WindowProcedureIndex = -4;
        private const uint CallbackMessage = 0x8001;
        private const uint LeftButtonUp = 0x0202;
        private const uint RightButtonUp = 0x0205;
        private const uint ContextMenu = 0x007B;
        private const uint NullMessage = 0x0000;
        private const uint AddIcon = 0;
        private const uint DeleteIcon = 2;
        private const uint IconMessage = 1;
        private const uint IconImage = 2;
        private const uint IconTip = 4;
        private const uint MenuString = 0;
        private const uint MenuSeparator = 0x0800;
        private const uint MenuRightButton = 0x0002;
        private const uint MenuReturnCommand = 0x0100;
        private const uint ShowCommand = 1;
        private const uint HideCommand = 2;
        private const uint ExitCommand = 3;
        private const int ApplicationIcon = 32512;
        private const uint IconId = 1;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct NotifyIconData
        {
            public uint Size;
            public IntPtr Window;
            public uint Id;
            public uint Flags;
            public uint Callback;
            public IntPtr Icon;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string Tip;
            public uint State;
            public uint StateMask;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)] public string Info;
            public uint Version;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)] public string InfoTitle;
            public uint InfoFlags;
            public Guid Guid;
            public IntPtr BalloonIcon;
        }

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        private delegate IntPtr WindowProcedure(IntPtr hwnd, uint message, IntPtr wParam, IntPtr lParam);

        [DllImport("shell32.dll", EntryPoint = "Shell_NotifyIconW", SetLastError = true)]
        private static extern bool ShellNotifyIcon(uint message, ref NotifyIconData data);
        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW", SetLastError = true)]
        private static extern IntPtr SetWindowLongPtr(IntPtr hwnd, int index, IntPtr value);
        [DllImport("user32.dll", EntryPoint = "CallWindowProcW")]
        private static extern IntPtr CallWindowProc(IntPtr previous, IntPtr hwnd, uint message, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern uint RegisterWindowMessage(string name);
        [DllImport("user32.dll")]
        private static extern IntPtr LoadIcon(IntPtr instance, IntPtr icon);
        [DllImport("user32.dll")]
        private static extern IntPtr CreatePopupMenu();
        [DllImport("user32.dll", EntryPoint = "AppendMenuW", CharSet = CharSet.Unicode)]
        private static extern bool AppendMenu(IntPtr menu, uint flags, uint id, string text);
        [DllImport("user32.dll")]
        private static extern int TrackPopupMenu(IntPtr menu, uint flags, int x, int y,
            int reserved, IntPtr hwnd, IntPtr rectangle);
        [DllImport("user32.dll")]
        private static extern bool DestroyMenu(IntPtr menu);
        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out Point point);
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hwnd);
        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hwnd, uint message, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        private struct Point
        {
            public int X;
            public int Y;
        }

        private readonly IntPtr window;
        private readonly NotifyIconData icon;
        private readonly WindowProcedure callback;
        private readonly IntPtr oldProcedure;
        private readonly uint taskbarCreated;
        private bool disposed;
        private bool visible = true;
        private DesktopWindowAction pendingActions;

        public bool IsReady { get; }

        public WindowsDesktopTray(IntPtr windowHandle)
        {
            window = windowHandle;
            taskbarCreated = RegisterWindowMessage("TaskbarCreated");
            callback = HandleMessage;
            oldProcedure = SetWindowLongPtr(window, WindowProcedureIndex,
                Marshal.GetFunctionPointerForDelegate(callback));
            if (oldProcedure == IntPtr.Zero)
            {
                Debug.LogError("Could not attach the desktop tray callback: " +
                    Marshal.GetLastWin32Error());
                return;
            }

            icon = new NotifyIconData
            {
                Size = (uint)Marshal.SizeOf(typeof(NotifyIconData)),
                Window = window,
                Id = IconId,
                Flags = IconMessage | IconImage | IconTip,
                Callback = CallbackMessage,
                Icon = LoadIcon(IntPtr.Zero, new IntPtr(ApplicationIcon)),
                Tip = "Sheep Isle"
            };
            IsReady = ShellNotifyIcon(AddIcon, ref icon);
            if (!IsReady)
            {
                Debug.LogError("Could not create the Sheep Isle tray icon: " +
                    Marshal.GetLastWin32Error());
            }
        }

        public DesktopWindowAction ConsumeActions()
        {
            DesktopWindowAction actions = pendingActions;
            pendingActions = DesktopWindowAction.None;
            return actions;
        }

        public void SetVisible(bool isVisible)
        {
            visible = isVisible;
        }

        private IntPtr HandleMessage(IntPtr hwnd, uint message, IntPtr wParam, IntPtr lParam)
        {
            if (message == taskbarCreated && IsReady)
            {
                var data = icon;
                ShellNotifyIcon(AddIcon, ref data);
            }
            else if (message == CallbackMessage)
            {
                uint action = unchecked((uint)lParam.ToInt64());
                if (action == LeftButtonUp)
                {
                    if (!visible)
                    {
                        pendingActions |= DesktopWindowAction.Show;
                    }
                }
                else if (action == RightButtonUp || action == ContextMenu)
                {
                    ShowMenu();
                }

                return IntPtr.Zero;
            }

            return CallWindowProc(oldProcedure, hwnd, message, wParam, lParam);
        }

        private void ShowMenu()
        {
            IntPtr menu = CreatePopupMenu();
            if (menu == IntPtr.Zero)
            {
                return;
            }

            if (!GetCursorPos(out var cursor))
            {
                DestroyMenu(menu);
                return;
            }

            AppendMenu(menu, MenuString, visible ? HideCommand : ShowCommand,
                visible ? "Hide Sheep Isle" : "Show Sheep Isle");
            AppendMenu(menu, MenuSeparator, 0, null);
            AppendMenu(menu, MenuString, ExitCommand, "Exit");
            SetForegroundWindow(window);
            int selected = TrackPopupMenu(menu, MenuRightButton | MenuReturnCommand,
                cursor.X, cursor.Y, 0, window, IntPtr.Zero);
            if (selected == ShowCommand)
            {
                pendingActions |= DesktopWindowAction.Show;
            }
            else if (selected == HideCommand)
            {
                pendingActions |= DesktopWindowAction.Hide;
            }
            else if (selected == ExitCommand)
            {
                pendingActions |= DesktopWindowAction.Quit;
            }

            DestroyMenu(menu);
            PostMessage(window, NullMessage, IntPtr.Zero, IntPtr.Zero);
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            if (IsReady)
            {
                var data = icon;
                ShellNotifyIcon(DeleteIcon, ref data);
            }

            if (oldProcedure != IntPtr.Zero)
            {
                SetWindowLongPtr(window, WindowProcedureIndex, oldProcedure);
            }
        }
#else
        public bool IsReady => false;

        public WindowsDesktopTray(IntPtr windowHandle)
        {
        }

        public DesktopWindowAction ConsumeActions()
        {
            return DesktopWindowAction.None;
        }

        public void SetVisible(bool isVisible)
        {
        }

        public void Dispose()
        {
        }
#endif
    }
}
