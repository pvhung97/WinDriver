using System.Runtime.InteropServices;
using UIA3Driver.actions.action;
using UIA3Driver.actions.inputsource;
using UIA3Driver.exception;
using UIA3Driver.win32native;
using static UIA3Driver.win32native.Win32Enum;
using static UIA3Driver.win32native.Win32Struct;

namespace UIA3Driver.actions.executor
{
    public class MouseActionExecutor
    {
        public static void MouseDown(PointerUpDownAction action)
        {
            InputState inputState = InputState.Instance();
            PointerInputSource? pointerSource = (PointerInputSource?)InputState.Instance().GetInputSource(action.id);
            if (pointerSource == null) 
            {
                throw new UnknownError("Cannot find input source with id " + action.id);
            }
            if (pointerSource.pressed.Contains((uint)action.button)) return;
            pointerSource.pressed.Add((uint)action.button);

            MouseButtonEnum button = (MouseButtonEnum)action.button;
            var mouseData = 0;
            if (button == MouseButtonEnum.X1) mouseData = 1;
            if (button == MouseButtonEnum.X2) mouseData = 2;
            var dwFlags = MOUSEEVENTF.LEFTDOWN;
            switch (button)
            {
                case MouseButtonEnum.MIDDLE:
                    dwFlags = MOUSEEVENTF.MIDDLEDOWN;
                    break;
                case MouseButtonEnum.RIGHT:
                    dwFlags = MOUSEEVENTF.RIGHTDOWN;
                    break;
                case MouseButtonEnum.X1:
                case MouseButtonEnum.X2:
                    dwFlags = MOUSEEVENTF.XDOWN;
                    break;
                default:
                    break;
            }
            SendMouseEvent(mouseData, dwFlags, pointerSource.x, pointerSource.y);

            inputState.AddActionToCancelList(action.Clone("pointerUp"));
        }

        public static void MouseUp(PointerUpDownAction action)
        {
            PointerInputSource? pointerSource = (PointerInputSource?)InputState.Instance().GetInputSource(action.id);
            if (pointerSource == null)
            {
                throw new UnknownError("Cannot find input source with id " + action.id);
            }
            if (!pointerSource.pressed.Contains((uint)action.button)) return;
            pointerSource.pressed.Remove((uint)action.button);

            MouseButtonEnum button = (MouseButtonEnum)action.button;
            var mouseData = 0;
            if (button == MouseButtonEnum.X1) mouseData = 1;
            if (button == MouseButtonEnum.X2) mouseData = 2;
            var dwFlags = MOUSEEVENTF.LEFTUP;
            switch (button)
            {
                case MouseButtonEnum.MIDDLE:
                    dwFlags = MOUSEEVENTF.MIDDLEUP;
                    break;
                case MouseButtonEnum.RIGHT:
                    dwFlags = MOUSEEVENTF.RIGHTUP;
                    break;
                case MouseButtonEnum.X1:
                case MouseButtonEnum.X2:
                    dwFlags = MOUSEEVENTF.XUP;
                    break;
                default:
                    break;
            }
            SendMouseEvent(mouseData, dwFlags, pointerSource.x, pointerSource.y);
        }

        public static void MouseMove(PointerMoveAction action, ActionOptions options, int tickDuration)
        {
            PointerInputSource? pointerSource = (PointerInputSource?)InputState.Instance().GetInputSource(action.id);
            if (pointerSource == null)
            {
                throw new UnknownError("Cannot find input source with id " + action.id);
            }

            var relativeCoordinate = options.GetRelativeCoordinate(pointerSource, action.x, action.y, action.origin);
            //Skip out of bound check since dialog window can be outside of top level window viewport
            //options.AssertPositionInViewPort(relativeCoordinate.X, relativeCoordinate.Y);  
            var windowLocation = options.GetCurrentWindowLocation();
            int xAbsolute = windowLocation.X + relativeCoordinate.X;
            int yAbsolute = windowLocation.Y + relativeCoordinate.Y;
            int startXAbsolute = windowLocation.X + pointerSource.x;
            int startYAbsolute = windowLocation.Y + pointerSource.y;

            int moveDuration = action.duration == null ? tickDuration : (int)action.duration;
            int minDuration = 50;
            int moveTime = moveDuration < minDuration ? 0 : 10;
            int pause = moveDuration / Math.Max(moveTime, 1);
            int moveXDistance = xAbsolute - startXAbsolute;
            int moveYDistance = yAbsolute - startYAbsolute;
            for (int i = 0; i < moveTime; i++)
            {
                double step = (double)i / moveTime;
                int currentX = (int) (startXAbsolute + moveXDistance * step);
                int currentY = (int) (startYAbsolute + moveYDistance * step);
                SetCursorPosition(currentX, currentY);

                PauseActionExecutor.Pause(pause);
            }
            SetCursorPosition(xAbsolute, yAbsolute);

            pointerSource.x = relativeCoordinate.X;
            pointerSource.y = relativeCoordinate.Y;
        }

        private static void SendMouseEvent(int mouseData, MOUSEEVENTF dwFlags, int x, int y)
        {
            INPUT[] inputs = [
                new INPUT
                {
                    type = InputType.INPUT_MOUSE,
                    U = new InputUnion
                    {
                        mi = new MOUSEINPUT
                        {
                            dx = 0,
                            dy = 0,
                            mouseData = mouseData,
                            dwFlags = dwFlags,
                            time = 0,
                            dwExtraInfo = Win32Methods.GetMessageExtraInfo()
                        }
                    }
                }
            ];

            Win32Methods.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        }



        private static void SetCursorPosition(int x, int y)
        {
            Win32Methods.SetCursorPos(x, y);
            //  SetCursorPos will not work correctly on multi monitor scenario
            //  See https://stackoverflow.com/questions/65519784/why-does-setcursorpos-reset-the-cursor-position-to-the-left-hand-side-of-the-dis
            Win32Methods.GetCursorPos(out var pos);
            if (pos.X != x || pos.Y != y)
            {
                Win32Methods.SetCursorPos(x, y);
            }
        }

    }
}
