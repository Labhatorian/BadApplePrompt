using System.Diagnostics;
using System.Drawing;
using System.Media;
using System.Text;

namespace BadAppleCMD
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string path = "C:\\Users\\Harris\\source\\repos\\BadAppleCMD\\BadAppleCMD\\bin\\Debug\\net6.0\\win-x64\\badapple.mp4";

            Console.WriteLine("Hello, World!");
            //Console.SetWindowSize(100, 100);

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
            string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string strWorkPath = System.IO.Path.GetDirectoryName(strExeFilePath);

            //Make temp hidden folder. ffmpeg can not create one on its own
            DirectoryInfo di = Directory.CreateDirectory(strWorkPath + "/frames");
            di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

            //Save frames and get audio
            //string parameter = "-i " + path + " " + strWorkPath + "/frames/%04d.png";
            //parameter = parameter.Replace("\\", "/");
            //Execute(".\\ffmpeg.exe", parameter);

            //parameter = "-i " + path + " " + strWorkPath + "/frames/audio.wav";
            //parameter = parameter.Replace("\\", "/");
            //Execute(".\\ffmpeg.exe", parameter);

            Console.WriteLine("Finished");

            //Go through every frame and print it
            //TODO Try one frame first before working on every frame at a set fps depending on how fast it can draw
            int fCount = Directory.GetFiles(strWorkPath + "/frames", "*", SearchOption.TopDirectoryOnly).Length;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.CursorVisible = false;
            Console.SetWindowSize(121, 45);
            Console.SetBufferSize(121, 45);

            Console.Clear();

            //Play audio
            SoundPlayer audio = new SoundPlayer(strWorkPath + "/frames/audio.wav");
            audio.Play();
            for (int i = 1; i <= fCount; i++)
            {
                await Task.Delay(30);
                Console.Write(ConvertToAscii(new Bitmap(strWorkPath + $"\\frames\\{i:0000}.png")));
                //Console.Clear();
            }

            //Delete temp folder
            audio.Stop();
            //Directory.Delete(strWorkPath + "/frames", true);
            //Keep console open
            for (; ; )
            {
                Thread.Sleep(100);
            }
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
            string[] _AsciiChars = { " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "█" };
            image = new Bitmap(image, new Size(image.Width / 4, image.Height / 4));

            Boolean toggle = false;
            int countlines = 0;
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
                    Color grayColor = Color.FromArgb(red, green, blue);
                    //Use the toggle flag to minimize height-wise stretch
                    if (!toggle)
                    {
                        int index = (grayColor.R * 10) / 255;
                        sb.Append(_AsciiChars[index]);
                    }
                }
                if (!toggle)
                {
                    sb.Append("\n");
                    countlines++;
                    toggle = true;
                }
                else
                {
                    toggle = false;
                }
            }
            
            return sb.ToString();
        }
    }
}