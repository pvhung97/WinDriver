using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UIADriver.win32native
{
    [ComImport, Guid("45BA127D-10A8-46EA-8AB7-56EA9078943C")]
    public class ApplicationActivationManager : IApplicationActivationManager
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public extern IntPtr ActivateApplication([In] string appUserModelId, [In] string arguments, [In] ActivateOptions options, [Out] out uint processId);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public extern IntPtr ActivateForFile([In] string appUserModelId, [In] IntPtr /*IShellItemArray*/ itemArray, [In] string verb, [Out] out uint processId);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public extern IntPtr ActivateForProtocol([In] string appUserModelId, [In] IntPtr /*IShellItemArray*/ itemArray, [Out] out uint processId);
    }

    [ComImport, Guid("2E941141-7F97-4756-BA1D-9DECDE894A3D"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IApplicationActivationManager
    {
        IntPtr ActivateApplication([In] string appUserModelId, [In] string arguments, [In] ActivateOptions options, [Out] out uint processId);
        IntPtr ActivateForFile([In] string appUserModelId, [In] IntPtr /*IShellItemArray*/ itemArray, [In] string verb, [Out] out uint processId);
        IntPtr ActivateForProtocol([In] string appUserModelId, [In] IntPtr /*IShellItemArray*/ itemArray, [Out] out uint processId);
    }

    [Flags]
    public enum ActivateOptions
    {
        None = 0x00000000,  // No flags set
        DesignMode = 0x00000001,  // The application is being activated for design mode, and thus will not be able to
                                  // to create an immersive window. Window creation must be done by design tools which
                                  // load the necessary components by communicating with a designer-specified service on
                                  // the site chain established on the activation manager.  The splash screen normally
                                  // shown when an application is activated will also not appear.  Most activations
                                  // will not use this flag.
        NoErrorUI = 0x00000002,  // Do not show an error dialog if the app fails to activate.                                
        NoSplashScreen = 0x00000004,  // Do not show the splash screen when activating the app.
    }

}
