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
                case UIA_PropertyIds.UIA_DragIsGrabbedPropertyId:
                case UIA_PropertyIds.UIA_RangeValueIsReadOnlyPropertyId:
                case UIA_PropertyIds.UIA_ScrollHorizontallyScrollablePropertyId:
                case UIA_PropertyIds.UIA_ScrollVerticallyScrollablePropertyId:
                case UIA_PropertyIds.UIA_SelectionItemIsSelectedPropertyId:
                case UIA_PropertyIds.UIA_SelectionCanSelectMultiplePropertyId:
                case UIA_PropertyIds.UIA_SelectionIsSelectionRequiredPropertyId:
                case UIA_PropertyIds.UIA_Transform2CanZoomPropertyId:
                case UIA_PropertyIds.UIA_TransformCanMovePropertyId:
                case UIA_PropertyIds.UIA_TransformCanResizePropertyId:
                case UIA_PropertyIds.UIA_TransformCanRotatePropertyId:
                case UIA_PropertyIds.UIA_ValueIsReadOnlyPropertyId:
                case UIA_PropertyIds.UIA_WindowCanMaximizePropertyId:
                case UIA_PropertyIds.UIA_WindowCanMinimizePropertyId:
                case UIA_PropertyIds.UIA_WindowIsTopmostPropertyId:
                case UIA_PropertyIds.UIA_WindowIsModalPropertyId:
                case UIA_PropertyIds.UIA_IsContentElementPropertyId:
                case UIA_PropertyIds.UIA_IsControlElementPropertyId:
                case UIA_PropertyIds.UIA_IsDataValidForFormPropertyId:
                case UIA_PropertyIds.UIA_IsDialogPropertyId:
                case UIA_PropertyIds.UIA_IsEnabledPropertyId:
                case UIA_PropertyIds.UIA_IsKeyboardFocusablePropertyId:
                case UIA_PropertyIds.UIA_IsOffscreenPropertyId:
                case UIA_PropertyIds.UIA_IsPasswordPropertyId:
                case UIA_PropertyIds.UIA_IsPeripheralPropertyId:
                case UIA_PropertyIds.UIA_IsRequiredForFormPropertyId:
                case UIA_PropertyIds.UIA_HasKeyboardFocusPropertyId:
                case UIA_PropertyIds.UIA_IsDockPatternAvailablePropertyId:
                case UIA_PropertyIds.UIA_IsExpandCollapsePatternAvailablePropertyId:
                case UIA_PropertyIds.UIA_IsGridItemPatternAvailablePropertyId:
                case UIA_PropertyIds.UIA_IsGridPatternAvailablePropertyId:
                case UIA_PropertyIds.UIA_IsInvokePatternAvailablePropertyId:
                case UIA_PropertyIds.UIA_IsMultipleViewPatternAvailablePropertyId:
                case UIA_PropertyIds.UIA_IsRangeValuePatternAvailablePropertyId:
                case UIA_PropertyIds.UIA_IsScrollPatternAvailablePropertyId:
                case UIA_PropertyIds.UIA_IsScrollItemPatternAvailablePropertyId:
                case UIA_PropertyIds.UIA_IsSelectionItemPatternAvailablePropertyId:
                case UIA_PropertyIds.UIA_IsSelectionPatternAvailablePropertyId:
                case UIA_PropertyIds.UIA_IsTablePatternAvailablePropertyId:
                case UIA_PropertyIds.UIA_IsTableItemPatternAvailablePropertyId:
                case UIA_PropertyIds.UIA_IsTextPatternAvailablePropertyId:
                case UIA_PropertyIds.UIA_IsTogglePatternAvailablePropertyId:
                case UIA_PropertyIds.UIA_IsTransformPatternAvailablePropertyId:
                case UIA_PropertyIds.UIA_IsValuePatternAvailablePropertyId:
                case UIA_PropertyIds.UIA_IsWindowPatternAvailablePropertyId:
                case UIA_PropertyIds.UIA_IsLegacyIAccessiblePatternAvailablePropertyId:
                case UIA_PropertyIds.UIA_IsItemContainerPatternAvailablePropertyId:
                case UIA_PropertyIds.UIA_IsVirtualizedItemPatternAvailablePropertyId:
                case UIA_PropertyIds.UIA_IsSynchronizedInputPatternAvailablePropertyId:
                case UIA_PropertyIds.UIA_IsObjectModelPatternAvailablePropertyId:
                case UIA_PropertyIds.UIA_IsAnnotationPatternAvailablePropertyId:
                case UIA_PropertyIds.UIA_IsTextPattern2AvailablePropertyId:
                case UIA_PropertyIds.UIA_IsStylesPatternAvailablePropertyId:
                case UIA_PropertyIds.UIA_IsSpreadsheetPatternAvailablePropertyId:
                case UIA_PropertyIds.UIA_IsSpreadsheetItemPatternAvailablePropertyId:
                case UIA_PropertyIds.UIA_IsTransformPattern2AvailablePropertyId:
                case UIA_PropertyIds.UIA_IsTextChildPatternAvailablePropertyId:
                case UIA_PropertyIds.UIA_IsDragPatternAvailablePropertyId:
                case UIA_PropertyIds.UIA_IsDropTargetPatternAvailablePropertyId:
                case UIA_PropertyIds.UIA_IsTextEditPatternAvailablePropertyId:
                case UIA_PropertyIds.UIA_IsCustomNavigationPatternAvailablePropertyId:
                case UIA_PropertyIds.UIA_IsSelectionPattern2AvailablePropertyId:
                    v = (bool)v;
                    break;

            }
            if (v is bool vb)
            {
                if (!vb) return null;
            }
            return v.ToString();
        }

    }
}
