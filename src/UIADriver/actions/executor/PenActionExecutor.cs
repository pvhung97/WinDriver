using UIA3Driver;
using UIA3Driver.actions;
using UIA3Driver.actions.action;
using UIA3Driver.actions.executor;
using UIA3Driver.actions.inputsource;
using UIA3Driver.exception;
using UIA3Driver.win32native;
using static UIA3Driver.win32native.Win32Enum;
using static UIA3Driver.win32native.Win32Struct;

namespace UIADriver.actions.executor
{
    public class PenActionExecutor
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
                    type = POINTER_INPUT_TYPE.PT_PEN,
                    U =
                    {
                        penInfo = new POINTER_PEN_INFO()
                        {
                            pointerInfo = new POINTER_INFO()
                            {
                                pointerType = POINTER_INPUT_TYPE.PT_PEN,
                                ptPixelLocation = new POINT
                                {
                                    X = absoluteCoordinate.X + windowLocation.X + pointerSource.x,
                                    Y = absoluteCoordinate.Y + windowLocation.Y + pointerSource.y
                                }
                            },
                            penMask = PEN_MASK.NONE,
                        }
                    }
                }
            };

            if (up)
            {
                pointerInfo[0].U.penInfo.pointerInfo.pointerFlags = POINTER_FLAGS.UP;
                pointerSource.pressed.Remove(0);
            } else
            {
                pointerInfo[0].U.penInfo.pointerInfo.pointerFlags = POINTER_FLAGS.INRANGE | POINTER_FLAGS.INCONTACT | POINTER_FLAGS.DOWN;
                pointerSource.pressed.Add(0);  
            }

            AddInfoToPointerInfo(pointerInfo[0].U.penInfo, action.pressure, action.tiltX, action.tiltY, action.twist);

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
            } else
            {
                var pointerInfo = new POINTER_TYPE_INFO[]
                {
                    new POINTER_TYPE_INFO()
                    {
                        type = POINTER_INPUT_TYPE.PT_PEN,
                        U =
                        {
                            penInfo = new POINTER_PEN_INFO()
                            {
                                pointerInfo = new POINTER_INFO()
                                {
                                    pointerType = POINTER_INPUT_TYPE.PT_PEN,
                                    pointerFlags = POINTER_FLAGS.INRANGE | POINTER_FLAGS.INCONTACT | POINTER_FLAGS.UPDATE,
                                    ptPixelLocation = new POINT
                                    {
                                        X = 0,
                                        Y = 0
                                    }
                                },
                                penMask = PEN_MASK.NONE,
                            }
                        }
                    }
                };
                AddInfoToPointerInfo(pointerInfo[0].U.penInfo, action.pressure, action.tiltX, action.tiltY, action.twist);

                for (int i = 0; i < moveTime; i++)
                {
                    double step = (double)i / moveTime;
                    int currentX = (int)(startXAbsolute + moveXDistance * step);
                    int currentY = (int)(startYAbsolute + moveYDistance * step);
                    pointerInfo[0].U.penInfo.pointerInfo.ptPixelLocation.X = currentX;
                    pointerInfo[0].U.penInfo.pointerInfo.ptPixelLocation.Y = currentY;
                    Win32Methods.InjectSyntheticPointerInput(pointerSource.device, pointerInfo, Convert.ToUInt32(pointerInfo.Length));

                    PauseActionExecutor.Pause(pause);
                }
                pointerInfo[0].U.penInfo.pointerInfo.ptPixelLocation.X = xAbsolute;
                pointerInfo[0].U.penInfo.pointerInfo.ptPixelLocation.Y = yAbsolute;
                Win32Methods.InjectSyntheticPointerInput(pointerSource.device, pointerInfo, Convert.ToUInt32(pointerInfo.Length));
            }

            pointerSource.x = relativeCoordinate.X;
            pointerSource.y = relativeCoordinate.Y;
        }
        
        private static void AddInfoToPointerInfo(POINTER_PEN_INFO penInfo, double? pressure, int? tiltX, int? tiltY, int? twist)
        {
            if (pressure != null)
            {
                penInfo.penMask |= PEN_MASK.PRESSURE;
                penInfo.pressure = Convert.ToUInt32(pressure);
            }

            if (tiltX != null)
            {
                penInfo.penMask |= PEN_MASK.TILT_X;
                penInfo.tiltX = (int)tiltX;
            }

            if (tiltY != null)
            {
                penInfo.penMask |= PEN_MASK.TILT_Y;
                penInfo.tiltY = (int)tiltY;
            }

            if (twist != null)
            {
                penInfo.penMask |= PEN_MASK.ROTATION;
                penInfo.rotation = Convert.ToUInt32(twist);
            }
        }
    }
}
