using System.Text.Json;
using System.Text.Json.Nodes;
using UIADriver.actions.inputsource;

namespace UIADriver.actions
{
    public abstract class ActionOptions
    {
        public bool IsElementOrigin(JsonNode origin)
        {
            if (origin.GetValueKind() == JsonValueKind.Object)
            {
                JsonObject originObj = origin.AsObject();
                originObj.TryGetPropertyValue("element-6066-11e4-a52e-4f735466cecf", out var elementId);
                if (elementId != null && elementId.GetValueKind() == JsonValueKind.String) return true;
            }
            return false;
        }

        public abstract void AssertPositionInViewPort(int x, int y);
        public abstract Point GetRelativeCoordinate(InputSource source, int xOffset, int yOffset, JsonNode origin);
        public abstract Point GetCurrentWindowLocation();
        public abstract int GetTopLevelProcessId();
    }
}
