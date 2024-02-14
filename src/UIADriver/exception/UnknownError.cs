using System.Net;

namespace UIADriver.exception
{
    public class UnknownError(string message) : WebDriverException(message)
    {
        public override string GetErrorCode()
        {
            return "unknown error";
        }

        public override HttpStatusCode GetStatusCode()
        {
            return HttpStatusCode.InternalServerError;
        }
    }
}
