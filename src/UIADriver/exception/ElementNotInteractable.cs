using System.Net;

namespace UIADriver.exception
{
    public class ElementNotInteractable(string message) : WebDriverException(message)
    {
        public override string GetErrorCode()
        {
            return "element not interactable";
        }

        public override HttpStatusCode GetStatusCode()
        {
            return HttpStatusCode.BadRequest;
        }
    }
}
