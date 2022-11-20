using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
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
        private static Boolean _Colour = true;
        private static Queue<int> colours = null;

        //Video player
        private static int _framecounter = 0;
        private static int _totalframecounter = 1;
        private static string _FPS = "FPS: 0";
        private static Boolean _Desync = false;
        private static SoundPlayer? audio;
        //TODO Figure out best factors
        //360p -> 4x - 1080p -> 16x
        private static int _Factor = 16;

        //Video information
        public static int VideoWidth { get; set; }
        public static int VideoHeight { get; set; }
        public static int VideoFrameRate { get; set; }

        private static int _VideoWidthColumns = 0;
        private static int _VideoHeightColumns = 0;

        static void Main(string[] args)
        {
            //TODO Make use of sections
            //TODO Write comments
            //TODO Write summaries
            //TODO Write tests in a testproject???
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(ExitApplication);
            Console.SetWindowSize(120, 30);///Default
            Console.SetBufferSize(120, 30);//Default
            Console.CursorVisible = false;

            //todo clear this out for release
            string path = "C:\\Users\\Harris\\source\\repos\\BadAppleCMD\\BadAppleCMD\\bin\\Debug\\net6.0-windows10.0.22621.0\\win-x64\\Boob.mp4";

            //todo add .jpg option? could we do that in a settings page before we start or pass as arg? Maybe both, just like optional colours
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
            //Get the path that we are workig in
            strWorkPath = Path.GetDirectoryName(AppContext.BaseDirectory);

            //Make temp hidden folder. ffmpeg can not create one on its for some reason
            DirectoryInfo di = Directory.CreateDirectory(strWorkPath + "/temp");
            di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

            //Show video that will get played
            //TODO Only show filename? Maybe get metadata
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

            parameter = "-i " + path + " " + strWorkPath + "/temp/%04d.png";
            parameter = parameter.Replace("\\", "/");
            Console.CursorVisible = false;
            task = Task.Run(() => { Execute(".\\ffmpeg.exe", parameter, true); });
            //TODO This does not work correctly on videos that are not bad apple
            //TODO resize as soon as frames get added -> FileSystemWatcher
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



            int fCount = Directory.GetFiles(strWorkPath + "/temp", "*", SearchOption.TopDirectoryOnly).Length;

            //UNDONE Resize every image to its factor -> Will get removed later
            Screens.WriteScreen(ConsoleColor.DarkBlue, "Resizing frames", "[LoadingBar - Work In Progress]");
            while (_totalframecounter < fCount)
            {
                Bitmap resizedImage;
                //todo make sure temp folder is empty before getting new files to prevent exemption here
                using (Bitmap image = new(strWorkPath + $"\\temp\\{_totalframecounter:0000}.png"))
                {
                    resizedImage = new(image, new Size(image.Width / _Factor, image.Height / _Factor));
                }
                resizedImage.Save(strWorkPath + $"\\temp\\{_totalframecounter:0000}.png", ImageFormat.Png);
                _totalframecounter++;
            }
            _totalframecounter = 1;

            //Get the right window sizes. Max is dependent on user screen resolution.
            using (Bitmap image = new(strWorkPath + $"\\temp\\{1:0000}.png"))
            {
                Console.Write(ConvertToAscii(image, true));
            }

            if (_VideoWidthColumns > Console.LargestWindowWidth)
            {
                _VideoWidthColumns = Console.LargestWindowWidth;
            }
            _VideoHeightColumns += 1; //For FPS Counter, and it needs one ex
            if (_VideoHeightColumns > Console.LargestWindowHeight)
            {
                _VideoHeightColumns = Console.LargestWindowHeight;
            }

            Console.SetWindowSize(_VideoWidthColumns, _VideoHeightColumns);
            Console.SetBufferSize(_VideoWidthColumns, _VideoHeightColumns);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();

            //TODO Disable window resizing and window maximising. This has to be done with P/Invoke, Windows DLLs and API

            //FPS counter timer and event
            Timer timer = new(1000);
            timer.Elapsed += OnTimedEvent;

            //Play audio of video
            audio = new SoundPlayer(strWorkPath + "/temp/audio.wav");
            if (File.Exists(strWorkPath + "/temp/audio.wav"))
            {
                audio?.Play();
            }
            timer.Start();

            while (_totalframecounter < fCount)
            {
                Console.CursorVisible = false; //It likes to turn itself back on
                if (_framecounter != VideoFrameRate)
                {
                    //todo calculate how many 0s are needed. bigger videos will require more
                    using (Bitmap image = new(strWorkPath + $"\\temp\\{_totalframecounter:0000}.png"))
                    {
                        //Console.Write(ConvertToAscii(image, false));
                        PrintColourVideo(ConvertToAscii(image, false));
                    }
                    Console.Write(_FPS);
                    File.Delete(strWorkPath + $"\\temp\\{_totalframecounter:0000}.png");
                    Thread.Sleep(VideoFrameRate / (_Factor / 2));
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
            //TODO Test different videos with different codes, resolutions, framerate, as testing resulted in a different video not getting right values for Time= and Duration=
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

        private static string ConvertToAscii(Bitmap image, Boolean GetColumns)
        {
            StringBuilder sb = new();
            colours = new();
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
                        if (red > 15)
                        {
                            sb.Append('█');
                        }
                        else
                        {
                            sb.Append(' ');
                        }

                        //Black       = 000,000,000-015,015,015 = 0
                        //DarkBlue    = 016,016,016-031,031,031 = 1
                        //DarkGreen   = 032,032,032-047,047,047 = 2
                        //DarkCyan    = 048,048,048-063,063,063 = 3
                        //DarkRed     = 064,064,064-079,079,079 = 4
                        //DarkMagenta = 080,080,080-095,095,095 = 5
                        //DarkYellow  = 096,096,096-111,111,111 = 6
                        //DarkGray    = 112,112,112-127,127,127 = 8
                        //Blue        = 128,128,128-143,143,143 = 9
                        //Green       = 144,144,144-159,159,159 = 10
                        //Cyan        = 160,160,160-175,175,175 = 11
                        //Red         = 176,176,176-191,191,191 = 12
                        //Magenta     = 192,192,192-207,207,207 = 13
                        //Yellow      = 208,208,208-223,223,223 = 14
                        //Gray        = 224,224,224-239,239,239 = 7
                        //White       = 240,240,240-255,255,255 = 15

                        if (_Colour)
                        {
                            if (red / 16 is >= 7 and < 14)
                            {
                                //Gray should be higher up but it isn't in ConsoleColour. So we need to account for everything inbetween
                                colours.Enqueue((red / 16) + 1); //7-13
                            }
                            else if (red / 16 == 14)
                            {
                                colours.Enqueue(7); //Gray 7
                            }
                            else
                            {
                                colours.Enqueue(red / 16); //0-6 and 15
                            }
                        }
                    }
                    sb.Append('\n');
                    h++;

                    _VideoWidthColumns = w;
                    height++;
                }
            }
            _VideoHeightColumns = height;
            sb.Length -= 1; //Last linebreak GONE
            return sb.ToString();
        }

        private static void PrintColourVideo(string stringtoprint)
        {
            //TODO TOO SLOW!!!
            Char[] array = stringtoprint.ToCharArray();
            foreach (char c in array)
            {
                if (colours.Count > 0)
                {
                    Console.ForegroundColor = (ConsoleColor)colours.Dequeue();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.Write(c);
            }
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
                    //No big deal, tests resulted this in always being one file which can stay in the temp folder. Only resulting in a few megabytes of lost space.
                    //Next time it will get overwritten!
                }
            }
        }
    }
}