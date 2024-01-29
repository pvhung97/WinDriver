using System.Net;

namespace UIA3Driver.exception
{
    public class InvalidArgument(string message) : WebDriverException(message)
    {
        public override string GetErrorCode()
        {
            return "invalid argument";
        }

        public override HttpStatusCode GetStatusCode()
        {
            return HttpStatusCode.BadRequest;
        }
    }
}
