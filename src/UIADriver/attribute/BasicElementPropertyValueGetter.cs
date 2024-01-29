using Interop.UIAutomationClient;
using System.Text;
using UIA3Driver.win32native;

namespace UIA3Driver.attribute
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
                    return GetLocalizeName((int)v);
                case UIA_PropertyIds.UIA_ControlTypePropertyId:
                    return Utilities.GetControlTypeString((int)v);
                case UIA_PropertyIds.UIA_LandmarkTypePropertyId:
                    return Utilities.GetLandmarkStr((int)v);
                case UIA_PropertyIds.UIA_LiveSettingPropertyId:
                    return ((LiveSetting)v).ToString();
                case UIA_PropertyIds.UIA_OrientationPropertyId:
                    return ((OrientationType)v).ToString();
                case UIA_PropertyIds.UIA_VisualEffectsPropertyId:
                    return Utilities.GetVisualEffect((int)v);
                case UIA_PropertyIds.UIA_NativeWindowHandlePropertyId:
                    return (int)v == 0 ? null : v.ToString();
                case UIA_PropertyIds.UIA_RuntimeIdPropertyId:
                    return string.Join(",", (int[])v);

            }
            if (v is bool vb)
            {
                if (!vb) return null;
            } 
            return v.ToString().Normalize();
        }

        private string GetLocalizeName(int localizeId)
        {
            var data = new StringBuilder(500);
            Win32Methods.LCIDToLocaleName(Convert.ToUInt32(localizeId), data, data.Capacity, 0);
            return data.ToString();
        }

    }
}
