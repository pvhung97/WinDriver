using System.Net;

namespace UIADriver.exception
{
    public class UnsupportedOperation(string message) : WebDriverException(message)
    {
        public override string GetErrorCode()
        {
            return "unsupported operation";
        }

        public override HttpStatusCode GetStatusCode()
        {
            return HttpStatusCode.InternalServerError;
        }
    }
}
