using System.Text.Json.Nodes;
using UIA3Driver.exception;

namespace UIA3Driver.actions.action
{
    public class PointerAction(string id, string subtype) : Action(id, "pointer", subtype)
    {
        public string pointerType = "mouse";

        public static Action ProcessAction(string id, string pointerType, JsonObject actionItem, ActionOptions actionOptions)
        {
            actionItem.TryGetPropertyValue("type", out var subtype);
            if (subtype == null)
            {
                throw new InvalidArgument("type must be one of pointerUp, pointerDown, pointerMove, pause for wheel input source");
            }
            switch (subtype.ToString())
            {
                case "pointerUp":
                case "pointerDown":
                case "pointerMove":
                case "pause":
                    break;
                default:
                    throw new InvalidArgument("type must be one of pointerUp, pointerDown, pointerMove, pause for wheel input source");
            }

            var action = new PointerAction(id, subtype.ToString());
            if (subtype.ToString().Equals("pause")) return PauseAction.ProcessAction(action, actionItem);
            action.pointerType = pointerType;
            switch (subtype.ToString())
            {
                case "pointerUp":
                case "pointerDown":
                    return PointerUpDownAction.ProcessAction(action, actionItem);
                default:
                    return PointerMoveAction.ProcessAction(action, actionItem, actionOptions);
            }
        }
    }
}
