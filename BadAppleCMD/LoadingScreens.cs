using System.Globalization;
using System.Text;

namespace BadAppleCMD
{
    public static class LoadingScreens
    {
        public static string? TotalDuration { get; set; }
        public static string? CurrentDuration { get; set; }
        public static bool LoadingFinished { get; set; }

        public static void WriteScreen(ConsoleColor BackgroundColour, string MainString, string SecondString)
        {
            Console.BackgroundColor = BackgroundColour;
            Console.Clear();
            Console.SetCursorPosition((Console.WindowWidth - MainString.Length) / 2, Console.WindowHeight / 2 - 3);
            Console.WriteLine(MainString);
            Console.SetCursorPosition((Console.WindowWidth - SecondString.Length) / 2, Console.WindowHeight / 2);
            Console.WriteLine(SecondString);
        }

        public static void InformationOrLoadingBar(string MainString, bool Information)
        {
            //todo add additional option to display a third line??? (For what again?)
            //todo Use WriteScreen() to save code
            Console.Clear();
            while (!LoadingFinished)
            {
                Console.SetCursorPosition((Console.WindowWidth - MainString.Length) / 2, Console.WindowHeight / 2 - 3);
                Console.WriteLine(MainString);


                //convert to percentage
                StringBuilder loadingbar = new("[");

                if (!Information)
                {
                    LoadingBar(loadingbar);
                }
                else
                {
                    InformationBar(loadingbar);
                }

                loadingbar.Append(']');
                Console.SetCursorPosition((Console.WindowWidth - loadingbar.ToString().Length) / 2, Console.WindowHeight / 2);
                Console.WriteLine(loadingbar.ToString());
            }
        }

        private static void LoadingBar(StringBuilder loadingbar)
        {
            int totaldurationseconds = 1;
            int currentdurationseconds = 0;

            if (TotalDuration is not null && CurrentDuration is not null)
            {
                try
                {
                    DateTime totaldurationTime = DateTime.ParseExact(TotalDuration, "HH:mm:ss.ff",
                                    CultureInfo.InvariantCulture);
                    totaldurationseconds = totaldurationTime.Hour * 60 * 60 + totaldurationTime.Minute * 60 + totaldurationTime.Second + totaldurationTime.Millisecond / 100;

                    DateTime currentdurationTime = DateTime.ParseExact(CurrentDuration, "HH:mm:ss.ff",
                                           CultureInfo.InvariantCulture);
                    currentdurationseconds = currentdurationTime.Hour * 60 * 60 + currentdurationTime.Minute * 60 + currentdurationTime.Second + currentdurationTime.Millisecond / 100;
                }
                catch (System.FormatException)
                {
                    //ffmpeg hasnt started working on the video yet, ignore
                }
            }

            //TODO Are we sure this is right?
            int totalBars = (int)Math.Ceiling((double)(((double)currentdurationseconds / (double)totaldurationseconds * (double)100) / (double)5));

            for (int i = 1; i <= 20; i++)
            {
                if (totalBars > 0)
                {
                    loadingbar.Append('█');
                    totalBars--;
                }
                else
                {
                    loadingbar.Append(' ');
                }
                if (i != 20)
                {
                    loadingbar.Append('|');
                }
            }
        }

        private static void InformationBar(StringBuilder loadingbar)
        {
            loadingbar.Append($"Resolution= {Program.VideoWidth}x{Program.VideoHeight}");
            loadingbar.Append(" | ");
            loadingbar.Append($"Framerate= {Program.VideoFrameRate} FPS");
        }
    }
}