using System.Net;

namespace UIADriver.exception
{
    public class StaleElementReference(string message) : WebDriverException(message)
    {
        public override string GetErrorCode()
        {
            return "stale element reference";
        }

        public override HttpStatusCode GetStatusCode()
        {
            return HttpStatusCode.NotFound;
        }
    }
}
