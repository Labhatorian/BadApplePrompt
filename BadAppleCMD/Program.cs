using System.Diagnostics;

namespace BadAppleCMD
{
    internal class Program
    {
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

            //Get every videoframe
            string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string strWorkPath = System.IO.Path.GetDirectoryName(strExeFilePath);

            //Make temp hidden folder. ffmpeg can not create one on its own
            DirectoryInfo di = Directory.CreateDirectory(strWorkPath + "/frames/%04d.jpg");
            di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

            //Save frames
            string parameter = "-i " + path + " " + strWorkPath + "/frames/%04d.jpg";
            parameter = parameter.Replace("\\", "/");

            Execute(".\\ffmpeg.exe", parameter );

            Console.WriteLine("Finished");

            //Delete temp folder
            Directory.Delete(strWorkPath + "/frames", true);

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
    }
}