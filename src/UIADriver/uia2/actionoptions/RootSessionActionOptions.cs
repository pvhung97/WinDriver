using System.Text.Json.Nodes;
using System.Windows.Automation;
using UIADriver.actions.inputsource;
using UIADriver.exception;
using UIADriver.services;

namespace UIADriver.uia2.actionoptions
{
    public class RootSessionActionOptions(AutomationElement topLevelWindow, ElementFinderService<AutomationElement, CacheRequest> elementFinder) : UIA2ActionOptions(topLevelWindow, elementFinder)
    {
        public override void AssertPositionInViewPort(int x, int y) { }

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
                    int x = (int)rect.X;
                    int y = (int)rect.Y;
                    int width = (int)rect.Width;
                    int height = (int)rect.Height;
                    return new Point(x + width / 2, y + height / 2);
                }
            }
            throw new UnknownError("Unknown origin");
        }
    }
}
