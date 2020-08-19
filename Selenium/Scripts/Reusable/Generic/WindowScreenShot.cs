using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Threading;

namespace Selenium.Scripts.Reusable.Generic
{
    internal class WindowScreenShot
    {
        public void CaptureApplication(string procName, string dest)
        {
            var rect = new User32.Rect();
            User32.GetWindowRect(Process.GetProcessById(getProcess(procName)).MainWindowHandle, ref rect);

            int width = rect.right - rect.left;
            int height = rect.bottom - rect.top;

            var bmp = new Bitmap(width, height - 10, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(bmp);
            graphics.CopyFromScreen(rect.left, rect.top, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);

            bmp.Save(dest, ImageFormat.Png);
        }

        private int getProcess(string proccesName)
        {
            Process[] localByName = Process.GetProcessesByName(proccesName);

            TestStack.White.Application application;
            for (int i = 0; i < localByName.Length; i++)
            {
                application = TestStack.White.Application.Attach(localByName[i]);

                Thread.Sleep(500);

                if ((application.GetWindows().Count > 0))
                {
                    return application.Process.Id;
                }
                else
                {
                    application = null;
                }
            }
            return 0;
        }
    }

    internal class User32
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
    }
}