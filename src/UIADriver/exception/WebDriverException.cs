using System.Net;
using UIADriver.dto.response;

namespace UIADriver.exception
{
    public abstract class WebDriverException : Exception
    {
        public string message;
        public WebDriverException(string message)
        {
            this.message = message;
        }

        public Error GetError()
        {
            string stacktrace = StackTrace != null ? this.StackTrace : string.Empty;
            return new Error(GetErrorCode(), message, stacktrace);
        }
        public abstract string GetErrorCode();
        public abstract HttpStatusCode GetStatusCode();
    }
}
