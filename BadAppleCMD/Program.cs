namespace BadAppleCMD
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            //Keep console open
            for (; ; )
            {
                Thread.Sleep(100);
            }
        }
    }
}