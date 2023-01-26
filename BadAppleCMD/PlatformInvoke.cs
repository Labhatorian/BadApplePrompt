using System.Runtime.InteropServices;

namespace BadAppleCMD
{
    public class PlatformInvoke
    {
        //Console and its menu
        private IntPtr handle = GetConsoleWindow();
        private IntPtr sysMenu;

        //Hex values for different options
        private const int MF_BYCOMMAND = 0x00000000;
        private const int MF_ENABLED = 0x0;
        private const int MF_GRAYED = 0x1;
        private const int MF_DISABLED = 0x2;

        //Values for window
        public const int SC_CLOSE = 0xF060;
        public const int SC_MINIMIZE = 0xF020;
        public const int SC_MAXIMIZE = 0xF030;
        public const int SC_SIZE = 0xF000;

        //Values for quick-edit
        const int STD_INPUT_HANDLE = -10;
        const int ENABLE_QUICK_EDIT = 0x0040;

        //TODO DISABLE NOT DELETE
        //For disabling console resize
        [DllImport("user32.dll")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        [DllImport("user32.dll")]
        static extern bool DrawMenuBar(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        //For disabling quick-edit
        [DllImport("kernel32.dll")]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, int mode);

        [DllImport("kernel32.dll")]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out int mode);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetStdHandle(int handle);


        public void PrepareConsole()
        {
            sysMenu = GetSystemMenu(handle, false);

            //Disable resizing the window
            if (handle != IntPtr.Zero)
            {
                EnableMenuItem(sysMenu, SC_CLOSE, MF_BYCOMMAND | MF_GRAYED);
                EnableMenuItem(sysMenu, SC_MAXIMIZE, MF_BYCOMMAND | MF_GRAYED);
                EnableMenuItem(sysMenu, SC_MINIMIZE, MF_BYCOMMAND | MF_GRAYED);
                EnableMenuItem(sysMenu, SC_SIZE, MF_BYCOMMAND | MF_DISABLED);
            }

            //Disable Quick-Edit mode and selecting
            int mode;
            //handle = GetStdHandle(STD_INPUT_HANDLE);

            if (handle != IntPtr.Zero)
            {
                GetConsoleMode(handle, out mode);
                mode &= ~ENABLE_QUICK_EDIT;
                SetConsoleMode(handle, mode);
            }
        }

        public void EnableCloseButton()
        {
            //Enable closing again
            EnableMenuItem(sysMenu, SC_CLOSE, MF_BYCOMMAND | MF_ENABLED);
        }
    }
}
