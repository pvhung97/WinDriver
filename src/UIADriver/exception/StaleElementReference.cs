using System.Net;

namespace UIA3Driver.exception
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
