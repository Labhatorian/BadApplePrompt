using System.Text;

namespace BadAppleCMD.Screens
{
    public static class LoadingScreens
    {
        public static string? Total { get; set; }
        public static string? Current { get; set; }
        public static bool LoadingFinished { get; set; }

        /// <summary>
        /// Function that writes the <paramref name="MainString"/> and <paramref name="SecondString"/> in the right way while loading the video
        /// </summary>
        /// <param name="BackgroundColour"></param>
        /// <param name="MainString"></param>
        /// <param name="SecondString"></param>
        /// <param name="ClearConsole"></param>
        public static void LoadingWriteScreen(ConsoleColor BackgroundColour, string MainString, string SecondString, bool ClearConsole = true)
        {
            Console.BackgroundColor = BackgroundColour;
            Console.Title = "BadApplePrompt - " + MainString;
            if ((bool)ClearConsole) Console.Clear(); //Do not write the entire screen while loading

            Console.SetCursorPosition((Console.WindowWidth - MainString.Length) / 2, Console.WindowHeight / 2 - 3);
            Console.WriteLine(MainString);

            if ((Console.WindowWidth - SecondString.Length) < 0) SecondString = Path.GetFileName(SecondString);
            Console.SetCursorPosition((Console.WindowWidth - SecondString.Length) / 2, Console.WindowHeight / 2);
            Console.WriteLine(SecondString);
        }

        /// <summary>
        /// Two types of loading is available. Information to show ffprobe info and the typical loading bar
        /// </summary>
        /// <param name="BackgroundColour"></param>
        /// <param name="MainString"></param>
        /// <param name="Information"></param>
        public static void InformationOrLoadingBar(ConsoleColor BackgroundColour, string MainString, bool Information)
        {
            Console.Clear();
            while (!LoadingFinished)
            {
                StringBuilder loadingbar = new("[");
                if (!Information) LoadingBar(loadingbar);
                else InformationBar(loadingbar);

                loadingbar.Append(']');
                LoadingWriteScreen(BackgroundColour, MainString, loadingbar.ToString(), false);
            }
            Total = null;
            Current = null;
        }

        /// <summary>
        /// Typical loading bar with space for every 5%
        /// </summary>
        /// <param name="LoadingBar"></param>
        private static void LoadingBar(StringBuilder LoadingBar)
        {
            int totalBars = 0;
            if (Total is not null && Current is not null)
            {
                int totaldurationseconds = int.Parse(Total);
                int currentdurationseconds = int.Parse(Current);
                totalBars = (int)Math.Ceiling((double)(currentdurationseconds / (double)totaldurationseconds * 100 / 5));
            }

            for (int i = 1; i <= 20; i++)
            {
                if (totalBars > 0)
                {
                    LoadingBar.Append('█');
                    totalBars--;
                }
                else LoadingBar.Append(' ');
                if (i != 20) LoadingBar.Append('|');
            }
        }

        /// <summary>
        /// FFProbe information bar
        /// </summary>
        /// <param name="LoadingBar"></param>
        private static void InformationBar(StringBuilder LoadingBar)
        {
            LoadingBar.Append($"Resolution= {Program.VideoWidth}x{Program.VideoHeight}");
            LoadingBar.Append(" | ");
            LoadingBar.Append($"Framerate= {Program.VideoFrameRate} FPS");
            LoadingBar.Append(" | ");
            LoadingBar.Append($"Frames= {Program.TotalVideoFrames}");
        }
    }
}