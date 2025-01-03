using Interop.UIAutomationClient;
using UIADriver.services;
using UIADriver.services.pattern;
using UIADriver.uia3.attribute;
using UIADriver.uia3.pattern;

namespace UIADriver.uia3.serviceProvider
{
    public abstract class UIA3ServiceProvider : ServiceProvider<IUIAutomationElement, IUIAutomationCacheRequest>
    {
        protected IUIAutomation automation;

        public UIA3ServiceProvider(SessionCapabilities capabilities, IUIAutomation automatiom) : base(capabilities)
        {
            this.automation = automatiom;
        }

        public override ScreenshotCapture GetScreenCaptureService()
        {
            if (ScreenCaptureService == null)
            {
                ScreenCaptureService = new ScreenshotCapture();
            }
            return ScreenCaptureService;
        }
        public override ElementAttributeService<IUIAutomationElement> GetElementAttributeService()
        {
            if (AttrService == null)
            {
                AttrService = new ElementAttributeGetter(automation);
            }
            return AttrService;
        }
        public override ElementFinderService<IUIAutomationElement, IUIAutomationCacheRequest> GetElementFinderService()
        {
            if (ElementFinderService == null)
            {
                ElementFinderService = new ElementFinder(automation, this);
            }
            return ElementFinderService;
        }
        public override ActionsService<IUIAutomationElement> GetActionsService()
        {
            if (ActionsService == null)
            {
                ActionsService = new UIA3ActionsService(automation, capabilities);
            }
            return ActionsService;
        }

        public override AnnotationPatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetAnnotationPatternService()
        {
            if (AnnotationPatternService == null)
            {
                AnnotationPatternService = new UIA3AnnotationPattern(this, automation);
            }
            return AnnotationPatternService;
        }
        public override BasePatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetBasePatternService()
        {
            if (BasePatternService == null)
            {
                BasePatternService = new UIA3BasePattern(this, automation);
            }
            return BasePatternService;
        }
        public override CustomNavigationPatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetCustomNavigationPatternService()
        {
            if (CustomNavigationPatternService == null)
            {
                CustomNavigationPatternService = new UIA3CustomNavigationPattern(this, automation);
            }
            return CustomNavigationPatternService;
        }
        public override DockPatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetDockPatternService()
        {
            if (DockPatternService == null)
            {
                DockPatternService = new UIA3DockPattern(this, automation);
            }
            return DockPatternService;
        }
        public override DragPatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetDragPatternService()
        {
            if (DragPatternService == null)
            {
                DragPatternService = new UIA3DragPattern(this, automation);
            }
            return DragPatternService;
        }
        public override DropTargetPatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetDropTargetPatternService()
        {
            if (DropTargetPatternService == null)
            {
                DropTargetPatternService = new UIA3DropTargetPattern(this, automation);
            }
            return DropTargetPatternService;
        }
        public override ExpandCollapsePatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetExpandCollapsePatternService()
        {
            if (ExpandCollapsePatternService == null)
            {
                ExpandCollapsePatternService = new UIA3ExpandCollapsePattern(this, automation);
            }
            return ExpandCollapsePatternService;
        }
        public override GridItemPatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetGridItemPatternService()
        {
            if (GridItemPatternService == null)
            {
                GridItemPatternService = new UIA3GridItemPattern(this, automation);
            }
            return GridItemPatternService;
        }
        public override GridPatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetGridPatternService()
        {
            if (GridPatternService == null)
            {
                GridPatternService = new UIA3GridPattern(this, automation);
            }
            return GridPatternService;
        }
        public override InvokePatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetInvokePatternService()
        {
            if (InvokePatternService == null)
            {
                InvokePatternService = new UIA3InvokePattern(this, automation);
            }
            return InvokePatternService;
        }
        public override MultipleViewPatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetMultipleViewPatternService()
        {
            if (MultipleViewPatternService == null)
            {
                MultipleViewPatternService = new UIA3MultipleViewPattern(this, automation);
            }
            return MultipleViewPatternService;
        }
        public override RangeValuePatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetRangeValuePatternService()
        {
            if (RangeValuePatternService == null)
            {
                RangeValuePatternService = new UIA3RangeValuePattern(this, automation);
            }
            return RangeValuePatternService;
        }
        public override ScrollItemPatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetScrollItemPatternService()
        {
            if (ScrollItemPatternService == null)
            {
                ScrollItemPatternService = new UIA3ScrollItemPattern(this, automation);
            }
            return ScrollItemPatternService;
        }
        public override ScrollPatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetScrollPatternService()
        {
            if (ScrollPatternService == null)
            {
                ScrollPatternService = new UIA3ScrollPattern(this, automation);
            }
            return ScrollPatternService;
        }
        public override SelectionItemPatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetSelectionItemPatternService()
        {
            if (SelectionItemPatternService == null)
            {
                SelectionItemPatternService = new UIA3SelectionItemPattern(this, automation);
            }
            return SelectionItemPatternService;
        }
        public override SelectionPattern2Service<IUIAutomationElement, IUIAutomationCacheRequest> GetSelectionPattern2Service()
        {
            if (SelectionPattern2Service == null)
            {
                SelectionPattern2Service = new UIA3SelectionPattern2(this, automation);
            }
            return SelectionPattern2Service;
        }
        public override SpreadSheetItemPatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetSpreadSheetItemPatternService()
        {
            if (SpreadSheetItemPatternService == null)
            {
                SpreadSheetItemPatternService = new UIA3SpreadSheetItemPattern(this, automation);
            }
            return SpreadSheetItemPatternService;
        }
        public override SpreadSheetPatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetSpreadSheetPatternService()
        {
            if (SpreadSheetPatternService == null)
            {
                SpreadSheetPatternService = new UIA3SpreadSheetPattern(this, automation);
            }
            return SpreadSheetPatternService;
        }
        public override TableItemPatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetTableItemPatternService()
        {
            if (TableItemPatternService == null)
            {
                TableItemPatternService = new UIA3TableItemPattern(this, automation);
            }
            return TableItemPatternService;
        }
        public override TablePatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetTablePatternService()
        {
            if (TablePatternService == null)
            {
                TablePatternService = new UIA3TablePattern(this, automation);
            }
            return TablePatternService;
        }
        public override TogglePatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetTogglePatternService()
        {
            if (TogglePatternService == null)
            {
                TogglePatternService = new UIA3TogglePattern(this, automation);
            }
            return TogglePatternService;
        }
        public override TransformPattern2Service<IUIAutomationElement, IUIAutomationCacheRequest> GetTransformPattern2Service()
        {
            if (TransformPattern2Service == null)
            {
                TransformPattern2Service = new UIA3TransformPattern2(this, automation);
            }
            return TransformPattern2Service;

        }
        public override TransformPatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetTransformPatternService()
        {
            if (TransformPatternService == null)
            {
                TransformPatternService = new UIA3TransformPattern(this, automation);
            }
            return TransformPatternService;
        }
        public override ValuePatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetValuePatternService()
        {
            if (ValuePatternService == null)
            {
                ValuePatternService = new UIA3ValuePattern(this, automation);
            }
            return ValuePatternService;
        }
        public override VirtualizedItemPatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetVirtualizedItemPatternService()
        {
            if (VirtualizedItemPatternService == null)
            {
                VirtualizedItemPatternService = new UIA3VirtualizedItemPattern(this, automation);
            }
            return VirtualizedItemPatternService;
        }
        public override WindowPatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetWindowPatternService()
        {
            if (WindowPatternService == null)
            {
                WindowPatternService = new UIA3WindowPattern(this, automation);
            }
            return WindowPatternService;
        }
    }
}
