﻿using BadAppleCMD.Screens;
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
        public int TotalFrameCounter = 1;
        private string FPSstring = "FPS: 0";
        private bool VideoAudioDesync = false;
        public SoundPlayer? Audio;
        private string Buffer;
        //360p -> 4x - 1080p -> 16x
        public int ResizeFactor = 4;
        public bool RunVideo = false;

        public void PlayVideo(string WorkingPath)
        {
            int SleepFor = (int)(Program.VideoFrameRate / (Math.Pow(Program.VideoFrameRate / 30, 2) * 2));

            //FPS counter timer and event
            Timer timer = new(1000);
            timer.Elapsed += OnTimedEvent;

            Audio = new SoundPlayer(WorkingPath + "/temp/Audio.wav");
            timer.Start();
            if (File.Exists(WorkingPath + "/temp/Audio.wav")) Audio?.Play();

            RunVideo = true;
            while (TotalFrameCounter <= Program.TotalVideoFrames && RunVideo)
            {
                if (FPScounter != Program.VideoFrameRate)
                {
                    Console.Write(Buffer);
                    try
                    {
                        using (Bitmap image = new(WorkingPath + $"\\temp\\{TotalFrameCounter + 1:00000000}." + Program.FrameFileExtension)) Buffer = ConvertToAscii(image);
                        File.Delete(WorkingPath + $"\\temp\\{TotalFrameCounter:00000000}." + Program.FrameFileExtension);
                    }
                    catch (Exception) { }
                    Thread.Sleep(SleepFor);
                    Console.SetCursorPosition(0, 0);
                    TotalFrameCounter++;
                    FPScounter++;
                }
            }
        }

        public void PrepareConsole(string WorkingPath)
        {
            //Display first frame to size window correctly and save in buffer for later
            using (Bitmap image = new(WorkingPath + $"\\temp\\{1:00000000}." + Program.FrameFileExtension)) Buffer = ConvertToAscii(image);
            Console.Write(Buffer);

            File.Delete(WorkingPath + $"\\temp\\{1:00000000}." + Program.FrameFileExtension);
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

        public void ConvertFrames(string WorkingPath)
        {
            LoadingScreens.Total = Program.TotalVideoFrames.ToString();
            TotalFrameCounter = 1;
            while (TotalFrameCounter < Program.TotalVideoFrames)
            {
                LoadingScreens.Current = TotalFrameCounter.ToString();
                Bitmap resizedImage;
                using (Bitmap image = new(WorkingPath + $"\\temp\\{TotalFrameCounter:00000000}." + Program.FrameFileExtension)) resizedImage = new(image, new Size(image.Width / ResizeFactor, image.Height / ResizeFactor));
                resizedImage.Save(WorkingPath + $"\\temp\\{TotalFrameCounter:00000000}." + Program.FrameFileExtension);
                TotalFrameCounter++;
            }
            LoadingScreens.LoadingFinished = true;
            TotalFrameCounter = 1;
        }

        private string ConvertToAscii(Bitmap image)
        {
            StringBuilder sb = new();
            int height = 0;
            using (image = new Bitmap(image))
            {
                for (int h = 0; h < image.Height; h++)
                {
                    int w;
                    for (w = 0; w < image.Width; w++)
                    {
                        Color pixelColor = image.GetPixel(w, h);
                        int average = (pixelColor.R + pixelColor.G + pixelColor.B) / 3; //Average out the RGB components to find the Gray Color

                        //Testing has resulted in 15 being the best value for BW videos
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
            //We also can't just not do this because it needs one extra heightcolumn to prevent video jumping around
            if (!ShowFPSCounter) FPSstring = new string(' ', FPSstring.Length);
            sb.Append(FPSstring);

            for (int i = VideoWidthColumns - FPSstring.Length; i > 0; i--)
            {
                sb.Append(' ');
            }
            return sb.ToString();
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            FPSstring = $"FPS: {FPScounter}";
            if (FPScounter < Program.VideoFrameRate && !VideoAudioDesync && ShowFPSCounter) VideoAudioDesync = true;
            else if (VideoAudioDesync) FPSstring += " - Video desynced!";
            FPScounter = 0;
        }
    }
}