using System.Text.Json;
using System.Text.Json.Nodes;
using UIADriver.exception;

namespace UIADriver.actions.action
{
    public class PointerMoveAction(string id) : PointerAction(id, "pointerMove")
    {
        public int? duration;
        public JsonNode origin = "viewport";
        public int x;
        public int y;
        public double? width;
        public double? height;
        public double? pressure;
        public int? tiltX;
        public int? tiltY;
        public int? twist;

        public static Action ProcessAction(PointerAction action, JsonObject actionItem, ActionOptions actionOptions)
        {
            var pointerAction = new PointerMoveAction(action.id);
            pointerAction.pointerType = action.pointerType;

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
                pointerAction.duration = duration;
            }
            
            JsonNode origin = "viewport";
            actionItem.TryGetPropertyValue("origin", out var originJson);
            if (originJson != null && !originJson.ToString().Equals("viewport") && !originJson.ToString().Equals("pointer") && !actionOptions.IsElementOrigin(originJson))
            {
                throw new InvalidArgument("invalid origin");
            }
            pointerAction.origin = originJson != null ? originJson : origin;

            actionItem.TryGetPropertyValue("x", out var x);
            if (x == null || x.GetValueKind() != JsonValueKind.Number)
            {
                throw new InvalidArgument("x must be an integer");
            }
            pointerAction.x = x.AsValue().GetValue<int>();

            actionItem.TryGetPropertyValue("y", out var y);
            if (y == null || y.GetValueKind() != JsonValueKind.Number)
            {
                throw new InvalidArgument("y must be an integer");
            }
            pointerAction.y = y.AsValue().GetValue<int>();

            actionItem.TryGetPropertyValue("width", out var widthJson);
            if (widthJson != null)
            {
                if (widthJson.GetValueKind() != JsonValueKind.Number)
                {
                    throw new InvalidArgument("width must be a number");
                }
                double width = widthJson.AsValue().GetValue<double>();
                if (width < 0)
                {
                    throw new InvalidArgument("width must be >= 0");
                }
                pointerAction.width = width;
            }

            actionItem.TryGetPropertyValue("height", out var heightJson);
            if (heightJson != null)
            {
                if (heightJson.GetValueKind() != JsonValueKind.Number)
                {
                    throw new InvalidArgument("height must be a number");
                }
                double height = heightJson.AsValue().GetValue<double>();
                if (height < 0)
                {
                    throw new InvalidArgument("height must be >= 0");
                }
                pointerAction.height = height;
            }

            actionItem.TryGetPropertyValue("pressure", out var pressureJson);
            if (pressureJson != null)
            {
                if (pressureJson.GetValueKind() != JsonValueKind.Number)
                {
                    throw new InvalidArgument("pressure must be a number");
                }
                double pressure = pressureJson.AsValue().GetValue<double>();
                if (pressure < 0 || pressure > 1)
                {
                    throw new InvalidArgument("pressure must be >= 0 and <= 1");
                }
                pointerAction.pressure = pressure;
            }

            actionItem.TryGetPropertyValue("tiltX", out var tiltXJson);
            if (tiltXJson != null)
            {
                if (tiltXJson.GetValueKind() != JsonValueKind.Number)
                {
                    throw new InvalidArgument("tiltX must be an integer");
                }
                int tiltX = tiltXJson.AsValue().GetValue<int>();
                if (tiltX < -90 || tiltX > 90)
                {
                    throw new InvalidArgument("tiltX must be >= -90 and <= 90");
                }
                pointerAction.tiltX = tiltX;
            }

            actionItem.TryGetPropertyValue("tiltY", out var tiltYJson);
            if (tiltYJson != null)
            {
                if (tiltYJson.GetValueKind() != JsonValueKind.Number)
                {
                    throw new InvalidArgument("tiltY must be an integer");
                }
                int tiltY = tiltYJson.AsValue().GetValue<int>();
                if (tiltY < -90 || tiltY > 90)
                {
                    throw new InvalidArgument("tiltY must be >= -90 and <= 90");
                }
                pointerAction.tiltY = tiltY;
            }

            actionItem.TryGetPropertyValue("twist", out var twistJson);
            if (twistJson != null)
            {
                if (twistJson.GetValueKind() != JsonValueKind.Number)
                {
                    throw new InvalidArgument("twist must be an integer");
                }
                int twist = twistJson.AsValue().GetValue<int>();
                if (twist < 0 || twist > 359)
                {
                    throw new InvalidArgument("twist must be >= 0 and <= 359");
                }
                pointerAction.twist = twist;
            }

            return pointerAction;
        }
    }
}
