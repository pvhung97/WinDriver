using UIADriver.actions.action;

namespace UIADriver.actions.executor
{
    public class PauseActionExecutor
    {
        public static void Pause(PauseAction action)
        {
            Pause(action.duration);
        }

        public static void Pause(int duration)
        {
            if (duration > 0)
            {
                Thread.Sleep(duration);
            }
        }
    }
}
