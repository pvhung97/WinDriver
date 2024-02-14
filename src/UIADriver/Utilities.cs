﻿using static UIADriver.win32native.Win32Struct;
using System.Runtime.InteropServices;
using UIADriver.win32native;
using System.Text;

namespace UIADriver
{
    public class Utilities
    {
        public static string GetControlTypeString(int controlTypeId)
        {
            return controlTypeId switch
            {
                50040 => "AppBar",
                50000 => "Button",
                50001 => "Calendar",
                50002 => "CheckBox",
                50003 => "ComboBox",
                50025 => "Custom",
                50028 => "DataGrid",
                50029 => "DataItem",
                50030 => "Document",
                50004 => "Edit",
                50026 => "Group",
                50034 => "Header",
                50035 => "HeaderItem",
                50005 => "Hyperlink",
                50006 => "Image",
                50008 => "List",
                50007 => "ListItem",
                50010 => "MenuBar",
                50009 => "Menu",
                50011 => "MenuItem",
                50033 => "Pane",
                50012 => "ProgressBar",
                50013 => "RadioButton",
                50014 => "ScrollBar",
                50039 => "SemanticZoom",
                50038 => "Separator",
                50015 => "Slider",
                50016 => "Spinner",
                50031 => "SplitButton",
                50017 => "StatusBar",
                50018 => "Tab",
                50019 => "TabItem",
                50036 => "Table",
                50020 => "Text",
                50027 => "Thumb",
                50037 => "TitleBar",
                50021 => "ToolBar",
                50022 => "ToolTip",
                50023 => "Tree",
                50024 => "TreeItem",
                50032 => "Window",
                _ => "Unknown",
            };
        }

        public static string? GetLandmarkStr(int landmarkId)
        {
            return landmarkId switch
            {
                80000 => "CustomLandmark",
                80001 => "FormLandmark",
                80002 => "MainLandmark",
                80003 => "NavigationLandmark",
                80004 => "SearchLandmark",
                _ => null,
            };
        }

        public static string? GetVisualEffect(int effectId)
        {
            return effectId switch
            {
                0x1 => "Shadow",
                0x2 => "Reflection",
                0x4 => "Glow",
                0x8 => "SoftEdges",
                0x10 => "Bevel",
                _ => null,
            };
        }

        public static Point getPrimaryMonitorAbsolutePixelCoordinate()
        {
            int minX = 0;
            int minY = 0;
            Win32Methods.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, MonitorDelegate, IntPtr.Zero);
            bool MonitorDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData)
            {
                var mi = new MonitorInfo();
                mi.size = (uint)Marshal.SizeOf(mi);
                var success = Win32Methods.GetMonitorInfo(hMonitor, ref mi);
                if (mi.monitor.left < minX)
                {
                    minX = mi.monitor.left;
                }
                if (mi.monitor.top < minY)
                {
                    minY = mi.monitor.top;
                }
                return true;
            }
            return new Point(Math.Abs(minX), Math.Abs(minY));
        }

        public static string GetLocalizeName(int localizeId)
        {
            var data = new StringBuilder(500);
            Win32Methods.LCIDToLocaleName(Convert.ToUInt32(localizeId), data, data.Capacity, 0);
            return data.ToString();
        }

        public static void BringWindowToTop(nint hwnd)
        {
            var wi = new WindowInfo();
            wi.size = (uint)Marshal.SizeOf(wi);
            Win32Methods.GetWindowInfo(hwnd, ref wi);
            var isTopMostWnd = (wi.dwExStyle & 0x00000008) == 0x00000008;
            if (!isTopMostWnd)
            {
                Win32Methods.SetWindowPos(hwnd, -1, 0, 0, 0, 0, 0x0002 | 0x0001 | 0x0040);  //  Make it a top most window
                Win32Methods.SetWindowPos(hwnd, -2, 0, 0, 0, 0, 0x0002 | 0x0001);  //  Then remove top most style
            }
            Win32Methods.SetForegroundWindow(hwnd);
        }
    }
}
