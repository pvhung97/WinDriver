using System.Text.Json.Nodes;
using System.Text.Json;
using UIADriver.actions.action;
using UIADriver.actions.inputsource;
using UIADriver.actions;
using UIADriver.dto.request;
using UIADriver.dto.response;
using UIADriver.exception;
using UIADriver.uia2.attribute;
using UIADriver.uia2.sourcebuilder;
using System.Windows.Automation;
using Action = UIADriver.actions.action.Action;

namespace UIADriver.uia2.session
{
    public abstract class UIA2Session : Session
    {
        protected ElementAttributeGetter attrGetter;
        protected ElementFinder elementFinder;

        protected abstract PageSourceBuilder PageSourceBuilder { get; }

        public UIA2Session(SessionCapabilities capabilities) : base(capabilities)
        {
            attrGetter = new ElementAttributeGetter();
            elementFinder = new ElementFinder(PageSourceBuilder);
        }

        protected abstract AutomationElement getCurrentWindow(CacheRequest? cacheRequest);
        protected abstract AutomationElement getCurrentWindowThenFocus(CacheRequest? cacheRequest);

        public override string GetCurrentWindowTitle()
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(AutomationElement.NameProperty);
            var wnd = getCurrentWindow(cacheRequest);
            return wnd.Cached.Name;
        }

        public override string GetCurrentWindowHdl()
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(AutomationElement.NativeWindowHandleProperty);
            var wnd = getCurrentWindow(cacheRequest);
            return wnd.Cached.NativeWindowHandle.ToString();
        }

        public override RectResponse GetCurrentWindowRect()
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(AutomationElement.BoundingRectangleProperty);
            var wnd = getCurrentWindow(cacheRequest);
            var rect = wnd.Cached.BoundingRectangle;
            return new RectResponse((int)rect.X, (int)rect.Y, double.IsInfinity(rect.Width) ? 0 : (int)rect.Width, double.IsInfinity(rect.Height) ? 0 : (int)rect.Height);
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
            return elementFinder.FindElement(rq, getCurrentWindow(null));
        }

        public override List<FindElementResponse> FindElements(JsonObject data)
        {
            FindElementRequest rq = FindElementRequest.Validate(data);
            return elementFinder.FindElements(rq, getCurrentWindow(null));
        }

        public override FindElementResponse FindElementFromElement(string elementId, JsonObject data)
        {
            FindElementRequest rq = FindElementRequest.Validate(data);
            AutomationElement? startPoint = null;
            try
            {
                startPoint = elementFinder.GetElement(elementId);
            }
            catch { }
            if (startPoint == null) throw new NoSuchElement("Cannot find any element with given parent element");

            return elementFinder.FindElement(rq, startPoint);
        }

        public override List<FindElementResponse> FindElementsFromElement(string elementId, JsonObject data)
        {
            FindElementRequest rq = FindElementRequest.Validate(data);
            AutomationElement? startPoint = null;
            try
            {
                startPoint = elementFinder.GetElement(elementId);
            }
            catch { }
            if (startPoint == null) return [];

            return elementFinder.FindElements(rq, startPoint);
        }

        public override string? GetElementAttribute(string id, string attribute)
        {
            var element = elementFinder.GetElement(id);
            return attrGetter.getAttribute(element, attribute);
        }

        public override string GetElementTagName(string id)
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(AutomationElement.ControlTypeProperty);
            var element = elementFinder.GetElement(id, cacheRequest);
            return Utilities.GetControlTypeString(element.Cached.ControlType.Id);
        }

        public override string GetElementText(string id)
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(AutomationElement.NameProperty);

            var element = elementFinder.GetElement(id, cacheRequest);
            var treeWalker = new TreeWalker(Condition.TrueCondition);
            var firstChild = treeWalker.GetFirstChild(element);
            if (firstChild != null) return "";

            return element.Cached.Name;
        }

        public override bool IsElementEnabled(string id)
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(AutomationElement.IsEnabledProperty);
            var element = elementFinder.GetElement(id, cacheRequest);
            return element.Cached.IsEnabled;
        }

        public override bool IsElementSelected(string id)
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(AutomationElement.IsSelectionItemPatternAvailableProperty);
            cacheRequest.Add(SelectionItemPattern.IsSelectedProperty);
            cacheRequest.Add(AutomationElement.IsTogglePatternAvailableProperty);
            cacheRequest.Add(TogglePattern.ToggleStateProperty);
            var element = elementFinder.GetElement(id, cacheRequest);

            if ((bool)element.GetCachedPropertyValue(AutomationElement.IsSelectionItemPatternAvailableProperty))
            {
                return (bool)element.GetCachedPropertyValue(SelectionItemPattern.IsSelectedProperty);
            }

            if ((bool)element.GetCachedPropertyValue(AutomationElement.IsTogglePatternAvailableProperty))
            {
                return (ToggleState)element.GetCachedPropertyValue(TogglePattern.ToggleStateProperty) == ToggleState.On;
            }

            return false;
        }

        public override bool IsElementDisplayed(string id)
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(AutomationElement.IsOffscreenProperty);
            var element = elementFinder.GetElement(id, cacheRequest);

            return !element.Cached.IsOffscreen;
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

            var elementCacheRequest = new CacheRequest();
            elementCacheRequest.Add(ScrollItemPattern.Pattern);
            elementCacheRequest.Add(AutomationElement.IsOffscreenProperty);
            elementCacheRequest.Add(AutomationElement.BoundingRectangleProperty);
            var element = elementFinder.GetElement(elementId, elementCacheRequest);
            var pattern = element.GetCachedPattern(ScrollItemPattern.Pattern);
            if (pattern != null && pattern is ScrollItemPattern scrollItemPatern)
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
            ActionOptions actionOptions = getActionOption();
            await inputState.DispatchAction(toDispatch.Select(a => new List<Action>() { a }).ToList(), actionOptions);
            await inputState.Release(actionOptions);
        }

        public override void ElementClear(string elementId)
        {
            getCurrentWindowThenFocus(null);

            var elementCacheRequest = new CacheRequest();
            elementCacheRequest.Add(ValuePattern.Pattern);
            elementCacheRequest.Add(ScrollItemPattern.Pattern);
            elementCacheRequest.Add(AutomationElement.IsOffscreenProperty);
            elementCacheRequest.Add(ValuePattern.IsReadOnlyProperty);
            elementCacheRequest.Add(AutomationElement.IsEnabledProperty);
            var element = elementFinder.GetElement(elementId, elementCacheRequest);
            var pattern = element.GetCachedPattern(ValuePattern.Pattern);
            if (pattern != null && pattern is ValuePattern valuePattern)
            {
                if ((bool)element.GetCachedPropertyValue(ValuePattern.IsReadOnlyProperty))
                {
                    throw new InvalidElementState("Element is readonly");
                }
                if (!element.Cached.IsEnabled)
                {
                    throw new InvalidElementState("Element is disabled");
                }

                var sPattern = element.GetCachedPattern(ScrollItemPattern.Pattern);
                if (sPattern != null && sPattern is ScrollItemPattern scrollItemPattern)
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

        public override async Task ElementSendKeys(string elementId, JsonObject data)
        {
            data.TryGetPropertyValue("text", out var text);
            if (text == null || text.GetValueKind() != JsonValueKind.String)
            {
                throw new InvalidArgument("text must be string");
            }

            getCurrentWindowThenFocus(null);

            var elementCacheRequest = new CacheRequest();
            elementCacheRequest.Add(ScrollItemPattern.Pattern);
            elementCacheRequest.Add(AutomationElement.IsOffscreenProperty);
            elementCacheRequest.Add(AutomationElement.IsEnabledProperty);
            elementCacheRequest.Add(AutomationElement.HasKeyboardFocusProperty);
            elementCacheRequest.Add(AutomationElement.IsKeyboardFocusableProperty);
            var element = elementFinder.GetElement(elementId, elementCacheRequest);
            var pattern = element.GetCachedPattern(ScrollItemPattern.Pattern);
            if (pattern != null && pattern is ScrollItemPattern scrollItemPatern)
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
            var element = elementFinder.GetElement(elementId);
            var rect = element.Cached.BoundingRectangle;

            using (Bitmap bmp = screenCapture.CaptureElementScreenshot((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height))
            {
                return screenCapture.ConvertToBase64(bmp);
            }
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
