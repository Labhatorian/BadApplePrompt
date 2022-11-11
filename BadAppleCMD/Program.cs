using System.Diagnostics;
using System.Drawing;
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
        private static SoundPlayer audio;
        private static string strWorkPath = "";
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(ExitApplication);

            //todo clear this out for release
            string path = "C:\\Users\\Harris\\source\\repos\\BadAppleCMD\\BadAppleCMD\\bin\\Debug\\net6.0\\win-x64\\badapple.mp4";

            //todo add .jpg option? could we do that in a settings page before we start or pass as arg? Maybe both, same for colour
            //todo make sure we calculate window size first and set it (probably not needed)
            if (args.Length != 0)
            {
                path = Path.GetDirectoryName(args[0])
                   + Path.DirectorySeparatorChar
                   + Path.GetFileNameWithoutExtension(args[0])
                   + Path.GetExtension(args[0]);
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.Clear();
                string errorstring = "No file has been provided";
                Console.SetCursorPosition((Console.WindowWidth - errorstring.Length) / 2, Console.WindowHeight / 2 - 3);
                Console.WriteLine(errorstring);

                string secondlineerrorstring = "Drag and drop a video onto the exe";
                Console.SetCursorPosition((Console.WindowWidth - secondlineerrorstring.Length) / 2, Console.WindowHeight / 2);
                Console.WriteLine(secondlineerrorstring);
            }
            //Prepare to get every frame
            string strExeFilePath = AppContext.BaseDirectory;
            strWorkPath = Path.GetDirectoryName(strExeFilePath);

            //Make temp hidden folder. ffmpeg can not create one on its own
            DirectoryInfo di = Directory.CreateDirectory(strWorkPath + "/temp");
            di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

            //Save frames and get audio
            //TODO Add nifty loading bar instead of outputting it. allow for verbose as arg/option
            string parameter = "-i " + path + " " + strWorkPath + "/temp/%04d.png";
            parameter = parameter.Replace("\\", "/");
            //todo get total runtime
            Execute(".\\ffmpeg.exe", parameter);
            //todo make screen here and calculate percentage and update screen


            parameter = "-i " + path + " " + strWorkPath + "/temp/audio.wav";
            parameter = parameter.Replace("\\", "/");

            //todo do same as getting video file
            if (!File.Exists(strWorkPath + "/temp/audio.wav"))
            {
                Execute(".\\ffmpeg.exe", parameter);
            }

            Console.WriteLine("Finished");

            //Go through every frame and print it
            int fCount = Directory.GetFiles(strWorkPath + "/temp", "*", SearchOption.TopDirectoryOnly).Length;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.CursorVisible = false;
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
                if (_framecounter != 30)
                {
                    using (Bitmap image = new Bitmap(strWorkPath + $"\\temp\\{_totalframecounter:0000}.png"))
                    {
                        Console.Write(ConvertToAscii(image));
                    }
                    Console.WriteLine(_FPS);
                    Thread.Sleep(15);
                    Console.SetCursorPosition(0, 0);
                    _framecounter++;
                    _totalframecounter++;
                }
            }
            Environment.Exit(0);
        }

        private static void Execute(string exePath, string parameters)
        {
            string result = String.Empty;

            using Process p = new();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.FileName = exePath;
            p.StartInfo.Arguments = parameters;

            p.OutputDataReceived += new DataReceivedEventHandler((s, e) =>
            {
                Console.WriteLine(e.Data);
            });
            p.ErrorDataReceived += new DataReceivedEventHandler((s, e) =>
            {
                Console.WriteLine(e.Data);
            });

            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            p.WaitForExit();
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
            _framecounter = 0;
        }

        static void ExitApplication(object sender, EventArgs e)
        {
            //todo test every case
            Console.Clear();
            //todo check for null
            audio.Stop();

            _totalframecounter = 999999999;
            GC.Collect();
            GC.WaitForPendingFinalizers();

            //Delete temp folder
            Directory.Delete(strWorkPath + "/temp", true);
        }
    }
}