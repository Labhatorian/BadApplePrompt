using System.Diagnostics;

namespace BadAppleCMD
{
    public class FFmpeg
    {
        public void GetVideoInformation()
        {
            string parameter = "-v error -select_streams v:0 -show_entries stream=width,height,avg_frame_rate -of default=nw=1 " + path;
            ExecuteFFprobe(parameter, "Getting information about the video...", true);
            Thread.Sleep(1000);
        }

        public void GetVideoFrames(string FilePath, string WorkingPath)
        {
            string parameter = "-i " + FilePath + " " + WorkingPath + "\\temp\\%08d.png";
            Console.CursorVisible = false;
            //TODO This does not work correctly on videos that are not bad apple
            //TODO resize as soon as frames get added -> FileSystemWatcher?
            ExecuteFFmpeg(parameter, "Getting every frame from the video...");
        }

        public void GetAudio(string FilePath, string WorkingPath)
        {
            string parameter = "-i " + FilePath + " " + WorkingPath + "\\temp\\audio.wav";
            if (File.Exists(WorkingPath + "/temp/audio.wav")) File.Delete(WorkingPath + "/temp/audio.wav"); //Prevents crash
            ExecuteFFmpeg(parameter, "Getting audio from the video...");
        }

        private void ExecuteFFprobe(string parameter, string screenText, bool screenLoadingBar = false)
        {
            Task task = Task.Run(() => { Execute(".\\ffprobe.exe", parameter, true); });
            if (!Program.Verbose) Screens.InformationOrLoadingBar(screenText, screenLoadingBar);
            task.Wait();
        }

        private void ExecuteFFmpeg(string parameter, string screenText, bool screenLoadingBar = false)
        {
            Task task = Task.Run(() => { Execute(".\\ffmpeg.exe", parameter, true); });
            if (!Program.Verbose) Screens.InformationOrLoadingBar(screenText, screenLoadingBar);
            task.Wait();
        }

        private void Execute(string exePath, string parameters, bool getinformation)
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