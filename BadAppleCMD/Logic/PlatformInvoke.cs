using System.Runtime.InteropServices;

namespace BadAppleCMD.Logic
{
    /// <summary>
    /// All logic and values for pInvoke functions are here
    /// </summary>
    public class PlatformInvoke
    {
        //Console and its menu
        private readonly IntPtr Handle = GetConsoleWindow();
        private IntPtr SysMenu;

        //Hexvalues of different options to apply
        private const int MF_BYCOMMAND = 0x00000000;
        private const int MF_DISABLED = 0x00000002;

        //Hexvalues of window menu options
        public const int SC_MINIMIZE = 0xF020;
        public const int SC_MAXIMIZE = 0xF030;
        public const int SC_SIZE = 0xF000;

        //Hexvalues for disabling Quick-Edit
        const int STD_INPUT_HANDLE = -10;
        const int ENABLE_QUICK_EDIT = 0x0040;

        //Menu editing
        [DllImport("user32.dll")]
        private static extern bool ModifyMenuA(IntPtr hMenu, uint uPosition, uint uFlags, IntPtr uIDNewItem);

        [DllImport("user32.dll")]
        private static extern bool DrawMenuBar(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        //Quick edit
        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, int mode);

        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out int mode);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetStdHandle(int handle);

        /// <summary>
        /// Disables maximise, minimise and quick-edit to prevent bugging out the video
        /// </summary>
        public void PrepareConsole()
        {
            SysMenu = GetSystemMenu(Handle, false);

            if (Handle == IntPtr.Zero) return;
            ModifyMenuA(SysMenu, SC_MAXIMIZE, MF_BYCOMMAND | MF_DISABLED, SysMenu);
            ModifyMenuA(SysMenu, SC_MINIMIZE, MF_BYCOMMAND | MF_DISABLED, SysMenu);
            ModifyMenuA(SysMenu, SC_SIZE, MF_BYCOMMAND | MF_DISABLED, SysMenu);
            DrawMenuBar(Handle);

            //Disable Quick-Edit mode and selecting
            IntPtr windowHandle = GetStdHandle(STD_INPUT_HANDLE);
            if (windowHandle == IntPtr.Zero) return;
            GetConsoleMode(windowHandle, out int mode);
            mode &= ~ENABLE_QUICK_EDIT;
            SetConsoleMode(windowHandle, mode);
        }
    }
}