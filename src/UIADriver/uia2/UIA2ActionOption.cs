using System.Text.Json.Nodes;
using UIADriver.actions;
using UIADriver.actions.inputsource;
using System.Windows.Automation;

namespace UIADriver.uia2
{
    public class UIA2ActionOption : ActionOptions
    {
        protected AutomationElement topLevelWindow;
        protected ElementFinder elementFinder;

        public UIA2ActionOption(AutomationElement topLevelWindow, ElementFinder elementFinder)
        {
            this.topLevelWindow = topLevelWindow;
            this.elementFinder = elementFinder;
        }

        public override void AssertPositionInViewPort(int x, int y)
        {
            throw new NotImplementedException();
        }

        public override Point GetCurrentWindowLocation()
        {
            throw new NotImplementedException();
        }

        public override Point GetRelativeCoordinate(InputSource source, int xOffset, int yOffset, JsonNode origin)
        {
            throw new NotImplementedException();
        }

        public override int GetTopLevelProcessId()
        {
            throw new NotImplementedException();
        }

        public override bool IsElementOrigin(JsonNode origin)
        {
            throw new NotImplementedException();
        }
    }
}
