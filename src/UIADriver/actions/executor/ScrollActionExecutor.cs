using System.Runtime.InteropServices;
using UIA3Driver.actions.action;
using UIA3Driver.actions.inputsource;
using UIA3Driver.exception;
using static UIA3Driver.win32native.Win32Enum;
using UIA3Driver.win32native;
using static UIA3Driver.win32native.Win32Struct;

namespace UIA3Driver.actions.executor
{
    public class ScrollActionExecutor
    {
        public static void Scroll(WheelAction action, ActionOptions options, int tickDuration)
        {
            WheelInpoutSource? inputSource = (WheelInpoutSource?)InputState.Instance().GetInputSource(action.id);
            if (inputSource == null)
            {
                throw new UnknownError("Cannot find input source with id " + action.id);
            }

            int currentDeltaX = 0;
            int currentDeltaY = 0;
            int targetDeltaX = action.deltaX;
            int targetDeltaY = action.deltaY;

            int moveDuration = action.duration == null ? tickDuration : (int)action.duration;
            int minDuration = 50;
            int moveTime = moveDuration < minDuration ? 0 : 10;
            int pause = moveDuration / Math.Max(moveTime, 1);

            for (int i = 0; i < moveTime; i++)
            {
                double step = (double)1 / moveTime;
                int xScroll = (int)(targetDeltaX * step);
                Scroll(xScroll, false);
                currentDeltaX += xScroll;
                int yScroll = (int)(targetDeltaY * step);
                currentDeltaY += yScroll;
                Scroll(yScroll, true);

                PauseActionExecutor.Pause(pause);
            }
            Scroll(targetDeltaX - currentDeltaX, false);
            Scroll(targetDeltaY - currentDeltaY, true);
        }

        private static void Scroll(int scroll, bool vertical)
        {
            if (scroll != 0)
            {
                MOUSEEVENTF dwFlags = vertical ? MOUSEEVENTF.WHEEL : MOUSEEVENTF.HWHEEL;
                INPUT[] inputs = [
                    new INPUT
                    {
                        type = InputType.INPUT_MOUSE,
                        U = new InputUnion
                        {
                            mi = new MOUSEINPUT
                            {
                                mouseData = scroll,
                                dwFlags = dwFlags,
                                dwExtraInfo = Win32Methods.GetMessageExtraInfo(),
                            }
                        }
                    }
                ];

                Win32Methods.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
            }
        }
    }
}
