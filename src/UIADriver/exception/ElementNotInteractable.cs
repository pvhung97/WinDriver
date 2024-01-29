using System.Net;

namespace UIA3Driver.exception
{
    public class ElementNotInteractable(string message) : WebDriverException(message)
    {
        public override string GetErrorCode()
        {
            return "element not interactable";
        }

        public override HttpStatusCode GetStatusCode()
        {
            return HttpStatusCode.NotFound;
        }
    }
}
