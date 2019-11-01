using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Peter_Griffin
{
    class Program
    {
        [DllImport("gdi32.dll")]
        static extern bool PlgBlt(IntPtr hdcDest, POINT[] lpPoint, IntPtr hdcSrc, int nXSrc, int nYSrc, int nWidth, int nHeight, IntPtr hbmMask, int xMask, int yMask);

        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern int GetSystemMetrics(int smIndex);

        [DllImport("gdi32.dll", SetLastError = true)]
        static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        static extern int GetObject(IntPtr hgdiobj, int cbBuffer, out BITMAP lpvObject);

        [DllImport("gdi32.dll")]
        static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll")]
        static extern bool StretchBlt(IntPtr hdcDest, int nXOriginDest, int nYOriginDest, int nWidthDest, int nHeightDest, IntPtr hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc, uint dwRop);

        [StructLayout(LayoutKind.Sequential)]
        struct BITMAP
        {
            public int Type;
            public int Width;
            public int Height;
            public int WidthBytes;
            public ushort Planes;
            public ushort BitsPixel;
            public IntPtr Bits;
        }

        struct POINT
        {
            public int x;
            public int y;
        }

        static void Glitch()
        {
            int width = GetSystemMetrics(0);
            int height = GetSystemMetrics(1);
            Random RNG = new Random();
            while (true)
            {
                foreach (Process proc in Process.GetProcesses())
                {
                    if (proc.MainWindowHandle != IntPtr.Zero)
                    {
                        IntPtr hdc = GetDC(proc.MainWindowHandle);
                        IntPtr hdcMem = CreateCompatibleDC(hdc);
                        IntPtr hbitmap = Properties.Resources._220px_Peter_Griffin_bmp.GetHbitmap();

                        IntPtr oldBitmap = SelectObject(hdcMem, hbitmap);
                        GetObject(hbitmap, Marshal.SizeOf<BITMAP>(), out BITMAP bmp);

                        for (int i = 0; i < 2; i++)
                        {
                            POINT point1 = new POINT() { x = RNG.Next(0, width), y = RNG.Next(0, height) };
                            POINT point2 = new POINT() { x = RNG.Next(0, width), y = RNG.Next(0, height) };
                            POINT point3 = new POINT() { x = RNG.Next(0, width), y = RNG.Next(0, height) };

                            PlgBlt(hdc, new POINT[] { point1, point2, point3 }, hdcMem, 0, 0, bmp.Width, bmp.Height, IntPtr.Zero, 0, 0);
                            StretchBlt(hdc, RNG.Next(0, width), RNG.Next(0, height), RNG.Next(0, width), RNG.Next(0, height), hdcMem, 0, 0, bmp.Width, bmp.Height, 0x00C000CA); // MERGECOPY
                            StretchBlt(hdc, RNG.Next(0, width), RNG.Next(0, height), RNG.Next(0, width), RNG.Next(0, height), hdcMem, 0, 0, bmp.Width, bmp.Height, 0x00BB0226); // MERGEPAINT
                            StretchBlt(hdc, RNG.Next(0, width), RNG.Next(0, height), RNG.Next(0, width), RNG.Next(0, height), hdcMem, 0, 0, bmp.Width, bmp.Height, 0x008800C6); // SRCAND
                            StretchBlt(hdc, RNG.Next(0, width), RNG.Next(0, height), RNG.Next(0, width), RNG.Next(0, height), hdcMem, 0, 0, bmp.Width, bmp.Height, 0x00440328); // SRCERASE
                        }

                        SelectObject(hdcMem, oldBitmap);
                        DeleteDC(hdcMem);
                        DeleteObject(hbitmap);
                    }
                }
            }
        }

        static void Main()
        {
            Thread th = new Thread(Glitch);
            th.Start();
            while (true)
            {
                if (Process.GetProcessesByName("taskmgr").Length != 0)
                {
                    Process.GetCurrentProcess().Kill();
                }
            }
        }
    }
}