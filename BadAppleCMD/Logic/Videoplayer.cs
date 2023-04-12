using BadAppleCMD.Screens;
using System.Media;
using System.Text;
using System.Timers;
using Timer = System.Timers.Timer;

namespace BadAppleCMD.Logic
{
    public class Videoplayer
    {
        private int VideoWidthColumns = 0;
        private int VideoHeightColumns = 0;

        private int FPScounter = 0;
        public bool ShowFPSCounter = true;
        private string FPSstring = "FPS: 0";
        private bool VideoAudioDesync = false;

        public int TotalFrameCounter = 1;
        public SoundPlayer? Audio;
        private string? Buffer;
        public int ResizeFactor = 4; //360p -> 4x - 1080p -> 16x
        public bool RunVideo = false;

        /// <summary>
        /// Main function that plays the video in the console.
        /// </summary>
        /// <param name="WorkPath"></param>
        public void PlayVideo(string WorkPath)
        {
            //FPS
            int sleepFor = (int)(Program.VideoFrameRate / (Math.Pow(Program.VideoFrameRate / 30, 2) * 2));
            Timer timer = new(1000);
            timer.Elapsed += OnTimedEvent;

            Audio = new SoundPlayer(WorkPath + "/temp/Audio.wav");
            timer.Start();
            if (File.Exists(WorkPath + "/temp/Audio.wav")) Audio?.Play();

            RunVideo = true;
            while (TotalFrameCounter <= Program.TotalVideoFrames && RunVideo)
            {
                if (FPScounter != Program.VideoFrameRate)
                {
                    Console.Write(Buffer);
                    try
                    {
                        using (Bitmap image = new(WorkPath + $"\\temp\\{TotalFrameCounter + 1:00000000}." + Program.FrameFileExtension)) Buffer = ConvertToAscii(image);
                        File.Delete(WorkPath + $"\\temp\\{TotalFrameCounter:00000000}." + Program.FrameFileExtension);
                    }
                    catch (Exception) { }

                    Thread.Sleep(sleepFor);
                    Console.SetCursorPosition(0, 0);
                    TotalFrameCounter++;
                    FPScounter++;
                }
            }
        }

        /// <summary>
        /// Prepare the console for playing the video. Sets the console- and buffer size
        /// </summary>
        /// <param name="WorkPath"></param>
        public void PrepareConsole(string WorkPath)
        {
            //Display first frame to size window correctly and save in buffer for later
            using (Bitmap image = new(WorkPath + $"\\temp\\{1:00000000}." + Program.FrameFileExtension)) Buffer = ConvertToAscii(image);
            Console.Write(Buffer);

            File.Delete(WorkPath + $"\\temp\\{1:00000000}." + Program.FrameFileExtension);
            TotalFrameCounter++;

            //Prevents crashing of too big of a window
            if (VideoWidthColumns > Console.LargestWindowWidth) VideoWidthColumns = Console.LargestWindowWidth;
            VideoHeightColumns += 1; //Required otherwise the video jumps around
            if (VideoHeightColumns > Console.LargestWindowHeight) VideoHeightColumns = Console.LargestWindowHeight;

            Console.SetWindowSize(VideoWidthColumns, VideoHeightColumns);
            Console.SetBufferSize(VideoWidthColumns, VideoHeightColumns);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
        }

        /// <summary>
        /// Converts the frames depending on the sizedown factor
        /// </summary>
        /// <param name="WorkPath"></param>
        public void ConvertFrames(string WorkPath)
        {
            LoadingScreens.Total = Program.TotalVideoFrames.ToString();
            TotalFrameCounter = 1;
            while (TotalFrameCounter < Program.TotalVideoFrames)
            {
                LoadingScreens.Current = TotalFrameCounter.ToString();
                Bitmap resizedImage;
                using (Bitmap image = new(WorkPath + $"\\temp\\{TotalFrameCounter:00000000}." + Program.FrameFileExtension)) resizedImage = new(image, new Size(image.Width / ResizeFactor, image.Height / ResizeFactor));
                resizedImage.Save(WorkPath + $"\\temp\\{TotalFrameCounter:00000000}." + Program.FrameFileExtension);
                TotalFrameCounter++;
            }
            LoadingScreens.LoadingFinished = true;
            TotalFrameCounter = 1;
        }

        /// <summary>
        /// Converts a <paramref name="Frame"/> from the temp folder to Ascii.
        /// </summary>
        /// <param name="Frame"></param>
        /// <returns></returns>
        private string ConvertToAscii(Bitmap Frame)
        {
            StringBuilder sb = new();
            int height = 0;
            using (Frame = new Bitmap(Frame))
            {
                for (int h = 0; h < Frame.Height; h++)
                {
                    int w;
                    for (w = 0; w < Frame.Width; w++)
                    {
                        Color pixelColor = Frame.GetPixel(w, h);
                        int average = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;

                        //Testing has resulted in 15 being the best value for Bad Apple
                        if (average > 15) sb.Append('█');
                        else sb.Append(' ');
                    }
                    sb.Append('\n');
                    h++;

                    VideoWidthColumns = w;
                    height++;
                }
            }
            VideoHeightColumns = height;

            //This might seem weird, but it prevents a yellow line bleeding through for whatever reason
            //We also can not just not do this because it needs one extra heightcolumn to prevent the video jumping around
            if (!ShowFPSCounter) FPSstring = new string(' ', FPSstring.Length);
            sb.Append(FPSstring);

            for (int i = VideoWidthColumns - FPSstring.Length; i > 0; i--) sb.Append(' ');
            return sb.ToString();
        }

        /// <summary>
        /// FPS Counter that also checks that the video has not been desynced.<br></br>
        /// Note that it will most likely still desync due to rounding in FPS when we got <see cref="Program.VideoFrameRate"/>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            FPSstring = $"FPS: {FPScounter}";
            if (FPScounter < Program.VideoFrameRate && !VideoAudioDesync && ShowFPSCounter) VideoAudioDesync = true;
            else if (VideoAudioDesync) FPSstring += " - Video desynced!";
            FPScounter = 0;
        }
    }
}