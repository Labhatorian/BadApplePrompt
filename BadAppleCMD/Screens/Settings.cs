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

        public void SettingsPage(Videoplayer videoplayer)
        {
            bool breakout = false;

            Menu.NotSelected();
            Console.Title = "BadApplePrompt - Settings";
            Console.Clear();

            string[] AppleSplit = Apple.Split("\n");
            for (int i = 0; i < AppleSplit.Length; i++)
            {
                Console.SetCursorPosition((Console.WindowWidth) - 37, 2 + i);
                Console.Write(AppleSplit[i]);
            }

            while (true)
            {
                Menu.NotSelected();

                Console.SetCursorPosition(0, 0);
                if (SelectedItem == 0) Menu.Selected();
                Console.Write("<= Return to menu");

                Menu.NotSelected();

                Console.SetCursorPosition(2, 3);
                Console.Write("[Video sizedown option]");

                Console.SetCursorPosition(2, 4);
                if (SelectedItem == 1) Menu.Selected();
                Console.Write(" [" + (videoplayer.ResizeFactor != 2 ? " " : "█") + "] 2x");

                Menu.NotSelected();

                Console.SetCursorPosition(2, 5);
                if (SelectedItem == 2) Menu.Selected();
                Console.Write(" [" + (videoplayer.ResizeFactor != 4 ? " " : "█") + "] 4x");

                Menu.NotSelected();

                Console.SetCursorPosition(2, 6);
                if (SelectedItem == 3) Menu.Selected();
                Console.Write(" [" + (videoplayer.ResizeFactor != 8 ? " " : "█") + "] 8x");

                Menu.NotSelected();

                Console.SetCursorPosition(2, 7);
                if (SelectedItem == 4) Menu.Selected();
                Console.Write(" [" + (videoplayer.ResizeFactor != 16 ? " " : "█") + "] 16x");

                Menu.NotSelected();

                Console.SetCursorPosition(2, 8);
                if (SelectedItem == 5) Menu.Selected();
                Console.Write(" [" + ((videoplayer.ResizeFactor == 2 || videoplayer.ResizeFactor == 4 || videoplayer.ResizeFactor == 8
                    || videoplayer.ResizeFactor == 16) ? " ] __x" : "█] " + videoplayer.ResizeFactor + "x"));

                Menu.NotSelected();

                Console.SetCursorPosition(2, 10);
                Console.Write("[Temp. frames file ext.]");

                Console.SetCursorPosition(2, 11);
                if (SelectedItem == 6) Menu.Selected();
                Console.Write(" [" + "                    " + "]");
                Console.SetCursorPosition(4 + 20 / 2 - (Program.FrameFileExtension.Length / 2), 11);
                Console.Write(Program.FrameFileExtension);

                Menu.NotSelected();

                Console.SetCursorPosition(2, 13);
                if (SelectedItem == 7) Menu.Selected();
                Console.Write("[FPS Counter] = ");
                Console.Write(" [" + (videoplayer.ShowFPSCounter ? "█" : " ") + "]");

                Menu.NotSelected();

                Console.SetCursorPosition(2, 15);
                Console.Write("[Convert option]");

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
                                videoplayer.ResizeFactor = 2;
                                break;
                            case 2:
                                videoplayer.ResizeFactor = 4;
                                break;
                            case 3:
                                videoplayer.ResizeFactor = 8;
                                break;
                            case 4:
                                videoplayer.ResizeFactor = 16;
                                break;
                            case 5:
                                breakout = true;
                                break;
                            case 6:
                                breakout = true;
                                break;
                            case 7:
                                videoplayer.ShowFPSCounter = !videoplayer.ShowFPSCounter;
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
            else if (SelectedItem == 5) InputScreenFactor(videoplayer, "Custom resize option");
            else if (SelectedItem == 6) InputScreenExtension(videoplayer, "Edit file extension");
            else SettingsPage(videoplayer);
        }

        public void InputScreenExtension(Videoplayer videoplayer, string MainString)
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
            SettingsPage(videoplayer);
        }

        public void InputScreenFactor(Videoplayer videoplayer, string MainString)
        {
            bool breakout = false;
            string factor = videoplayer.ResizeFactor.ToString();
            InputScreenInitialLayout(MainString, factor, 2, 4);
            while (!breakout)
            {
                InputScreenEditablePart(factor, 2, 4);

                ConsoleKey key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.Enter:
                        breakout = true;
                        videoplayer.ResizeFactor = int.Parse(factor);
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
            SettingsPage(videoplayer);
        }

        private static void InputScreenInitialLayout(string MainString, string option, int Inputsize, int Maxinputsize)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Clear();

            Console.SetCursorPosition((Console.WindowWidth - MainString.Length) / 2, Console.WindowHeight / 2 - 3);
            Console.WriteLine(MainString);

            InputScreenEditablePart(option, Inputsize, Maxinputsize);

            string explanation = "ENTER to save - ESCAPE to quit without saving";
            Console.SetCursorPosition((Console.WindowWidth - explanation.Length) / 2, Console.WindowHeight / 2 + 2);
            Console.WriteLine(explanation);
        }

        private static void InputScreenEditablePart(string option, int Inputsize, int Maxinputsize)
        {
            Console.SetCursorPosition((Console.WindowWidth - Maxinputsize) / 2, Console.WindowHeight / 2);
            int split = (Inputsize - option.Length) / 2;
            string SecondString = "[" + option.PadLeft(split + option.Length).PadRight(Inputsize) + "]";
            Console.WriteLine(SecondString);
        }
    }
}