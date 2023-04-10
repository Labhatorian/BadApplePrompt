using System.Runtime.InteropServices;

namespace BadAppleCMD.Logic
{
    public class PlatformInvoke
    {
        //Console and its menu
        public IntPtr handle = GetConsoleWindow();
        public IntPtr sysMenu;

        //Hexvalues of different options to apply
        private const int MF_BYCOMMAND = 0x00000000;
        public const int MF_ENABLED = 0x00000000;
        private const int MF_DISABLED = 0x00000002;

        //Hexvalues of window menu options
        public const int SC_MINIMIZE = 0xF020;
        public const int SC_MAXIMIZE = 0xF030;
        public const int SC_SIZE = 0xF000;

        //Hexvalue for disabling Quick-Edit
        const int STD_INPUT_HANDLE = -10;
        const int ENABLE_QUICK_EDIT = 0x0040;

        //Menu editing
        [DllImport("user32.dll")]
        static extern bool ModifyMenuA(IntPtr hMenu, uint uPosition, uint uFlags, IntPtr uIDNewItem);

        [DllImport("user32.dll")]
        public static extern bool DrawMenuBar(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        //Quick edit
        [DllImport("kernel32.dll")]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, int mode);

        [DllImport("kernel32.dll")]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out int mode);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetStdHandle(int handle);

        public void PrepareConsole()
        {
            sysMenu = GetSystemMenu(handle, false);

            if (handle == IntPtr.Zero) return;
            ModifyMenuA(sysMenu, SC_MAXIMIZE, MF_BYCOMMAND | MF_DISABLED, sysMenu);
            ModifyMenuA(sysMenu, SC_MINIMIZE, MF_BYCOMMAND | MF_DISABLED, sysMenu);
            ModifyMenuA(sysMenu, SC_SIZE, MF_BYCOMMAND | MF_DISABLED, sysMenu);
            DrawMenuBar(handle);

            //Disable Quick-Edit mode and selecting
            IntPtr consoleHandle = GetStdHandle(STD_INPUT_HANDLE);
            if (consoleHandle == IntPtr.Zero) return;
            GetConsoleMode(consoleHandle, out int mode);
            mode &= ~ENABLE_QUICK_EDIT;
            SetConsoleMode(consoleHandle, mode);
        }
    }
}