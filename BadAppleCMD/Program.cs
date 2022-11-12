using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Media;
using System.Text;
using System.Timers;
using Timer = System.Timers.Timer;

namespace BadAppleCMD
{
    internal class Program
    {
        private static int _framecounter = 0;
        private static int _totalframecounter = 1;
        private static string _FPS = "FPS: 0";
        private static Boolean _Desync = false;
        private static SoundPlayer audio;
        private static string strWorkPath = "";

        //For loading bars
        private static string totalduration;
        private static string currentdurationframe;
        private static Boolean loadingFinished;

        //Video information
        private static int _VideoWidth;
        private static int _VideoHeight;
        private static int _VideoFrameRate;

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(ExitApplication);

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
                WriteScreen(ConsoleColor.DarkRed, "No file has been provided", "Drag and drop a video on the executable. The program will close in 5 seconds.");
                //Thread.Sleep(5000);
                //Environment.Exit(0);
            }
            //Prepare to get every frame
            string strExeFilePath = AppContext.BaseDirectory;
            strWorkPath = Path.GetDirectoryName(strExeFilePath);

            //Make temp hidden folder. ffmpeg can not create one on its own
            DirectoryInfo di = Directory.CreateDirectory(strWorkPath + "/temp");
            di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

            //Save frames and get audio
            //todo convert to use method
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Clear();
            string MainString = "Now Playing:";
            Console.SetCursorPosition((Console.WindowWidth - MainString.Length) / 2, Console.WindowHeight / 2 - 3);
            Console.WriteLine(MainString);

            Console.SetCursorPosition((Console.WindowWidth - path.Length) / 2, Console.WindowHeight / 2);
            Console.WriteLine(path);
            Thread.Sleep(2000);

            //Get information about video
            //todo loading bar but instead with information WOW!
            string parameter = "-v error -select_streams v:0 -show_entries stream=width,height,avg_frame_rate -of default=nw=1 " + path;
            Task task = Task.Run(() => { Execute(".\\ffprobe.exe", parameter, true); });
            LoadingBar("Getting information about the video...");

            //TODO Add nifty loading bar instead of outputting it. allow for verbose as arg/option
            parameter = "-i " + path + " " + strWorkPath + "/temp/%04d.png";
            parameter = parameter.Replace("\\", "/");
            Console.CursorVisible = false;
            task = Task.Run(() => { Execute(".\\ffmpeg.exe", parameter, true); });
            LoadingBar("Getting every frame from the video...");

            parameter = "-i " + path + " " + strWorkPath + "/temp/audio.wav";
            parameter = parameter.Replace("\\", "/");

            //todo do same as getting video file
            if (!File.Exists(strWorkPath + "/temp/audio.wav"))
            {
                task = Task.Run(() => { Execute(".\\ffmpeg.exe", parameter, false); });
                LoadingBar("Getting audio from the video...");
            }

            Console.WriteLine("Finished");

            //Go through every frame and print it
            int fCount = Directory.GetFiles(strWorkPath + "/temp", "*", SearchOption.TopDirectoryOnly).Length;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();

            //todo use video resolution to get these values. Figure out max for 1920x1080?
            Console.SetWindowSize(120, 46 + 1);
            Console.SetBufferSize(120, 46 + 1);
            //TODO Disable window resizing and window maximising

            //FPS counter
            Timer timer = new(1000);
            timer.Elapsed += OnTimedEvent;

            //Play audio
            audio = new SoundPlayer(strWorkPath + "/temp/audio.wav");
            audio.Play();
            timer.Start();

            //todo get fps from video to use as values here
            while (_totalframecounter < fCount)
            {
                if (_framecounter != _VideoFrameRate)
                {
                    //todo calculate how many 0s are needed
                    using (Bitmap image = new Bitmap(strWorkPath + $"\\temp\\{_totalframecounter:0000}.png"))
                    {
                        Console.Write(ConvertToAscii(image));
                    }
                    Console.WriteLine(_FPS);
                    File.Delete(strWorkPath + $"\\temp\\{_totalframecounter:0000}.png");
                    Thread.Sleep(_VideoFrameRate/2);
                    Console.SetCursorPosition(0, 0);
                    _framecounter++;
                    _totalframecounter++;
                }
            }
            Environment.Exit(0);
        }

        private static void WriteScreen(ConsoleColor BackgroundColour, string MainString, string SecondString)
        {
            Console.BackgroundColor = BackgroundColour;
            Console.Clear();
            Console.SetCursorPosition((Console.WindowWidth - MainString.Length) / 2, Console.WindowHeight / 2 - 3);
            Console.WriteLine(MainString);
            Console.SetCursorPosition((Console.WindowWidth - SecondString.Length) / 2, Console.WindowHeight / 2);
            Console.WriteLine(SecondString);
        }

        private static void LoadingBar(string MainString)
        {
            //todo add additional option to display a third line
            //todo Use WriteScreen() to save code
            Console.Clear();
            while (!loadingFinished)
            {
                Console.SetCursorPosition((Console.WindowWidth - MainString.Length) / 2, Console.WindowHeight / 2 - 3);
                Console.WriteLine(MainString);
                int totaldurationseconds = 1;
                int currentdurationseconds = 0;

                //convert to percentage
                if (totalduration is not null && currentdurationframe is not null)
                {
                    try
                    {
                        DateTime totaldurationTime = DateTime.ParseExact(totalduration, "HH:mm:ss.ff",
                                        CultureInfo.InvariantCulture);
                        totaldurationseconds = totaldurationTime.Hour * 60 * 60 + totaldurationTime.Minute * 60 + totaldurationTime.Second + totaldurationTime.Millisecond / 100;

                        DateTime currentdurationTime = DateTime.ParseExact(currentdurationframe, "HH:mm:ss.ff",
                                               CultureInfo.InvariantCulture);
                        currentdurationseconds = currentdurationTime.Hour * 60 * 60 + currentdurationTime.Minute * 60 + currentdurationTime.Second + currentdurationTime.Millisecond / 100;
                    }
                    catch (System.FormatException)
                    {
                        //ffmpeg hasnt started properly yet, ignore
                    }
                }

                StringBuilder loadingbar = new("[");
                int totalBars = (int)Math.Ceiling((double)(((double)currentdurationseconds / (double)totaldurationseconds * (double)100) / (double)5));

                for (int i = 1; i <= 20; i++)
                {
                    if (totalBars > 0)
                    {
                        loadingbar.Append("█");
                        totalBars--;
                    }
                    else
                    {
                        loadingbar.Append(" ");
                    }
                    if (i != 20)
                    {
                        loadingbar.Append("|");
                    }
                }
                loadingbar.Append("]");
                Console.SetCursorPosition((Console.WindowWidth - loadingbar.ToString().Length) / 2, Console.WindowHeight / 2);
                Console.WriteLine(loadingbar.ToString());
            }
        }

        private static void Execute(string exePath, string parameters, Boolean getinformation)
        {
            string result = String.Empty;
            loadingFinished = false;

            using Process p = new();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.FileName = exePath;
            p.StartInfo.Arguments = parameters;

            p.OutputDataReceived += new DataReceivedEventHandler((s, e) =>
            {
                ///Console.WriteLine(e.Data);
                if (e.Data is not null)
                {
                    if (exePath.Contains("ffprobe.exe") && getinformation)
                    {
                        if (e.Data.Contains("width="))
                        {
                            _VideoWidth = int.Parse(e.Data.Substring(e.Data.LastIndexOf("width=") + 6));
                        }

                        if (e.Data.Contains("height="))
                        {
                            _VideoHeight = int.Parse(e.Data.Substring(e.Data.LastIndexOf("height=") + 7));
                        }

                        if (e.Data.Contains("avg_frame_rate="))
                        {
                            string frameratefraction = e.Data.Substring(e.Data.LastIndexOf("avg_frame_rate=") + 15);
                            int valueOne = int.Parse(frameratefraction.Split('/')[0]);
                            int valueTwo = int.Parse(frameratefraction.Substring(frameratefraction.LastIndexOf('/') + 1));
                            _VideoFrameRate = valueOne / valueTwo;
                        }
                    }
                }
            });

            p.ErrorDataReceived += new DataReceivedEventHandler((s, e) =>
            {
                ///Console.WriteLine(e.Data);
                if (e.Data is not null)
                {
                    if (exePath.Contains("ffmpeg.exe"))
                    {
                        if (e.Data.Contains("Duration:") && getinformation)
                        {
                            totalduration = e.Data.Substring(e.Data.LastIndexOf("Duration:") + 10, e.Data.LastIndexOf("Duration:") + 9);
                        }
                        if (e.Data.Contains("time="))
                        {
                            currentdurationframe = e.Data.Substring(e.Data.LastIndexOf("time=") + 5, e.Data.LastIndexOf("time=:") + 12);
                        }
                    }

                }
            });

            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            p.WaitForExit();
            loadingFinished = true;
        }

        private static string ConvertToAscii(Bitmap image)
        {
            //todo add option for colours. however, black and white should be default. run with args or add settings?
            StringBuilder sb = new();
            using (image = new Bitmap(image, new Size(image.Width / 4, image.Height / 4)))
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
            if (_framecounter < _VideoFrameRate && !_Desync)
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
            //todo different message on program error
            WriteScreen(ConsoleColor.DarkBlue, "Thank you for playing!", "Cleaning up and exiting...");

            //todo check for null
            if (audio is not null)
            {
                audio.Stop();
            }

            _totalframecounter = 999999999;
            GC.Collect();
            GC.WaitForPendingFinalizers();

            //Delete temp folder
            if(Directory.Exists(strWorkPath + "/temp"))
            {
                Directory.Delete(strWorkPath + "/temp", true);
            }
        }
    }
}