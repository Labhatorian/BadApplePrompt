using BadAppleCMD.Screens;
using System.Drawing.Imaging;
using System.Media;
using System.Text;
using System.Timers;
using Timer = System.Timers.Timer;

namespace BadAppleCMD.Logic
{
    public class Videoplayer
    {
        private int TotalFileCount;
        private int VideoWidthColumns = 0;
        private int VideoHeightColumns = 0;
        private int FPScounter = 0;
        public int TotalFrameCounter = 1;
        private string FPSstring = "FPS: 0";
        private bool VideoAudioDesync = false;
        public SoundPlayer? Audio;
        private string Buffer;
        //TODO Figure out best factors
        //360p -> 4x - 1080p -> 16x
        public int ResizeFactor = 4;

        public void PlayVideo(string WorkingPath)
        {
            int SleepFor = (int)(Program.VideoFrameRate / (Math.Pow(Program.VideoFrameRate / 30, 2) * 2));

            //FPS counter timer and event
            Timer timer = new(1000);
            timer.Elapsed += OnTimedEvent;

            Audio = new SoundPlayer(WorkingPath + "/temp/Audio.wav");
            timer.Start();
            if (File.Exists(WorkingPath + "/temp/Audio.wav")) Audio?.Play();

            //TODO Exiting crashes this as it is not stopping everything before deleting anymore
            while (TotalFrameCounter <= TotalFileCount)
            {
                if (FPScounter != Program.VideoFrameRate)
                {
                    Console.Write(Buffer);
                    using (Bitmap image = new(WorkingPath + $"\\temp\\{TotalFrameCounter + 1:00000000}.png")) Buffer = ConvertToAscii(image);
                    File.Delete(WorkingPath + $"\\temp\\{TotalFrameCounter:00000000}.png");
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
            using (Bitmap image = new(WorkingPath + $"\\temp\\{1:00000000}.png")) Buffer = ConvertToAscii(image);
            Console.Write(Buffer);
            File.Delete(WorkingPath + $"\\temp\\{1:00000000}.png");
            TotalFrameCounter++;

            //Prevents crashing of too big of a window
            if (VideoWidthColumns > Console.LargestWindowWidth) VideoWidthColumns = Console.LargestWindowWidth;
            VideoHeightColumns += 1; //For FPS Counter
            if (VideoHeightColumns > Console.LargestWindowHeight) VideoHeightColumns = Console.LargestWindowHeight;

            Console.SetWindowSize(VideoWidthColumns, VideoHeightColumns);
            Console.SetBufferSize(VideoWidthColumns, VideoHeightColumns);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
        }

        public void ResizeFrames(string WorkingPath)
        {
            TotalFileCount = Directory.GetFiles(WorkingPath + "/temp", "*", SearchOption.TopDirectoryOnly).Length;

            //UNDONE Resize every image to its factor
            LoadingScreens.WriteScreen(ConsoleColor.DarkBlue, "Resizing frames", "[LoadingBar - Work In Progress]");

            while (TotalFrameCounter < TotalFileCount)
            {
                Bitmap resizedImage;
                using (Bitmap image = new(WorkingPath + $"\\temp\\{TotalFrameCounter:00000000}.png")) resizedImage = new(image, new Size(image.Width / ResizeFactor, image.Height / ResizeFactor));
                resizedImage.Save(WorkingPath + $"\\temp\\{TotalFrameCounter:00000000}.png", ImageFormat.Png);
                TotalFrameCounter++;
            }
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
            if (FPScounter < Program.VideoFrameRate && !VideoAudioDesync) VideoAudioDesync = true;
            else if (VideoAudioDesync) FPSstring += " - Video desynced!";
            FPScounter = 0;
        }
    }
}