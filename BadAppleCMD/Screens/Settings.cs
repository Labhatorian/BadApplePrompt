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
                if (value <= 0)
                {
                    _SelectedItem = 0;
                }
                else if (value >= 7)
                {
                    _SelectedItem = 7;
                }
                else
                {
                    _SelectedItem = value;
                }
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

        //todo improve
        public void SettingsPage(Videoplayer videoplayer)
        {
            bool breakout = false;

            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;
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
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.ForegroundColor = ConsoleColor.White;

                Console.SetCursorPosition(0, 0);
                if (SelectedItem == 0)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                Console.Write("<= Return to menu");

                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.ForegroundColor = ConsoleColor.White;

                Console.SetCursorPosition(2, 3);
                Console.Write("[Video sizedown factor]");

                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.ForegroundColor = ConsoleColor.White;

                Console.SetCursorPosition(2, 4);
                if (SelectedItem == 1)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                if (videoplayer.ResizeFactor != 2)
                {
                    Console.Write(" [ ] 2x");
                }
                else
                {
                    Console.Write(" [█] 2x");
                }

                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.ForegroundColor = ConsoleColor.White;

                Console.SetCursorPosition(2, 5);
                if (SelectedItem == 2)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                if (videoplayer.ResizeFactor != 4)
                {
                    Console.Write(" [ ] 4x");
                }
                else
                {
                    Console.Write(" [█] 4x");
                }

                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.ForegroundColor = ConsoleColor.White;

                Console.SetCursorPosition(2, 6);
                if (SelectedItem == 3)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                if (videoplayer.ResizeFactor != 8)
                {
                    Console.Write(" [ ] 8x");
                }
                else
                {
                    Console.Write(" [█] 8x");
                }

                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.ForegroundColor = ConsoleColor.White;

                Console.SetCursorPosition(2, 7);
                if (SelectedItem == 4)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                if (videoplayer.ResizeFactor != 16)
                {
                    Console.Write(" [ ] 16x");
                }
                else
                {
                    Console.Write(" [█] 16x");
                }

                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.ForegroundColor = ConsoleColor.White;

                Console.SetCursorPosition(2, 9);
                Console.Write("[Temp. frames file ext.]");

                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.ForegroundColor = ConsoleColor.White;

                Console.SetCursorPosition(2, 10);
                if (SelectedItem == 5)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                Console.Write(" [" + "                    " + "]");
                Console.SetCursorPosition(4 + 20 / 2 - (Program.FrameFileExtension.Length / 2), 10);
                Console.Write(Program.FrameFileExtension);

                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.ForegroundColor = ConsoleColor.White;

                Console.SetCursorPosition(2, 12);
                if (SelectedItem == 6)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                Console.Write("[FPS Counter] = ");
                if (videoplayer.ShowFPSCounter)
                {
                    Console.Write("[█]");
                }
                else
                {
                    Console.Write("[ ]");
                }

                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.ForegroundColor = ConsoleColor.White;

                Console.SetCursorPosition(2, 14);
                if (SelectedItem == 7)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                Console.Write("[Verbose mode] = ");
                if (Program.Verbose)
                {
                    Console.Write("[█]");
                }
                else
                {
                    Console.Write("[ ]");
                }

                if (breakout) break;
                var key = Console.ReadKey(false).Key;
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
                                Program.Verbose = !Program.Verbose;
                                break;
                        }
                        break;
                }
            }

            if (SelectedItem == 0)
            {
                return;
            }
            else if (SelectedItem == 5)
            {
                InputScreen(videoplayer, "Edit file extension");
            }
            else
            {
                SettingsPage(videoplayer);
            }
        }

        //todo make
        public void InputScreen(Videoplayer videoplayer, string MainString)
        {
            bool breakout = true;

            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Clear();
            string SecondString = " [" + "                    " + "]";

            while (breakout)
            {
                Console.SetCursorPosition((Console.WindowWidth - MainString.Length) / 2, Console.WindowHeight / 2 - 3);
                Console.WriteLine(MainString);
                Console.SetCursorPosition((Console.WindowWidth - SecondString.Length) / 2, Console.WindowHeight / 2);
                Console.WriteLine(SecondString);
                Console.SetCursorPosition((Console.WindowWidth - SecondString.Length) / 2 + (Program.FrameFileExtension.Length / 2), Console.WindowHeight / 2);
                Console.Write(Program.FrameFileExtension);
            }
        }
    }
}