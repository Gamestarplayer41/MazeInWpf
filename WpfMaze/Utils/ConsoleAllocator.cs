using System;
using System.Runtime.InteropServices;

namespace WpfMaze.Utils
{
    public static class ConsoleAllocator
    {
        private const int SwHide = 0;
        private const int SwShow = 5;

        [DllImport(@"kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        [DllImport(@"kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport(@"user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);


        public static void ShowConsoleWindow()
        {
            IntPtr handle = GetConsoleWindow();

            if (handle == IntPtr.Zero)
                AllocConsole();
            else
                ShowWindow(handle, SwShow);
        }

        public static void HideConsoleWindow()
        {
            IntPtr handle = GetConsoleWindow();
            ShowWindow(handle, SwHide);
        }
    }
}