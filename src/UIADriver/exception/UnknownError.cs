using System.Net;

namespace UIA3Driver.exception
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
