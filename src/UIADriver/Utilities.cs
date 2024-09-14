using static UIADriver.win32native.Win32Struct;
using System.Runtime.InteropServices;
using UIADriver.win32native;
using System.Text;
using Interop.UIAutomationClient;
using System.Globalization;

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

        public static string? GetLocalizeName(int localizeId)
        {
            if (localizeId != 0)
            {
                try
                {
                    var culture = new CultureInfo(localizeId);
                    return culture.Name;
                } catch { }
            }
            return null;
        }

        public static string GetLocalizedStateText(int state)
        {
            var stateBuilder = new StringBuilder();
            var length = Win32Methods.GetStateText(Convert.ToUInt32(state), null, 0);
            Win32Methods.GetStateText(Convert.ToUInt32(state), stateBuilder, length + 1);
            return stateBuilder.ToString();
        }

        public static string GetLocalizedRoleText(int role)
        {
            var roleBuilder = new StringBuilder();
            var length = Win32Methods.GetRoleText(Convert.ToUInt32(role), null, 0);
            Win32Methods.GetRoleText(Convert.ToUInt32(role), roleBuilder, length + 1);
            return roleBuilder.ToString();
        }

        public static void BringWindowToTop(nint hwnd)
        {
            Win32Methods.SetWindowPos(hwnd, -2, 0, 0, 0, 0, 0x0002 | 0x0001);
            Win32Methods.SetWindowPos(hwnd, -1, 0, 0, 0, 0, 0x0002 | 0x0001);
        }
    }
}
