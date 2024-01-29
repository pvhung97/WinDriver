using System.Net;

namespace UIA3Driver.exception
{
    public class NoSuchElement(string message) : WebDriverException(message)
    {
        public override string GetErrorCode()
        {
            return "no such element";
        }

        public override HttpStatusCode GetStatusCode()
        {
            return HttpStatusCode.NotFound;
        }
    }
}
