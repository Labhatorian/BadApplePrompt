using System.Diagnostics;
using System.Drawing;
using System.Media;
using System.Text;
using System.Timers;
using Timer = System.Timers.Timer;

namespace BadAppleCMD
{
    public class Program
    {   //Program
        private static string? strWorkPath = "";
        private static Boolean _Verbose = false;

        //Video player
        private static int _framecounter = 0;
        private static int _totalframecounter = 1;
        private static string _FPS = "FPS: 0";
        private static Boolean _Desync = false;
        private static SoundPlayer? audio;
        private static int _Factor = 4;

        //Video information
        public static int VideoWidth { get; set; }
        public static int VideoHeight { get; set; }
        public static int VideoFrameRate { get; set; }

        static void Main(string[] args)
        {
            //TODO Make use of sections
            //TODO Write comments
            //TODO Write summaries
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(ExitApplication);
            Console.CursorVisible = false;

            //todo clear this out for release
            string path = "C:\\Users\\Harris\\source\\repos\\BadAppleCMD\\BadAppleCMD\\bin\\Debug\\net6.0\\win-x64\\badapple.mp4";

            //todo add .jpg option? could we do that in a settings page before we start or pass as arg? Maybe both, same for colour
            //todo make sure we calculate window size first and set it (probably not needed)
            //todo when no file is dropped, allow to input url?
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
            //Prepare to get videe ready for play
            string strExeFilePath = AppContext.BaseDirectory;
            strWorkPath = Path.GetDirectoryName(strExeFilePath);

            //Make temp hidden folder. ffmpeg can not create one on its for some reason
            DirectoryInfo di = Directory.CreateDirectory(strWorkPath + "/temp");
            di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

            Screens.WriteScreen(ConsoleColor.DarkBlue, "Now Playing", path);
            Thread.Sleep(2000);

            //TODO Make these ffmpeg functions their own function to remove the clutter in this part of the code
            //Get information about video
            Console.Clear();
            string parameter = "-v error -select_streams v:0 -show_entries stream=width,height,avg_frame_rate -of default=nw=1 " + path;
            Task task = Task.Run(() => { Execute(".\\ffprobe.exe", parameter, true); });
            if (!_Verbose)
            {
                Screens.InformationOrLoadingBar("Getting information about the video...", true);
            }
            Thread.Sleep(1000);
            task.Wait();

            //Get frames
            parameter = "-i " + path + " " + strWorkPath + "/temp/%04d.png";
            parameter = parameter.Replace("\\", "/");
            Console.CursorVisible = false;
            task = Task.Run(() => { Execute(".\\ffmpeg.exe", parameter, true); });
            if (!_Verbose)
            {
                Screens.InformationOrLoadingBar("Getting every frame from the video...", false);
            }
            task.Wait();

            parameter = "-i " + path + " " + strWorkPath + "/temp/audio.wav";
            parameter = parameter.Replace("\\", "/");

            //Get audio - ffmpeg cries when a file already exists, unlike with images
            if (File.Exists(strWorkPath + "/temp/audio.wav"))
            {
                File.Delete(strWorkPath + "/temp/audio.wav");
            }
            task = Task.Run(() => { Execute(".\\ffmpeg.exe", parameter, false); });
            if (!_Verbose)
            {
                Screens.InformationOrLoadingBar("Getting audio from the video...", false);
            }
            task.Wait();


            Console.WriteLine("Finished");

            //Go through every frame and print it
            int fCount = Directory.GetFiles(strWorkPath + "/temp", "*", SearchOption.TopDirectoryOnly).Length;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();

            //todo use video resolution to get these values. Figure out max for 1920x1080?
            //Bad Apple is 480x360 - Using 120, 46 + 1 (one extra for fps counter) - Difference of 360 and 314
            //Width is /4. Same factor as the ASCII converter
            //TODO Height is more complicated with / 4 / 2 + 2. Will this work with other videos and factors?
            Console.SetWindowSize(VideoWidth / _Factor, VideoHeight / _Factor / 2 + 2);
            Console.SetBufferSize(VideoWidth / _Factor, VideoHeight / _Factor / 2 + 2);

            //TODO Disable window resizing and window maximising. This has to be done with P/Invoke, Windows DLLs and API

            //FPS counter
            Timer timer = new(1000);
            timer.Elapsed += OnTimedEvent;

            //Play audio
            audio = new SoundPlayer(strWorkPath + "/temp/audio.wav");
            audio.Play();
            timer.Start();

            while (_totalframecounter < fCount)
            {
                if (_framecounter != VideoFrameRate)
                {
                    //todo calculate how many 0s are needed
                    using (Bitmap image = new(strWorkPath + $"\\temp\\{_totalframecounter:0000}.png"))
                    {
                        Console.Write(ConvertToAscii(image));
                    }
                    Console.WriteLine(_FPS);
                    File.Delete(strWorkPath + $"\\temp\\{_totalframecounter:0000}.png");
                    Thread.Sleep(VideoFrameRate / 2);
                    Console.SetCursorPosition(0, 0);
                    _framecounter++;
                    _totalframecounter++;
                }
            }
            Environment.Exit(0);
        }


        //TODO Move this to its own class alongside the full suite of ffmpeg/ffprobe section in the program???
        private static void Execute(string exePath, string parameters, Boolean getinformation)
        {
            string result = String.Empty;
            Screens.LoadingFinished = false;

            using Process p = new();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.FileName = exePath;
            p.StartInfo.Arguments = parameters;
            //TODO Test different videos with different codes, resolutions, framerate
            //For ffprobe
            p.OutputDataReceived += new DataReceivedEventHandler((s, e) =>
            {
                if (_Verbose)
                {
                    Console.WriteLine(e.Data);
                }
                if (e.Data is not null)
                {
                    if (exePath.Contains("ffprobe.exe") && getinformation)
                    {
                        if (e.Data.Contains("width="))
                        {
                            VideoWidth = int.Parse(e.Data[(e.Data.LastIndexOf("width=") + 6)..]);
                        }

                        if (e.Data.Contains("height="))
                        {
                            VideoHeight = int.Parse(e.Data[(e.Data.LastIndexOf("height=") + 7)..]);
                        }

                        if (e.Data.Contains("avg_frame_rate="))
                        {
                            string frameratefraction = e.Data[(e.Data.LastIndexOf("avg_frame_rate=") + 15)..];
                            int valueOne = int.Parse(frameratefraction.Split('/')[0]);
                            int valueTwo = int.Parse(frameratefraction[(frameratefraction.LastIndexOf('/') + 1)..]);
                            VideoFrameRate = valueOne / valueTwo;
                        }
                    }
                }
            });

            //For ffmpeg
            p.ErrorDataReceived += new DataReceivedEventHandler((s, e) =>
            {
                if (_Verbose)
                {
                    Console.WriteLine(e.Data);
                }
                if (e.Data is not null)
                {
                    if (exePath.Contains("ffmpeg.exe"))
                    {
                        if (e.Data.Contains("Duration:") && getinformation)
                        {
                            Screens.TotalDuration = e.Data.Substring(e.Data.LastIndexOf("Duration:") + 10, e.Data.LastIndexOf("Duration:") + 9);
                        }
                        if (e.Data.Contains("time="))
                        {
                            Screens.CurrentDuration = e.Data.Substring(e.Data.LastIndexOf("time=") + 5, e.Data.LastIndexOf("time=:") + 12);
                        }
                    }

                }
            });

            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            p.WaitForExit();
            Screens.LoadingFinished = true;
        }

        private static string ConvertToAscii(Bitmap image)
        {
            //todo add option for colours. however, black and white should be default. run with args or add settings?
            StringBuilder sb = new();
            using (image = new Bitmap(image, new Size(image.Width / _Factor, image.Height / _Factor)))
            {
                for (int h = 0; h < image.Height; h++)
                {
                    for (int w = 0; w < image.Width; w++)
                    {
                        Color pixelColor = image.GetPixel(w, h);
                        //Average out the RGB components to find the Gray Color
                        int red = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                        int green = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                        int blue = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                        if (red > 15 && green > 15 && blue > 15)
                        {
                            sb.Append('█');
                        }
                        else
                        {
                            sb.Append(' ');
                        }
                    }
                    sb.Append('\n');
                    h++;
                }
            }
            return sb.ToString();
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            _FPS = $"FPS: {_framecounter}";
            if (_framecounter < VideoFrameRate && !_Desync)
            {
                _Desync = true;
            }
            else if (_Desync)
            {
                _FPS += " - Video desynced!";
            }
            _framecounter = 0;
        }

        static void ExitApplication(object sender, EventArgs e)
        {
            //todo test every case
            audio?.Stop();
            _totalframecounter = 999999999;
            GC.Collect();

            Thread.Sleep(500);
            Screens.WriteScreen(ConsoleColor.DarkBlue, "Thank you!", "Cleaning up and exiting...");
            Thread.Sleep(500);

            GC.WaitForPendingFinalizers();

            //Delete temp folder
            if (Directory.Exists(strWorkPath + "/temp"))
            {
                try
                {
                    Directory.Delete(strWorkPath + "/temp", true);
                }
                catch (IOException)
                {
                    //Some or one file is unable to get deleted. Presumably because the user just caught the program still using it.
                    //No big deal, tests resulted this in always being one file which can stay in the temp folder. Next time it will get overwritten
                }
            }
        }
    }
}