using Interop.UIAutomationClient;
using System.Text.Json.Nodes;
using UIADriver.actions;
using UIADriver.actions.action;
using UIADriver.actions.inputsource;
using UIADriver.exception;
using UIADriver.services;
using Action = UIADriver.actions.action.Action;

namespace UIADriver.uia3
{
    public class UIA3ActionsService : ActionsService<IUIAutomationElement>
    {
        protected IUIAutomation automation;
        protected SessionCapabilities capabilities;

        public UIA3ActionsService(IUIAutomation automation, SessionCapabilities capabilities)
        { 
            this.automation = automation;
            this.capabilities = capabilities;
        }

        public override void ElementClear(IUIAutomationElement element)
        {
            var elementCacheRequest = automation.CreateCacheRequest();
            elementCacheRequest.AddPattern(UIA_PatternIds.UIA_ValuePatternId);
            elementCacheRequest.AddPattern(UIA_PatternIds.UIA_ScrollItemPatternId);
            elementCacheRequest.AddProperty(UIA_PropertyIds.UIA_IsOffscreenPropertyId);
            elementCacheRequest.AddProperty(UIA_PropertyIds.UIA_ValueIsReadOnlyPropertyId);
            elementCacheRequest.AddProperty(UIA_PropertyIds.UIA_IsEnabledPropertyId);
            element = element.BuildUpdatedCache(elementCacheRequest);
            var pattern = element.GetCachedPattern(UIA_PatternIds.UIA_ValuePatternId);
            if (pattern is IUIAutomationValuePattern valuePattern)
            {
                if ((bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_ValueIsReadOnlyPropertyId))
                {
                    throw new InvalidElementState("Element is readonly");
                }
                if (!(bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsEnabledPropertyId))
                {
                    throw new InvalidElementState("Element is disabled");
                }

                var sPattern = element.GetCachedPattern(UIA_PatternIds.UIA_ScrollItemPatternId);
                if (sPattern is IUIAutomationScrollItemPattern scrollItemPattern)
                {
                    scrollItemPattern.ScrollIntoView();
                    element = WaitUntilElementPropertyEqual(element, elementCacheRequest, UIA_PropertyIds.UIA_IsOffscreenPropertyId, false, capabilities.delayAfterFocus);
                }
                if ((bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsOffscreenPropertyId))
                {
                    throw new ElementNotInteractable("Element is offscreen");
                }

                element.SetFocus();
                valuePattern.SetValue("");
            }
            else throw new InvalidElementState("Element is not editable");
        }

        public override async Task ElementClick(string elementId, IUIAutomationElement element, ActionOptions options)
        {
            var elementCacheRequest = automation.CreateCacheRequest();
            elementCacheRequest.AddPattern(UIA_PatternIds.UIA_ScrollItemPatternId);
            elementCacheRequest.AddProperty(UIA_PropertyIds.UIA_IsOffscreenPropertyId);
            elementCacheRequest.AddProperty(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
            element = element.BuildUpdatedCache(elementCacheRequest);
            var pattern = element.GetCachedPattern(UIA_PatternIds.UIA_ScrollItemPatternId);
            if (pattern is IUIAutomationScrollItemPattern scrollItemPatern)
            {
                scrollItemPatern.ScrollIntoView();
                element = WaitUntilElementPropertyEqual(element, elementCacheRequest, UIA_PropertyIds.UIA_IsOffscreenPropertyId, false, capabilities.delayAfterFocus);
            }
            if ((bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsOffscreenPropertyId))
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

        public override async Task ElementSendKeys(IUIAutomationElement element, string text, ActionOptions options)
        {
            var elementCacheRequest = automation.CreateCacheRequest();
            elementCacheRequest.AddPattern(UIA_PatternIds.UIA_ScrollItemPatternId);
            elementCacheRequest.AddPattern(UIA_PatternIds.UIA_TextPattern2Id);
            elementCacheRequest.AddProperty(UIA_PropertyIds.UIA_IsOffscreenPropertyId);
            elementCacheRequest.AddProperty(UIA_PropertyIds.UIA_IsEnabledPropertyId);
            elementCacheRequest.AddProperty(UIA_PropertyIds.UIA_HasKeyboardFocusPropertyId);
            elementCacheRequest.AddProperty(UIA_PropertyIds.UIA_IsKeyboardFocusablePropertyId);
            element = element.BuildUpdatedCache(elementCacheRequest);
            var pattern = element.GetCachedPattern(UIA_PatternIds.UIA_ScrollItemPatternId);
            if (pattern is IUIAutomationScrollItemPattern scrollItemPatern)
            {
                scrollItemPatern.ScrollIntoView();
                element = WaitUntilElementPropertyEqual(element, elementCacheRequest, UIA_PropertyIds.UIA_IsOffscreenPropertyId, false, capabilities.delayAfterFocus);
            }
            if ((bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsOffscreenPropertyId)
                || !(bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsEnabledPropertyId)
                || !(bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsKeyboardFocusablePropertyId))
            {
                throw new ElementNotInteractable("Element is not keyboard-interactable");
            }

            InputState inputState = InputState.Instance();
            string inputId = Guid.NewGuid().ToString();
            KeyInputSource keySource = (KeyInputSource)inputState.CreateInputSource(inputId, "key", "");
            var toDispatch = new List<Action>();

            var alreadyHasKbFocus = (bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_HasKeyboardFocusPropertyId);
            if (!alreadyHasKbFocus)
            {
                element.SetFocus();
                element = WaitUntilElementPropertyEqual(element, elementCacheRequest, UIA_PropertyIds.UIA_HasKeyboardFocusPropertyId, true, capabilities.delayAfterFocus);
                var tpattern = element.GetCachedPattern(UIA_PatternIds.UIA_TextPattern2Id);
                if (tpattern != null)
                {
                    //  Send End to move caret to the end
                    var endDown = new KeyAction(inputId, "keyDown");
                    endDown.value = "\uE010";
                    toDispatch.Add(endDown);
                    toDispatch.Add(endDown.Clone("keyUp"));
                }
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

        public override async Task PerformActions(List<List<Action>> actionsByTick, ActionOptions options)
        {
            InputState inputState = InputState.Instance();
            await inputState.DispatchAction(actionsByTick, options);
        }

        public override async Task ReleaseActions(ActionOptions options)
        {
            InputState inputState = InputState.Instance();
            await inputState.Release(options);
        }

        protected IUIAutomationElement WaitUntilElementPropertyEqual(IUIAutomationElement element, IUIAutomationCacheRequest cacheRequest, int propId, object expectedValue, int timeout)
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
                pointer = pointer.BuildUpdatedCache(cacheRequest);
            }
            return pointer;
        }
    }
}
