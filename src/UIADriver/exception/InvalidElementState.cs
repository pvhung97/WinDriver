using System.Net;

namespace UIA3Driver.exception
{
    public class InvalidElementState(string message) : WebDriverException(message)
    {
        public override string GetErrorCode()
        {
            return "invalid element state";
        }

        public override HttpStatusCode GetStatusCode()
        {
            return HttpStatusCode.NotFound;
        }
    }
}
