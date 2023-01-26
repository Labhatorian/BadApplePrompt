using System.Drawing;
using System.Drawing.Imaging;
using System.Media;
using System.Text;
using System.Timers;

namespace BadAppleCMD
{
    public class Videoplayer
    {
        private int TotalFileCount;
        private int _VideoWidthColumns = 0;
        private int _VideoHeightColumns = 0;
        private int _framecounter = 0;
        public int _totalframecounter = 1;
        private string _FPS = "FPS: 0";
        private bool _Desync = false;
        public SoundPlayer? audio;
        private string Buffer;
        //TODO Figure out best factors
        //360p -> 4x - 1080p -> 16x
        private int _Factor = 4;

        public void PlayVideo(string WorkingPath)
        {
            //FPS counter timer and event
            System.Timers.Timer timer = new(1000);
            timer.Elapsed += OnTimedEvent;

            //Play audio of video
            audio = new SoundPlayer(WorkingPath + "/temp/audio.wav");
            if (File.Exists(WorkingPath + "/temp/audio.wav")) audio?.Play();
            timer.Start();

            //Start playing video
            while (_totalframecounter < TotalFileCount)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.CursorVisible = false; //It likes to turn itself back on
                if (_framecounter != Program.VideoFrameRate)
                {
                    Console.Write(Buffer);
                    //Console.Write(_FPS);
                    using (Bitmap image = new(WorkingPath + $"\\temp\\{_totalframecounter:00000000}.png")) Buffer = ConvertToAscii(image);
                    File.Delete(WorkingPath + $"\\temp\\{_totalframecounter:00000000}.png");
                    Thread.Sleep(Program.VideoFrameRate / 2);
                    Console.SetCursorPosition(0, 0);

                    _framecounter++;
                    _totalframecounter++;
                }
            }
        }

        public void PrepareConsole(string WorkingPath)
        {
            //Get the right window sizes. Max is dependent on user screen resolution.
            //TODO what?
            using (Bitmap image = new(WorkingPath + $"\\temp\\{1:00000000}.png"))
            {
                Buffer = ConvertToAscii(image);
                Console.Write(Buffer);
            }

            if (_VideoWidthColumns > Console.LargestWindowWidth) _VideoWidthColumns = Console.LargestWindowWidth;

            _VideoHeightColumns += 1; //For FPS Counter, and it needs one extra
            if (_VideoHeightColumns > Console.LargestWindowHeight) _VideoHeightColumns = Console.LargestWindowHeight;

            Console.SetWindowSize(_VideoWidthColumns, _VideoHeightColumns);
            Console.SetBufferSize(_VideoWidthColumns, _VideoHeightColumns);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
        }

        public void ResizeFrames(string WorkingPath)
        {
            TotalFileCount = Directory.GetFiles(WorkingPath + "/temp", "*", SearchOption.TopDirectoryOnly).Length;

            //UNDONE Resize every image to its factor -> Will get removed later as it's probably slower
            Screens.WriteScreen(ConsoleColor.DarkBlue, "Resizing frames", "[LoadingBar - Work In Progress]");

            while (_totalframecounter < TotalFileCount)
            {
                Bitmap resizedImage;
                using (Bitmap image = new(WorkingPath + $"\\temp\\{_totalframecounter:00000000}.png")) resizedImage = new(image, new Size(image.Width / _Factor, image.Height / _Factor));
                resizedImage.Save(WorkingPath + $"\\temp\\{_totalframecounter:00000000}.png", ImageFormat.Png);
                _totalframecounter++;
            }
            _totalframecounter = 1;
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
                        //Average out the RGB components to find the Gray Color
                        int red = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                        //Testing has resulted in 15 being the best value for BW videos
                        if (red > 15) sb.Append('█');
                        else sb.Append(' ');
                    }
                    sb.Append('\n');
                    h++;

                    _VideoWidthColumns = w;
                    height++;
                }
            }
            _VideoHeightColumns = height;
            //sb.Length -= 1; //Remove last linebreak
            sb.Append(_FPS);
            for (int i = _VideoWidthColumns - _FPS.Length; i > 0; i--)
            {
                sb.Append(' ');
            }
            return sb.ToString();
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            _FPS = $"FPS: {_framecounter}";
            if (_framecounter < Program.VideoFrameRate && !_Desync) _Desync = true;
            else if (_Desync) _FPS += " - Video desynced!";
            _framecounter = 0;
        }
    }
}