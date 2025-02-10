using System.Text.Json.Nodes;
using System.Windows.Automation;
using UIADriver.actions;
using UIADriver.actions.action;
using UIADriver.actions.inputsource;
using UIADriver.exception;
using UIADriver.services;
using Action = UIADriver.actions.action.Action;

namespace UIADriver.uia2
{
    public class UIA2ActionsService : ActionsService<AutomationElement>
    {
        protected SessionCapabilities capabilities;

        public UIA2ActionsService(SessionCapabilities capabilities)
        {
            this.capabilities = capabilities;
        }

        public override void ElementClear(AutomationElement element)
        {
            var elementCacheRequest = new CacheRequest();
            elementCacheRequest.Add(ValuePattern.Pattern);
            elementCacheRequest.Add(ScrollItemPattern.Pattern);
            elementCacheRequest.Add(AutomationElement.IsOffscreenProperty);
            elementCacheRequest.Add(ValuePattern.IsReadOnlyProperty);
            elementCacheRequest.Add(AutomationElement.IsEnabledProperty);
            element = element.GetUpdatedCache(elementCacheRequest);
            element.TryGetCachedPattern(ValuePattern.Pattern, out var pattern);
            if (pattern is ValuePattern valuePattern)
            {
                if ((bool)element.GetCachedPropertyValue(ValuePattern.IsReadOnlyProperty))
                {
                    throw new InvalidElementState("Element is readonly");
                }
                if (!element.Cached.IsEnabled)
                {
                    throw new InvalidElementState("Element is disabled");
                }

                element.TryGetCachedPattern(ScrollItemPattern.Pattern, out var sPattern);
                if (sPattern is ScrollItemPattern scrollItemPattern)
                {
                    scrollItemPattern.ScrollIntoView();
                    element = WaitUntilElementPropertyEqual(element, elementCacheRequest, AutomationElement.IsOffscreenProperty, false, capabilities.delayAfterFocus);
                }
                if (element.Cached.IsOffscreen)
                {
                    throw new ElementNotInteractable("Element is offscreen");
                }

                element.SetFocus();
                valuePattern.SetValue("");
            }
            else throw new InvalidElementState("Element is not editable");
        }

        public override async Task ElementClick(string elementId, AutomationElement element, ActionOptions options)
        {
            var elementCacheRequest = new CacheRequest();
            elementCacheRequest.Add(ScrollItemPattern.Pattern);
            elementCacheRequest.Add(AutomationElement.IsOffscreenProperty);
            elementCacheRequest.Add(AutomationElement.BoundingRectangleProperty);
            element = element.GetUpdatedCache(elementCacheRequest);
            element.TryGetCachedPattern(ScrollItemPattern.Pattern, out var pattern);
            if (pattern is ScrollItemPattern scrollItemPatern)
            {
                scrollItemPatern.ScrollIntoView();
                element = WaitUntilElementPropertyEqual(element, elementCacheRequest, AutomationElement.IsOffscreenProperty, false, capabilities.delayAfterFocus);
            }
            if (element.Cached.IsOffscreen)
            {
                throw new ElementNotInteractable("Element is offscreen");
            }

            InputState inputState = InputState.Instance();
            string inputId = Guid.NewGuid().ToString();
            inputState.CreateInputSource(inputId, "pointer", "mouse");
            var moveAction = new PointerMoveAction(inputId);
            var origin = new JsonObject();
            origin["element-6066-11e4-a52e-4f735466cecf"] = elementId;
            moveAction.origin = origin;
            var pointerDownAction = new PointerUpDownAction(inputId, "pointerDown");
            var pointerUpAction = new PointerUpDownAction(inputId, "pointerUp");
            List<Action> toDispatch = new List<Action>() { moveAction, pointerDownAction, pointerUpAction };
            await inputState.DispatchAction(toDispatch.Select(a => new List<Action>() { a }).ToList(), options);
            await inputState.Release(options);
        }

        public override async Task ElementSendKeys(AutomationElement element, string text, ActionOptions options)
        {
            var elementCacheRequest = new CacheRequest();
            elementCacheRequest.Add(ScrollItemPattern.Pattern);
            elementCacheRequest.Add(AutomationElement.IsOffscreenProperty);
            elementCacheRequest.Add(AutomationElement.IsEnabledProperty);
            elementCacheRequest.Add(AutomationElement.HasKeyboardFocusProperty);
            elementCacheRequest.Add(AutomationElement.IsKeyboardFocusableProperty);
            element = element.GetUpdatedCache(elementCacheRequest);
            element.TryGetCachedPattern(ScrollItemPattern.Pattern, out var pattern);
            if (pattern is ScrollItemPattern scrollItemPatern)
            {
                scrollItemPatern.ScrollIntoView();
                element = WaitUntilElementPropertyEqual(element, elementCacheRequest, AutomationElement.IsOffscreenProperty, false, capabilities.delayAfterFocus);
            }
            if (element.Cached.IsOffscreen
                || !element.Cached.IsEnabled
                || !element.Cached.IsKeyboardFocusable)
            {
                throw new ElementNotInteractable("Element is not keyboard-interactable");
            }

            InputState inputState = InputState.Instance();
            string inputId = Guid.NewGuid().ToString();
            KeyInputSource keySource = (KeyInputSource)inputState.CreateInputSource(inputId, "key", "");
            var toDispatch = new List<Action>();

            var alreadyHasKbFocus = element.Cached.HasKeyboardFocus;
            if (!alreadyHasKbFocus)
            {
                element.SetFocus();
                element = WaitUntilElementPropertyEqual(element, elementCacheRequest, AutomationElement.HasKeyboardFocusProperty, true, capabilities.delayAfterFocus);

                //  Send End to move caret to the end
                var endDown = new KeyAction(inputId, "keyDown");
                endDown.value = "\uE010";
                toDispatch.Add(endDown);
                toDispatch.Add(endDown.Clone("keyUp"));
            }
            string normalized = text.ToString();
            var resetModifier = new KeyAction(inputId, "keyDown");
            resetModifier.value = "\uE000";
            toDispatch.Add(resetModifier);
            foreach (var c in normalized)
            {
                switch (c)
                {
                    case '\uE008':
                        {
                            var modifierKey = new KeyAction(inputId, !keySource.shift ? "keyDown" : "keyUp");
                            modifierKey.value = $"{c}";
                            keySource.shift = !keySource.shift;
                            toDispatch.Add(modifierKey);
                            break;
                        }
                    case '\uE009':
                        {
                            var modifierKey = new KeyAction(inputId, !keySource.ctrl ? "keyDown" : "keyUp");
                            modifierKey.value = $"{c}";
                            keySource.ctrl = !keySource.ctrl;
                            toDispatch.Add(modifierKey);
                            break;
                        }
                    case '\uE00A':
                        {
                            var modifierKey = new KeyAction(inputId, !keySource.alt ? "keyDown" : "keyUp");
                            modifierKey.value = $"{c}";
                            keySource.alt = !keySource.alt;
                            toDispatch.Add(modifierKey);
                            break;
                        }
                    case '\uE03D':
                        {
                            var modifierKey = new KeyAction(inputId, !keySource.meta ? "keyDown" : "keyUp");
                            modifierKey.value = $"{c}";
                            keySource.meta = !keySource.meta;
                            toDispatch.Add(modifierKey);
                            break;
                        }
                    default:
                        {
                            var keyAction = new KeyAction(inputId, "keyDown");
                            keyAction.value = $"{c}";
                            toDispatch.Add(keyAction);
                            toDispatch.Add(keyAction.Clone("keyUp"));
                            break;
                        }

                }
            }

            await inputState.DispatchAction(toDispatch.Select(a => new List<Action>() { a }).ToList(), options);
            await inputState.Release(options);
        }

        public override async Task PerformActions(List<List<actions.action.Action>> actionsByTick, ActionOptions options)
        {
            InputState inputState = InputState.Instance();
            await inputState.DispatchAction(actionsByTick, options);
        }

        public override async Task ReleaseActions(ActionOptions options)
        {
            InputState inputState = InputState.Instance();
            await inputState.Release(options);
        }

        protected AutomationElement WaitUntilElementPropertyEqual(AutomationElement element, CacheRequest cacheRequest, AutomationProperty propId, object expectedValue, int timeout)
        {
            var pointer = element;
            var start = DateTime.Now;
            while (true)
            {
                object value = pointer.GetCachedPropertyValue(propId);
                if (expectedValue.Equals(value) || (DateTime.Now - start).TotalMilliseconds > timeout)
                {
                    break;
                }

                Thread.Sleep(50);
                pointer = pointer.GetUpdatedCache(cacheRequest);
            }
            return pointer;
        }
    }
}
