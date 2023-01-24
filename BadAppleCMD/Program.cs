using System.Drawing;
using System.Drawing.Imaging;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;
using Timer = System.Timers.Timer;

namespace BadAppleCMD
{
    public class Program
    {   //Program
        private static string? strWorkPath = "";
        public static bool Verbose = false;

        //Video player
        private static int _VideoWidthColumns = 0;
        private static int _VideoHeightColumns = 0;
        private static int _framecounter = 0;
        private static int _totalframecounter = 1;
        private static string _FPS = "FPS: 0";
        private static bool _Desync = false;
        private static SoundPlayer? audio;
        //TODO Figure out best factors
        //360p -> 4x - 1080p -> 16x
        private static int _Factor = 4;

        //Video information
        public static int VideoWidth { get; set; }
        public static int VideoHeight { get; set; }
        public static int VideoFrameRate { get; set; }

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

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(ExitApplication);
            Console.SetWindowSize(120, 30);//Default
            Console.SetBufferSize(120, 30);//Default
            Console.CursorVisible = false;

            //Disable resizing the window
            IntPtr handle = GetConsoleWindow();
            IntPtr sysMenu = GetSystemMenu(handle, false);

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

            //todo clear this out for release
            string path = "C:\\Users\\Harris\\source\\repos\\BadAppleCMD\\BadAppleCMD\\bin\\Debug\\net6.0-windows10.0.22621.0\\win-x64\\Badapple.mp4";

            //todo add png option
            if (args.Length != 0)
            {
                path = Path.GetDirectoryName(args[0])
                   + Path.DirectorySeparatorChar
                   + Path.GetFileNameWithoutExtension(args[0])
                   + Path.GetExtension(args[0]);
            }
            else
            {
                //todo clear this out for release
                Screens.WriteScreen(ConsoleColor.DarkRed, "No file has been provided", "Drag and drop a video on the executable. The program will close in 5 seconds.");
                //Thread.Sleep(5000);
                //Environment.Exit(0);
            }
            //Get the path that we are workig in
            strWorkPath = Path.GetDirectoryName(AppContext.BaseDirectory);

            //Make temp hidden folder. ffmpeg can not create one on its for some reason
            DeleteTemp();
            DirectoryInfo di = Directory.CreateDirectory(strWorkPath + "/temp");
            di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

            //Show video that will get played
            Screens.WriteScreen(ConsoleColor.DarkBlue, "Now Playing", path);
            Thread.Sleep(2000);
            Console.Clear();

            #region FFmpeg
            string parameter;

            //Get information about video
            parameter = "-v error -select_streams v:0 -show_entries stream=width,height,avg_frame_rate -of default=nw=1 " + path;
            FFmpeg.ExecuteFFprobe(parameter, out Task task, "Getting information about the video...", true);
            Thread.Sleep(1000);

            //Get frames
            parameter = "-i " + path + " " + strWorkPath + "\\temp\\%08d.png";
            Console.CursorVisible = false;
            //TODO This does not work correctly on videos that are not bad apple
            //TODO resize as soon as frames get added -> FileSystemWatcher
            FFmpeg.ExecuteFFmpeg(parameter, out task, "Getting every frame from the video...");

            //Get audio
            parameter = "-i " + path + " " + strWorkPath + "\\temp\\audio.wav";
            if (File.Exists(strWorkPath + "/temp/audio.wav")) File.Delete(strWorkPath + "/temp/audio.wav"); //Prevents crash
            FFmpeg.ExecuteFFmpeg(parameter, out task, "Getting audio from the video...");
            #endregion

            #region Prepare frames and console
            int fCount = Directory.GetFiles(strWorkPath + "/temp", "*", SearchOption.TopDirectoryOnly).Length;

            //UNDONE Resize every image to its factor -> Will get removed later as it's probably slower
            Screens.WriteScreen(ConsoleColor.DarkBlue, "Resizing frames", "[LoadingBar - Work In Progress]");

            while (_totalframecounter < fCount)
            {
                Bitmap resizedImage;
                using (Bitmap image = new(strWorkPath + $"\\temp\\{_totalframecounter:00000000}.png")) resizedImage = new(image, new Size(image.Width / _Factor, image.Height / _Factor));
                resizedImage.Save(strWorkPath + $"\\temp\\{_totalframecounter:00000000}.png", ImageFormat.Png);
                _totalframecounter++;
            }
            _totalframecounter = 1;

            //Get the right window sizes. Max is dependent on user screen resolution.
            using (Bitmap image = new(strWorkPath + $"\\temp\\{1:00000000}.png"))
            {
                Console.Write(ConvertToAscii(image));
            }

            if (_VideoWidthColumns > Console.LargestWindowWidth) _VideoWidthColumns = Console.LargestWindowWidth;

            _VideoHeightColumns += 1; //For FPS Counter, and it needs one extra
            if (_VideoHeightColumns > Console.LargestWindowHeight) _VideoHeightColumns = Console.LargestWindowHeight;

            Console.SetWindowSize(_VideoWidthColumns, _VideoHeightColumns);
            Console.SetBufferSize(_VideoWidthColumns, _VideoHeightColumns);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
            #endregion

            #region Play video
            //FPS counter timer and event
            Timer timer = new(1000);
            timer.Elapsed += OnTimedEvent;

            //Enable closing again
            EnableMenuItem(sysMenu, SC_CLOSE, MF_BYCOMMAND | MF_ENABLED);

            //Play audio of video
            audio = new SoundPlayer(strWorkPath + "/temp/audio.wav");
            if (File.Exists(strWorkPath + "/temp/audio.wav")) audio?.Play();
            timer.Start();

            //Start playing video
            while (_totalframecounter < fCount)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.CursorVisible = false; //It likes to turn itself back on
                if (_framecounter != VideoFrameRate)
                {
                    using (Bitmap image = new(strWorkPath + $"\\temp\\{_totalframecounter:00000000}.png")) Console.Write(ConvertToAscii(image));
                    Console.Write(_FPS);
                    File.Delete(strWorkPath + $"\\temp\\{_totalframecounter:00000000}.png");
                    Thread.Sleep(VideoFrameRate / (_Factor / 2));
                    Console.SetCursorPosition(0, 0);

                    _framecounter++;
                    _totalframecounter++;
                }
            }
            #endregion

            Environment.Exit(0);
        }

        private static string ConvertToAscii(Bitmap image)
        {
            StringBuilder sb = new();
            int height = 0;
            using (image = new Bitmap(image))
            {
                for (int h = 0; h < image.Height; h++)
                {
                    int w;
                    for (w = 0; w < image.Width; w++)
                    {
                        Color pixelColor = image.GetPixel(w, h);
                        //Average out the RGB components to find the Gray Color
                        int red = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                        //Testing has resulted in 15 being the best value for BW videos
                        if (red > 15) sb.Append('█');
                        else sb.Append(' ');
                    }
                    sb.Append('\n');
                    h++;

                    _VideoWidthColumns = w;
                    height++;
                }
            }
            _VideoHeightColumns = height;
            sb.Length -= 1; //Last linebreak GONE so everything fits correctly in the window
            return sb.ToString();
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            _FPS = $"FPS: {_framecounter}";
            if (_framecounter < VideoFrameRate && !_Desync) _Desync = true;
            else if (_Desync) _FPS += " - Video desynced!";
            _framecounter = 0;
        }

        static void ExitApplication(object sender, EventArgs e)
        {
            audio?.Stop();
            _totalframecounter = 999999999;
            GC.Collect();

            Thread.Sleep(500);
            Screens.WriteScreen(ConsoleColor.DarkBlue, "Thank you!", "Cleaning up and exiting...");
            Thread.Sleep(500);

            GC.WaitForPendingFinalizers();
            DeleteTemp();
        }

        private static void DeleteTemp()
        {
            if (Directory.Exists(strWorkPath + "/temp"))
            {
                try
                {
                    Directory.Delete(strWorkPath + "/temp", true);
                }
                catch (IOException)
                {
                    //Some or one file is unable to get deleted. Presumably because the user just caught the program still using it.
                    //No big deal, tests resulted this in always being one file which can stay in the temp folder. Only resulting in a few megabytes of lost space.
                    //Next time it will get overwritten!
                }
            }
        }
    }
}