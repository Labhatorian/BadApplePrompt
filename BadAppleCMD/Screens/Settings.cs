using BadAppleCMD.Logic;
using System.Text.RegularExpressions;

namespace BadAppleCMD.Screens
{
    public class Settings
    {
        private int _SelectedItem = 0;
        private int SelectedItem
        {
            get => _SelectedItem;
            set
            {
                if (value <= 0) _SelectedItem = 0;
                else if (value >= 10) _SelectedItem = 10;
                else _SelectedItem = value;
            }
        }

        private readonly string Apple =
            "                             ___\r\n" +
            "                          _/`.-'`.\r\n" +
            "                       _/` .  _.'\r\n" +
            "       .........    /` _.'_./\r\n" +
            "     .oooooooooo\\ \\o/.-'__.'o.\r\n" +
            "    .ooooooooo`._\\_|_.'`oooooob.\r\n" +
            "  .ooooooooooooooooooooooooooooob.\r\n" +
            " .ooooooooooooooooooooooooooooooob.\r\n" +
            ".ooooooooooooooooooooooooooooooooob.\r\n" +
            "doooooooooooooooooooooooooooooooooob\r\n" +
            "doooooooooooooooooooooooooooooooooob\r\n" +
            "doooooooooooooooooooooooooooooooooob\r\n" +
            "doooooooooooooooooooooooooooooooooob\r\n" +
            "`doooooooooooooooooooooooooooooooob'\r\n" +
            " `doooooooooooooooooooooooooooooob'\r\n" +
            "  `doooooooooooooooooooooooooooob'\r\n" +
            "   `doooooooooooooooooooooooooob'\r\n" +
            "    `doooooooooooooooooooooooob'\r\n" +
            "     `doooooooooooooooooooooob'\r\n" +
            "      `dooooooooobodoooooooob'\r\n" +
            "       `doooooooob dooooooob'\r\n" +
            "         `\"\"\"\"\"\"\"' `\"\"\"\"\"\"'";

        public void SettingsPage(Videoplayer VideoPlayer)
        {
            bool breakout = false;

            Menu.NotSelected();
            Console.Title = "BadApplePrompt - Settings";
            Console.Clear();

            string[] appleSplit = Apple.Split("\n");
            for (int i = 0; i < appleSplit.Length; i++)
            {
                Console.SetCursorPosition((Console.WindowWidth) - 37, 2 + i);
                Console.Write(appleSplit[i]);
            }

            while (true)
            {
                Menu.NotSelected();

                Console.SetCursorPosition(0, 0);
                if (SelectedItem == 0) Menu.Selected();
                Console.Write("<= Return to menu");

                Menu.NotSelected();

                Console.SetCursorPosition(2, 3);
                Console.Write("[Video sizedown Value]");
                #region Factors
                Console.SetCursorPosition(2, 4);
                if (SelectedItem == 1) Menu.Selected();
                Console.Write(" [" + (VideoPlayer.ResizeFactor != 2 ? " " : "█") + "] 2x");

                Menu.NotSelected();

                Console.SetCursorPosition(2, 5);
                if (SelectedItem == 2) Menu.Selected();
                Console.Write(" [" + (VideoPlayer.ResizeFactor != 4 ? " " : "█") + "] 4x");

                Menu.NotSelected();

                Console.SetCursorPosition(2, 6);
                if (SelectedItem == 3) Menu.Selected();
                Console.Write(" [" + (VideoPlayer.ResizeFactor != 8 ? " " : "█") + "] 8x");

                Menu.NotSelected();

                Console.SetCursorPosition(2, 7);
                if (SelectedItem == 4) Menu.Selected();
                Console.Write(" [" + (VideoPlayer.ResizeFactor != 16 ? " " : "█") + "] 16x");

                Menu.NotSelected();

                Console.SetCursorPosition(2, 8);
                if (SelectedItem == 5) Menu.Selected();
                Console.Write(" [" + ((VideoPlayer.ResizeFactor == 2 || VideoPlayer.ResizeFactor == 4 || VideoPlayer.ResizeFactor == 8
                    || VideoPlayer.ResizeFactor == 16) ? " ] __x" : "█] " + VideoPlayer.ResizeFactor + "x"));

                Menu.NotSelected();
                #endregion
                Console.SetCursorPosition(2, 10);
                Console.Write("[Extracted frames exte.]");

                Console.SetCursorPosition(2, 11);
                if (SelectedItem == 6) Menu.Selected();
                Console.Write(" [" + "                    " + "]");
                Console.SetCursorPosition(4 + 20 / 2 - (Program.FrameFileExtension.Length / 2), 11);
                Console.Write(Program.FrameFileExtension);

                Menu.NotSelected();

                Console.SetCursorPosition(2, 13);
                if (SelectedItem == 7) Menu.Selected();
                Console.Write("[FPS Counter] = ");
                Console.Write(" [" + (VideoPlayer.ShowFPSCounter ? "█" : " ") + "]");

                Menu.NotSelected();

                Console.SetCursorPosition(2, 15);
                Console.Write("[Convert options]");

                Console.SetCursorPosition(2, 16);
                if (SelectedItem == 8) Menu.Selected();
                Console.Write(" Resize frames          = [" + (Program.Resize ? "█" : " ") + "]");

                Menu.NotSelected();

                Console.SetCursorPosition(2, 17);
                if (SelectedItem == 9) Menu.Selected();
                Console.Write(" Convert to black&white = [" + (Program.BlackWhite ? "█" : " ") + "]");

                Menu.NotSelected();

                Console.SetCursorPosition(2, 19);
                if (SelectedItem == 10) Menu.Selected();
                Console.Write("[Verbose mode] = ");
                Console.Write(" [" + (Program.Verbose ? "█" : " ") + "]");

                if (breakout) break;
                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        SelectedItem -= 1;
                        break;
                    case ConsoleKey.DownArrow:
                        SelectedItem += 1;
                        break;
                    case ConsoleKey.Enter:
                        switch (SelectedItem)
                        {
                            case 0:
                                breakout = true;
                                break;
                            case 1:
                                VideoPlayer.ResizeFactor = 2;
                                break;
                            case 2:
                                VideoPlayer.ResizeFactor = 4;
                                break;
                            case 3:
                                VideoPlayer.ResizeFactor = 8;
                                break;
                            case 4:
                                VideoPlayer.ResizeFactor = 16;
                                break;
                            case 5:
                                breakout = true;
                                break;
                            case 6:
                                breakout = true;
                                break;
                            case 7:
                                VideoPlayer.ShowFPSCounter = !VideoPlayer.ShowFPSCounter;
                                break;
                            case 8:
                                Program.Resize = !Program.Resize;
                                break;
                            case 9:
                                Program.BlackWhite = !Program.BlackWhite;
                                break;
                            case 10:
                                Program.Verbose = !Program.Verbose;
                                break;
                        }
                        break;
                }
            }

            if (SelectedItem == 0) return;
            else if (SelectedItem == 5) InputScreenFactor(VideoPlayer, "Custom sizedown Value");
            else if (SelectedItem == 6) InputScreenExtension(VideoPlayer, "Edit file extension");
            else SettingsPage(VideoPlayer);
        }

        /// <summary>
        /// Input screen for the file extension
        /// </summary>
        /// <param name="VideoPlayer"></param>
        /// <param name="MainString"></param>
        private void InputScreenExtension(Videoplayer VideoPlayer, string MainString)
        {
            bool breakout = false;
            string fileExtension = Program.FrameFileExtension;
            InputScreenInitialLayout(MainString, fileExtension, 20, 22);

            while (!breakout)
            {
                InputScreenEditablePart(fileExtension, 20, 22);

                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.Enter:
                        breakout = true;
                        Program.FrameFileExtension = fileExtension;
                        break;
                    case ConsoleKey.Escape:
                        breakout = true;
                        break;
                    case ConsoleKey.Backspace:
                        if (fileExtension.Length > 0) fileExtension = fileExtension.Remove(fileExtension.Length - 1);
                        break;
                    case ConsoleKey.Delete:
                        if (fileExtension.Length > 0) fileExtension = fileExtension.Remove(fileExtension.Length - 1);
                        break;
                    case ConsoleKey.Spacebar:
                        break;
                    default:
                        if (fileExtension.Length < 20) fileExtension += key;
                        break;
                }
            }
            SettingsPage(VideoPlayer);
        }

        /// <summary>
        /// Inputscreen for the sizedown factor
        /// </summary>
        /// <param name="VideoPlayer"></param>
        /// <param name="MainString"></param>
        public void InputScreenFactor(Videoplayer VideoPlayer, string MainString)
        {
            bool breakout = false;
            string factor = VideoPlayer.ResizeFactor.ToString();
            InputScreenInitialLayout(MainString, factor, 2, 4);

            while (!breakout)
            {
                InputScreenEditablePart(factor, 2, 4);

                ConsoleKey key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.Enter:
                        breakout = true;
                        VideoPlayer.ResizeFactor = int.Parse(factor);
                        break;
                    case ConsoleKey.Escape:
                        breakout = true;
                        break;
                    case ConsoleKey.Backspace:
                        if (factor.Length > 0) factor = factor.Remove(factor.Length - 1);
                        break;
                    case ConsoleKey.Delete:
                        if (factor.Length > 0) factor = factor.Remove(factor.Length - 1);
                        break;
                    case ConsoleKey.Spacebar:
                        break;
                    default:
                        string keyToEnter = key.ToString().TrimStart('D');
                        if (factor.Length < 2 && Regex.IsMatch(keyToEnter, @"^[0-9]+$")) factor += keyToEnter;
                        break;
                }
            }
            SettingsPage(VideoPlayer);
        }

        /// <summary>
        /// Initialise the initial layout of the input screen
        /// </summary>
        /// <param name="MainString"></param>
        /// <param name="Value"></param>
        /// <param name="InputSize"></param>
        /// <param name="MaxSize"></param>
        private static void InputScreenInitialLayout(string MainString, string Value, int InputSize, int MaxSize)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Clear();
            Console.SetCursorPosition((Console.WindowWidth - MainString.Length) / 2, Console.WindowHeight / 2 - 3);
            Console.WriteLine(MainString);

            InputScreenEditablePart(Value, InputSize, MaxSize);
            string explanation = "ENTER to save - ESCAPE to quit without saving";
            Console.SetCursorPosition((Console.WindowWidth - explanation.Length) / 2, Console.WindowHeight / 2 + 2);
            Console.WriteLine(explanation);
        }

        /// <summary>
        /// The editable part of the input screen. Application will only have to rewrite this part
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="InputSize"></param>
        /// <param name="MaxSize"></param>
        private static void InputScreenEditablePart(string Value, int InputSize, int MaxSize)
        {
            Console.SetCursorPosition((Console.WindowWidth - MaxSize) / 2, Console.WindowHeight / 2);
            int split = (InputSize - Value.Length) / 2;
            string SecondString = "[" + Value.PadLeft(split + Value.Length).PadRight(InputSize) + "]";
            Console.WriteLine(SecondString);
        }
    }
}