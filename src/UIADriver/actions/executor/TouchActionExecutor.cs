using static UIA3Driver.win32native.Win32Enum;
using static UIA3Driver.win32native.Win32Struct;
using UIA3Driver.actions.action;
using UIA3Driver.actions.executor;
using UIA3Driver.actions.inputsource;
using UIA3Driver.actions;
using UIA3Driver.exception;
using UIA3Driver.win32native;
using UIA3Driver;

namespace UIADriver.actions.executor
{
    public class TouchActionExecutor
    {
        public static void PoiterUpDown(PointerUpDownAction action, ActionOptions options, bool up)
        {
            InputState inputState = InputState.Instance();
            PointerInputSource? pointerSource = (PointerInputSource?)inputState.GetInputSource(action.id);
            if (pointerSource == null)
            {
                throw new UnknownError("Cannot find input source with id " + action.id);
            }

            if (up && !pointerSource.pressed.Contains(0)) return;
            if (!up && pointerSource.pressed.Contains(0)) return;

            var absoluteCoordinate = Utilities.getPrimaryMonitorAbsolutePixelCoordinate();
            var windowLocation = options.GetCurrentWindowLocation();

            var pointerInfo = new POINTER_TYPE_INFO[]
            {
                new POINTER_TYPE_INFO()
                {
                    type = POINTER_INPUT_TYPE.PT_TOUCH,
                    U =
                    {
                        touchInfo = new POINTER_TOUCH_INFO()
                        {
                            pointerInfo = new POINTER_INFO()
                            {
                                 pointerType = POINTER_INPUT_TYPE.PT_TOUCH,
                                 ptPixelLocation = new POINT
                                 {
                                    X = absoluteCoordinate.X + windowLocation.X + pointerSource.x,
                                    Y = absoluteCoordinate.Y + windowLocation.Y + pointerSource.y
                                 }
                            },
                            touchMask = TOUCH_MASK.NONE,
                        }
                    }
                }
            };

            if (up)
            {
                pointerInfo[0].U.touchInfo.pointerInfo.pointerFlags = POINTER_FLAGS.UP;
                pointerSource.pressed.Remove(0);
            }
            else
            {
                pointerInfo[0].U.touchInfo.pointerInfo.pointerFlags = POINTER_FLAGS.INRANGE | POINTER_FLAGS.INCONTACT | POINTER_FLAGS.DOWN;
                pointerSource.pressed.Add(0);
            }

            AddInfoToPointerInfo(pointerInfo[0].U.touchInfo, action.pressure, action.twist, action.width, action.height);

            Win32Methods.InjectSyntheticPointerInput(pointerSource.device, pointerInfo, Convert.ToUInt32(pointerInfo.Length));

            if (!up)
            {
                inputState.AddActionToCancelList(action.Clone("pointerUp"));
            }
        }

        public static void PointerMove(PointerMoveAction action, ActionOptions options, int tickDuration)
        {
            PointerInputSource? pointerSource = (PointerInputSource?)InputState.Instance().GetInputSource(action.id);
            if (pointerSource == null)
            {
                throw new UnknownError("Cannot find input source with id " + action.id);
            }

            var absoluteCoordinate = Utilities.getPrimaryMonitorAbsolutePixelCoordinate();

            var relativeCoordinate = options.GetRelativeCoordinate(pointerSource, action.x, action.y, action.origin);
            //Skip out of bound check since dialog window can be outside of top level window viewport
            //options.AssertPositionInViewPort(relativeCoordinate.X, relativeCoordinate.Y);
            var windowLocation = options.GetCurrentWindowLocation();
            int xAbsolute = windowLocation.X + relativeCoordinate.X + absoluteCoordinate.X;
            int yAbsolute = windowLocation.Y + relativeCoordinate.Y + absoluteCoordinate.Y;
            int startXAbsolute = windowLocation.X + pointerSource.x + absoluteCoordinate.X;
            int startYAbsolute = windowLocation.Y + pointerSource.y + absoluteCoordinate.Y;

            int moveDuration = action.duration == null ? tickDuration : (int)action.duration;
            int minDuration = 50;
            int moveTime = moveDuration < minDuration ? 0 : 10;
            int pause = moveDuration / Math.Max(moveTime, 1);
            int moveXDistance = xAbsolute - startXAbsolute;
            int moveYDistance = yAbsolute - startYAbsolute;

            if (!pointerSource.pressed.Contains(0))
            {
                PauseActionExecutor.Pause(moveTime);
            }
            else
            {
                var pointerInfo = new POINTER_TYPE_INFO[]
                {
                    new POINTER_TYPE_INFO()
                    {
                        type = POINTER_INPUT_TYPE.PT_TOUCH,
                        U =
                        {
                            touchInfo = new POINTER_TOUCH_INFO()
                            {
                                pointerInfo = new POINTER_INFO()
                                {
                                    pointerType = POINTER_INPUT_TYPE.PT_TOUCH,
                                    pointerFlags = POINTER_FLAGS.INRANGE | POINTER_FLAGS.INCONTACT | POINTER_FLAGS.UPDATE,
                                    ptPixelLocation = new POINT
                                    {
                                        X = 0,
                                        Y = 0
                                    }
                                },
                                touchMask = TOUCH_MASK.NONE,
                            }
                        }
                    }
                };
                AddInfoToPointerInfo(pointerInfo[0].U.touchInfo, action.pressure, action.twist, action.width, action.height);

                for (int i = 0; i < moveTime; i++)
                {
                    double step = (double)i / moveTime;
                    int currentX = (int)(startXAbsolute + moveXDistance * step);
                    int currentY = (int)(startYAbsolute + moveYDistance * step);
                    pointerInfo[0].U.touchInfo.pointerInfo.ptPixelLocation.X = currentX;
                    pointerInfo[0].U.touchInfo.pointerInfo.ptPixelLocation.Y = currentY;
                    Win32Methods.InjectSyntheticPointerInput(pointerSource.device, pointerInfo, Convert.ToUInt32(pointerInfo.Length));

                    PauseActionExecutor.Pause(pause);
                }
                pointerInfo[0].U.touchInfo.pointerInfo.ptPixelLocation.X = xAbsolute;
                pointerInfo[0].U.touchInfo.pointerInfo.ptPixelLocation.Y = yAbsolute;
                Win32Methods.InjectSyntheticPointerInput(pointerSource.device, pointerInfo, Convert.ToUInt32(pointerInfo.Length));
            }

            pointerSource.x = relativeCoordinate.X;
            pointerSource.y = relativeCoordinate.Y;
        }

        private static void AddInfoToPointerInfo(POINTER_TOUCH_INFO touchInfo, double? pressure, int? twist, double? width, double? height)
        {
            if (pressure != null)
            {
                touchInfo.touchMask |= TOUCH_MASK.PRESSURE;
                touchInfo.pressure = Convert.ToUInt32(pressure);
            }

            if (twist != null)
            {
                touchInfo.touchMask |= TOUCH_MASK.ORIENTATION;
                touchInfo.orientation = Convert.ToUInt32(twist);
            }

            if (width != null || height != null)
            {
                touchInfo.touchMask |= TOUCH_MASK.CONTACTAREA;
                touchInfo.rcContact = new RECT();
                if (width != null)
                {
                    touchInfo.rcContact.left = (int)(touchInfo.pointerInfo.ptPixelLocation.X - width / 2);
                    touchInfo.rcContact.right = (int)(touchInfo.pointerInfo.ptPixelLocation.X + width / 2);
                }
                if (height != null)
                {
                    touchInfo.rcContact.top = (int)(touchInfo.pointerInfo.ptPixelLocation.Y - height / 2);
                    touchInfo.rcContact.bottom = (int)(touchInfo.pointerInfo.ptPixelLocation.Y + height / 2);
                }
            }
        }
    }
}
