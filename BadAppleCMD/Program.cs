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

        private static string FilePath = "";
        private static string WorkPath = "";
        public static string FrameFileExtension = "PNG";
        public static bool Verbose = false;
        public static bool Resize = true;
        public static bool BlackWhite = false;
        public static bool AutoStart = false;

        public static int VideoWidth { get; set; }
        public static int VideoHeight { get; set; }
        public static int VideoFrameRate { get; set; }
        public static int TotalVideoFrames { get; set; }

        [STAThread]
        static void Main(string[] args)
        {
            Initialise(args);
            PInvoke.PrepareConsole();

            if (AutoStart) MakeAndPlayVideo();
            else StartMenu();
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

        private async static void MakeAndPlayVideo()
        {
            LoadingScreens.WriteScreen(ConsoleColor.DarkBlue, "Now Playing", FilePath);
            Thread.Sleep(2000);
            Console.Clear();

            FFmpegExecution.GetVideoInformation(FilePath);
            FFmpegExecution.GetVideoFrames(FilePath, WorkPath, FrameFileExtension);
            FFmpegExecution.GetAudio(FilePath, WorkPath);

            if (Resize)
            {
                Task task = Task.Run(() => { VideoPlayer.ConvertFrames(WorkPath); });
                LoadingScreens.LoadingFinished = false;
                LoadingScreens.InformationOrLoadingBar(ConsoleColor.DarkBlue, "Converting frames...", false);
                task.Wait();
            }

            VideoPlayer.PrepareConsole(WorkPath);
            Console.Title = "Now Playing: " + Path.GetFileName(FilePath);
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

            if (!File.Exists(".\\ffmpeg.exe") && !File.Exists(".\\ffprobe.exe"))
            {
                LoadingScreens.WriteScreen(ConsoleColor.DarkRed, "Missing requisites", "FFmpeg and FFprobe are required to run BadApplePrompt");
                Thread.Sleep(5000);
                Environment.Exit(0);
            }

            if (args.Length != 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    string argument = args[i];
                    argument.TrimStart('-');

                    switch (argument)
                    {
                        case "FileExtension":
                            FrameFileExtension = args[i + 1];
                            break;
                        case "Factor":
                            VideoPlayer.ResizeFactor = int.Parse(args[i + 1]);
                            break;
                        case "Verbose":
                            Verbose = true;
                            break;
                        case "AutoStart":
                            AutoStart = true;
                            break;
                        case "Resize":
                            Resize = false;
                            break;
                        case "ConvertBlackWhite":
                            BlackWhite = true;
                            break;
                        case "FPSCounter":
                            VideoPlayer.ShowFPSCounter = false;
                            break;
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
                FilePath = SelectFileDialog();
                if (FilePath is null) Environment.Exit(0);
            }

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
            VideoPlayer.RunVideo = false;
            GC.Collect();

            Thread.Sleep(500);
            Console.Title = "Exiting BadApplePrompt...";
            Console.ForegroundColor = ConsoleColor.White;
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