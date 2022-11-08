namespace BadAppleCMD
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "";

            Console.WriteLine("Hello, World!");

            if (args.Length != 0)
            {
                path = Path.GetDirectoryName(args[0])
                   + Path.DirectorySeparatorChar
                   + Path.GetFileNameWithoutExtension(args[0])
                   + "_unwrapped" + Path.GetExtension(args[0]);
            } else
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.Clear();
                string errorstring = "No file has been provided";
                Console.SetCursorPosition((Console.WindowWidth - errorstring.Length) / 2, Console.WindowHeight / 2 - 3);
                Console.WriteLine(errorstring);

                string secondlineerrorstring = "Drag and drop a video onto the exe";
                Console.SetCursorPosition((Console.WindowWidth - secondlineerrorstring.Length) / 2, Console.WindowHeight / 2);
                Console.WriteLine(secondlineerrorstring);
            }

            Console.WriteLine(path);

            //Keep console open
            for (; ; )
            {
                Thread.Sleep(100);
            }
        }
    }
}