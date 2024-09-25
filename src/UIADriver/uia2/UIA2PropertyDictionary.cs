using System.Windows.Automation;

namespace UIADriver.uia2
{
    public class UIA2PropertyDictionary
    {
        public static readonly Dictionary<string, int> propertyDictionary = new()
        {
            { "AcceleratorKey", AutomationElement.AcceleratorKeyProperty.Id },
            { "IsSelectionItemPatternAvailable", AutomationElement.IsSelectionItemPatternAvailableProperty.Id },
            { "IsSelectionPatternAvailable", AutomationElement.IsSelectionPatternAvailableProperty.Id },
            { "IsSynchronizedInputPatternAvailable", AutomationElement.IsSynchronizedInputPatternAvailableProperty.Id },
            { "IsTableItemPatternAvailable", AutomationElement.IsTableItemPatternAvailableProperty.Id },
            { "IsTablePatternAvailable", AutomationElement.IsTablePatternAvailableProperty.Id },
            { "IsTextPatternAvailable", AutomationElement.IsTextPatternAvailableProperty.Id },
            { "IsTogglePatternAvailable", AutomationElement.IsTogglePatternAvailableProperty.Id },
            { "IsTransformPatternAvailable", AutomationElement.IsTransformPatternAvailableProperty.Id },
            { "IsValuePatternAvailable", AutomationElement.IsValuePatternAvailableProperty.Id },
            { "IsVirtualizedItemPatternAvailable", AutomationElement.IsVirtualizedItemPatternAvailableProperty.Id },
            { "IsWindowPatternAvailable", AutomationElement.IsWindowPatternAvailableProperty.Id },
            { "ItemStatus", AutomationElement.ItemStatusProperty.Id },
            { "ItemType", AutomationElement.ItemTypeProperty.Id },
            { "LocalizedControlType", AutomationElement.LocalizedControlTypeProperty.Id },
            { "Name", AutomationElement.NameProperty.Id },
            { "NativeWindowHandle", AutomationElement.NativeWindowHandleProperty.Id },
            { "Orientation", AutomationElement.OrientationProperty.Id },
            { "PositionInSet", AutomationElement.PositionInSetProperty.Id },
            { "ProcessId", AutomationElement.ProcessIdProperty.Id },
            { "RuntimeId", AutomationElement.RuntimeIdProperty.Id },
            { "SizeOfSet", AutomationElement.SizeOfSetProperty.Id },
            { "IsScrollPatternAvailable", AutomationElement.IsScrollPatternAvailableProperty.Id },
            { "IsScrollItemPatternAvailable", AutomationElement.IsScrollItemPatternAvailableProperty.Id },
            { "LabeledBy", AutomationElement.LabeledByProperty.Id },
            { "IsRangeValuePatternAvailable", AutomationElement.IsRangeValuePatternAvailableProperty.Id },
            { "IsRequiredForForm", AutomationElement.IsRequiredForFormProperty.Id },
            { "AccessKey", AutomationElement.AccessKeyProperty.Id },
            { "AutomationId", AutomationElement.AutomationIdProperty.Id },
            { "BoundingRectangle", AutomationElement.BoundingRectangleProperty.Id },
            { "ClassName", AutomationElement.ClassNameProperty.Id },
            { "ClickablePoint", AutomationElement.ClickablePointProperty.Id },
            { "ControlType", AutomationElement.ControlTypeProperty.Id },
            { "FrameworkId", AutomationElement.FrameworkIdProperty.Id },
            { "HasKeyboardFocus", AutomationElement.HasKeyboardFocusProperty.Id },
            { "HeadingLevel", AutomationElement.HeadingLevelProperty.Id },
            { "HelpText", AutomationElement.HelpTextProperty.Id },
            { "Culture", AutomationElement.CultureProperty.Id },
            { "IsKeyboardFocusable", AutomationElement.IsKeyboardFocusableProperty.Id },
            { "IsControlElement", AutomationElement.IsControlElementProperty.Id },
            { "IsDialog", AutomationElement.IsDialogProperty.Id },
            { "IsDockPatternAvailable", AutomationElement.IsDockPatternAvailableProperty.Id },
            { "IsEnabled", AutomationElement.IsEnabledProperty.Id },
            { "IsExpandCollapsePatternAvailable", AutomationElement.IsExpandCollapsePatternAvailableProperty.Id },
            { "IsGridItemPatternAvailable", AutomationElement.IsGridItemPatternAvailableProperty.Id },
            { "IsGridPatternAvailable", AutomationElement.IsGridPatternAvailableProperty.Id },
            { "IsInvokePatternAvailable", AutomationElement.IsInvokePatternAvailableProperty.Id },
            { "IsItemContainerPatternAvailable", AutomationElement.IsItemContainerPatternAvailableProperty.Id },
            { "IsContentElement", AutomationElement.IsContentElementProperty.Id },
            { "IsMultipleViewPatternAvailable", AutomationElement.IsMultipleViewPatternAvailableProperty.Id },
            { "IsOffscreen", AutomationElement.IsOffscreenProperty.Id },
            { "IsPassword", AutomationElement.IsPasswordProperty.Id },

            { "SelectionItem_IsSelected", SelectionItemPattern.IsSelectedProperty.Id },
            { "SelectionItem_SelectionContainer", SelectionItemPattern.SelectionContainerProperty.Id },

            { "Selection_CanSelectMultiple", SelectionPattern.CanSelectMultipleProperty.Id },
            { "Selection_IsSelectionRequired", SelectionPattern.IsSelectionRequiredProperty.Id },
            { "Selection_Selection", SelectionPattern.SelectionProperty.Id },

            { "TableItem_ColumnHeaderItems", TableItemPattern.ColumnHeaderItemsProperty.Id },
            { "TableItem_RowHeaderItems", TableItemPattern.RowHeaderItemsProperty.Id },

            { "Table_ColumnHeaders", TablePattern.ColumnHeadersProperty.Id },
            { "Table_RowHeaders", TablePattern.RowHeadersProperty.Id },
            { "Table_RowOrColumnMajor", TablePattern.RowOrColumnMajorProperty.Id },

            { "Toggle_ToggleState", TogglePattern.ToggleStateProperty.Id },

            { "Transform_CanMove", TransformPattern.CanMoveProperty.Id },
            { "Transform_CanResize", TransformPattern.CanResizeProperty.Id },
            { "Transform_CanRotate", TransformPattern.CanRotateProperty.Id },

            { "Value_IsReadOnly", ValuePattern.IsReadOnlyProperty.Id },
            { "Value_Value", ValuePattern.ValueProperty.Id },

            { "Window_CanMaximize", WindowPattern.CanMaximizeProperty.Id },
            { "Window_CanMinimize", WindowPattern.CanMinimizeProperty.Id },
            { "Window_IsModal", WindowPattern.IsModalProperty.Id },
            { "Window_IsTopmost", WindowPattern.IsTopmostProperty.Id },
            { "Window_WindowInteractionState", WindowPattern.WindowInteractionStateProperty.Id },
            { "Window_WindowVisualState", WindowPattern.WindowVisualStateProperty.Id },

            { "Scroll_HorizontallyScrollable", ScrollPattern.HorizontallyScrollableProperty.Id },
            { "Scroll_HorizontalScrollPercent", ScrollPattern.HorizontalScrollPercentProperty.Id },
            { "Scroll_HorizontalViewSize", ScrollPattern.HorizontalViewSizeProperty.Id },
            { "Scroll_VerticallyScrollable", ScrollPattern.VerticallyScrollableProperty.Id },
            { "Scroll_VerticalScrollPercent", ScrollPattern.VerticalScrollPercentProperty.Id },
            { "Scroll_VerticalViewSize", ScrollPattern.VerticalViewSizeProperty.Id },

            { "RangeValue_IsReadOnly", RangeValuePattern.IsReadOnlyProperty.Id },
            { "RangeValue_LargeChange", RangeValuePattern.LargeChangeProperty.Id },
            { "RangeValue_Maximum", RangeValuePattern.MaximumProperty.Id },
            { "RangeValue_Minimum", RangeValuePattern.MinimumProperty.Id },
            { "RangeValue_SmallChange", RangeValuePattern.SmallChangeProperty.Id },
            { "RangeValue_Value", RangeValuePattern.ValueProperty.Id },

            { "Dock_DockPosition", DockPattern.DockPositionProperty.Id },

            { "ExpandCollapse_ExpandCollapseState", ExpandCollapsePattern.ExpandCollapseStateProperty.Id },

            { "GridItem_Column", GridItemPattern.ColumnProperty.Id },
            { "GridItem_ColumnSpan", GridItemPattern.ColumnSpanProperty.Id },
            { "GridItem_ContainingGrid", GridItemPattern.ContainingGridProperty.Id },
            { "GridItem_Row", GridItemPattern.RowProperty.Id },
            { "GridItem_RowSpan", GridItemPattern.RowSpanProperty.Id },

            { "Grid_ColumnCount", GridPattern.ColumnCountProperty.Id },
            { "Grid_RowCount", GridPattern.RowCountProperty.Id },

            { "MultipleView_CurrentView", MultipleViewPattern.CurrentViewProperty.Id },
            { "MultipleView_SupportedViews", MultipleViewPattern.SupportedViewsProperty.Id },
        };

        public static string? GetAutomationPropertyName(int id)
        {
            return propertyDictionary.FirstOrDefault(entry => entry.Value == id).Key;
        }

        public static AutomationProperty? GetAutomationProperty(string propertyName)
        {
            var hasId = propertyDictionary.TryGetValue(propertyName, out var propertyId);
            if (hasId)
            {
                return AutomationProperty.LookupById(propertyId);
            }
            return null;
        }
    }
}
