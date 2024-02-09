using System.Runtime.InteropServices;
using static UIADriver.win32native.Win32Enum;

namespace UIADriver.win32native
{
    public class Win32Struct
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public MOUSEEVENTF dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public KEYEVENTF dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct InputUnion
        {
            [FieldOffset(0)]
            public MOUSEINPUT mi;
            [FieldOffset(0)]
            public KEYBDINPUT ki;
            [FieldOffset(0)]
            public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT
        {
            public InputType type;
            public InputUnion U;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINTER_INFO
        {
            public POINTER_INPUT_TYPE pointerType;
            public uint pointerId;
            public uint frameId;
            public POINTER_FLAGS pointerFlags;
            public IntPtr sourceDevice;
            public IntPtr hwndTarget;
            public POINT ptPixelLocation;
            public POINT ptPixelLocationRaw;
            public POINT ptHimetricLocation;
            public POINT ptHimetricLocationRaw;
            public uint dwTime;
            public uint historyCount;
            public uint inputData;
            public uint dwKeyStates;
            public ulong PerformanceCount;
            public POINTER_BUTTON_CHANGE_TYPE ButtonChangeType;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINTER_TOUCH_INFO
        {
            public POINTER_INFO pointerInfo;
            public TOUCH_FLAGS touchFlags;
            public TOUCH_MASK touchMask;
            public RECT rcContact;
            public RECT rcContactRaw;
            public uint orientation;
            public uint pressure;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINTER_PEN_INFO
        {
            public POINTER_INFO pointerInfo;
            public PEN_FLAGS penFlags;
            public PEN_MASK penMask;
            public uint pressure;
            public uint rotation;
            public int tiltX;
            public int tiltY;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct POINTER_TYPE_INFO_UNION
        {
            [FieldOffset(0)]
            public POINTER_TOUCH_INFO touchInfo;
            [FieldOffset(0)]
            public POINTER_PEN_INFO penInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINTER_TYPE_INFO
        {
            public POINTER_INPUT_TYPE type;
            public POINTER_TYPE_INFO_UNION U;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MonitorInfo
        {
            public uint size;
            public RECT monitor;
            public RECT work;
            public uint flags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WindowInfo
        {
            public uint size;
            public RECT rcWindow;
            public RECT rcClient;
            public uint dwStyle;
            public uint dwExStyle;
            public uint dwWindowStatus;
            public uint cxWindowBorders;
            public uint cyWindowBorders;
            public ushort atomWindowType;
            public ushort wCreatorVersion;
        }
    }
}
