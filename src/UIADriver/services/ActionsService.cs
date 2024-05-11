using UIADriver.actions;
using Action = UIADriver.actions.action.Action;

namespace UIADriver.services
{
    public abstract class ActionsService<T>
    {
        public abstract Task PerformActions(List<List<Action>> actionsByTick, ActionOptions options);
        public abstract Task ReleaseActions(ActionOptions options);
        public abstract Task ElementClick(string elementId, T element, ActionOptions options);
        public abstract void ElementClear(T element);
        public abstract Task ElementSendKeys(T element, string text, ActionOptions options);
    }
}
