using UIA3Driver.win32native;

namespace UIA3Driver
{
    public class ScreenshotCapture
    {
        public Bitmap CaptureAllMonitor()
        {
            int screenLeft = SystemInformation.VirtualScreen.Left;
            int screenTop = SystemInformation.VirtualScreen.Top;
            int screenWidth = SystemInformation.VirtualScreen.Width;
            int screenHeight = SystemInformation.VirtualScreen.Height;

            var bmp = new Bitmap(screenWidth, screenHeight);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(screenLeft, screenTop, 0, 0, bmp.Size);
            }
            return bmp;
        }

        public Bitmap CaptureWindowScreenshot(nint hdl)
        {
            Win32Methods.GetWindowRect(hdl, out var rect);
            int width = rect.right - rect.left;
            int height = rect.bottom - rect.top;

            var bmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(rect.left, rect.top, 0, 0, bmp.Size);
            }
            return bmp;
        }

        public Bitmap CaptureElementScreenshot(int x, int y, int width, int height)
        {
            var bmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(x, y, 0, 0, bmp.Size);
            }
            return bmp;
        }

        public string ConvertToBase64(Bitmap bitmap)
        {
            using var ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return Convert.ToBase64String(ms.ToArray());
        }
    }
}
