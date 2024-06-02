using Interop.UIAutomationClient;

namespace UIADriver.uia3.attribute
{
    public class BasicElementPropertyValueGetter
    {
        public string? GetPropertyStrValue(IUIAutomationElement element, int propertyId)
        {
            var v = element.GetCachedPropertyValue(propertyId);
            if (v == null) return null;

            switch (propertyId)
            {
                case UIA_PropertyIds.UIA_CulturePropertyId:
                    return Utilities.GetLocalizeName((int)v);
                case UIA_PropertyIds.UIA_ControlTypePropertyId:
                    return Utilities.GetControlTypeString((int)v);
                case UIA_PropertyIds.UIA_LandmarkTypePropertyId:
                    return Utilities.GetLandmarkStr((int)v);
                case UIA_PropertyIds.UIA_LiveSettingPropertyId:
                    return ((LiveSetting)v).ToString();
                case UIA_PropertyIds.UIA_OrientationPropertyId:
                    return ((OrientationType)v).ToString().Replace("OrientationType_", "");
                case UIA_PropertyIds.UIA_VisualEffectsPropertyId:
                    return Utilities.GetVisualEffect((int)v);
                case UIA_PropertyIds.UIA_NativeWindowHandlePropertyId:
                    return (int)v == 0 ? null : v.ToString();
                case UIA_PropertyIds.UIA_RuntimeIdPropertyId:
                    return string.Join(",", (int[])v);
                case UIA_PropertyIds.UIA_DockDockPositionPropertyId:
                    return ((DockPosition)v).ToString().Replace("DockPosition_", "");
                case UIA_PropertyIds.UIA_ExpandCollapseExpandCollapseStatePropertyId:
                    return ((ExpandCollapseState)v).ToString().Replace("ExpandCollapseState_", "");
                case UIA_PropertyIds.UIA_ToggleToggleStatePropertyId:
                    return ((ToggleState)v).ToString().Replace("ToggleState_", "");
                case UIA_PropertyIds.UIA_TableRowOrColumnMajorPropertyId:
                    return ((RowOrColumnMajor)v).ToString().Replace("RowOrColumnMajor_", "");
                case UIA_PropertyIds.UIA_WindowWindowInteractionStatePropertyId:
                    return ((WindowInteractionState)v).ToString().Replace("WindowInteractionState_", "");
                case UIA_PropertyIds.UIA_WindowWindowVisualStatePropertyId:
                    return ((WindowVisualState)v).ToString().Replace("WindowVisualState_", "");

            }
            if (v is bool vb)
            {
                if (!vb) return null;
            }
            return v.ToString()?.Normalize();
        }

    }
}
