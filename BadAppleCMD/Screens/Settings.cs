using BadAppleCMD.Logic;

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
                else if (value >= 9) _SelectedItem = 9;
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
                Console.Write("[Video sizedown factor]");

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

                Console.SetCursorPosition(2, 9);
                Console.Write("[Temp. frames file ext.]");

                Console.SetCursorPosition(2, 10);
                if (SelectedItem == 5) Menu.Selected();
                Console.Write(" [" + "                    " + "]");
                Console.SetCursorPosition(4 + 20 / 2 - (Program.FrameFileExtension.Length / 2), 10);
                Console.Write(Program.FrameFileExtension);

                Menu.NotSelected();

                Console.SetCursorPosition(2, 12);
                if (SelectedItem == 6) Menu.Selected();
                Console.Write("[FPS Counter] = ");
                Console.Write(" [" + (videoplayer.ShowFPSCounter ? "█" : " ") + "]");

                Menu.NotSelected();

                Console.SetCursorPosition(2, 14);
                Console.Write("[Convert option]");

                Console.SetCursorPosition(2, 15);
                if (SelectedItem == 7) Menu.Selected();
                Console.Write(" Resize frames          = [" + (Program.Resize ? "█]" : " ] - Factor options ignored"));

                Menu.NotSelected();

                Console.SetCursorPosition(2, 16);
                if (SelectedItem == 8) Menu.Selected();
                Console.Write(" Convert to black&white = [" + (Program.BlackWhite ? "█" : " ") + "]");

                Menu.NotSelected();

                Console.SetCursorPosition(2, 18);
                if (SelectedItem == 9) Menu.Selected();
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
                                videoplayer.ShowFPSCounter = !videoplayer.ShowFPSCounter;
                                break;
                            case 7:
                                Program.Resize = !Program.Resize;
                                break;
                            case 8:
                                Program.BlackWhite = !Program.BlackWhite;
                                break;
                            case 9:
                                Program.Verbose = !Program.Verbose;
                                break;
                        }
                        break;
                }
            }

            if (SelectedItem == 0) return;
            else if (SelectedItem == 5) InputScreen("Edit file extension");
            else SettingsPage(videoplayer);
        }

        public void InputScreen(string MainString)
        {
            bool breakout = false;

            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Clear();
            string explanation = "ENTER to save - ESCAPE to quit without saving";
            string fileExtension = Program.FrameFileExtension;
            while (!breakout)
            {
                Console.SetCursorPosition((Console.WindowWidth - MainString.Length) / 2, Console.WindowHeight / 2 - 3);
                Console.WriteLine(MainString);

                Console.SetCursorPosition((Console.WindowWidth - 22) / 2, Console.WindowHeight / 2);
                int split = (20 - fileExtension.Length) / 2;
                string SecondString = "[" + fileExtension.PadLeft(split + fileExtension.Length).PadRight(20) + "]";
                Console.WriteLine(SecondString);

                Console.SetCursorPosition((Console.WindowWidth - explanation.Length) / 2, Console.WindowHeight / 2 + 2);
                Console.WriteLine(explanation);

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
        }
    }
}