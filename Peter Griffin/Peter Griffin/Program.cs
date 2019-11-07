using System;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;

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

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr CreateWindowExA(int dwExStyle, [MarshalAs(UnmanagedType.LPStr)] string lpClassName, [MarshalAs(UnmanagedType.LPStr)] string lpWindowName, int dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32")]
        public static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowsProc lpEnumFunc, string lParam);

        [DllImport("user32")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, string lParam);

        [DllImport("user32")]
        public static extern IntPtr GetDesktopWindow();
        
        [DllImport("user32")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32")]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        public delegate bool EnumWindowsProc(IntPtr hWnd, string lParam);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAP
        {
            public int bmType;
            public int bmWidth;
            public int bmHeight;
            public int bmWidthBytes;
            public int bmPlanes;
            public int bmBitsPixel;
            public IntPtr bmBits;
        }

        public static int width = GetSystemMetrics(0);
        public static int height = GetSystemMetrics(1);

        public static bool callback(IntPtr hwnd, string lParam)
        {
            int length = GetWindowTextLength(hwnd);
            StringBuilder lpString = new StringBuilder(length + 1);
            GetWindowText(hwnd, lpString, lpString.Capacity);
            string text = lpString.ToString();

            // Will not redraw the text if it is already "hey lois". This should hopefully increase the speed of the code.
            if (text != "hey lois")
            {
                SendMessage(hwnd, 12, IntPtr.Zero, "hey lois");
            }
            return true;
        }

        public static void Peter()
        {
            Random RNG = new Random();
            while (true)
            {
                foreach (Process proc in Process.GetProcesses())
                {
                    if (proc.MainWindowHandle != IntPtr.Zero)
                    {
                        IntPtr hdc = GetDC(proc.MainWindowHandle);
                        IntPtr hdcMem = CreateCompatibleDC(hdc);
                        IntPtr hBitmap = Properties.Resources._220px_Peter_Griffin_bmp.GetHbitmap();

                        IntPtr oldBitmap = SelectObject(hdcMem, hBitmap);
                        GetObject(hBitmap, Marshal.SizeOf<BITMAP>(), out BITMAP bmp);

                        for (int i = 0; i < 2; i++)
                        {
                            POINT point1 = new POINT() { x = RNG.Next(0, width), y = RNG.Next(0, height) };
                            POINT point2 = new POINT() { x = RNG.Next(0, width), y = RNG.Next(0, height) };
                            POINT point3 = new POINT() { x = RNG.Next(0, width), y = RNG.Next(0, height) };

                            PlgBlt(hdc, new POINT[] { point1, point2, point3 }, hdcMem, 0, 0, bmp.bmWidth, bmp.bmHeight, IntPtr.Zero, 0, 0);
                            StretchBlt(hdc, RNG.Next(0, width), RNG.Next(0, height), RNG.Next(0, width), RNG.Next(0, height), hdcMem, 0, 0, bmp.bmWidth, bmp.bmHeight, 0x00C000CA); // MERGECOPY
                            StretchBlt(hdc, RNG.Next(0, width), RNG.Next(0, height), RNG.Next(0, width), RNG.Next(0, height), hdcMem, 0, 0, bmp.bmWidth, bmp.bmHeight, 0x00BB0226); // MERGEPAINT
                            StretchBlt(hdc, RNG.Next(0, width), RNG.Next(0, height), RNG.Next(0, width), RNG.Next(0, height), hdcMem, 0, 0, bmp.bmWidth, bmp.bmHeight, 0x008800C6); // SRCAND
                            StretchBlt(hdc, RNG.Next(0, width), RNG.Next(0, height), RNG.Next(0, width), RNG.Next(0, height), hdcMem, 0, 0, bmp.bmWidth, bmp.bmHeight, 0x00440328); // SRCERASE
                        }

                        SelectObject(hdcMem, oldBitmap);
                        DeleteDC(hdcMem);
                        DeleteObject(hBitmap);
                    }
                }
            }
        }

        public static void hey_lois()
        {
            Random RNG = new Random();
            while (true)
            {
                Thread.Sleep(500);
                CreateWindowExA(0, "Static", "hey lois", /*WS_VISIBLE*/0x10000000, RNG.Next(0, width), RNG.Next(0, height), 0, 0, IntPtr.Zero, IntPtr.Zero, GetWindowLong(IntPtr.Zero, -6), IntPtr.Zero);
                EnumChildWindows(GetDesktopWindow(), callback, null);
            }
        }

        public static void Main(string[] args)
        {
            Thread th = new Thread(Peter);
            Thread th2 = new Thread(hey_lois);
            th.Start();
            th2.Start();
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
