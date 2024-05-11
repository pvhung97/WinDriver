using Interop.UIAutomationClient;
using System.Text.Json.Nodes;
using UIADriver.actions.inputsource;
using UIADriver.exception;
using UIADriver.services;

namespace UIADriver.uia3.actionoptions
{
    public class RootSessionActionOptions(IUIAutomation automation, IUIAutomationElement topLevelWindow, ElementFinderService<IUIAutomationElement, IUIAutomationCacheRequest> elementFinder) : UIA3ActionOptions(automation, topLevelWindow, elementFinder)
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
                    double[] rect = (double[])element.GetCachedPropertyValue(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
                    int x = (int)rect[0];
                    int y = (int)rect[1];
                    int width = (int)rect[2];
                    int height = (int)rect[3];
                    return new Point(x + width / 2, y + height / 2);
                }
            }
            throw new UnknownError("Unknown origin");
        }
    }
}
