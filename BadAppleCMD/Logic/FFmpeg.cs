using BadAppleCMD.Screens;
using System.Diagnostics;
using System.Globalization;

namespace BadAppleCMD.Logic
{
    public class FFmpeg
    {
        public void GetVideoInformation(string FilePath)
        {
            string parameter = "-v error -select_streams v:0 -count_frames -show_entries stream=nb_read_frames,width,height,avg_frame_rate -of default=nw=1 " + FilePath;
            ExecuteFFprobe(parameter, "Getting information about the video...", true);
            Thread.Sleep(2000);
        }

        public void GetVideoFrames(string FilePath, string WorkingPath, string FileExtension)
        {
            string parameter = "-i " + FilePath + (Program.BlackWhite ? " -vf hue=s=0" : "") + " -f image2 " + WorkingPath + "\\temp\\%08d." + FileExtension;
            Console.CursorVisible = false;
            //TODO This does not work correctly on videos that are not bad apple
            ExecuteFFmpeg(parameter, "Getting every frame from the video...");
        }

        public void GetAudio(string FilePath, string WorkingPath)
        {
            string parameter = "-i " + FilePath + " " + WorkingPath + "\\temp\\Audio.wav";
            if (File.Exists(WorkingPath + "/temp/Audio.wav")) File.Delete(WorkingPath + "/temp/Audio.wav"); //Prevents crash
            ExecuteFFmpeg(parameter, "Getting Audio from the video...");
        }

        private void ExecuteFFprobe(string parameter, string screenText, bool screenLoadingBar = false)
        {
            Task task = Task.Run(() => { Execute(".\\ffprobe.exe", parameter, true); });
            if (!Program.Verbose) LoadingScreens.InformationOrLoadingBar(ConsoleColor.DarkBlue, screenText, screenLoadingBar);
            task.Wait();
        }

        private void ExecuteFFmpeg(string parameter, string screenText, bool screenLoadingBar = false)
        {
            Task task = Task.Run(() => { Execute(".\\ffmpeg.exe", parameter, true); });
            if (!Program.Verbose) LoadingScreens.InformationOrLoadingBar(ConsoleColor.DarkBlue, screenText, screenLoadingBar);
            task.Wait();
        }

        /// <summary>
        /// <paramref name="getinformation"/> is used to not attempt to read lines that are not needed for getting audio
        /// </summary>
        /// <param name="exePath"></param>
        /// <param name="parameters"></param>
        /// <param name="getinformation"></param>
        private void Execute(string exePath, string parameters, bool getinformation)
        {
            string result = string.Empty;
            LoadingScreens.LoadingFinished = false;

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

                        if (e.Data.Contains("nb_read_frames"))
                        {
                            Program.TotalVideoFrames = int.Parse(e.Data[(e.Data.LastIndexOf("nb_read_frames=") + 15)..]);
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
                        try
                        {
                            if (e.Data.Contains("Duration:") && getinformation)
                            {
                                string totaltime = e.Data.Substring(e.Data.LastIndexOf("Duration:") + 10, e.Data.LastIndexOf("Duration:") + 9);

                                DateTime totaldurationTime = DateTime.ParseExact(totaltime, "HH:mm:ss.ff",
                                        CultureInfo.InvariantCulture);
                                LoadingScreens.Total = (totaldurationTime.Hour * 60 * 60 + totaldurationTime.Minute * 60 + totaldurationTime.Second + totaldurationTime.Millisecond / 100).ToString();
                            }
                            if (e.Data.Contains("time="))
                            {
                                string current = e.Data.Substring(e.Data.LastIndexOf("time=") + 5, e.Data.LastIndexOf("time=:") + 12);
                                DateTime currentTime = DateTime.ParseExact(current, "HH:mm:ss.ff",
                                       CultureInfo.InvariantCulture);
                                LoadingScreens.Current = (currentTime.Hour * 60 * 60 + currentTime.Minute * 60 + currentTime.Second + currentTime.Millisecond / 100).ToString();
                            }
                        }
                        catch (FormatException)
                        {
                            //ignore
                        }
                    }

                }
            });

            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            p.WaitForExit();
            LoadingScreens.LoadingFinished = true;
        }
    }
}