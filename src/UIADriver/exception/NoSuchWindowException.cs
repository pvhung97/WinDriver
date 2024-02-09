using System.Net;

namespace UIADriver.exception
{
    public class NoSuchWindowException(string message) : WebDriverException(message)
    {
        public override string GetErrorCode()
        {
            return "no such window";
        }

        public override HttpStatusCode GetStatusCode()
        {
            return HttpStatusCode.NotFound;
        }
    }
}
