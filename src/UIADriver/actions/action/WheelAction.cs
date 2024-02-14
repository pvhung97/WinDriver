using System.Text.Json;
using System.Text.Json.Nodes;
using UIADriver.exception;

namespace UIADriver.actions.action
{
    public class WheelAction(string id, string subtype) : Action(id, "wheel", subtype)
    {
        public int? duration;
        public JsonNode origin = "viewport";
        public int x;
        public int y;
        public int deltaX;
        public int deltaY;

        public static Action processAction(string id, JsonObject actionItem, ActionOptions actionOptions)
        {
            actionItem.TryGetPropertyValue("type", out var subtype);
            if (subtype == null)
            {
                throw new InvalidArgument("type must be one of scroll, pause for wheel input source");
            }
            switch (subtype.ToString())
            {
                case "scroll":
                case "pause":
                    break;
                default:
                    throw new InvalidArgument("type must be one of scroll, pause for wheel input source");
            }

            var action = new WheelAction(id, subtype.ToString());
            if (subtype.ToString().Equals("pause")) return PauseAction.ProcessAction(action, actionItem);

            actionItem.TryGetPropertyValue("duration", out var durationJson);
            if (durationJson != null)
            {
                if (durationJson.GetValueKind() != JsonValueKind.Number)
                {
                    throw new InvalidArgument("duration must be an integer");
                }
                int duration = durationJson.AsValue().GetValue<int>();
                if (duration < 0)
                {
                    throw new InvalidArgument("duration must be >= 0");
                }
                action.duration = duration;
            }

            JsonNode origin = "viewport";
            actionItem.TryGetPropertyValue("origin", out var originJson);
            if (originJson != null && !originJson.ToString().Equals("viewport"))
            {
                throw new InvalidArgument("invalid origin, currently support only viewport origin");
            }
            action.origin = originJson != null ? originJson : origin;

            actionItem.TryGetPropertyValue("x", out var x);
            if (x == null || x.GetValueKind() != JsonValueKind.Number) 
            {
                throw new InvalidArgument("x must be an integer");
            }
            action.x = x.AsValue().GetValue<int>();
            if (action.x != 0) throw new InvalidArgument("scroll by offset is not supported");

            actionItem.TryGetPropertyValue("y", out var y);
            if (y == null || y.GetValueKind() != JsonValueKind.Number)
            {
                throw new InvalidArgument("y must be an integer");
            }
            action.y = y.AsValue().GetValue<int>();
            if (action.y != 0) throw new InvalidArgument("scroll by offset is not supported");

            actionItem.TryGetPropertyValue("deltaX", out var deltaX);
            if (deltaX == null || deltaX.GetValueKind() != JsonValueKind.Number)
            {
                throw new InvalidArgument("deltaX must be an integer");
            }
            action.deltaX = deltaX.AsValue().GetValue<int>();

            actionItem.TryGetPropertyValue("deltaY", out var deltaY);
            if (deltaY == null || deltaY.GetValueKind() != JsonValueKind.Number)
            {
                throw new InvalidArgument("deltaY must be an integer");
            }
            action.deltaY = deltaY.AsValue().GetValue<int>();

            return action;
        }
    }
}
