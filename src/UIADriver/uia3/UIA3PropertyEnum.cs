﻿using Interop.UIAutomationClient;

namespace UIADriver.uia3
{
    public enum UIA3PropertyEnum
    {
        RuntimeId = UIA_PropertyIds.UIA_RuntimeIdPropertyId,
        BoundingRectangle = UIA_PropertyIds.UIA_BoundingRectanglePropertyId,
        ProcessId = UIA_PropertyIds.UIA_ProcessIdPropertyId,
        ControlType = UIA_PropertyIds.UIA_ControlTypePropertyId,
        LocalizedControlType = UIA_PropertyIds.UIA_LocalizedControlTypePropertyId,
        Name = UIA_PropertyIds.UIA_NamePropertyId,
        AcceleratorKey = UIA_PropertyIds.UIA_AcceleratorKeyPropertyId,
        AccessKey = UIA_PropertyIds.UIA_AccessKeyPropertyId,
        HasKeyboardFocus = UIA_PropertyIds.UIA_HasKeyboardFocusPropertyId,
        IsKeyboardFocusable = UIA_PropertyIds.UIA_IsKeyboardFocusablePropertyId,
        IsEnabled = UIA_PropertyIds.UIA_IsEnabledPropertyId,
        AutomationId = UIA_PropertyIds.UIA_AutomationIdPropertyId,
        ClassName = UIA_PropertyIds.UIA_ClassNamePropertyId,
        HelpText = UIA_PropertyIds.UIA_HelpTextPropertyId,
        ClickablePoint = UIA_PropertyIds.UIA_ClickablePointPropertyId,
        Culture = UIA_PropertyIds.UIA_CulturePropertyId,
        IsControlElement = UIA_PropertyIds.UIA_IsControlElementPropertyId,
        IsContentElement = UIA_PropertyIds.UIA_IsContentElementPropertyId,
        LabeledBy = UIA_PropertyIds.UIA_LabeledByPropertyId,
        IsPassword = UIA_PropertyIds.UIA_IsPasswordPropertyId,
        NativeWindowHandle = UIA_PropertyIds.UIA_NativeWindowHandlePropertyId,
        ItemType = UIA_PropertyIds.UIA_ItemTypePropertyId,
        IsOffscreen = UIA_PropertyIds.UIA_IsOffscreenPropertyId,
        Orientation = UIA_PropertyIds.UIA_OrientationPropertyId,
        FrameworkId = UIA_PropertyIds.UIA_FrameworkIdPropertyId,
        IsRequiredForForm = UIA_PropertyIds.UIA_IsRequiredForFormPropertyId,
        ItemStatus = UIA_PropertyIds.UIA_ItemStatusPropertyId,
        IsDockPatternAvailable = UIA_PropertyIds.UIA_IsDockPatternAvailablePropertyId,
        IsExpandCollapsePatternAvailable = UIA_PropertyIds.UIA_IsExpandCollapsePatternAvailablePropertyId,
        IsGridItemPatternAvailable = UIA_PropertyIds.UIA_IsGridItemPatternAvailablePropertyId,
        IsGridPatternAvailable = UIA_PropertyIds.UIA_IsGridPatternAvailablePropertyId,
        IsInvokePatternAvailable = UIA_PropertyIds.UIA_IsInvokePatternAvailablePropertyId,
        IsMultipleViewPatternAvailable = UIA_PropertyIds.UIA_IsMultipleViewPatternAvailablePropertyId,
        IsRangeValuePatternAvailable = UIA_PropertyIds.UIA_IsRangeValuePatternAvailablePropertyId,
        IsScrollPatternAvailable = UIA_PropertyIds.UIA_IsScrollPatternAvailablePropertyId,
        IsScrollItemPatternAvailable = UIA_PropertyIds.UIA_IsScrollItemPatternAvailablePropertyId,
        IsSelectionItemPatternAvailable = UIA_PropertyIds.UIA_IsSelectionItemPatternAvailablePropertyId,
        IsSelectionPatternAvailable = UIA_PropertyIds.UIA_IsSelectionPatternAvailablePropertyId,
        IsTablePatternAvailable = UIA_PropertyIds.UIA_IsTablePatternAvailablePropertyId,
        IsTableItemPatternAvailable = UIA_PropertyIds.UIA_IsTableItemPatternAvailablePropertyId,
        IsTextPatternAvailable = UIA_PropertyIds.UIA_IsTextPatternAvailablePropertyId,
        IsTogglePatternAvailable = UIA_PropertyIds.UIA_IsTogglePatternAvailablePropertyId,
        IsTransformPatternAvailable = UIA_PropertyIds.UIA_IsTransformPatternAvailablePropertyId,
        IsValuePatternAvailable = UIA_PropertyIds.UIA_IsValuePatternAvailablePropertyId,
        IsWindowPatternAvailable = UIA_PropertyIds.UIA_IsWindowPatternAvailablePropertyId,
        Value_Value = UIA_PropertyIds.UIA_ValueValuePropertyId,
        Value_IsReadOnly = UIA_PropertyIds.UIA_ValueIsReadOnlyPropertyId,
        RangeValue_Value = UIA_PropertyIds.UIA_RangeValueValuePropertyId,
        RangeValue_IsReadOnly = UIA_PropertyIds.UIA_RangeValueIsReadOnlyPropertyId,
        RangeValue_Minimum = UIA_PropertyIds.UIA_RangeValueMinimumPropertyId,
        RangeValue_Maximum = UIA_PropertyIds.UIA_RangeValueMaximumPropertyId,
        RangeValue_LargeChange = UIA_PropertyIds.UIA_RangeValueLargeChangePropertyId,
        RangeValue_SmallChange = UIA_PropertyIds.UIA_RangeValueSmallChangePropertyId,
        Scroll_HorizontalScrollPercent = UIA_PropertyIds.UIA_ScrollHorizontalScrollPercentPropertyId,
        Scroll_HorizontalViewSize = UIA_PropertyIds.UIA_ScrollHorizontalViewSizePropertyId,
        Scroll_VerticalScrollPercent = UIA_PropertyIds.UIA_ScrollVerticalScrollPercentPropertyId,
        Scroll_VerticalViewSize = UIA_PropertyIds.UIA_ScrollVerticalViewSizePropertyId,
        Scroll_HorizontallyScrollable = UIA_PropertyIds.UIA_ScrollHorizontallyScrollablePropertyId,
        Scroll_VerticallyScrollable = UIA_PropertyIds.UIA_ScrollVerticallyScrollablePropertyId,
        Selection_Selection = UIA_PropertyIds.UIA_SelectionSelectionPropertyId,
        Selection_CanSelectMultiple = UIA_PropertyIds.UIA_SelectionCanSelectMultiplePropertyId,
        Selection_IsSelectionRequired = UIA_PropertyIds.UIA_SelectionIsSelectionRequiredPropertyId,
        Grid_RowCount = UIA_PropertyIds.UIA_GridRowCountPropertyId,
        Grid_ColumnCount = UIA_PropertyIds.UIA_GridColumnCountPropertyId,
        Grid_ItemRow = UIA_PropertyIds.UIA_GridItemRowPropertyId,
        Grid_ItemColumn = UIA_PropertyIds.UIA_GridItemColumnPropertyId,
        Grid_ItemRowSpan = UIA_PropertyIds.UIA_GridItemRowSpanPropertyId,
        Grid_ItemColumnSpan = UIA_PropertyIds.UIA_GridItemColumnSpanPropertyId,
        Grid_ItemContainingGrid = UIA_PropertyIds.UIA_GridItemContainingGridPropertyId,
        Dock_DockPosition = UIA_PropertyIds.UIA_DockDockPositionPropertyId,
        ExpandCollapse_ExpandCollapseState = UIA_PropertyIds.UIA_ExpandCollapseExpandCollapseStatePropertyId,
        MultipleView_CurrentView = UIA_PropertyIds.UIA_MultipleViewCurrentViewPropertyId,
        MultipleView_SupportedViews = UIA_PropertyIds.UIA_MultipleViewSupportedViewsPropertyId,
        Window_CanMaximize = UIA_PropertyIds.UIA_WindowCanMaximizePropertyId,
        Window_CanMinimize = UIA_PropertyIds.UIA_WindowCanMinimizePropertyId,
        Window_WindowVisualState = UIA_PropertyIds.UIA_WindowWindowVisualStatePropertyId,
        Window_WindowInteractionState = UIA_PropertyIds.UIA_WindowWindowInteractionStatePropertyId,
        Window_IsModal = UIA_PropertyIds.UIA_WindowIsModalPropertyId,
        Window_IsTopmost = UIA_PropertyIds.UIA_WindowIsTopmostPropertyId,
        SelectionItem_IsSelected = UIA_PropertyIds.UIA_SelectionItemIsSelectedPropertyId,
        SelectionItem_SelectionContainer = UIA_PropertyIds.UIA_SelectionItemSelectionContainerPropertyId,
        Table_RowHeaders = UIA_PropertyIds.UIA_TableRowHeadersPropertyId,
        Table_ColumnHeaders = UIA_PropertyIds.UIA_TableColumnHeadersPropertyId,
        Table_RowOrColumnMajor = UIA_PropertyIds.UIA_TableRowOrColumnMajorPropertyId,
        TableItem_RowHeaderItems = UIA_PropertyIds.UIA_TableItemRowHeaderItemsPropertyId,
        TableItem_ColumnHeaderItems = UIA_PropertyIds.UIA_TableItemColumnHeaderItemsPropertyId,
        Toggle_ToggleState = UIA_PropertyIds.UIA_ToggleToggleStatePropertyId,
        Transform_CanMove = UIA_PropertyIds.UIA_TransformCanMovePropertyId,
        Transform_CanResize = UIA_PropertyIds.UIA_TransformCanResizePropertyId,
        Transform_CanRotate = UIA_PropertyIds.UIA_TransformCanRotatePropertyId,
        IsLegacyIAccessiblePatternAvailable = UIA_PropertyIds.UIA_IsLegacyIAccessiblePatternAvailablePropertyId,
        LegacyIAccessible_ChildId = UIA_PropertyIds.UIA_LegacyIAccessibleChildIdPropertyId,
        LegacyIAccessible_Name = UIA_PropertyIds.UIA_LegacyIAccessibleNamePropertyId,
        LegacyIAccessible_Value = UIA_PropertyIds.UIA_LegacyIAccessibleValuePropertyId,
        LegacyIAccessible_Description = UIA_PropertyIds.UIA_LegacyIAccessibleDescriptionPropertyId,
        LegacyIAccessible_Role = UIA_PropertyIds.UIA_LegacyIAccessibleRolePropertyId,
        LegacyIAccessible_State = UIA_PropertyIds.UIA_LegacyIAccessibleStatePropertyId,
        LegacyIAccessible_Help = UIA_PropertyIds.UIA_LegacyIAccessibleHelpPropertyId,
        LegacyIAccessible_KeyboardShortcut = UIA_PropertyIds.UIA_LegacyIAccessibleKeyboardShortcutPropertyId,
        LegacyIAccessible_Selection = UIA_PropertyIds.UIA_LegacyIAccessibleSelectionPropertyId,
        LegacyIAccessible_DefaultAction = UIA_PropertyIds.UIA_LegacyIAccessibleDefaultActionPropertyId,
        AriaRole = UIA_PropertyIds.UIA_AriaRolePropertyId,
        AriaProperties = UIA_PropertyIds.UIA_AriaPropertiesPropertyId,
        IsDataValidForForm = UIA_PropertyIds.UIA_IsDataValidForFormPropertyId,
        ControllerFor = UIA_PropertyIds.UIA_ControllerForPropertyId,
        DescribedBy = UIA_PropertyIds.UIA_DescribedByPropertyId,
        FlowsTo = UIA_PropertyIds.UIA_FlowsToPropertyId,
        ProviderDescription = UIA_PropertyIds.UIA_ProviderDescriptionPropertyId,
        IsItemContainerPatternAvailable = UIA_PropertyIds.UIA_IsItemContainerPatternAvailablePropertyId,
        IsVirtualizedItemPatternAvailable = UIA_PropertyIds.UIA_IsVirtualizedItemPatternAvailablePropertyId,
        IsSynchronizedInputPatternAvailable = UIA_PropertyIds.UIA_IsSynchronizedInputPatternAvailablePropertyId,
        OptimizeForVisualContent = UIA_PropertyIds.UIA_OptimizeForVisualContentPropertyId,
        IsObjectModelPatternAvailable = UIA_PropertyIds.UIA_IsObjectModelPatternAvailablePropertyId,
        Annotation_AnnotationTypeId = UIA_PropertyIds.UIA_AnnotationAnnotationTypeIdPropertyId,
        Annotation_AnnotationTypeName = UIA_PropertyIds.UIA_AnnotationAnnotationTypeNamePropertyId,
        Annotation_Author = UIA_PropertyIds.UIA_AnnotationAuthorPropertyId,
        Annotation_DateTime = UIA_PropertyIds.UIA_AnnotationDateTimePropertyId,
        Annotation_Target = UIA_PropertyIds.UIA_AnnotationTargetPropertyId,
        IsAnnotationPatternAvailable = UIA_PropertyIds.UIA_IsAnnotationPatternAvailablePropertyId,
        IsTextPattern2Available = UIA_PropertyIds.UIA_IsTextPattern2AvailablePropertyId,
        Styles_StyleId = UIA_PropertyIds.UIA_StylesStyleIdPropertyId,
        Styles_StyleName = UIA_PropertyIds.UIA_StylesStyleNamePropertyId,
        Styles_FillColor = UIA_PropertyIds.UIA_StylesFillColorPropertyId,
        Styles_FillPatternStyle = UIA_PropertyIds.UIA_StylesFillPatternStylePropertyId,
        Styles_Shape = UIA_PropertyIds.UIA_StylesShapePropertyId,
        Styles_FillPatternColor = UIA_PropertyIds.UIA_StylesFillPatternColorPropertyId,
        Styles_ExtendedProperties = UIA_PropertyIds.UIA_StylesExtendedPropertiesPropertyId,
        IsStylesPatternAvailable = UIA_PropertyIds.UIA_IsStylesPatternAvailablePropertyId,
        IsSpreadsheetPatternAvailable = UIA_PropertyIds.UIA_IsSpreadsheetPatternAvailablePropertyId,
        SpreadsheetItem_Formula = UIA_PropertyIds.UIA_SpreadsheetItemFormulaPropertyId,
        SpreadsheetItem_AnnotationObjects = UIA_PropertyIds.UIA_SpreadsheetItemAnnotationObjectsPropertyId,
        SpreadsheetItem_AnnotationTypes = UIA_PropertyIds.UIA_SpreadsheetItemAnnotationTypesPropertyId,
        IsSpreadsheetItemPatternAvailable = UIA_PropertyIds.UIA_IsSpreadsheetItemPatternAvailablePropertyId,
        Transform2_CanZoom = UIA_PropertyIds.UIA_Transform2CanZoomPropertyId,
        IsTransformPattern2Available = UIA_PropertyIds.UIA_IsTransformPattern2AvailablePropertyId,
        LiveSetting = UIA_PropertyIds.UIA_LiveSettingPropertyId,
        IsTextChildPatternAvailable = UIA_PropertyIds.UIA_IsTextChildPatternAvailablePropertyId,
        IsDragPatternAvailable = UIA_PropertyIds.UIA_IsDragPatternAvailablePropertyId,
        Drag_IsGrabbed = UIA_PropertyIds.UIA_DragIsGrabbedPropertyId,
        Drag_DropEffect = UIA_PropertyIds.UIA_DragDropEffectPropertyId,
        Drag_DropEffects = UIA_PropertyIds.UIA_DragDropEffectsPropertyId,
        IsDropTargetPatternAvailable = UIA_PropertyIds.UIA_IsDropTargetPatternAvailablePropertyId,
        DropTarget_DropTargetEffect = UIA_PropertyIds.UIA_DropTargetDropTargetEffectPropertyId,
        DropTarget_DropTargetEffects = UIA_PropertyIds.UIA_DropTargetDropTargetEffectsPropertyId,
        Drag_GrabbedItems = UIA_PropertyIds.UIA_DragGrabbedItemsPropertyId,
        Transform2_ZoomLevel = UIA_PropertyIds.UIA_Transform2ZoomLevelPropertyId,
        Transform2_ZoomMinimum = UIA_PropertyIds.UIA_Transform2ZoomMinimumPropertyId,
        Transform2_ZoomMaximum = UIA_PropertyIds.UIA_Transform2ZoomMaximumPropertyId,
        FlowsFrom = UIA_PropertyIds.UIA_FlowsFromPropertyId,
        IsTextEditPatternAvailable = UIA_PropertyIds.UIA_IsTextEditPatternAvailablePropertyId,
        IsPeripheral = UIA_PropertyIds.UIA_IsPeripheralPropertyId,
        IsCustomNavigationPatternAvailable = UIA_PropertyIds.UIA_IsCustomNavigationPatternAvailablePropertyId,
        PositionInSet = UIA_PropertyIds.UIA_PositionInSetPropertyId,
        SizeOfSet = UIA_PropertyIds.UIA_SizeOfSetPropertyId,
        Level = UIA_PropertyIds.UIA_LevelPropertyId,
        AnnotationTypes = UIA_PropertyIds.UIA_AnnotationTypesPropertyId,
        AnnotationObjects = UIA_PropertyIds.UIA_AnnotationObjectsPropertyId,
        LandmarkType = UIA_PropertyIds.UIA_LandmarkTypePropertyId,
        LocalizedLandmarkType = UIA_PropertyIds.UIA_LocalizedLandmarkTypePropertyId,
        FullDescription = UIA_PropertyIds.UIA_FullDescriptionPropertyId,
        FillColor = UIA_PropertyIds.UIA_FillColorPropertyId,
        OutlineColor = UIA_PropertyIds.UIA_OutlineColorPropertyId,
        FillType = UIA_PropertyIds.UIA_FillTypePropertyId,
        VisualEffects = UIA_PropertyIds.UIA_VisualEffectsPropertyId,
        OutlineThickness = UIA_PropertyIds.UIA_OutlineThicknessPropertyId,
        CenterPoint = UIA_PropertyIds.UIA_CenterPointPropertyId,
        Rotation = UIA_PropertyIds.UIA_RotationPropertyId,
        Size = UIA_PropertyIds.UIA_SizePropertyId,
        IsSelectionPattern2Available = UIA_PropertyIds.UIA_IsSelectionPattern2AvailablePropertyId,
        Selection2_FirstSelectedItem = UIA_PropertyIds.UIA_Selection2FirstSelectedItemPropertyId,
        Selection2_LastSelectedItem = UIA_PropertyIds.UIA_Selection2LastSelectedItemPropertyId,
        Selection2_CurrentSelectedItem = UIA_PropertyIds.UIA_Selection2CurrentSelectedItemPropertyId,
        Selection2_ItemCount = UIA_PropertyIds.UIA_Selection2ItemCountPropertyId,
        HeadingLevel = UIA_PropertyIds.UIA_HeadingLevelPropertyId,
        IsDialog = UIA_PropertyIds.UIA_IsDialogPropertyId,
    }
}