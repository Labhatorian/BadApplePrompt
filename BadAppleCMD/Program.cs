using BadAppleCMD.Logic;
using BadAppleCMD.Screens;
using Menu = BadAppleCMD.Screens.Menu;

namespace BadAppleCMD
{
    public class Program
    {
        private static PlatformInvoke PInvoke = new();
        private static FFmpeg FFmpegExecution = new();
        private static Videoplayer VideoPlayer = new();

        private static Menu MainMenu = new();
        private static Settings SettingsMenu = new();

        //todo clear this out for release
        private static string FilePath = "C:\\Users\\Harris\\source\\repos\\BadAppleCMD\\BadAppleCMD\\bin\\Debug\\net6.0-windows10.0.22621.0\\win-x64\\Badapple.mp4";
        private static string WorkPath = "";
        public static string FrameFileExtension = "JPEG";
        public static bool Verbose = false;
        public static bool AutoStart = false;

        public static int VideoWidth { get; set; }
        public static int VideoHeight { get; set; }
        public static int VideoFrameRate { get; set; }
        public static int TotalVideoFrames { get; set; } //TODO use ffprobe to get max amount of frames

        [STAThread]
        static void Main(string[] args)
        {
            Initialise(args);
            PInvoke.PrepareConsole();

            if (AutoStart)
            {
                MakeAndPlayVideo();
            }
            else
            {
                StartMenu();
            }
        }

        private static void StartMenu()
        {
            int SelectedOption = MainMenu.MainMenu();
            if (SelectedOption == 0)
            {
                MakeAndPlayVideo();
            }
            else
            {
                SettingsMenu.SettingsPage(VideoPlayer);
                StartMenu();
            }
        }

        private static void MakeAndPlayVideo()
        {
            LoadingScreens.WriteScreen(ConsoleColor.DarkBlue, "Now Playing", FilePath);
            Thread.Sleep(2000);
            Console.Clear();

            FFmpegExecution.GetVideoInformation(FilePath);
            FFmpegExecution.GetVideoFrames(FilePath, WorkPath, FrameFileExtension);
            FFmpegExecution.GetAudio(FilePath, WorkPath);

            VideoPlayer.ResizeFrames(WorkPath);
            VideoPlayer.PrepareConsole(WorkPath);
            VideoPlayer.PlayVideo(WorkPath);

            Environment.Exit(0);
        }

        private static void Initialise(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(ExitApplication);
            Console.SetWindowSize(120, 30);//Default
            Console.SetBufferSize(120, 30);//Default
            Console.CursorVisible = false;
            WorkPath = Path.GetDirectoryName(AppContext.BaseDirectory);

            if (args.Length != 0)
            {
                //todo checks
                for (int i = 0; i < args.Length; i++)
                {
                    string argument = args[i];

                    if (argument.Equals("-FileExtension"))
                    {
                        FrameFileExtension = args[i + 1];
                    }
                    if (argument.Equals("-Factor"))
                    {
                        VideoPlayer.ResizeFactor = int.Parse(args[i + 1]);
                    }
                    if (argument.Equals("-Verbose"))
                    {
                        Verbose = true;
                    }
                    if (argument.Equals("-AutoStart"))
                    {
                        AutoStart = true;
                    }
                }

                FilePath = Path.GetDirectoryName(args[args.Length - 1])
                    + Path.DirectorySeparatorChar
                    + Path.GetFileNameWithoutExtension(args[args.Length - 1])
                    + Path.GetExtension(args[args.Length - 1]);
            }
            else
            {
                LoadingScreens.WriteScreen(ConsoleColor.DarkRed, "No file has been provided", "Select a videofile or quit");
                //FilePath = SelectFileDialog();
                //if (FilePath is null) Environment.Exit(0);
            }

            //TODO filepath check

            //Make hidden temp folder. ffmpeg can not create one
            DeleteTemp();
            DirectoryInfo di = Directory.CreateDirectory(WorkPath + "/temp");
            di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
        }

        private static string SelectFileDialog()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = WorkPath;
                openFileDialog.Filter = "All Video Media Files|*.wmv;*.avi;*.mpg;*.mpeg;*.m1v;*.mpe;*.mp4;*.mov;*.3g2;*.3gp2;*.3gp;*.3gpp;*.m4a;*.mkv;*.WMV;*.AVI;*.MPG;*.MPEG;*.M1V;*.MPE;*.MP4;*.MOV;*.3G2;*.3GP2;*.3GP;*.3GPP;*.M4A;*.MKV;";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    return openFileDialog.FileName;
                }
            }
            return null;
        }

        static void ExitApplication(object sender, EventArgs e)
        {
            VideoPlayer.Audio?.Stop();
            VideoPlayer.TotalFrameCounter = 2147483646;
            GC.Collect();

            Thread.Sleep(500);
            LoadingScreens.WriteScreen(ConsoleColor.DarkBlue, "Thank you!", "Cleaning up and exiting...");
            Thread.Sleep(500);

            GC.WaitForPendingFinalizers();
            DeleteTemp();
        }

        private static void DeleteTemp()
        {
            if (Directory.Exists(WorkPath + "/temp"))
            {
                try
                {
                    Directory.Delete(WorkPath + "/temp", true);
                }
                catch (IOException)
                {
                    //Unable to delete, file still in use
                    //Keep it, only small amount of space left.
                    //Gets overwritten next time
                }
            }
        }
    }
}