using System.Text.Json.Nodes;
using UIADriver.actions.inputsource;

namespace UIADriver.actions
{
    public abstract class ActionOptions
    {
        public abstract bool IsElementOrigin(JsonNode origin);
        public abstract void AssertPositionInViewPort(int x, int y);
        public abstract Point GetRelativeCoordinate(InputSource source, int xOffset, int yOffset, JsonNode origin);
        public abstract Point GetCurrentWindowLocation();
        public abstract int GetTopLevelProcessId();
    }
}
