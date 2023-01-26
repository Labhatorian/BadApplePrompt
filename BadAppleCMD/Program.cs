namespace BadAppleCMD
{
    public class Program
    {
        private static PlatformInvoke PInvoke = new();
        private static FFmpeg FFmpegExecution = new();
        private static Videoplayer VideoPlayer = new();
        //todo clear this out for release
        private static string FilePath = "C:\\Users\\Harris\\source\\repos\\BadAppleCMD\\BadAppleCMD\\bin\\Debug\\net6.0-windows10.0.22621.0\\win-x64\\Badapple.mp4";
        private static string WorkPath = "";
        public static bool Verbose = false;

        public static int VideoWidth { get; set; }
        public static int VideoHeight { get; set; }
        public static int VideoFrameRate { get; set; }
        public static int TotalVideoFrames { get; set; } //TODO use ffprobe to get max amount of frames

        static void Main(string[] args)
        {
            Initialise(args);

            LoadingScreens.WriteScreen(ConsoleColor.DarkBlue, "Now Playing", FilePath);
            Thread.Sleep(2000);
            Console.Clear();

            FFmpegExecution.GetVideoInformation();
            FFmpegExecution.GetVideoFrames(FilePath, WorkPath);
            FFmpegExecution.GetAudio(FilePath, WorkPath);

            VideoPlayer.ResizeFrames(WorkPath);
            VideoPlayer.PrepareConsole(WorkPath);
            PInvoke.EnableCloseButton();
            VideoPlayer.PlayVideo(WorkPath);

            Environment.Exit(0);
        }

        private static void Initialise(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(ExitApplication);
            Console.SetWindowSize(120, 30);//Default
            Console.SetBufferSize(120, 30);//Default
            Console.CursorVisible = false;

            //todo add png option for running in cmd
            if (args.Length != 0)
            {
                FilePath = Path.GetDirectoryName(args[0])
                   + Path.DirectorySeparatorChar
                   + Path.GetFileNameWithoutExtension(args[0])
                   + Path.GetExtension(args[0]);
            }
            else
            {
                //todo clear this out for release
                LoadingScreens.WriteScreen(ConsoleColor.DarkRed, "No file has been provided", "Drag and drop a video on the executable. The program will close in 5 seconds.");
                //Thread.Sleep(5000);
                //Environment.Exit(0);
            }

            PInvoke.PrepareConsole();
            WorkPath = Path.GetDirectoryName(AppContext.BaseDirectory);

            //Make temp hidden folder. ffmpeg can not create one
            DeleteTemp();
            DirectoryInfo di = Directory.CreateDirectory(WorkPath + "/temp");
            di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
        }

        static void ExitApplication(object sender, EventArgs e)
        {
            VideoPlayer.audio?.Stop();
            VideoPlayer._totalframecounter = 999999999;
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