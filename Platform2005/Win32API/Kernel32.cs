namespace Platform.Win32API
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    public sealed class Kernel32
    {
        public const uint CONSOLE_TEXTMODE_BUFFER = 1;
        public const int ENABLE_ECHO_INPUT = 4;
        public const int ENABLE_LINE_INPUT = 2;
        public const int ENABLE_MOUSE_INPUT = 0x10;
        public const int ENABLE_PROCESSED_INPUT = 1;
        public const int ENABLE_PROCESSED_OUTPUT = 1;
        public const int ENABLE_WINDOW_INPUT = 8;
        public const int ENABLE_WRAP_AT_EOL_OUTPUT = 2;
        public const uint FILE_SHARE_DELETE = 4;
        public const uint FILE_SHARE_READ = 1;
        public const uint FILE_SHARE_WRITE = 2;
        public const uint GENERIC_ALL = 0x10000000;
        public const uint GENERIC_EXECUTE = 0x20000000;
        public const uint GENERIC_READ = 0x80000000;
        public const uint GENERIC_WRITE = 0x40000000;
        public const int STD_ERROR_HANDLE = -12;
        public const int STD_INPUT_HANDLE = -10;
        public const int STD_OUTPUT_HANDLE = -11;

        [DllImport("kernel32.dll")]
        public static extern bool AllocConsole();
        [DllImport("kernel32.dll")]
        public static extern bool AttachConsole(int dwProcessId);
        [DllImport("kernel32.dll")]
        public static extern bool Beep(int dwFreq, int dwDuration);
        public static IntPtr CreateConsole(int width, int height, bool removeCtrlC)
        {
            int id = Process.GetCurrentProcess().Id;
            FreeConsole(id);
            AllocConsole();
            AttachConsole(id);
            SetConsoleCtrlHandler(IntPtr.Zero, removeCtrlC);
            IntPtr consoleWindow = GetConsoleWindow();
            IntPtr hConsoleOutput = CreateConsoleScreenBuffer(0xc0000000, 3, IntPtr.Zero, 1, IntPtr.Zero);
            COORD dwSize = new COORD();
            dwSize.X = (short) width;
            dwSize.Y = (short) height;
            SetConsoleScreenBufferSize(hConsoleOutput, dwSize);
            SetConsoleActiveScreenBuffer(hConsoleOutput);
            SetStdHandle(-10, hConsoleOutput);
            SetStdHandle(-11, hConsoleOutput);
            SetStdHandle(-12, hConsoleOutput);
            return consoleWindow;
        }

        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateConsoleScreenBuffer(uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwFlags, IntPtr lpScreenBufferData);
        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole(int dwProcessId);
        [DllImport("kernel32.dll")]
        public static extern int GetConsoleTitle(ref string lpConsoleTitle, int nSize);
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();
        [DllImport("kernel32.dll")]
        public static extern int GetLastError();
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetStdHandle(int nStdHandle);
        [DllImport("kernel32.dll")]
        public static extern bool SetConsoleActiveScreenBuffer(IntPtr hConsoleOutput);
        [DllImport("kernel32.dll")]
        public static extern bool SetConsoleCtrlHandler(IntPtr HandlerRoutine, bool Add);
        [DllImport("kernel32.dll")]
        public static extern bool SetConsoleMode(IntPtr hConsoleHandle, int dwMode);
        [DllImport("kernel32.dll")]
        public static extern bool SetConsoleScreenBufferSize(IntPtr hConsoleOutput, COORD dwSize);
        [DllImport("kernel32.dll")]
        public static extern uint SetErrorMode(uint uMode);
        [DllImport("kernel32.dll")]
        public static extern int SetLastError(int dwErrCode);
        [DllImport("kernel32.dll")]
        public static extern bool SetStdHandle(int nStdHandle, IntPtr hHandle);

        [StructLayout(LayoutKind.Sequential)]
        public struct COORD
        {
            public short X;
            public short Y;
        }
    }
}
