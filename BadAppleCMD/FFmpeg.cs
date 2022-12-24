using System.Diagnostics;

namespace BadAppleCMD
{
    public static class FFmpeg
    {
        public static void ExecuteFFprobe(string parameter, out Task task, string screenText, bool screenLoadingBar = false)
        {
            task = Task.Run(() => { Execute(".\\ffprobe.exe", parameter, true); });
            if (!Program.Verbose) Screens.InformationOrLoadingBar(screenText, screenLoadingBar);
        }

        public static void ExecuteFFmpeg(string parameter, out Task task, string screenText, bool screenLoadingBar = false)
        {
            task = Task.Run(() => { Execute(".\\ffmpeg.exe", parameter, true); });
            if (!Program.Verbose) Screens.InformationOrLoadingBar(screenText, screenLoadingBar);
        }

        private static void Execute(string exePath, string parameters, bool getinformation)
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
                if (Program.Verbose)
                {
                    Console.WriteLine(e.Data);
                }
                if (e.Data is not null)
                {
                    if (exePath.Contains("ffprobe.exe") && getinformation)
                    {
                        if (e.Data.Contains("width="))
                        {
                            Program.VideoWidth = int.Parse(e.Data[(e.Data.LastIndexOf("width=") + 6)..]);
                        }

                        if (e.Data.Contains("height="))
                        {
                            Program.VideoHeight = int.Parse(e.Data[(e.Data.LastIndexOf("height=") + 7)..]);
                        }

                        if (e.Data.Contains("avg_frame_rate="))
                        {
                            string frameratefraction = e.Data[(e.Data.LastIndexOf("avg_frame_rate=") + 15)..];
                            int valueOne = int.Parse(frameratefraction.Split('/')[0]);
                            int valueTwo = int.Parse(frameratefraction[(frameratefraction.LastIndexOf('/') + 1)..]);
                            Program.VideoFrameRate = valueOne / valueTwo;
                        }
                    }
                }
            });

            //For ffmpeg
            p.ErrorDataReceived += new DataReceivedEventHandler((s, e) =>
            {
                if (Program.Verbose)
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
    }
}