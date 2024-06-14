using UIADriver.dto.request;
using UIADriver.dto.response;
using UIADriver.win32;
using UIADriver.win32native;

namespace UIADriver.services.pattern
{
    public abstract class WindowPatternService<T, U> : PatternService<T, U>
    {
        protected WindowPatternService(ElementFinderService<T, U> finderService, ElementAttributeService<T> attributeService) : base(finderService, attributeService) { }

        public abstract void SetVisualState(string elementId, string state);
        public abstract void WaitForInputIdle(string elementId, int timeout);
        public abstract void Close(string elementId);
        public abstract RectResponse GetRect(string elementId);
        protected abstract nint GetHandle(string elementId);

        public void BringToTop(string elementId)
        {
            Utilities.BringWindowToTop(GetHandle(elementId));
        }

        public void SetWindowRect(string elementId, SetRectRequest rect)
        {
            nint hdl = GetHandle(elementId);
            var currentRect = GetRect(elementId);
            Win32Methods.ShowWindow(hdl, Win32Constants.SW_RESTORE);

            if (rect.x == null) rect.x = currentRect.x;
            if (rect.y == null) rect.y = currentRect.y;
            if (rect.width == null) rect.width = currentRect.width;
            if (rect.height == null) rect.height = currentRect.height;

            Win32Methods.MoveWindow(hdl, (int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height, true);
        }
    }
}
