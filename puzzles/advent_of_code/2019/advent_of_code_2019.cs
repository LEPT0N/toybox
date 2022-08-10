using System;
using System.Diagnostics;
using advent_of_code_2019.days;

namespace advent_of_code_2019
{
    internal class advent_of_code_2019
    {
        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            int day = Int32.Parse(args[0]);
            int part = Int32.Parse(args[1]);
            string input = args[2];

            bool pretty = (args.Length >= 4 && args[3] == "pretty");

            switch (day, part)
            {
                default:
                    Console.WriteLine("Unexpected day '{0}'", args[0]);
                    break;
            }

            stopwatch.Stop();

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Time taken = {0}", stopwatch.Elapsed);
            Console.ResetColor();
        }
    }
}
