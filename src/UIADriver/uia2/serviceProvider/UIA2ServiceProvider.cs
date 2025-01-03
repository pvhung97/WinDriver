using UIADriver.services;
using System.Windows.Automation;
using UIADriver.exception;
using UIADriver.services.pattern;
using UIADriver.uia2.pattern;
using UIADriver.uia2.attribute;

namespace UIADriver.uia2.serviceProvider
{
    public abstract class UIA2ServiceProvider : ServiceProvider<AutomationElement, CacheRequest>
    {
        public UIA2ServiceProvider(SessionCapabilities capabilities) : base(capabilities) { }

        public override ScreenshotCapture GetScreenCaptureService()
        {
            if (ScreenCaptureService == null)
            {
                ScreenCaptureService = new ScreenshotCapture();
            }
            return ScreenCaptureService;
        }
        public override ElementAttributeService<AutomationElement> GetElementAttributeService()
        {
            if (AttrService == null)
            {
                AttrService = new ElementAttributeGetter();
            }
            return AttrService;
        }
        public override ElementFinderService<AutomationElement, CacheRequest> GetElementFinderService()
        {
            if (ElementFinderService == null)
            {
                ElementFinderService = new ElementFinder(this);
            }
            return ElementFinderService;
        }
        public override ActionsService<AutomationElement> GetActionsService()
        {
            if (ActionsService == null)
            {
                ActionsService = new UIA2ActionsService(capabilities);
            }
            return ActionsService;
        }

        public override AnnotationPatternService<AutomationElement, CacheRequest> GetAnnotationPatternService()
        {
            throw new UnsupportedOperation("AnnotationPattern is not supported in UIA2");
        }
        public override BasePatternService<AutomationElement, CacheRequest> GetBasePatternService()
        {
            if (BasePatternService == null)
            {
                BasePatternService = new UIA2BasePattern(this);
            }
            return BasePatternService;
        }
        public override CustomNavigationPatternService<AutomationElement, CacheRequest> GetCustomNavigationPatternService()
        {
            throw new UnsupportedOperation("CustomNavigationPattern is not supported in UIA2");
        }
        public override DockPatternService<AutomationElement, CacheRequest> GetDockPatternService()
        {
            if (DockPatternService == null)
            {
                DockPatternService = new UIA2DockPattern(this);
            }
            return DockPatternService;
        }
        public override DragPatternService<AutomationElement, CacheRequest> GetDragPatternService()
        {
            throw new UnsupportedOperation("DragPattern is not supported in UIA2");
        }
        public override DropTargetPatternService<AutomationElement, CacheRequest> GetDropTargetPatternService()
        {
            throw new UnsupportedOperation("DropTargetPattern is not supported in UIA2");
        }
        public override ExpandCollapsePatternService<AutomationElement, CacheRequest> GetExpandCollapsePatternService()
        {
            if (ExpandCollapsePatternService == null)
            {
                ExpandCollapsePatternService = new UIA2ExpandCollapsePattern(this);
            }
            return ExpandCollapsePatternService;
        }
        public override GridItemPatternService<AutomationElement, CacheRequest> GetGridItemPatternService()
        {
            if (GridItemPatternService == null)
            {
                GridItemPatternService = new UIA2GridItemPattern(this);
            }
            return GridItemPatternService;
        }
        public override GridPatternService<AutomationElement, CacheRequest> GetGridPatternService()
        {
            if (GridPatternService == null)
            {
                GridPatternService = new UIA2GridPattern(this);
            }
            return GridPatternService;
        }
        public override InvokePatternService<AutomationElement, CacheRequest> GetInvokePatternService()
        {
            if (InvokePatternService == null)
            {
                InvokePatternService = new UIA2InvokePattern(this);
            }
            return InvokePatternService;
        }
        public override MultipleViewPatternService<AutomationElement, CacheRequest> GetMultipleViewPatternService()
        {
            if (MultipleViewPatternService == null)
            {
                MultipleViewPatternService = new UIA2MultipleViewPattern(this);
            }
            return MultipleViewPatternService;
        }
        public override RangeValuePatternService<AutomationElement, CacheRequest> GetRangeValuePatternService()
        {
            if (RangeValuePatternService == null)
            {
                RangeValuePatternService = new UIA2RangeValuePattern(this);
            }
            return RangeValuePatternService;
        }
        public override ScrollItemPatternService<AutomationElement, CacheRequest> GetScrollItemPatternService()
        {
            if (ScrollItemPatternService == null)
            {
                ScrollItemPatternService = new UIA2ScrollItemPattern(this);
            }
            return ScrollItemPatternService;
        }
        public override ScrollPatternService<AutomationElement, CacheRequest> GetScrollPatternService()
        {
            if (ScrollPatternService == null)
            {
                ScrollPatternService = new UIA2ScrollPattern(this);
            }
            return ScrollPatternService;
        }
        public override SelectionItemPatternService<AutomationElement, CacheRequest> GetSelectionItemPatternService()
        {
            if (SelectionItemPatternService == null)
            {
                SelectionItemPatternService = new UIA2SelectionItemPattern(this);
            }
            return SelectionItemPatternService;
        }
        public override SelectionPattern2Service<AutomationElement, CacheRequest> GetSelectionPattern2Service()
        {
            throw new UnsupportedOperation("SelectionPattern2 is not supported in UIA2");
        }
        public override SpreadSheetItemPatternService<AutomationElement, CacheRequest> GetSpreadSheetItemPatternService()
        {
            throw new UnsupportedOperation("SpreadSheetItemPattern is not supported in UIA2");
        }
        public override SpreadSheetPatternService<AutomationElement, CacheRequest> GetSpreadSheetPatternService()
        {
            throw new UnsupportedOperation("SpreadSheetPattern is not supported in UIA2");
        }
        public override TableItemPatternService<AutomationElement, CacheRequest> GetTableItemPatternService()
        {
            if (TableItemPatternService == null)
            {
                TableItemPatternService = new UIA2TableItemPattern(this);
            }
            return TableItemPatternService;
        }
        public override TablePatternService<AutomationElement, CacheRequest> GetTablePatternService()
        {
            if (TablePatternService == null)
            {
                TablePatternService = new UIA2TablePattern(this);
            }
            return TablePatternService;
        }
        public override TogglePatternService<AutomationElement, CacheRequest> GetTogglePatternService()
        {
            if (TogglePatternService == null)
            {
                TogglePatternService = new UIA2TogglePattern(this);
            }
            return TogglePatternService;
        }
        public override TransformPattern2Service<AutomationElement, CacheRequest> GetTransformPattern2Service()
        {
            throw new UnsupportedOperation("TransformPattern2 is not supported in UIA2");

        }
        public override TransformPatternService<AutomationElement, CacheRequest> GetTransformPatternService()
        {
            if (TransformPatternService == null)
            {
                TransformPatternService = new UIA2TransformPattern(this);
            }
            return TransformPatternService;
        }
        public override ValuePatternService<AutomationElement, CacheRequest> GetValuePatternService()
        {
            if (ValuePatternService == null)
            {
                ValuePatternService = new UIA2ValuePattern(this);
            }
            return ValuePatternService;
        }
        public override VirtualizedItemPatternService<AutomationElement, CacheRequest> GetVirtualizedItemPatternService()
        {
            if (VirtualizedItemPatternService == null)
            {
                VirtualizedItemPatternService = new UIA2VirtualizedItemPattern(this);
            }
            return VirtualizedItemPatternService;
        }
        public override WindowPatternService<AutomationElement, CacheRequest> GetWindowPatternService()
        {
            if (WindowPatternService == null)
            {
                WindowPatternService = new UIA2WindowPattern(this);
            }
            return WindowPatternService;
        }
    }
}
