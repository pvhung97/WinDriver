using System.Runtime.InteropServices;
using UIADriver.actions.action;
using UIADriver.actions.inputsource;
using UIADriver.exception;
using UIADriver.win32native;
using static UIADriver.win32native.Win32Enum;
using static UIADriver.win32native.Win32Struct;

namespace UIADriver.actions.executor
{
    public class KeyActionExecutor
    {
        public static void KeyDown(KeyAction action, ActionOptions option)
        {
            InputState inputState = InputState.Instance();
            KeyInputSource? inputSource = (KeyInputSource?)InputState.Instance().GetInputSource(action.id);
            if (inputSource == null)
            {
                throw new UnknownError("Cannot find input source with id " + action.id);
            }

            string key = action.value.Normalize();
            char c = key[0];

            HandleKey(inputSource, c, false, option);

            inputState.AddActionToCancelList(action.Clone("keyUp"));
        }

        public static void KeyUp(KeyAction action, ActionOptions option)
        {
            InputState inputState = InputState.Instance();
            KeyInputSource? inputSource = (KeyInputSource?)InputState.Instance().GetInputSource(action.id);
            if (inputSource == null)
            {
                throw new UnknownError("Cannot find input source with id " + action.id);
            }

            string key = action.value.Normalize();
            char c = key[0];

            HandleKey(inputSource, c, true, option);
        }

        private static void SendInput(KeyInfo info, bool keyUp)
        {
            KEYEVENTF dwFlags = keyUp ? KEYEVENTF.KEYUP : 0;
            if (info.isExtendedKey) dwFlags |= KEYEVENTF.EXTENDEDKEY;
            if (info.useScanCode) dwFlags |= KEYEVENTF.SCANCODE;
            if (info.useUnicode) dwFlags |= KEYEVENTF.UNICODE;

            INPUT[] inputs = [
                new INPUT
                {
                    type = InputType.INPUT_KEYBOARD,
                    U = new InputUnion
                    {
                        ki = new KEYBDINPUT
                        {
                            wVk = info.keycode,
                            wScan = info.scancode,
                            dwFlags = dwFlags,
                            time = 0,
                            dwExtraInfo = Win32Methods.GetMessageExtraInfo(),
                        }
                    }
                }
            ];

            Win32Methods.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        private static void UpdatePressedKey(KeyInputSource source, char c, bool keyUp)
        {
            if (keyUp)
            {
                source.pressed.Remove(c);
            } else
            {
                source.pressed.Add(c);
            }
        }

        private static void HandleKey(KeyInputSource inputSource, char c, bool keyUp, ActionOptions option)
        {
            int pid = option.GetTopLevelProcessId();
            KeyInfo keyInfo = getKeyInfo(c, pid);

            if (c == '\uE000')
            {
                List<KeyInfo> toRelease = new List<KeyInfo>();
                if (inputSource.shift)
                {
                    if (inputSource.pressed.Contains('\uE008'))
                    {
                        toRelease.Add(getKeyInfo('\uE008', pid));
                        UpdatePressedKey(inputSource, c, true);
                    }
                    if (inputSource.pressed.Contains('\uE050'))
                    {
                        toRelease.Add(getKeyInfo('\uE050', pid));
                        UpdatePressedKey(inputSource, c, true);
                    }
                    inputSource.shift = false;
                }
                if (inputSource.ctrl)
                {
                    if (inputSource.pressed.Contains('\uE009'))
                    {
                        toRelease.Add(getKeyInfo('\uE009', pid));
                        UpdatePressedKey(inputSource, c, true);
                    }
                    if (inputSource.pressed.Contains('\uE051'))
                    {
                        toRelease.Add(getKeyInfo('\uE051', pid));
                        UpdatePressedKey(inputSource, c, true);
                    }
                    inputSource.ctrl = false;
                }
                if (inputSource.alt)
                {
                    if (inputSource.pressed.Contains('\uE00A'))
                    {
                        toRelease.Add(getKeyInfo('\uE00A', pid));
                        UpdatePressedKey(inputSource, c, true);
                    }
                    if (inputSource.pressed.Contains('\uE052'))
                    {
                        toRelease.Add(getKeyInfo('\uE052', pid));
                        UpdatePressedKey(inputSource, c, true);
                    }
                    inputSource.alt = false;
                }
                if (inputSource.meta)
                {
                    if (inputSource.pressed.Contains('\uE03D'))
                    {
                        toRelease.Add(getKeyInfo('\uE03D', pid));
                        UpdatePressedKey(inputSource, c, true);
                    }
                    if (inputSource.pressed.Contains('\uE053'))
                    {
                        toRelease.Add(getKeyInfo('\uE053', pid));
                        UpdatePressedKey(inputSource, c, true);
                    }
                    inputSource.meta = false;
                }

                foreach (var item in toRelease)
                {
                    SendInput(item, true);
                }
            }

            if (isModifierKey(c))
            {
                switch (c)
                {
                    case '\uE008':
                    case '\uE050':
                        if (inputSource.shift != keyUp) return;
                        SendInput(keyInfo, keyUp);
                        inputSource.shift = !keyUp;
                        UpdatePressedKey(inputSource, c, keyUp);
                        break;
                    case '\uE009':
                    case '\uE051':
                        if (inputSource.ctrl != keyUp) return;
                        SendInput(keyInfo, keyUp);
                        inputSource.ctrl = !keyUp;
                        UpdatePressedKey(inputSource, c, keyUp);
                        break;
                    case '\uE00A':
                    case '\uE052':
                        if (inputSource.alt != keyUp) return;
                        SendInput(keyInfo, keyUp);
                        inputSource.alt = !keyUp;
                        UpdatePressedKey(inputSource, c, keyUp);
                        break;
                    case '\uE03D':
                    case '\uE053':
                        if (inputSource.meta != keyUp) return;
                        SendInput(keyInfo, keyUp);
                        inputSource.meta = !keyUp;
                        UpdatePressedKey(inputSource, c, keyUp);
                        break;
                }
            }
            else
            {
                inputSource.pressed.Add(c);
                if (keyInfo.useUnicode && keyInfo.keycode == 0)
                {
                    keyInfo.scancode = Convert.ToUInt16(c);
                    keyInfo.keycode = 0;
                    SendInput(keyInfo, keyUp);
                }
                else
                {
                    var modifierKeyInfo = keyInfo.keycode >> 8;
                    if (modifierKeyInfo != 0)
                    {
                        bool needShift = (modifierKeyInfo & 1) != 0 && !inputSource.shift;
                        bool needCtrl = (modifierKeyInfo & 2) != 0 && !inputSource.ctrl;
                        bool needAlt = (modifierKeyInfo & 4) != 0 && !inputSource.alt;

                        if (!keyUp)
                        {
                            if (needShift)
                            {
                                KeyInfo shiftKeyInfo = getKeyInfo('\uE008', pid);
                                SendInput(shiftKeyInfo, false);
                            }

                            if (needCtrl)
                            {
                                KeyInfo ctrlKeyInfo = getKeyInfo('\uE009', pid);
                                SendInput(ctrlKeyInfo, false);
                            }

                            if (needAlt)
                            {
                                KeyInfo altKeyInfo = getKeyInfo('\uE00A', pid);
                                SendInput(altKeyInfo, false);
                            }
                        }

                        keyInfo.useScanCode = true;
                        SendInput(keyInfo, keyUp);

                        if (keyUp)
                        {
                            if (needShift)
                            {
                                KeyInfo shiftKeyInfo = getKeyInfo('\uE008', pid);
                                SendInput(shiftKeyInfo, true);
                            }

                            if (needCtrl)
                            {
                                KeyInfo ctrlKeyInfo = getKeyInfo('\uE009', pid);
                                SendInput(ctrlKeyInfo, true);
                            }

                            if (needAlt)
                            {
                                KeyInfo altKeyInfo = getKeyInfo('\uE00A', pid);
                                SendInput(altKeyInfo, true);
                            }
                        }
                    } else
                    {
                        SendInput(keyInfo, keyUp);
                    }
                }
                
            }
        }

        private static bool isModifierKey(char c)
        {
            switch (c)
            {
                case '\uE008':  //  SHIFT
                case '\uE050':  //  Right SHIFT
                case '\uE009':  //  CONTROL
                case '\uE051':  //  Right CONTROL
                case '\uE00A':  //  ALT
                case '\uE052':  //  Right ALT
                case '\uE03D':  //  META
                case '\uE053':  //  Right META
                    return true;
                default:
                    return false;
            }
        }

        private static ushort convertToUint16(short i)
        {
            if (i < 0) return 0;
            return (ushort)i;
        }

        private static KeyInfo getKeyInfo(char c, int processId)
        {
            KeyInfo keyInfo = new KeyInfo();
            IntPtr kbLayout = Win32Methods.GetKeyboardLayout((uint)processId);
            switch (c)
            {
                case '\uE000':  //  Null
                    keyInfo.keycode = 0;
                    keyInfo.scancode = 0;
                    break;
                case '\uE001':  //  Cancel
                    keyInfo.keycode = (ushort)VirtualKeyShort.CANCEL;
                    keyInfo.scancode = (ushort)ScanCodeShort.CANCEL;
                    keyInfo.isExtendedKey = true;
                    break;
                case '\uE002':  //  Help
                    keyInfo.keycode = (ushort)VirtualKeyShort.HELP;
                    keyInfo.scancode = (ushort)ScanCodeShort.HELP;
                    break;
                case '\uE003':  //  Backspace
                    keyInfo.keycode = (ushort)VirtualKeyShort.BACK;
                    keyInfo.scancode = (ushort)ScanCodeShort.BACK;
                    break;
                case '\uE004':  //  Tab
                    keyInfo.keycode = (ushort)VirtualKeyShort.TAB;
                    keyInfo.scancode = (ushort)ScanCodeShort.TAB;
                    break;
                case '\uE005':  //  Clear
                    keyInfo.keycode = (ushort)VirtualKeyShort.CLEAR;
                    keyInfo.scancode = (ushort)ScanCodeShort.CLEAR;
                    break;
                case '\uE006':  //  Return
                    keyInfo.keycode = (ushort)VirtualKeyShort.RETURN;
                    keyInfo.scancode = (ushort)ScanCodeShort.RETURN;
                    break;
                case '\uE007':  //  Enter
                    keyInfo.keycode = (ushort)VirtualKeyShort.RETURN;
                    keyInfo.scancode = (ushort)ScanCodeShort.RETURN;
                    keyInfo.isExtendedKey = true;
                    break;
                case '\uE008':  //  Shift
                    keyInfo.keycode = (ushort)VirtualKeyShort.SHIFT;
                    keyInfo.scancode = (ushort)ScanCodeShort.SHIFT;
                    break;
                case '\uE009':  //  Control
                    keyInfo.keycode = (ushort)VirtualKeyShort.CONTROL;
                    keyInfo.scancode = (ushort)ScanCodeShort.CONTROL;
                    break;
                case '\uE00A':  //  Alt
                    keyInfo.keycode = (ushort)VirtualKeyShort.MENU;
                    keyInfo.scancode = (ushort)ScanCodeShort.MENU;
                    break;
                case '\uE00B':  //  Pause
                    keyInfo.keycode = (ushort)VirtualKeyShort.PAUSE;
                    keyInfo.scancode = (ushort)ScanCodeShort.PAUSE;
                    keyInfo.isExtendedKey = true;
                    break;
                case '\uE00C':  //  Escape
                    keyInfo.keycode = (ushort)VirtualKeyShort.ESCAPE;
                    keyInfo.scancode = (ushort)ScanCodeShort.ESCAPE;
                    break;
                case '\uE00D':  //  Space
                    keyInfo.keycode = (ushort)VirtualKeyShort.SPACE;
                    keyInfo.scancode = (ushort)ScanCodeShort.SPACE;
                    break;
                case '\uE00E':  //  PageUp
                    keyInfo.keycode = (ushort)VirtualKeyShort.PRIOR;
                    keyInfo.scancode = (ushort)ScanCodeShort.PRIOR;
                    keyInfo.isExtendedKey = true;
                    break;
                case '\uE00F':  //  PageDown
                    keyInfo.keycode = (ushort)VirtualKeyShort.NEXT;
                    keyInfo.scancode = (ushort)ScanCodeShort.NEXT;
                    keyInfo.isExtendedKey = true;
                    break;
                case '\uE010':  //  End
                    keyInfo.keycode = (ushort)VirtualKeyShort.END;
                    keyInfo.scancode = (ushort)ScanCodeShort.END;
                    keyInfo.isExtendedKey = true;
                    break;
                case '\uE011':  //  Home
                    keyInfo.keycode = (ushort)VirtualKeyShort.HOME;
                    keyInfo.scancode = (ushort)ScanCodeShort.HOME;
                    keyInfo.isExtendedKey = true;
                    break;
                case '\uE012':  //  LeftArrow
                    keyInfo.keycode = (ushort)VirtualKeyShort.LEFT;
                    keyInfo.scancode = (ushort)ScanCodeShort.LEFT;
                    keyInfo.isExtendedKey = true;
                    break;
                case '\uE013':  //  UpArrow
                    keyInfo.keycode = (ushort)VirtualKeyShort.UP;
                    keyInfo.scancode = (ushort)ScanCodeShort.UP;
                    keyInfo.isExtendedKey = true;
                    break;
                case '\uE014':  //  RightArrow
                    keyInfo.keycode = (ushort)VirtualKeyShort.RIGHT;
                    keyInfo.scancode = (ushort)ScanCodeShort.RIGHT;
                    keyInfo.isExtendedKey = true;
                    break;
                case '\uE015':  //  DownArrow
                    keyInfo.keycode = (ushort)VirtualKeyShort.DOWN;
                    keyInfo.scancode = (ushort)ScanCodeShort.DOWN;
                    keyInfo.isExtendedKey = true;
                    break;
                case '\uE016':  //  Insert
                    keyInfo.keycode = (ushort)VirtualKeyShort.INSERT;
                    keyInfo.scancode = (ushort)ScanCodeShort.INSERT;
                    keyInfo.isExtendedKey = true;
                    break;
                case '\uE017':  //  Delete
                    keyInfo.keycode = (ushort)VirtualKeyShort.DELETE;
                    keyInfo.scancode = (ushort)ScanCodeShort.DELETE;
                    keyInfo.isExtendedKey = true;
                    break;
                case '\uE018':  //  Semicolon
                    keyInfo.keycode = convertToUint16(Win32Methods.VkKeyScanEx(';', kbLayout));
                    keyInfo.scancode = Convert.ToUInt16(Win32Methods.MapVirtualKeyEx((uint)(c & 0xFF), 0, kbLayout));
                    break;
                case '\uE019':  //  Equals
                    keyInfo.keycode = convertToUint16(Win32Methods.VkKeyScanEx('=', kbLayout));
                    keyInfo.scancode = Convert.ToUInt16(Win32Methods.MapVirtualKeyEx((uint)(c & 0xFF), 0, kbLayout));
                    break;
                case '\uE01A':  //  Nunpad0
                    keyInfo.keycode = (ushort)VirtualKeyShort.NUMPAD0;
                    keyInfo.scancode = (ushort)ScanCodeShort.NUMPAD0;
                    keyInfo.isExtendedKey = true;
                    break;
                case '\uE01B':  //  Nunpad1
                    keyInfo.keycode = (ushort)VirtualKeyShort.NUMPAD1;
                    keyInfo.scancode = (ushort)ScanCodeShort.NUMPAD1;
                    keyInfo.isExtendedKey = true;
                    break;
                case '\uE01C':  //  Nunpad2
                    keyInfo.keycode = (ushort)VirtualKeyShort.NUMPAD2;
                    keyInfo.scancode = (ushort)ScanCodeShort.NUMPAD2;
                    keyInfo.isExtendedKey = true;
                    break;
                case '\uE01D':  //  Nunpad3
                    keyInfo.keycode = (ushort)VirtualKeyShort.NUMPAD3;
                    keyInfo.scancode = (ushort)ScanCodeShort.NUMPAD3;
                    keyInfo.isExtendedKey = true;
                    break;
                case '\uE01E':  //  Nunpad4
                    keyInfo.keycode = (ushort)VirtualKeyShort.NUMPAD4;
                    keyInfo.scancode = (ushort)ScanCodeShort.NUMPAD4;
                    keyInfo.isExtendedKey = true;
                    break;
                case '\uE01F':  //  Nunpad5
                    keyInfo.keycode = (ushort)VirtualKeyShort.NUMPAD5;
                    keyInfo.scancode = (ushort)ScanCodeShort.NUMPAD5;
                    keyInfo.isExtendedKey = true;
                    break;
                case '\uE020':  //  Nunpad6
                    keyInfo.keycode = (ushort)VirtualKeyShort.NUMPAD6;
                    keyInfo.scancode = (ushort)ScanCodeShort.NUMPAD6;
                    keyInfo.isExtendedKey = true;
                    break;
                case '\uE021':  //  Nunpad7
                    keyInfo.keycode = (ushort)VirtualKeyShort.NUMPAD7;
                    keyInfo.scancode = (ushort)ScanCodeShort.NUMPAD7;
                    keyInfo.isExtendedKey = true;
                    break;
                case '\uE022':  //  Nunpad8
                    keyInfo.keycode = (ushort)VirtualKeyShort.NUMPAD8;
                    keyInfo.scancode = (ushort)ScanCodeShort.NUMPAD8;
                    keyInfo.isExtendedKey = true;
                    break;
                case '\uE023':  //  Nunpad9
                    keyInfo.keycode = (ushort)VirtualKeyShort.NUMPAD9;
                    keyInfo.scancode = (ushort)ScanCodeShort.NUMPAD9;
                    keyInfo.isExtendedKey = true;
                    break;
                case '\uE024':  //  Multiply
                    keyInfo.keycode = (ushort)VirtualKeyShort.MULTIPLY;
                    keyInfo.scancode = (ushort)ScanCodeShort.MULTIPLY;
                    keyInfo.isExtendedKey = true;
                    break;
                case '\uE025':  //  Add
                    keyInfo.keycode = (ushort)VirtualKeyShort.ADD;
                    keyInfo.scancode = (ushort)ScanCodeShort.ADD;
                    keyInfo.isExtendedKey = true;
                    break;
                case '\uE026':  //  Separator
                    keyInfo.keycode = convertToUint16(Win32Methods.VkKeyScanEx(',', kbLayout));
                    keyInfo.scancode = Convert.ToUInt16(Win32Methods.MapVirtualKeyEx((uint)(c & 0xFF), 0, kbLayout));
                    break;
                case '\uE027':  //  Subtract
                    keyInfo.keycode = (ushort)VirtualKeyShort.SUBTRACT;
                    keyInfo.scancode = (ushort)ScanCodeShort.SUBTRACT;
                    keyInfo.isExtendedKey = true;
                    break;
                case '\uE028':  //  Decimal
                    keyInfo.keycode = (ushort)VirtualKeyShort.DECIMAL;
                    keyInfo.scancode = (ushort)ScanCodeShort.DECIMAL;
                    keyInfo.isExtendedKey = true;
                    break;
                case '\uE029':  //  Divide
                    keyInfo.keycode = (ushort)VirtualKeyShort.DIVIDE;
                    keyInfo.scancode = (ushort)ScanCodeShort.DIVIDE;
                    keyInfo.isExtendedKey = true;
                    break;
                case '\uE031': // F1
                    keyInfo.keycode = (ushort)VirtualKeyShort.F1;
                    keyInfo.scancode = (ushort)ScanCodeShort.F1;
                    break;
                case '\uE032': // F2
                    keyInfo.keycode = (ushort)VirtualKeyShort.F2;
                    keyInfo.scancode = (ushort)ScanCodeShort.F2;
                    break;
                case '\uE033': // F3
                    keyInfo.keycode = (ushort)VirtualKeyShort.F3;
                    keyInfo.scancode = (ushort)ScanCodeShort.F3;
                    break;
                case '\uE034': // F4
                    keyInfo.keycode = (ushort)VirtualKeyShort.F4;
                    keyInfo.scancode = (ushort)ScanCodeShort.F4;
                    break;
                case '\uE035': // F5
                    keyInfo.keycode = (ushort)VirtualKeyShort.F5;
                    keyInfo.scancode = (ushort)ScanCodeShort.F5;
                    break;
                case '\uE036': // F6
                    keyInfo.keycode = (ushort)VirtualKeyShort.F6;
                    keyInfo.scancode = (ushort)ScanCodeShort.F6;
                    break;
                case '\uE037': // F7
                    keyInfo.keycode = (ushort)VirtualKeyShort.F7;
                    keyInfo.scancode = (ushort)ScanCodeShort.F7;
                    break;
                case '\uE038': // F8
                    keyInfo.keycode = (ushort)VirtualKeyShort.F8;
                    keyInfo.scancode = (ushort)ScanCodeShort.F8;
                    break;
                case '\uE039': // F9
                    keyInfo.keycode = (ushort)VirtualKeyShort.F9;
                    keyInfo.scancode = (ushort)ScanCodeShort.F9;
                    break;
                case '\uE03A': // F10
                    keyInfo.keycode = (ushort)VirtualKeyShort.F10;
                    keyInfo.scancode = (ushort)ScanCodeShort.F10;
                    break;
                case '\uE03B': // F11
                    keyInfo.keycode = (ushort)VirtualKeyShort.F11;
                    keyInfo.scancode = (ushort)ScanCodeShort.F11;
                    break;
                case '\uE03C': // F12
                    keyInfo.keycode = (ushort)VirtualKeyShort.F12;
                    keyInfo.scancode = (ushort)ScanCodeShort.F12;
                    break;
                case '\uE03D': // Meta
                    keyInfo.keycode = (ushort)VirtualKeyShort.LWIN;
                    keyInfo.scancode = (ushort)ScanCodeShort.LWIN;
                    break;
                case '\uE050':  //  RightShift
                    keyInfo.keycode = (ushort)VirtualKeyShort.RSHIFT;
                    keyInfo.scancode = (ushort)ScanCodeShort.RSHIFT;
                    keyInfo.isExtendedKey = true;
                    break;
                case '\uE051':  //  RightControl
                    keyInfo.keycode = (ushort)VirtualKeyShort.RCONTROL;
                    keyInfo.scancode = (ushort)ScanCodeShort.RCONTROL;
                    keyInfo.isExtendedKey = true;
                    break;
                case '\uE052':  //  RightAlt
                    keyInfo.keycode = (ushort)VirtualKeyShort.RMENU;
                    keyInfo.scancode = (ushort)ScanCodeShort.RMENU;
                    keyInfo.isExtendedKey = true;
                    break;
                case '\uE053':  //  RightMeta
                    keyInfo.keycode = (ushort)VirtualKeyShort.RWIN;
                    keyInfo.scancode = (ushort)ScanCodeShort.RWIN;
                    keyInfo.isExtendedKey = true;
                    break;
                case '\uE054':  //  RightPageUp
                    keyInfo.keycode = (ushort)VirtualKeyShort.NUMPAD9;
                    keyInfo.scancode = (ushort)Win32Methods.MapVirtualKeyEx((uint)(c & 0xFF), 0, kbLayout);
                    keyInfo.useScanCode = true;
                    break;
                case '\uE055':  //  RightPageDown
                    keyInfo.keycode = (ushort)VirtualKeyShort.NUMPAD3;
                    keyInfo.scancode = (ushort)Win32Methods.MapVirtualKeyEx((uint)(c & 0xFF), 0, kbLayout);
                    keyInfo.useScanCode = true;
                    break;
                case '\uE056':  //  RightEnd
                    keyInfo.keycode = (ushort)VirtualKeyShort.NUMPAD1;
                    keyInfo.scancode = (ushort)Win32Methods.MapVirtualKeyEx((uint)(c & 0xFF), 0, kbLayout);
                    keyInfo.useScanCode = true;
                    break;
                case '\uE057':  //  RightHome
                    keyInfo.keycode = (ushort)VirtualKeyShort.NUMPAD7;
                    keyInfo.scancode = (ushort)Win32Methods.MapVirtualKeyEx((uint)(c & 0xFF), 0, kbLayout);
                    keyInfo.useScanCode = true;
                    break;
                case '\uE058':  //  RightLeftArrow
                    keyInfo.keycode = (ushort)VirtualKeyShort.NUMPAD4;
                    keyInfo.scancode = (ushort)Win32Methods.MapVirtualKeyEx((uint)(c & 0xFF), 0, kbLayout);
                    keyInfo.useScanCode = true;
                    break;
                case '\uE059':  //  RightUpArrow
                    keyInfo.keycode = (ushort)VirtualKeyShort.NUMPAD8;
                    keyInfo.scancode = (ushort)Win32Methods.MapVirtualKeyEx((uint)(c & 0xFF), 0, kbLayout);
                    keyInfo.useScanCode = true;
                    break;
                case '\uE05A':  //  RightRightArrow
                    keyInfo.keycode = (ushort)VirtualKeyShort.NUMPAD6;
                    keyInfo.scancode = (ushort)Win32Methods.MapVirtualKeyEx((uint)(c & 0xFF), 0, kbLayout);
                    keyInfo.useScanCode = true;
                    break;
                case '\uE05B':  //  RightDownArrow
                    keyInfo.keycode = (ushort)VirtualKeyShort.NUMPAD2;
                    keyInfo.scancode = (ushort)Win32Methods.MapVirtualKeyEx((uint)(c & 0xFF), 0, kbLayout);
                    keyInfo.useScanCode = true;
                    break;
                case '\uE05C':  //  RightInsert
                    keyInfo.keycode = (ushort)VirtualKeyShort.NUMPAD0;
                    keyInfo.scancode = (ushort)Win32Methods.MapVirtualKeyEx((uint)(c & 0xFF), 0, kbLayout);
                    keyInfo.useScanCode = true;
                    break;
                case '\uE05D':  //  RightDelete
                    keyInfo.keycode = (ushort)VirtualKeyShort.DECIMAL;
                    keyInfo.scancode = (ushort)Win32Methods.MapVirtualKeyEx((uint)(c & 0xFF), 0, kbLayout);
                    keyInfo.useScanCode = true;
                    break;
                default:
                    keyInfo.keycode = convertToUint16(Win32Methods.VkKeyScanEx(c, kbLayout));
                    keyInfo.scancode = Convert.ToUInt16(Win32Methods.MapVirtualKeyEx((uint)(c & 0xFF), 0, kbLayout));
                    keyInfo.useUnicode = true;
                    break;
            }
            return keyInfo;
        }

        private class KeyInfo
        {
            public ushort keycode;
            public ushort scancode;
            public bool isExtendedKey;
            public bool useScanCode;
            public bool useUnicode;
        }
    }
}
