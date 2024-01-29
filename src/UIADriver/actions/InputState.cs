using System.Text.Json;
using System.Text.Json.Nodes;
using UIA3Driver.actions.action;
using UIA3Driver.actions.executor;
using UIA3Driver.actions.inputsource;
using UIA3Driver.exception;
using UIADriver.actions.executor;
using Action = UIA3Driver.actions.action.Action;

namespace UIA3Driver.actions
{
    public class InputState : IDisposable
    {
        private static InputState? instance;

        private InputState() { }

        public static InputState Instance()
        {
            if (instance == null) instance = new InputState();
            return instance;
        }

        private Dictionary<string, InputSource> inputStateMap = [];
        private List<Action> inputCancelList = [];
        private Queue<List<List<Action>>> actionsQueue = [];

        public InputSource? GetInputSource(string id)
        {
            inputStateMap.TryGetValue(id, out var inputSource);
            return inputSource;
        }

        public void AddActionToCancelList(Action action)
        {
            inputCancelList.Add(action);
        }

        public List<List<Action>> ExtractActionSequence(JsonObject data, ActionOptions actionOptions) 
        {
            data.TryGetPropertyValue("actions", out var actionsNode);
            if (actionsNode == null || actionsNode.GetValueKind() != JsonValueKind.Array) 
            {
                throw new InvalidArgument("actions must be a json array");
            }
            var actionsArray = actionsNode.AsArray();
            List<List<Action>> actionsByTick = [];
            foreach (var inputSource in actionsArray)
            {
                if (inputSource == null || inputSource.GetValueKind() != JsonValueKind.Object)
                {
                    throw new InvalidArgument("actions must contain only json object");
                }
                List<Action> processed = ProcessActionSequence(inputSource.AsObject(), actionOptions);
                for (int i = 0; i < processed.Count; i++)
                {
                    if (actionsByTick.Count < i + 1)
                    {
                        actionsByTick.Add([]);
                    }
                    actionsByTick[i].Add(processed[i]);
                }
            }
            return actionsByTick;
        }

        private List<Action> ProcessActionSequence(JsonObject inputSource, ActionOptions actionOptions)
        {
            inputSource.TryGetPropertyValue("type", out var type);
            if (type == null || type.GetValueKind() != JsonValueKind.String)
            {
                throw new InvalidArgument("type must be a string");
            }
            switch(type.ToString())
            {
                case "key":
                case "pointer":
                case "wheel":
                case "none":
                    break;
                default:
                    throw new InvalidArgument("type can only be one of these: key, pointer, wheel, none");
            }

            inputSource.TryGetPropertyValue("id", out var id);
            if (id == null || id.GetValueKind() != JsonValueKind.String)
            {
                throw new InvalidArgument("id must be a string");
            }

            inputSource.TryGetPropertyValue("parameters", out var parameters);
            string pointerParameter = ProcessPointerParameter(parameters);

            var source = GetOrCreateInputSource(id.ToString(), type.ToString(), pointerParameter);
            if (parameters != null && source is PointerInputSource pointerSource && !pointerSource.subtype.Equals(pointerParameter))
            {
                throw new InvalidArgument("Invalid input source, subtype not match");
            }

            inputSource.TryGetPropertyValue("actions", out var actionItems);
            if (actionItems == null || actionItems.GetValueKind() != JsonValueKind.Array) 
            {
                throw new InvalidArgument("actions must be an json array");
            }
            var actionItemsArr = actionItems.AsArray();

            var rs = new List<Action>();
            foreach (var actionItem in actionItemsArr)
            {
                if (actionItem == null || actionItem.GetValueKind() != JsonValueKind.Object)
                {
                    throw new InvalidArgument("actions must contain only json object");
                }

                Action? action = null;
                switch (type.ToString())
                {
                    case "none":
                        action = NullAction.ProcessAction(id.ToString(), actionItem.AsObject());
                        break;
                    case "key":
                        action = KeyAction.ProcessAction(id.ToString(), actionItem.AsObject());
                        break;
                    case "pointer":
                        action = PointerAction.ProcessAction(id.ToString(), pointerParameter, actionItem.AsObject(), actionOptions);
                        break;
                    case "wheel":
                        action = WheelAction.processAction(id.ToString(), actionItem.AsObject(), actionOptions);
                        break;
                }
                if (action != null) rs.Add(action);
            }
            return rs;
        }

        private string ProcessPointerParameter(JsonNode? parameters)
        {
            string parameter = "mouse";
            if (parameters == null) return parameter;
            if (parameters.GetValueKind() != JsonValueKind.Object) 
            {
                throw new InvalidArgument("parameters must be a json object");
            }
            parameters.AsObject().TryGetPropertyValue("pointerType", out var pointerType);
            if (pointerType == null) return parameter;
            if (pointerType.GetValueKind() != JsonValueKind.String)
            {
                throw new InvalidArgument("pointerType must be string");
            }
            switch (pointerType.ToString())
            {
                case "mouse":
                case "pen":
                case "touch":
                    return pointerType.ToString();
                default:
                    throw new InvalidArgument("pointerType can only be one of these: mouse, pen, touch");
            }
        }

        private int getPointerId(string subtype)
        {
            if (subtype.Equals("mouse")) return 0;

            int minimumId = 2;
            List<int> ids = [];
            foreach (var source in inputStateMap.Values)
            {
                if (source is PointerInputSource pointerSource)
                {
                    ids.Add(pointerSource.pointerId);
                }
            }
            while (ids.Contains(minimumId))
            {
                minimumId++;
                if (minimumId == 100)
                {
                    throw new InvalidArgument("Out of pointerid to assign");
                }
            }
            return minimumId;
        }

        public InputSource CreateInputSource(string inputId, string type, string subtype)
        {
            InputSource inputSource;
            switch (type)
            {
                case "none":
                    inputSource = new NullInputSource();
                    break;
                case "key":
                    inputSource = new KeyInputSource();
                    break;
                case "pointer":
                    inputSource = new PointerInputSource(subtype, getPointerId(subtype));
                    break;
                case "wheel":
                    inputSource = new WheelInpoutSource();
                    break;
                default:
                    throw new InvalidArgument("Unsupported input source " + type);
            }
            inputStateMap[inputId] = inputSource;
            return inputSource;
        }

        private InputSource GetOrCreateInputSource(string inputId, string type, string subtype)
        {
            inputStateMap.TryGetValue(inputId, out var inputSource);
            if (inputSource != null && !inputSource.GetSourceType().Equals(type))
            {
                throw new InvalidArgument("Invalid input source, an input source with same id with different type already exists on input state map");
            }
            if (inputSource == null)
            {
                inputSource = CreateInputSource(inputId, type, subtype);
            }
            return inputSource;
        }

        public async Task DispatchAction(List<List<Action>> actionByTicks, ActionOptions option)
        {
            actionsQueue.Enqueue(actionByTicks);
            while (actionsQueue.Peek() != actionByTicks)
            {
                Thread.Sleep(50);
            }

            try
            {
                foreach (var tick in actionByTicks)
                {
                    await DispatchTickAction(tick, option);
                }
            } finally
            {
                actionsQueue.Dequeue();
            }
        }

        public async Task Release(ActionOptions option)
        {
            List<Action> toRelease = [.. inputCancelList];
            toRelease.Reverse();
            await DispatchTickAction(toRelease, option);
            foreach (var item in inputStateMap)
            {
                if (item.Value is IDisposable disposeable) 
                { 
                    disposeable.Dispose();
                }
            }
            inputStateMap.Clear();
            inputCancelList.Clear();
            actionsQueue.Clear();
        }

        private async Task DispatchTickAction(List<Action> tick, ActionOptions option)
        {
            List<Task> allActionCompleted = [];
            int duration = CalculateTickDuration(tick);
            foreach (var action in tick)
            {
                var source = inputStateMap[action.id];
                switch (action.subtype) 
                {
                    case "pause":
                        allActionCompleted.Add(DispatchPauseAction((PauseAction) action));
                        break;
                    case "keyDown":
                        allActionCompleted.Add(DispatchKeyDownAction((KeyAction)action, option));
                        break;
                    case "keyUp":
                        allActionCompleted.Add(DispatchKeyUpAction((KeyAction)action, option));
                        break;
                    case "pointerDown":
                        allActionCompleted.Add(DispatchPointerDownAction((PointerUpDownAction)action, option));
                        break;
                    case "pointerUp":
                        allActionCompleted.Add(DispatchPointerUpAction((PointerUpDownAction)action, option));
                        break;
                    case "pointerMove":
                        allActionCompleted.Add(DispatchPointerMoveAction((PointerMoveAction)action, option, duration));
                        break;
                    case "scroll":
                        allActionCompleted.Add(DispatchScrollAction((WheelAction)action, option, duration));
                        break;
                }
            }
            await Task.WhenAll(allActionCompleted);
        }

        private int CalculateTickDuration(List<Action> actions)
        {
            int duration = 0;
            foreach (var action in actions)
            {
                if (action is PauseAction p)
                {
                    if (p.duration > duration) duration = p.duration;
                } 
                else if (action is PointerMoveAction pm)
                {
                    if (pm.duration != null && pm.duration > duration) duration = (int)pm.duration;
                }
                else if (action is WheelAction wheel)
                {
                    if (wheel.duration != null && wheel.duration > duration) duration = (int)wheel.duration;
                }
            }
            return duration;
        }

        private Task DispatchPauseAction(PauseAction action)
        {
            return Task.Run(() => PauseActionExecutor.Pause(action.duration));
        }

        private Task DispatchKeyDownAction(KeyAction action, ActionOptions option)
        {
            return Task.Run(() => KeyActionExecutor.KeyDown(action, option));
        }

        private Task DispatchKeyUpAction(KeyAction action, ActionOptions option)
        {
            return Task.Run(() => KeyActionExecutor.KeyUp(action, option));
        }

        private Task DispatchPointerDownAction(PointerUpDownAction action, ActionOptions option)
        {
            return Task.Run(() =>
            {
                switch (action.pointerType)
                {
                    case "mouse":
                        MouseActionExecutor.MouseDown(action);
                        break;
                    case "touch":
                        TouchActionExecutor.PoiterUpDown(action, option, false);
                        break;
                    case "pen":
                        PenActionExecutor.PoiterUpDown(action, option, false);
                        break;
                }
            });
        }

        private Task DispatchPointerUpAction(PointerUpDownAction action, ActionOptions option)
        {
            return Task.Run(() => {
                switch (action.pointerType)
                {
                    case "mouse":
                        MouseActionExecutor.MouseUp(action);
                        break;
                    case "touch":
                        TouchActionExecutor.PoiterUpDown(action, option, true);
                        break;
                    case "pen":
                        PenActionExecutor.PoiterUpDown(action, option, true);
                        break;
                }
            });
        }

        private Task DispatchPointerMoveAction(PointerMoveAction action, ActionOptions option, int tickDuration)
        {
            return Task.Run(() => {
                switch (action.pointerType)
                {
                    case "mouse":
                        MouseActionExecutor.MouseMove(action, option, tickDuration);
                        break;
                    case "touch":
                        TouchActionExecutor.PointerMove(action, option, tickDuration);
                        break;
                    case "pen":
                        PenActionExecutor.PointerMove(action, option, tickDuration);
                        break;
                }
            });
        }

        private Task DispatchScrollAction(WheelAction action, ActionOptions option, int tickDuration)
        {
            return Task.Run(() => ScrollActionExecutor.Scroll(action, option, tickDuration));
        }

        public void Dispose()
        {
            foreach (var item in inputStateMap)
            {
                if (item.Value is IDisposable disposable) disposable.Dispose();
            }
        }
    }
}
