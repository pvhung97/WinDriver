using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace WindowsDriver
{
    public class CapabilitiesValidator
    {
        public async Task<JsonObject?> ValidateCap(HttpContext context)
        {
            Capabilities cap = new Capabilities();
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            try
            {
                using (StreamReader reader = new StreamReader(context.Request.Body))
                {
                    string bodyContent = await reader.ReadToEndAsync();
                    cap = JsonSerializer.Deserialize<Capabilities>(bodyContent);
                }
            }
            catch (Exception)
            {
                Error err = new Error("invalid argument", "Incorrect format of capabilities");
                context.Response.StatusCode = ((int)HttpStatusCode.BadRequest);
                await context.Response.WriteAsJsonAsync(new Response(err));
                return null;
            }

            JsonObject mergedBase = new JsonObject();
            if (cap.capabilities.alwaysMatch != null)
            {
                foreach (var item in cap.capabilities.alwaysMatch)
                {
                    mergedBase[item.Key] = item.Value?.DeepClone();
                }
            }
            List<JsonObject> merged = new List<JsonObject>();
            if (cap.capabilities.firstMatch != null && cap.capabilities.firstMatch.Count > 0)
            {
                foreach (var jo in cap.capabilities.firstMatch)
                {
                    if (jo != null && jo is JsonObject jsonObj)
                    {
                        JsonObject newMerged = new JsonObject();
                        foreach (var item in mergedBase)
                        {
                            newMerged[item.Key] = item.Value?.DeepClone();
                        }

                        foreach (var item in jsonObj)
                        {
                            newMerged[item.Key] = item.Value?.DeepClone();
                        }
                        merged.Add(newMerged);
                    }
                }

            }
            else merged.Add(mergedBase);

            merged = unpackWinOptions(merged);
            if (merged.Count == 1)
            {
                var err = ValidateMergedCap(merged[0]);
                if (err != null)
                {
                    context.Response.StatusCode = ((int)HttpStatusCode.BadRequest);
                    await context.Response.WriteAsJsonAsync(new Response(err));
                    return null;
                }
                return merged[0];
            }
            else
            {
                foreach (var item in merged)
                {
                    var err = ValidateMergedCap(item);
                    if (err == null) return item;
                }
            }
            return null;
        }

        private List<JsonObject> unpackWinOptions(List<JsonObject> options)
        {
            List<JsonObject> rs = new List<JsonObject>();
            foreach (var item in options)
            {
                var hasProp = item.TryGetPropertyValue("windriver:options", out var driverOptionJson);
                if (hasProp && driverOptionJson is JsonObject driverOption)
                {
                    JsonObject unpacked = new JsonObject();
                    foreach (var entry in item)
                    {
                        if (!entry.Key.Equals("windriver:options"))
                        {
                            unpacked[entry.Key] = entry.Value?.DeepClone();
                        }
                    }

                    foreach (var entry in driverOption)
                    {
                        string key = "windriver:" + entry.Key;
                        if (!unpacked.TryGetPropertyValue(key, out var smt))
                        {
                            unpacked[key] = entry.Value?.DeepClone();
                        }
                    }
                    rs.Add(unpacked);
                }
                else rs.Add(item);
            }
            return rs;
        }

        private Error? ValidateMergedCap(JsonObject merged)
        {
            merged.TryGetPropertyValue("platformName", out var platform);
            if (platform == null || !string.Equals("windows", platform.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return new Error("invalid argument", "platformName must be windows");
            }

            merged.TryGetPropertyValue("windriver:automationName", out var automationName);
            if (automationName == null || (!string.Equals("uia2", automationName.ToString(), StringComparison.OrdinalIgnoreCase)) && !string.Equals("uia3", automationName.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return new Error("invalid argument", "windriver:automationName must be uia3 or uia2");
            }

            int count = 0;
            merged.TryGetPropertyValue("windriver:appPath", out var appPath);
            if (appPath != null)
            {
                if (!appPath.GetValueKind().Equals(JsonValueKind.String)) return new Error("invalid argument", "windriver:appPath must be string");
                count++;
            }

            merged.TryGetPropertyValue("windriver:aumid", out var aumid);
            if (aumid != null)
            {
                if (!aumid.GetValueKind().Equals (JsonValueKind.String)) return new Error("invalid argument", "windriver:aumid must be string");
                count++;
            }

            merged.TryGetPropertyValue("windriver:nativeWindowHandle", out var nativeWindowHandle);
            if (nativeWindowHandle != null)
            {
                if (!nativeWindowHandle.GetValueKind().Equals(JsonValueKind.Number)) return new Error("invalid argument", "windriver:nativeWindowHandle must be number");
                count++;
            }

            merged.TryGetPropertyValue("windriver:windowNameRegex", out var windowNameRegex);
            if (windowNameRegex != null)
            {
                if (!windowNameRegex.GetValueKind().Equals(JsonValueKind.String)) return new Error("invalid argument", "windriver:windowNameRegex must be string");
                count++;
            }

            if (count > 1) return new Error("invalid argument", "Use only 1 of these appPath, aumid, nativeWindowHandle, windowNameRegex");

            merged.TryGetPropertyValue("windriver:delayAfterOpenApp", out var delayAfterOpenApp);
            if (delayAfterOpenApp != null)
            {
                if (!delayAfterOpenApp.GetValueKind().Equals(JsonValueKind.Number)) return new Error("invalid argument", "windriver:delayAfterOpenApp must be number");
            }

            merged.TryGetPropertyValue("windriver:maxTreeDepth", out var maxTreeDepth);
            if (maxTreeDepth != null)
            {
                if (!maxTreeDepth.GetValueKind().Equals(JsonValueKind.Number)) return new Error("invalid argument", "windriver:maxTreeDepth must be number");
            }

            merged.TryGetPropertyValue("windriver:delayAfterFocus", out var delayAfterFocus);
            if (delayAfterFocus != null)
            {
                if (!delayAfterFocus.GetValueKind().Equals(JsonValueKind.Number)) return new Error("invalid argument", "windriver:delayAfterFocus must be number");
            }

            merged.TryGetPropertyValue("windriver:appArgument", out var appArgument);
            if (appArgument != null)
            {
                if (!appArgument.GetValueKind().Equals(JsonValueKind.Array)) return new Error("invalid argument", "windriver:appArgument must be string array");
                foreach (var item in appArgument.AsArray())
                {
                    if (item == null || !item.GetValueKind().Equals(JsonValueKind.String)) return new Error("invalid argument", "windriver:appArgument must be string array");
                }
            }

            merged.TryGetPropertyValue("windriver:commandTimeout", out var commandTimeout);
            if (commandTimeout != null)
            {
                if (!commandTimeout.GetValueKind().Equals(JsonValueKind.Number)) return new Error("invalid argument", "windriver:commandTimeout must be number");
            }

            merged.TryGetPropertyValue("windriver:workingDirectory", out var workingDirectory);
            if (workingDirectory != null)
            {
                if (!workingDirectory.GetValueKind().Equals(JsonValueKind.String)) return new Error("invalid argument", "windriver:workingDirectory must be string");
            }

            merged.TryGetPropertyValue("windriver:additionalPageSourcePattern", out var additionalPageSourcePattern);
            if (additionalPageSourcePattern != null)
            {
                if (!additionalPageSourcePattern.GetValueKind().Equals(JsonValueKind.Array)) return new Error("invalid argument", "windriver:additionalPageSourcePattern must be string array");
                foreach (var item in additionalPageSourcePattern.AsArray())
                {
                    if (item == null || !item.GetValueKind().Equals(JsonValueKind.String)) return new Error("invalid argument", "windriver:additionalPageSourcePattern must be string array");
                }
            }

            return null;
        }
    }
}
