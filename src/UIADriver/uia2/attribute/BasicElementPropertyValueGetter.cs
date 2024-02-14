
using System.Globalization;
using System.Windows.Automation;

namespace UIADriver.uia2.attribute
{
    public class BasicElementPropertyValueGetter
    {
        public string? GetPropertyStrValue(AutomationElement element, AutomationProperty propertyId)
        {
            var v = element.GetCachedPropertyValue(propertyId);
            if (v == null) return null;

            var cultureId = AutomationElement.CultureProperty.Id;
            var controlTypeId = AutomationElement.ControlTypeProperty.Id;
            var orientationId = AutomationElement.OrientationProperty.Id;
            var nativeWindowHandleId = AutomationElement.NativeWindowHandleProperty.Id;
            var runtimeId = AutomationElement.RuntimeIdProperty.Id;

            if (propertyId.Id == cultureId)
            {
                return ((CultureInfo)v).Name;
            } else if (propertyId.Id == controlTypeId)
            {
                return Utilities.GetControlTypeString(((ControlType)v).Id);
            } else if (propertyId.Id == orientationId)
            {
                return ((OrientationType)v).ToString();
            } else if (propertyId.Id == nativeWindowHandleId)
            {
                return (int)v == 0 ? null : v.ToString();
            } else if (propertyId.Id == runtimeId)
            {
                return string.Join(",", (int[])v);
            }

            if (v is bool vb)
            {
                if (!vb) return null;
            }
            return v.ToString()?.Normalize();
        }

    }
}
