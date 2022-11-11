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
        private static int _totalframecounter = 0;
        private static string _FPS = "FPS: 0";
        static void Main(string[] args)
        {
            string path = "C:\\Users\\Harris\\source\\repos\\BadAppleCMD\\BadAppleCMD\\bin\\Debug\\net6.0\\win-x64\\badapple.mp4";

            Console.WriteLine("Hello, World!");
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

            Console.WriteLine(path);

            ////Get every videoframe
            string strExeFilePath = System.AppContext.BaseDirectory;
            string strWorkPath = System.IO.Path.GetDirectoryName(strExeFilePath);

            Console.WriteLine(strExeFilePath);
            Console.WriteLine(strWorkPath);

            //Make temp hidden folder. ffmpeg can not create one on its own
            DirectoryInfo di = Directory.CreateDirectory(strWorkPath + "/temp");
            di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

            //Save frames and get audio
            //TODO Add nifty loading bar instead of outputting it
            string parameter = "-i " + path + " " + strWorkPath + "/temp/%04d.png";
            parameter = parameter.Replace("\\", "/");
            Execute(".\\ffmpeg.exe", parameter);

            parameter = "-i " + path + " " + strWorkPath + "/temp/audio.wav";
            parameter = parameter.Replace("\\", "/");

            if(!File.Exists(strWorkPath + "/temp/audio.wav"))
            {
                Execute(".\\ffmpeg.exe", parameter);
            }
            
            Console.WriteLine("Finished");

            //Go through every frame and print it
            int fCount = Directory.GetFiles(strWorkPath + "/temp", "*", SearchOption.TopDirectoryOnly).Length;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.CursorVisible = false;
            Console.Clear();
            Console.SetWindowSize(120+1, 46+1);
            Console.SetBufferSize(120+1, 46 + 1);
            //TODO Disable window resizing and window maximising

            //FPS counter
            Timer timer = new Timer(1000);
            timer.Elapsed += OnTimedEvent;

            //Play audio
            SoundPlayer audio = new SoundPlayer(strWorkPath + "/temp/audio.wav");
            audio.Play();
            timer.Start();

            Boolean finished = false;
            while (_totalframecounter <= fCount)
            {
                if (_framecounter != 30) {
                    _framecounter++;
                    _totalframecounter++;
                    Console.Write(ConvertToAscii(new Bitmap(strWorkPath + $"\\temp\\{_totalframecounter:0000}.png")));
                    Console.WriteLine(_FPS);
                    Thread.Sleep(22);
                    Console.SetCursorPosition(0, 0);
                }
            }

            //Delete temp folder
            audio.Stop();
            Directory.Delete(strWorkPath + "/temp", true);
        }

        private static void Execute(string exePath, string parameters)
        {
            string result = String.Empty;

            using (Process p = new Process())
            {
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
        }

        private static string ConvertToAscii(Bitmap image)
        {
            int rowcount = 0;
            int heightcount = 0;
            image = new Bitmap(image, new Size(image.Width / 4, image.Height / 4));
            Boolean toggle = false;
            StringBuilder sb = new StringBuilder();
            for (int h = 0; h < image.Height; h++)
            {
                for (int w = 0; w < image.Width; w++)
                {
                    Color pixelColor = image.GetPixel(w, h);
                    //Average out the RGB components to find the Gray Color
                    int red = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                    int green = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                    int blue = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                    
                    //Use the toggle flag to minimize height-wise stretch
                    if (!toggle)
                    {
                        if (red == 255 && green == 255 && blue == 255)
                        {
                            sb.Append("█");
                        } else
                        {
                            sb.Append(" ");
                        }
                    }
                    rowcount++;
                }
                //TODO Can this be better?
                if (!toggle)
                {
                    sb.Append("\n");
                    heightcount++;
                    h++;
                    toggle = true;
                }
                else
                {
                    toggle = false;
                }
            }
            return sb.ToString();
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            _FPS = $"FPS: {_framecounter}";
            _framecounter= 0 ;
        }
    }
}