namespace BadAppleCMD.Screens
{
    public class Menu
    {
        private int _SelectedItem = 0;
        private int SelectedItem
        {
            get => _SelectedItem;
            set
            {
                if (value <= 0) _SelectedItem = 0;
                else if (value >= 1) _SelectedItem = 1;
                else _SelectedItem = value;
            }
        }

        private readonly string Logo =
            "/$$$$$$$                  /$$        /$$$$$$                      /$$          \r\n" +
            "| $$__  $$                | $$       /$$__  $$                    | $$          \r\n" +
            "| $$  \\ $$  /$$$$$$   /$$$$$$$      | $$  \\ $$  /$$$$$$   /$$$$$$ | $$  /$$$$$$ \r\n" +
            "| $$$$$$$  |____  $$ /$$__  $$      | $$$$$$$$ /$$__  $$ /$$__  $$| $$ /$$__  $$\r\n" +
            "| $$__  $$  /$$$$$$$| $$  | $$      | $$__  $$| $$  \\ $$| $$  \\ $$| $$| $$$$$$$$\r\n" +
            "| $$  \\ $$ /$$__  $$| $$  | $$      | $$  | $$| $$  | $$| $$  | $$| $$| $$_____/\r\n" +
            "| $$$$$$$/|  $$$$$$$|  $$$$$$$      | $$  | $$| $$$$$$$/| $$$$$$$/| $$|  $$$$$$$\r\n" +
            "|_______/  \\_______/ \\_______/      |__/  |__/| $$____/ | $$____/ |__/ \\_______/\r\n" +
            "                                              | $$      | $$                    \r\n" +
            "                                              | $$      | $$                    \r\n" +
            "                                              |__/      |__/                    \r\n";

        public int MainMenu()
        {
            while (true)
            {
                NotSelected();
                Console.Title = "BadApplePrompt";
                Console.Clear();

                string[] LogoSplit = Logo.Split("\n");
                Console.ForegroundColor = (SelectedItem == 0) ? ConsoleColor.White : ConsoleColor.Black;

                for (int i = 0; i < LogoSplit.Length; i++)
                {
                    Console.SetCursorPosition((Console.WindowWidth) / 2 - 40, Console.WindowHeight / 2 - 12 + i);
                    Console.Write(LogoSplit[i]);
                }

                NotSelected();

                Console.SetCursorPosition((Console.WindowWidth) / 2 - 15, Console.WindowHeight / 2 + 5);
                if (SelectedItem == 0) Selected();
                Console.Write("Play");

                NotSelected();

                if (SelectedItem == 1) Selected();

                Console.SetCursorPosition((Console.WindowWidth) / 2 + 8, Console.WindowHeight / 2 + 5);
                Console.Write("Settings");

                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.LeftArrow:
                        SelectedItem -= 1;
                        break;
                    case ConsoleKey.RightArrow:
                        SelectedItem += 1;
                        break;
                    case ConsoleKey.Enter:
                        return SelectedItem;
                }
            }
        }

        public static void NotSelected()
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Selected()
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
        }
    }
}