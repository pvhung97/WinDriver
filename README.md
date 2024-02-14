# WinDriver

Windriver is a driver to support testing on Windows Application (UWP, WinForms, WPF, Win32 App). This driver was designed to follow [W3C WebDriver protocol](https://www.w3.org/TR/webdriver/) as much as it can.

This driver using [UI Automation](https://learn.microsoft.com/en-us/dotnet/framework/ui-automation/ui-automation-overview) as its core. Some of the ideas are from [FlaUI](https://github.com/FlaUI/FlaUI), [Selenium IEDriver](https://github.com/SeleniumHQ/selenium/tree/trunk/cpp/iedriver)

This driver is divided into 2 application WindowsDriver and UIADriver. WindowsDriver will manage session, routing request to corresponding UIADriver session. UIADriver is the automation core. The reason for this is to support both UIA2 and UIA3.

# Run WinDriver

Require .NET Framework 4.8.1 Runtime to run

Use bellow command to start:
```
WindowsDriver.exe --urls http://127.0.0.1:5000
```

# Sample Test Script
You can find some sample code inside [WindowsDriverSample](./src/WindowsDriverSample/)

# Supported Capabilities

| Capabilities                 	| Description                                                          	| Default Value 	|
|------------------------------	|----------------------------------------------------------------------	|---------------	|
| platformName                  | Must be "windows"                                                    	|               	|
| windriver:automationName     	| Must be "uia3" or "uia2"                                              |               	|
| windriver:appPath            	| Executable path                                                      	|               	|
| windriver:aumid              	| Window store application's aumid                                     	|               	|
| windriver:nativeWindowHandle 	| NativeWindowHandle of top level window to inject                     	|               	|
| windriver:appArgument        	| App argument as string array                                         	| []            	|
| windriver:workingDirectory   	| Application working directory (use with app path only)               	|               	|
| windriver:delayAfterOpenApp  	| Delay after open app (ms)                                            	| 3000          	|
| windriver:maxTreeDepth       	| Maximum depth for automation tree, useful when use with root session 	| 50            	|
| windriver:commandTimeout     	| Timeout for command (ms)                                             	| 100000        	|

# Supported Locators

| Locator Strategy 	| Mapped Attributes                                 	|
|------------------	|---------------------------------------------------	|
| automation id    	| AutomationId                                      	|
| id               	| RuntimeId (Join all number in array with ',')     	|
| name             	| Name                                              	|
| tag name         	| ControlType (UIA_ToolBarControlTypeId -> ToolBar) 	|
| xpath            	|                                                   	|

# Element Attribute

List of element attribute can be found [here](https://learn.microsoft.com/en-us/windows/win32/winauto/uiauto-automation-element-propids)

Element attribute name example: UIA_AcceleratorKeyPropertyId -> AcceleratorKey

Attribute specific for a control pattern can be found [here](https://learn.microsoft.com/en-us/windows/win32/winauto/uiauto-control-pattern-propids)

Specific pattern attribute example: UIA_ValueValuePropertyId -> Value_Value

# Supported API

| Method 	| Path                                                        	|
|--------	|-------------------------------------------------------------	|
| GET    	| /status                                                     	|
| POST   	| /session                                                    	|
| DELETE 	| /session/{id}/title                                         	|
| GET    	| /session/{id}/window                                        	|
| DELETE 	| /session/{id}/window                                        	|
| GET    	| /session/{id}/title                                         	|
| POST   	| /session/{id}/window                                        	|
| GET    	| /session/{id}/window/handles                                	|
| GET    	| /session/{id}/window/rect                                   	|
| POST   	| /session/{id}/window/rect                                   	|
| POST   	| /session/{id}/window/minimize                               	|
| POST   	| /session/{id}/window/maximize                               	|
| GET    	| /session/{id}/window/screenshot                             	|
| GET    	| /session/{id}/window/source                                 	|
| POST   	| /session/{id}/element                                       	|
| POST   	| /session/{id}/elements                                      	|
| POST   	| /session/{id}/element/{elementId}/element                   	|
| POST   	| /session/{id}/element/{elementId}/elements                  	|
| GET    	| /session/{id}/element/active                                	|
| GET    	| /session/{id}/element/{elementId}/attribute/{attributeName} 	|
| GET    	| /session/{id}/element/{elementId}/name                      	|
| GET    	| /session/{id}/element/{elementId}/rect                      	|
| GET    	| /session/{id}/element/{elementId}/text                      	|
| GET    	| /session/{id}/element/{elementId}/selected                  	|
| GET    	| /session/{id}/element/{elementId}/enabled                   	|
| GET    	| /session/{id}/element/{elementId}/displayed                 	|
| POST   	| /session/{id}/actions                                       	|
| DELETE 	| /session/{id}/actions                                       	|
| POST   	| /session/{id}/element/{elementId}/click                     	|
| POST   	| /session/{id}/element/{elementId}/clear                     	|
| POST   	| /session/{id}/element/{elementId}/value                     	|
| GET    	| /session/{id}/element/{elementId}/screenshot                	|
