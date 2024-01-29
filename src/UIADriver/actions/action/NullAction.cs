using System.Text.Json.Nodes;
using UIA3Driver.exception;

namespace UIA3Driver.actions.action
{
    public class NullAction(string id, string subtype) : Action(id, "none", subtype)
    {
        public static Action ProcessAction(string id, JsonObject actionItem)
        {
            actionItem.TryGetPropertyValue("type", out var subtype);
            if (subtype == null || !subtype.ToString().Equals("pause"))
            {
                throw new InvalidArgument("type must be pause for null input source");
            }
            var nullAction = new NullAction(id, subtype.ToString());
            return PauseAction.ProcessAction(nullAction, actionItem);
        }
    }
}
