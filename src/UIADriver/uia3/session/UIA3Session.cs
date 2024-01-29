using Interop.UIAutomationClient;
using System.Text.Json;
using System.Text.Json.Nodes;
using UIA3Driver;
using UIA3Driver.actions;
using UIA3Driver.actions.action;
using UIA3Driver.actions.inputsource;
using UIA3Driver.attribute;
using UIA3Driver.dto.request;
using UIA3Driver.dto.response;
using UIA3Driver.exception;
using UIADriver.uia3.sourcebuilder;
using Action = UIA3Driver.actions.action.Action;

namespace UIADriver.uia3.session
{
    public abstract class UIA3Session : Session
    {
        protected IUIAutomation automation;
        protected ElementAttributeGetter attrGetter;
        protected ElementFinder elementFinder;

        protected abstract PageSourceBuilder PageSourceBuilder { get; }

        public UIA3Session(SessionCapabilities capabilities) : base(capabilities)
        {
            try
            {
                automation = new CUIAutomation8();
            }
            catch
            {
                automation = new CUIAutomation();
            }
            attrGetter = new ElementAttributeGetter();
            elementFinder = new ElementFinder(automation, PageSourceBuilder);
        }

        protected abstract IUIAutomationElement getCurrentWindow(IUIAutomationCacheRequest? cacheRequest);
        protected abstract IUIAutomationElement getCurrentWindowThenFocus(IUIAutomationCacheRequest? cacheRequest);

        public override string GetCurrentWindowTitle()
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_NamePropertyId);
            var wnd = getCurrentWindow(cacheRequest);
            return (string)wnd.GetCachedPropertyValue(UIA_PropertyIds.UIA_NamePropertyId);
        }

        public override string GetCurrentWindowHdl()
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId);
            var wnd = getCurrentWindow(cacheRequest);
            return ((int)wnd.GetCachedPropertyValue(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId)).ToString();
        }

        public override RectResponse GetCurrentWindowRect()
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
            var wnd = getCurrentWindow(cacheRequest);
            double[] rect = (double[])wnd.GetCachedPropertyValue(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
            return new RectResponse((int)rect[0], (int)rect[1], double.IsInfinity(rect[2]) ? 0 : (int)rect[2], double.IsInfinity(rect[3]) ? 0 : (int)rect[3]);
        }

        public override string GetScreenshot()
        {
            var currentHdl = GetCurrentWindowHdl();
            using (Bitmap bmp = screenCapture.CaptureWindowScreenshot(int.Parse(currentHdl)))
            {
                return screenCapture.ConvertToBase64(bmp);
            }
        }

        public override string GetPageSource()
        {
            var wnd = getCurrentWindow(null);
            return PageSourceBuilder.buildPageSource(wnd).pageSource.ToString();
        }

        public override FindElementResponse FindElement(JsonObject data)
        {
            FindElementRequest rq = FindElementRequest.Validate(data);
            return elementFinder.findElement(rq, getCurrentWindow(null));
        }

        public override List<FindElementResponse> FindElements(JsonObject data)
        {
            FindElementRequest rq = FindElementRequest.Validate(data);
            return elementFinder.findElements(rq, getCurrentWindow(null));
        }

        public override FindElementResponse FindElementFromElement(string elementId, JsonObject data)
        {
            FindElementRequest rq = FindElementRequest.Validate(data);
            IUIAutomationElement? startPoint = null;
            try
            {
                startPoint = elementFinder.getElement(elementId);
            }
            catch { }
            if (startPoint == null) throw new NoSuchElement("Cannot find any element with given parent element");

            return elementFinder.findElement(rq, startPoint);
        }

        public override List<FindElementResponse> FindElementsFromElement(string elementId, JsonObject data)
        {
            FindElementRequest rq = FindElementRequest.Validate(data);
            IUIAutomationElement? startPoint = null;
            try
            {
                startPoint = elementFinder.getElement(elementId);
            }
            catch { }
            if (startPoint == null) return [];

            return elementFinder.findElements(rq, startPoint);
        }

        public override string? GetElementAttribute(string id, string attribute)
        {
            var element = elementFinder.getElement(id);
            return attrGetter.getAttribute(automation, element, attribute);
        }

        public override string GetElementTagName(string id)
        {
            var element = elementFinder.getElement(id);
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_ControlTypePropertyId);
            element = element.BuildUpdatedCache(cacheRequest);
            return Utilities.GetControlTypeString((int)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_ControlTypePropertyId));
        }

        public override string GetElementText(string id)
        {
            var element = elementFinder.getElement(id);
            var treeWalker = automation.CreateTreeWalker(automation.CreateTrueCondition());
            var firstChild = treeWalker.GetFirstChildElement(element);
            if (firstChild != null) return "";

            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_NamePropertyId);
            element = element.BuildUpdatedCache(cacheRequest);
            return (string)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_NamePropertyId);
        }

        public override bool IsElementEnabled(string id)
        {
            var element = elementFinder.getElement(id);
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_IsEnabledPropertyId);
            element = element.BuildUpdatedCache(cacheRequest);
            return (bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsEnabledPropertyId);
        }

        public override bool IsElementSelected(string id)
        {
            var element = elementFinder.getElement(id);
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_IsSelectionItemPatternAvailablePropertyId);
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_SelectionItemIsSelectedPropertyId);
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_IsTogglePatternAvailablePropertyId);
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_ToggleToggleStatePropertyId);
            element = element.BuildUpdatedCache(cacheRequest);

            if ((bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsSelectionItemPatternAvailablePropertyId))
            {
                return (bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_SelectionItemIsSelectedPropertyId);
            }

            if ((bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsTogglePatternAvailablePropertyId))
            {
                return (ToggleState)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_ToggleToggleStatePropertyId) == ToggleState.ToggleState_On;
            }

            return false;
        }

        public override bool IsElementDisplayed(string id)
        {
            var element = elementFinder.getElement(id);
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_IsOffscreenPropertyId);
            element = element.BuildUpdatedCache(cacheRequest);

            return !(bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsOffscreenPropertyId);
        }

        protected abstract ActionOptions getActionOption();

        public override async Task PerformActions(JsonObject action)
        {
            InputState inputState = InputState.Instance();
            ActionOptions option = getActionOption();
            var actionsByTick = inputState.ExtractActionSequence(action, option);
            await inputState.DispatchAction(actionsByTick, option);
        }

        public override async Task ReleaseActions()
        {
            InputState inputState = InputState.Instance();
            ActionOptions option = getActionOption();
            await inputState.Release(option);
        }

        public override async Task ElementClick(string elementId)
        {
            getCurrentWindowThenFocus(null);

            var element = elementFinder.getElement(elementId);
            var elementCacheRequest = automation.CreateCacheRequest();
            elementCacheRequest.AddPattern(UIA_PatternIds.UIA_ScrollItemPatternId);
            elementCacheRequest.AddProperty(UIA_PropertyIds.UIA_IsOffscreenPropertyId);
            elementCacheRequest.AddProperty(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
            element = element.BuildUpdatedCache(elementCacheRequest);
            var pattern = element.GetCachedPattern(UIA_PatternIds.UIA_ScrollItemPatternId);
            if (pattern != null && pattern is IUIAutomationScrollItemPattern scrollItemPatern)
            {
                scrollItemPatern.ScrollIntoView();
                Thread.Sleep(capabilities.delayAfterFocus);
                element = element.BuildUpdatedCache(elementCacheRequest);
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
            ActionOptions actionOptions = getActionOption();
            await inputState.DispatchAction(toDispatch.Select(a => new List<Action>() { a }).ToList(), actionOptions);
            await inputState.Release(actionOptions);
        }

        public override void ElementClear(string elementId)
        {
            getCurrentWindowThenFocus(null);

            var element = elementFinder.getElement(elementId);
            var elementCacheRequest = automation.CreateCacheRequest();
            elementCacheRequest.AddPattern(UIA_PatternIds.UIA_ValuePatternId);
            elementCacheRequest.AddPattern(UIA_PatternIds.UIA_ScrollItemPatternId);
            elementCacheRequest.AddProperty(UIA_PropertyIds.UIA_IsOffscreenPropertyId);
            elementCacheRequest.AddProperty(UIA_PropertyIds.UIA_ValueIsReadOnlyPropertyId);
            elementCacheRequest.AddProperty(UIA_PropertyIds.UIA_IsEnabledPropertyId);
            element = element.BuildUpdatedCache(elementCacheRequest);
            var pattern = element.GetCachedPattern(UIA_PatternIds.UIA_ValuePatternId);
            if (pattern != null && pattern is IUIAutomationValuePattern valuePattern)
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
                if (sPattern != null && sPattern is IUIAutomationScrollItemPattern scrollItemPattern)
                {
                    scrollItemPattern.ScrollIntoView();
                    Thread.Sleep(capabilities.delayAfterFocus);
                    element = element.BuildUpdatedCache(elementCacheRequest);
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

        public override async Task ElementSendKeys(string elementId, JsonObject data)
        {
            data.TryGetPropertyValue("text", out var text);
            if (text == null || text.GetValueKind() != JsonValueKind.String)
            {
                throw new InvalidArgument("text must be string");
            }

            getCurrentWindowThenFocus(null);

            var element = elementFinder.getElement(elementId);
            var elementCacheRequest = automation.CreateCacheRequest();
            elementCacheRequest.AddPattern(UIA_PatternIds.UIA_ScrollItemPatternId);
            elementCacheRequest.AddPattern(UIA_PatternIds.UIA_TextPattern2Id);
            elementCacheRequest.AddProperty(UIA_PropertyIds.UIA_IsOffscreenPropertyId);
            elementCacheRequest.AddProperty(UIA_PropertyIds.UIA_IsEnabledPropertyId);
            elementCacheRequest.AddProperty(UIA_PropertyIds.UIA_HasKeyboardFocusPropertyId);
            elementCacheRequest.AddProperty(UIA_PropertyIds.UIA_IsKeyboardFocusablePropertyId);
            element = element.BuildUpdatedCache(elementCacheRequest);
            var pattern = element.GetCachedPattern(UIA_PatternIds.UIA_ScrollItemPatternId);
            if (pattern != null && pattern is IUIAutomationScrollItemPattern scrollItemPatern)
            {
                scrollItemPatern.ScrollIntoView();
                Thread.Sleep(capabilities.delayAfterFocus);
                element = element.BuildUpdatedCache(elementCacheRequest);
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
                Thread.Sleep(capabilities.delayAfterFocus);
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
            string normalized = text.ToString().Normalize();
            var resetModifier = new KeyAction(inputId, "keyDown");
            resetModifier.value = "\uE000";
            toDispatch.Add(resetModifier);
            foreach (var c in normalized)
            {
                switch (c)
                {
                    case '\uE008':
                        {
                            var modifierKey = new KeyAction(inputId, keySource.shift ? "keyDown" : "keyUp");
                            modifierKey.value = $"{c}";
                            keySource.shift = !keySource.shift;
                            toDispatch.Add(modifierKey);
                            break;
                        }
                    case '\uE009':
                        {
                            var modifierKey = new KeyAction(inputId, keySource.ctrl ? "keyDown" : "keyUp");
                            modifierKey.value = $"{c}";
                            keySource.ctrl = !keySource.ctrl;
                            toDispatch.Add(modifierKey);
                            break;
                        }
                    case '\uE00A':
                        {
                            var modifierKey = new KeyAction(inputId, keySource.alt ? "keyDown" : "keyUp");
                            modifierKey.value = $"{c}";
                            keySource.alt = !keySource.alt;
                            toDispatch.Add(modifierKey);
                            break;
                        }
                    case '\uE03D':
                        {
                            var modifierKey = new KeyAction(inputId, keySource.meta ? "keyDown" : "keyUp");
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

            ActionOptions actionOptions = getActionOption();
            await inputState.DispatchAction(toDispatch.Select(a => new List<Action>() { a }).ToList(), actionOptions);
            await inputState.Release(actionOptions);
        }

        public override string GetElementScreenshot(string elementId)
        {
            getCurrentWindow(null);
            //  Get element already include rect in cache
            var element = elementFinder.getElement(elementId);
            double[] rect = (double[])element.GetCachedPropertyValue(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);

            using (Bitmap bmp = screenCapture.CaptureElementScreenshot((int)rect[0], (int)rect[1], (int)rect[2], (int)rect[3]))
            {
                return screenCapture.ConvertToBase64(bmp);
            }
        }

    }
}
