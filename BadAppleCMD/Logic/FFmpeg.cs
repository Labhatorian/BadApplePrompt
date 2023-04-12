using BadAppleCMD.Screens;
using System.Diagnostics;
using System.Globalization;

namespace BadAppleCMD.Logic
{
    /// <summary>
    /// All logic related to FFmpeg or FFprobe
    /// </summary>
    public static class FFmpeg
    {
        /// <summary>
        /// Sets Parameters and executes FFprobe to get video information like FPS, size and frames
        /// </summary>
        /// <param name="FilePath"></param>
        public static void GetVideoInformation(string FilePath)
        {
            string parameters = "-v error -select_streams v:0 -count_frames -show_entries stream=nb_read_frames,width,height,avg_frame_rate -of default=nw=1 " + FilePath;
            ExecuteFFprobe(parameters, "Getting information about the video...", true);
            Thread.Sleep(2000);
        }

        /// <summary>
        /// Sets Parameters and executes FFmpeg to extract every frame from the video and put it in /temp<br></br>
        /// <see cref="Program.BlackWhite"/> will attempt it to convert the images to be in grayscale
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="WorkPath"></param>
        /// <param name="FileExtension"></param>
        public static void GetVideoFrames(string FilePath, string WorkPath, string FileExtension)
        {
            string parameters = "-i " + FilePath + (Program.BlackWhite ? " -vf colorchannelmixer=.3:.4:.3:0:.3:.4:.3:0:.3" : "") + " -f image2 " + WorkPath + "\\temp\\%08d." + FileExtension;
            Console.CursorVisible = false;
            ExecuteFFmpeg(parameters, "Getting every frame from the video...");
        }

        /// <summary>
        /// Sets Parameters and executes FFmpeg to extract the audio and put it in /temp
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="WorkPath"></param>
        public static void GetAudio(string FilePath, string WorkPath)
        {
            string parameters = "-i " + FilePath + " " + WorkPath + "\\temp\\Audio.wav";
            if (File.Exists(WorkPath + "/temp/Audio.wav")) File.Delete(WorkPath + "/temp/Audio.wav"); //Prevents crash
            ExecuteFFmpeg(parameters, "Getting Audio from the video...", false, true);
        }

        /// <summary>
        /// Execute FFprobe with <paramref name="Parameters"/> in a <see cref="Task"/> and show its progress with a loading screen
        /// </summary>
        /// <param name="Parameters"></param>
        /// <param name="MainString"></param>
        /// <param name="ShowInformation"></param>
        private static void ExecuteFFprobe(string Parameters, string MainString, bool ShowInformation = false)
        {
            Task task = Task.Run(() => { Execute(".\\ffprobe.exe", Parameters, true, false); });
            if (!Program.Verbose) LoadingScreens.InformationOrLoadingBar(ConsoleColor.DarkBlue, MainString, ShowInformation);
            task.Wait();
        }

        /// <summary>
        /// Execute FFmpeg with <paramref name="Parameters"/> in a <see cref="Task"/> and show its progress with a loading screen<br></br>
        /// <paramref name="GetAudio"/> will make it skip some code while it goes through each outputted line by FFMpeg
        /// </summary>
        /// <param name="Parameters"></param>
        /// <param name="MainString"></param>
        /// <param name="ShowInformation"></param>
        /// <param name="GetAudio"></param>
        private static void ExecuteFFmpeg(string Parameters, string MainString, bool ShowInformation = false, bool GetAudio = false)
        {
            Task task = Task.Run(() => { Execute(".\\ffmpeg.exe", Parameters, true, GetAudio); });
            if (!Program.Verbose) LoadingScreens.InformationOrLoadingBar(ConsoleColor.DarkBlue, MainString, ShowInformation);
            task.Wait();
        }

        /// <summary>
        /// Creates a <see cref="Process"/> and runs either FFmpeg and FFprobe. Each output line is checked to extract data for
        /// the application or the loading bar.<br></br>
        /// <br></br>
        /// <paramref name="GetInformation"/> and <paramref name="GetAudio"/> is used to not attempt to read lines 
        /// that are not needed depending who and where this is being executed
        /// </summary>
        /// <param name="ExePath"></param>
        /// <param name="Paramaters"></param>
        /// <param name="GetInformation"></param>
        private static void Execute(string ExePath, string Paramaters, bool GetInformation, bool GetAudio)
        {
            string result = string.Empty;
            LoadingScreens.LoadingFinished = false;

            using Process p = new();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.FileName = ExePath;
            p.StartInfo.Arguments = Paramaters;
            //For ffprobe
            p.OutputDataReceived += new DataReceivedEventHandler((s, e) =>
            {
                if (Program.Verbose) Console.WriteLine(e.Data);
                if (e.Data is not null)
                {
                    if (ExePath.Contains("ffprobe.exe") && GetInformation)
                    {
                        if (e.Data.Contains("width=")) Program.VideoWidth = int.Parse(e.Data[(e.Data.LastIndexOf("width=") + 6)..]);

                        if (e.Data.Contains("height=")) Program.VideoHeight = int.Parse(e.Data[(e.Data.LastIndexOf("height=") + 7)..]);

                        if (e.Data.Contains("avg_frame_rate="))
                        {
                            string frameratefraction = e.Data[(e.Data.LastIndexOf("avg_frame_rate=") + 15)..];
                            int valueOne = int.Parse(frameratefraction.Split('/')[0]);
                            int valueTwo = int.Parse(frameratefraction[(frameratefraction.LastIndexOf('/') + 1)..]);
                            double result = (double)valueOne / valueTwo;
                            Program.VideoFrameRate = (int)Math.Round(result);
                        }

                        if (e.Data.Contains("nb_read_frames")) Program.TotalVideoFrames = int.Parse(e.Data[(e.Data.LastIndexOf("nb_read_frames=") + 15)..]);
                    }
                }
            });

            //For ffmpeg
            p.ErrorDataReceived += new DataReceivedEventHandler((s, e) =>
            {
                if (Program.Verbose) Console.WriteLine(e.Data);
                if (e.Data is not null)
                {
                    if (ExePath.Contains("ffmpeg.exe"))
                    {
                        try
                        {
                            if (e.Data.Contains("Duration:") && GetInformation && GetAudio)
                            {
                                string totaltime = e.Data.Substring(e.Data.LastIndexOf("Duration:") + 10, e.Data.LastIndexOf("Duration:") + 9);

                                DateTime totaldurationTime = DateTime.ParseExact(totaltime, "HH:mm:ss.ff",
                                        CultureInfo.InvariantCulture);
                                LoadingScreens.Total = (totaldurationTime.Hour * 60 * 60 + totaldurationTime.Minute * 60 + totaldurationTime.Second + totaldurationTime.Millisecond / 100).ToString();
                            }

                            if (e.Data.Contains("time=") && GetAudio)
                            {
                                string current = e.Data.Substring(e.Data.LastIndexOf("time=") + 5, e.Data.LastIndexOf("time=:") + 12);
                                DateTime currentTime = DateTime.ParseExact(current, "HH:mm:ss.ff",
                                       CultureInfo.InvariantCulture);
                                LoadingScreens.Current = (currentTime.Hour * 60 * 60 + currentTime.Minute * 60 + currentTime.Second + currentTime.Millisecond / 100).ToString();
                            }

                            if (e.Data.Contains("frame=") && !GetAudio)
                            {
                                string current = e.Data.Substring(e.Data.LastIndexOf("frame=") + 6, e.Data.LastIndexOf("frame=") + 5);
                                LoadingScreens.Total = Program.TotalVideoFrames.ToString();
                                LoadingScreens.Current = int.Parse(current).ToString();
                            }
                        }
                        catch (FormatException) { }
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