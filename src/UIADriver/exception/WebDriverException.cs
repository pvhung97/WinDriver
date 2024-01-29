using System.Net;
using UIA3Driver.dto.response;

namespace UIA3Driver.exception
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
