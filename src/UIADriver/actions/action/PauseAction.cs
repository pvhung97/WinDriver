using System.Text.Json;
using System.Text.Json.Nodes;
using UIADriver.exception;

namespace UIADriver.actions.action
{
    public class PauseAction : NullAction
    {
        public int duration;
        public PauseAction(string id) : base(id, "pause") { }
        public PauseAction(string id, string type) : base(id, "pause") 
        { 
            this.type = type;
        }

        public static Action ProcessAction(Action action, JsonObject actionItem)
        {
            var pauseAction = new PauseAction(action.id, action.type);

            actionItem.TryGetPropertyValue("duration", out var durationJson);
            if (durationJson == null || durationJson.GetValueKind() != JsonValueKind.Number)
            {
                throw new InvalidArgument("duration must be an integer");
            }
            int duration = durationJson.AsValue().GetValue<int>();
            if (duration < 0)
            {
                throw new InvalidArgument("duration must be >= 0");
            }
            pauseAction.duration = duration;

            return pauseAction;
        }

    }
}
