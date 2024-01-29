using System.Text.Json;
using System.Text.Json.Nodes;

namespace UIA3Driver
{
    public struct SessionCapabilities
    {
        public string? appPath;
        public string? aumid;
        public int? nativeWindowHandle;
        public int delayAfterOpenApp = 3000;
        public string[] appArgument = [];
        public string? workingDirectory;
        public int maxTreeDepth = 50;
        public int delayAfterFocus = 200;

        public SessionCapabilities() {}

        public static SessionCapabilities ParseCapabilities(JsonObject data)
        {
            data.TryGetPropertyValue("windriver:appPath", out var appPath);
            var cap = new SessionCapabilities();
            if (appPath != null)
            {
                cap.appPath = appPath.ToString();
                
            }

            data.TryGetPropertyValue("windriver:aumid", out var aumid);
            if (aumid != null)
            {
                cap.aumid = aumid.ToString();
                
            }

            data.TryGetPropertyValue("windriver:nativeWindowHandle", out var nativeWindowHandle);
            if (nativeWindowHandle != null)
            {
                cap.nativeWindowHandle = nativeWindowHandle.GetValue<int>();
                
            }

            data.TryGetPropertyValue("windriver:delayAfterOpenApp", out var delayAfterOpenApp);
            if (delayAfterOpenApp != null)
            {
                cap.delayAfterOpenApp = delayAfterOpenApp.GetValue<int>();
            }

            data.TryGetPropertyValue("windriver:appArgument", out var appArgument);
            if (appArgument != null)
            {
                var arguments = JsonSerializer.Deserialize<string[]>(appArgument);
                if (arguments != null)
                {
                    cap.appArgument = arguments;
                }
            }

            data.TryGetPropertyValue("windriver:maxTreeDepth", out var maxTreeDepth);
            if (maxTreeDepth != null)
            {
                cap.maxTreeDepth = maxTreeDepth.GetValue<int>();
            }

            data.TryGetPropertyValue("windriver:delayAfterFocus", out var delayAfterFocus);
            if (delayAfterFocus != null)
            {
                cap.delayAfterFocus = delayAfterFocus.GetValue<int>();
            }

            data.TryGetPropertyValue("windriver:workingDirectory", out var workingDirectory);
            if (workingDirectory != null)
            {
                cap.workingDirectory = workingDirectory.ToString();
            }

            return cap;
        }
    }
}
