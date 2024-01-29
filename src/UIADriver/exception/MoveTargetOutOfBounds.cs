using System.Net;

namespace UIA3Driver.exception
{
    public class MoveTargetOutofBounds(string message) : WebDriverException(message)
    {
        public override string GetErrorCode()
        {
            return "move target out of bounds";
        }

        public override HttpStatusCode GetStatusCode()
        {
            return HttpStatusCode.InternalServerError;
        }
    }
}
