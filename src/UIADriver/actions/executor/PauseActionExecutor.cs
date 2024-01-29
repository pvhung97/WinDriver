using UIA3Driver.actions.action;

namespace UIA3Driver.actions.executor
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
