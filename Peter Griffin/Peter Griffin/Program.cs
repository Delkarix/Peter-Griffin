using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

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

        [DllImport("gdi32.dll")]
        static extern IntPtr CreateDC(string lpszDriver, string lpszDevice, string lpszOutput, IntPtr lpInitData);

        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC", SetLastError = true)]
        static extern IntPtr CreateCompatibleDC([In] IntPtr hdc);

        [DllImport("gdi32.dll", EntryPoint = "BitBlt", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool BitBlt([In] IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, [In] IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

        [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
        public static extern IntPtr SelectObject([In] IntPtr hdc, [In] IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        static extern int GetObject(IntPtr hgdiobj, int cbBuffer, out BITMAP lpvObject);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteDC([In] IntPtr hdc);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        [DllImport("gdi32.dll")]
        static extern bool StretchBlt(IntPtr hdcDest, int nXOriginDest, int nYOriginDest, int nWidthDest, int nHeightDest, IntPtr hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc, TernaryRasterOperations dwRop);

        [StructLayout(LayoutKind.Sequential)]
        private struct BITMAP
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

        /// <summary>
        ///     Specifies a raster-operation code. These codes define how the color data for the
        ///     source rectangle is to be combined with the color data for the destination
        ///     rectangle to achieve the final color.
        /// </summary>
        enum TernaryRasterOperations : uint
        {
            /// <summary>dest = source</summary>
            SRCCOPY = 0x00CC0020,
            /// <summary>dest = source OR dest</summary>
            SRCPAINT = 0x00EE0086,
            /// <summary>dest = source AND dest</summary>
            SRCAND = 0x008800C6,
            /// <summary>dest = source XOR dest</summary>
            SRCINVERT = 0x00660046,
            /// <summary>dest = source AND (NOT dest)</summary>
            SRCERASE = 0x00440328,
            /// <summary>dest = (NOT source)</summary>
            NOTSRCCOPY = 0x00330008,
            /// <summary>dest = (NOT src) AND (NOT dest)</summary>
            NOTSRCERASE = 0x001100A6,
            /// <summary>dest = (source AND pattern)</summary>
            MERGECOPY = 0x00C000CA,
            /// <summary>dest = (NOT source) OR dest</summary>
            MERGEPAINT = 0x00BB0226,
            /// <summary>dest = pattern</summary>
            PATCOPY = 0x00F00021,
            /// <summary>dest = DPSnoo</summary>
            PATPAINT = 0x00FB0A09,
            /// <summary>dest = pattern XOR dest</summary>
            PATINVERT = 0x005A0049,
            /// <summary>dest = (NOT dest)</summary>
            DSTINVERT = 0x00550009,
            /// <summary>dest = BLACK</summary>
            BLACKNESS = 0x00000042,
            /// <summary>dest = WHITE</summary>
            WHITENESS = 0x00FF0062,
            /// <summary>
            /// Capture window as seen on screen.  This includes layered windows
            /// such as WPF windows with AllowsTransparency="true"
            /// </summary>
            CAPTUREBLT = 0x40000000
        }

        public static void Glitch()
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
                        IntPtr hdc = GetDC(proc.MainWindowHandle);//CreateDC("DISPLAY", null, null, IntPtr.Zero);
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
                            StretchBlt(hdc, RNG.Next(0, width), RNG.Next(0, height), RNG.Next(0, width), RNG.Next(0, height), hdcMem, 0, 0, bmp.Width, bmp.Height, TernaryRasterOperations.MERGECOPY); //MERGECOPY, MERGEPAINT, SRCAND, SRCERASE
                            StretchBlt(hdc, RNG.Next(0, width), RNG.Next(0, height), RNG.Next(0, width), RNG.Next(0, height), hdcMem, 0, 0, bmp.Width, bmp.Height, TernaryRasterOperations.MERGEPAINT); //MERGECOPY, MERGEPAINT, SRCAND, SRCERASE
                            StretchBlt(hdc, RNG.Next(0, width), RNG.Next(0, height), RNG.Next(0, width), RNG.Next(0, height), hdcMem, 0, 0, bmp.Width, bmp.Height, TernaryRasterOperations.SRCAND); //MERGECOPY, MERGEPAINT, SRCAND, SRCERASE
                            StretchBlt(hdc, RNG.Next(0, width), RNG.Next(0, height), RNG.Next(0, width), RNG.Next(0, height), hdcMem, 0, 0, bmp.Width, bmp.Height, TernaryRasterOperations.SRCERASE); //MERGECOPY, MERGEPAINT, SRCAND, SRCERASE
                        }

                        //BitBlt(hdc, 0, 0, bmp.Width, bmp.Height, hdcMem, 0, 0, 0x00CC0020);

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
