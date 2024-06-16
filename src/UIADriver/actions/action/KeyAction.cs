using System.Text.Json;
using System.Text.Json.Nodes;
using UIADriver.exception;

namespace UIADriver.actions.action
{
    public class KeyAction(string id, string subType) : Action(id, "key", subType)
    {
        public string value = "";

        public static Action ProcessAction(string id, JsonObject actionItem)
        {
            actionItem.TryGetPropertyValue("type", out var subtype);
            if (subtype == null)
            {
                throw new InvalidArgument("type must be one of keyUp, keyDown, pause for key input source");
            }
            switch (subtype.ToString())
            {
                case "keyUp":
                case "keyDown":
                case "pause":
                    break;
                default:
                    throw new InvalidArgument("type must be one of keyUp, keyDown, pause for key input source");
            }

            var action = new KeyAction(id, subtype.ToString());
            if (subtype.ToString().Equals("pause")) return PauseAction.ProcessAction(action, actionItem);
            actionItem.TryGetPropertyValue("value", out var value);
            if (value == null || value.GetValueKind() != JsonValueKind.String) 
            {
                throw new InvalidArgument("value must be a single unicode point");
            }
            action.value = value.ToString();
            string normalized = action.value;
            if (normalized.Length != 1) throw new InvalidArgument("value must be a single unicode point");

            return action;
        }

        public Action Clone(string subtype)
        {
            var action = new KeyAction(id, subtype);
            action.value = value;
            return action;
        }

    }
}
