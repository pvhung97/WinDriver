using System.Text.Json.Nodes;
using UIADriver.actions;
using UIADriver.actions.inputsource;
using System.Windows.Automation;
using UIADriver.exception;
using UIADriver.services;

namespace UIADriver.uia2.actionoptions
{
    public class UIA2ActionOptions : ActionOptions
    {
        protected AutomationElement topLevelWindow;
        protected ElementFinderService<AutomationElement, CacheRequest> elementFinder;

        public UIA2ActionOptions(AutomationElement topLevelWindow, ElementFinderService<AutomationElement, CacheRequest> elementFinder)
        {
            this.topLevelWindow = topLevelWindow;
            this.elementFinder = elementFinder;
        }

        public override void AssertPositionInViewPort(int x, int y)
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(AutomationElement.BoundingRectangleProperty);
            var updated = topLevelWindow.GetUpdatedCache(cacheRequest);
            var rect = updated.Cached.BoundingRectangle;
            int width = (int)rect.Width;
            int height = (int)rect.Height;
            if (x < 0 || x > width || y < 0 || y > height)
            {
                throw new MoveTargetOutofBounds($"Location {x},{y} out of bound [0,0,{width},{height}]");
            }
        }

        public override Point GetCurrentWindowLocation()
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(AutomationElement.BoundingRectangleProperty);
            var updated = topLevelWindow.GetUpdatedCache(cacheRequest);
            var rect = updated.Cached.BoundingRectangle;
            return new Point((int)rect.X, (int)rect.Y);
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
                    var element = elementFinder.GetElement(elementId.ToString());
                    var rect = element.Cached.BoundingRectangle;

                    var windowLocation = GetCurrentWindowLocation();

                    int x = (int)rect.X;
                    int y = (int)rect.Y;
                    int width = (int)rect.Width;
                    int height = (int)rect.Height;
                    return new Point(x - windowLocation.X + width / 2 + xOffset, y - windowLocation.Y + height / 2 + yOffset);
                }
            }
            throw new UnknownError("Unknown origin");
        }

        public override int GetTopLevelProcessId()
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(AutomationElement.ProcessIdProperty);
            var updated = topLevelWindow.GetUpdatedCache(cacheRequest);
            return updated.Cached.ProcessId;
        }

    }
}
