﻿using System.Text;

namespace BadAppleCMD.Screens
{
    public static class LoadingScreens
    {
        public static string? Total { get; set; }
        public static string? Current { get; set; }
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
            Total = null;
            Current = null;
        }

        private static void LoadingBar(StringBuilder loadingbar)
        {
            int totaldurationseconds = 1;
            int currentdurationseconds = 0;
            int totalBars = 0;

            if (Total is not null && Current is not null)
            {
                totaldurationseconds = int.Parse(Total);
                currentdurationseconds = int.Parse(Current);

                //TODO Are we sure this is right?
                totalBars = (int)Math.Ceiling((double)(currentdurationseconds / (double)totaldurationseconds * 100 / 5));
            }

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