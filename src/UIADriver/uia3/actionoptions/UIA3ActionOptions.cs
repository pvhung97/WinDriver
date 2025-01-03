using Interop.UIAutomationClient;
using System.Text.Json.Nodes;
using UIADriver.actions;
using UIADriver.actions.inputsource;
using UIADriver.exception;
using UIADriver.services;

namespace UIADriver.uia3.actionoptions
{
    public class UIA3ActionOptions : ActionOptions
    {
        protected IUIAutomation automation;
        protected IUIAutomationElement topLevelWindow;
        protected ServiceProvider<IUIAutomationElement, IUIAutomationCacheRequest> serviceProvider;

        public UIA3ActionOptions(IUIAutomation automation, IUIAutomationElement topLevelWindow, ServiceProvider<IUIAutomationElement, IUIAutomationCacheRequest> serviceProvider)
        {
            this.automation = automation;
            this.topLevelWindow = topLevelWindow;
            this.serviceProvider = serviceProvider;
        }

        public override void AssertPositionInViewPort(int x, int y)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
            var updated = topLevelWindow.BuildUpdatedCache(cacheRequest);
            double[] rect = (double[])updated.GetCachedPropertyValue(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
            int width = (int)rect[2];
            int height = (int)rect[3];
            if (x < 0 || x > width || y < 0 || y > height)
            {
                throw new MoveTargetOutofBounds($"Location {x},{y} out of bound [0,0,{width},{height}]");
            }
        }

        public override Point GetCurrentWindowLocation()
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
            var updated = topLevelWindow.BuildUpdatedCache(cacheRequest);
            double[] rect = (double[])updated.GetCachedPropertyValue(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
            return new Point((int)rect[0], (int)rect[1]);
        }

        public override Point GetRelativeCoordinate(InputSource source, int xOffset, int yOffset, JsonNode origin)
        {
            if (origin.ToString() == "viewport") return new Point(xOffset, yOffset);
            if (origin.ToString() == "pointer")
            {
                if (source is PointerInputSource pointerInputSource) return new Point(pointerInputSource.x + xOffset, pointerInputSource.y + yOffset);
            }
            if (IsElementOrigin(origin))
            {
                origin.AsObject().TryGetPropertyValue("element-6066-11e4-a52e-4f735466cecf", out var elementId);
                if (elementId != null)
                {
                    var element = serviceProvider.GetElementFinderService().GetElement(elementId.ToString());
                    double[] rect = (double[])element.GetCachedPropertyValue(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);

                    var windowLocation = GetCurrentWindowLocation();

                    int x = (int)rect[0];
                    int y = (int)rect[1];
                    int width = (int)rect[2];
                    int height = (int)rect[3];
                    return new Point(x - windowLocation.X + width / 2 + xOffset, y - windowLocation.Y + height / 2 + yOffset);
                }
            }
            throw new UnknownError("Unknown origin");
        }

        public override int GetTopLevelProcessId()
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_ProcessIdPropertyId);
            var updated = topLevelWindow.BuildUpdatedCache(cacheRequest);
            return (int)updated.GetCachedPropertyValue(UIA_PropertyIds.UIA_ProcessIdPropertyId);
        }

    }
}
