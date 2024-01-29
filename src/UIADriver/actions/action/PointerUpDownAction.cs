using System.Text.Json;
using System.Text.Json.Nodes;
using UIA3Driver.exception;

namespace UIA3Driver.actions.action
{
    public class PointerUpDownAction(string id, string subtype) : PointerAction(id, subtype)
    {
        public int button;
        public double? width;
        public double? height;
        public double? pressure;
        public double? tangentialPressure;
        public int? tiltX;
        public int? tiltY;
        public int? twist;
        public double? altitudeAngle;
        public double? azimuthAngle;

        public static Action ProcessAction(PointerAction action, JsonObject actionItem)
        {
            var pointerAction = new PointerUpDownAction(action.id, action.subtype);
            pointerAction.pointerType = action.pointerType;

            actionItem.TryGetPropertyValue("button", out var buttonJson);
            if (buttonJson == null || buttonJson.GetValueKind() != JsonValueKind.Number)
            {
                throw new InvalidArgument("button must be an integer");
            }
            int button = buttonJson.AsValue().GetValue<int>();
            if (button < 0)
            {
                throw new InvalidArgument("button must be >= 0");
            }
            pointerAction.button = button;

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

            actionItem.TryGetPropertyValue("tangentialPressure", out var tangentialPressureJson);
            if (tangentialPressureJson != null)
            {
                if (tangentialPressureJson.GetValueKind() != JsonValueKind.Number)
                {
                    throw new InvalidArgument("tangentialPressure must be a number");
                }
                double tangentialPressure = tangentialPressureJson.AsValue().GetValue<double>();
                if (tangentialPressure < -1 || tangentialPressure > 1)
                {
                    throw new InvalidArgument("pressure must be >= -1 and <= 1");
                }
                pointerAction.tangentialPressure = tangentialPressure;
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

            actionItem.TryGetPropertyValue("altitudeAngle", out var altitudeAngleJson);
            if (altitudeAngleJson != null)
            {
                if (altitudeAngleJson.GetValueKind() != JsonValueKind.Number)
                {
                    throw new InvalidArgument("altitudeAngle must be a number");
                }
                double altitudeAngle = altitudeAngleJson.AsValue().GetValue<double>();
                if (altitudeAngle < 0 || altitudeAngle > double.Pi / 2)
                {
                    throw new InvalidArgument("altitudeAngle must be >= 0 and <= π/2");
                }
                pointerAction.altitudeAngle = altitudeAngle;
            }

            actionItem.TryGetPropertyValue("azimuthAngle", out var azimuthAngleJson);
            if (azimuthAngleJson != null)
            {
                if (azimuthAngleJson.GetValueKind() != JsonValueKind.Number)
                {
                    throw new InvalidArgument("azimuthAngle must be a number");
                }
                double azimuthAngle = azimuthAngleJson.AsValue().GetValue<double>();
                if (azimuthAngle < 0 || azimuthAngle > double.Pi * 2)
                {
                    throw new InvalidArgument("azimuthAngle must be >= 0 and <= 2π");
                }
                pointerAction.azimuthAngle = azimuthAngle;
            }

            return pointerAction;
        }

        public Action Clone(string subtype)
        {
            var action = new PointerUpDownAction(id, subtype);
            action.button = button;
            action.width = width;
            action.height = height;
            action.pressure = pressure;
            action.tangentialPressure = tangentialPressure;
            action.tiltX = tiltX;
            action.tiltY = tiltY;
            action.twist = twist;
            action.altitudeAngle = azimuthAngle;
            action.azimuthAngle = azimuthAngle;
            return action;
        }
    }
}
