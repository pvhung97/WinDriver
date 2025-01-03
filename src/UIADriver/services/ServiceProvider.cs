using UIADriver.services.pattern;

namespace UIADriver.services
{
    public abstract class ServiceProvider<T, U>
    {
        protected SessionCapabilities capabilities;

        public ServiceProvider(SessionCapabilities capabilities)
        {
            this.capabilities = capabilities;
        }

        protected ScreenshotCapture? ScreenCaptureService;
        protected ElementAttributeService<T>? AttrService;
        protected PageSourceService<T, U>? PageSourceService;
        protected ElementFinderService<T, U>? ElementFinderService;
        protected WindowManageService<T, U>? WindowManageService;
        protected ActionsService<T>? ActionsService;

        protected AnnotationPatternService<T, U>? AnnotationPatternService;
        protected BasePatternService<T, U>? BasePatternService;
        protected CustomNavigationPatternService<T, U>? CustomNavigationPatternService;
        protected DockPatternService<T, U>? DockPatternService;
        protected DragPatternService<T, U>? DragPatternService;
        protected DropTargetPatternService<T, U>? DropTargetPatternService;
        protected ExpandCollapsePatternService<T, U>? ExpandCollapsePatternService;
        protected GridItemPatternService<T, U>? GridItemPatternService;
        protected GridPatternService<T, U>? GridPatternService;
        protected InvokePatternService<T, U>? InvokePatternService;
        protected MultipleViewPatternService<T, U>? MultipleViewPatternService;
        protected RangeValuePatternService<T, U>? RangeValuePatternService;
        protected ScrollItemPatternService<T, U>? ScrollItemPatternService;
        protected ScrollPatternService<T, U>? ScrollPatternService;
        protected SelectionItemPatternService<T, U>? SelectionItemPatternService;
        protected SelectionPattern2Service<T, U>? SelectionPattern2Service;
        protected SpreadSheetItemPatternService<T, U>? SpreadSheetItemPatternService;
        protected SpreadSheetPatternService<T, U>? SpreadSheetPatternService;
        protected TableItemPatternService<T, U>? TableItemPatternService;
        protected TablePatternService<T, U>? TablePatternService;
        protected TogglePatternService<T, U>? TogglePatternService;
        protected TransformPattern2Service<T, U>? TransformPattern2Service;
        protected TransformPatternService<T, U>? TransformPatternService;
        protected ValuePatternService<T, U>? ValuePatternService;
        protected VirtualizedItemPatternService<T, U>? VirtualizedItemPatternService;
        protected WindowPatternService<T, U>? WindowPatternService;

        public abstract ScreenshotCapture GetScreenCaptureService();
        public abstract ElementAttributeService<T> GetElementAttributeService();
        public abstract PageSourceService<T, U> GetPageSourceService();
        public abstract ElementFinderService<T, U> GetElementFinderService();
        public abstract WindowManageService<T, U> GetWindowManageService();
        public abstract ActionsService<T> GetActionsService();

        public abstract AnnotationPatternService<T, U> GetAnnotationPatternService();
        public abstract BasePatternService<T, U> GetBasePatternService();
        public abstract CustomNavigationPatternService<T, U> GetCustomNavigationPatternService();
        public abstract DockPatternService<T, U> GetDockPatternService();
        public abstract DragPatternService<T, U> GetDragPatternService();
        public abstract DropTargetPatternService<T, U> GetDropTargetPatternService();
        public abstract ExpandCollapsePatternService<T, U> GetExpandCollapsePatternService();
        public abstract GridItemPatternService<T, U> GetGridItemPatternService();
        public abstract GridPatternService<T, U> GetGridPatternService();
        public abstract InvokePatternService<T, U> GetInvokePatternService();
        public abstract MultipleViewPatternService<T, U> GetMultipleViewPatternService();
        public abstract RangeValuePatternService<T, U> GetRangeValuePatternService();
        public abstract ScrollItemPatternService<T, U> GetScrollItemPatternService();
        public abstract ScrollPatternService<T, U> GetScrollPatternService();
        public abstract SelectionItemPatternService<T, U> GetSelectionItemPatternService();
        public abstract SelectionPattern2Service<T, U> GetSelectionPattern2Service();
        public abstract SpreadSheetItemPatternService<T, U> GetSpreadSheetItemPatternService();
        public abstract SpreadSheetPatternService<T, U> GetSpreadSheetPatternService();
        public abstract TableItemPatternService<T, U> GetTableItemPatternService();
        public abstract TablePatternService<T, U> GetTablePatternService();
        public abstract TogglePatternService<T, U> GetTogglePatternService();
        public abstract TransformPattern2Service<T, U> GetTransformPattern2Service();
        public abstract TransformPatternService<T, U> GetTransformPatternService();
        public abstract ValuePatternService<T, U> GetValuePatternService();
        public abstract VirtualizedItemPatternService<T, U> GetVirtualizedItemPatternService();
        public abstract WindowPatternService<T, U> GetWindowPatternService();
    }
}
