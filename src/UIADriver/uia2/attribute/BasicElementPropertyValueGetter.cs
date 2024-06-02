
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

            if (propertyId.Id == AutomationElement.CultureProperty.Id)
            {
                return ((CultureInfo)v).Name;
            } else if (propertyId.Id == AutomationElement.ControlTypeProperty.Id)
            {
                return Utilities.GetControlTypeString(((ControlType)v).Id);
            } else if (propertyId.Id == AutomationElement.OrientationProperty.Id)
            {
                return ((OrientationType)v).ToString();
            } else if (propertyId.Id == AutomationElement.NativeWindowHandleProperty.Id)
            {
                return (int)v == 0 ? null : v.ToString();
            } else if (propertyId.Id == AutomationElement.RuntimeIdProperty.Id)
            {
                return string.Join(",", (int[])v);
            } else if (propertyId.Id == DockPattern.DockPositionProperty.Id)
            {
                return ((DockPosition)v).ToString();
            } else if (propertyId.Id == ExpandCollapsePattern.ExpandCollapseStateProperty.Id)
            {
                return ((ExpandCollapseState)v).ToString();
            } else if (propertyId.Id == TogglePattern.ToggleStateProperty.Id)
            {
                return ((ToggleState)v).ToString();
            } else if (propertyId.Id == TablePattern.RowOrColumnMajorProperty.Id)
            {
                return ((RowOrColumnMajor)v).ToString();
            } else if (propertyId.Id == WindowPattern.WindowInteractionStateProperty.Id)
            {
                return ((WindowInteractionState)v).ToString();
            } else if (propertyId.Id == WindowPattern.WindowVisualStateProperty.Id)
            {
                return ((WindowVisualState)v).ToString();
            }

            if (v is bool vb)
            {
                if (!vb) return null;
            }
            return v.ToString()?.Normalize();
        }

    }
}
