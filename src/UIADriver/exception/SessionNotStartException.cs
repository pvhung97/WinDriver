using System.Net;

namespace UIADriver.exception
{
    public class SessionNotStartException(string message) : WebDriverException(message)
    {
        public override string GetErrorCode()
        {
            return "session not created";
        }

        public override HttpStatusCode GetStatusCode()
        {
            return HttpStatusCode.InternalServerError;
        }
    }
}
