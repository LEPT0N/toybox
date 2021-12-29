using System;
using System.Diagnostics;
using advent_of_code_2020.Days;

namespace advent_of_code_2020
{
    internal class Program
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
                case (1, 1): Day_01.Part_1(input, pretty); break;
                case (1, 2): Day_01.Part_2(input, pretty); break;
                case (2, 1): Day_02.Part_1(input, pretty); break;
                case (2, 2): Day_02.Part_2(input, pretty); break;
                case (3, 1): Day_03.Part_1(input, pretty); break;
                case (3, 2): Day_03.Part_2(input, pretty); break;
                case (4, 1): Day_04.Part_1(input, pretty); break;
                case (4, 2): Day_04.Part_2(input, pretty); break;
                case (5, 1): Day_05.Part_1(input, pretty); break;
                case (5, 2): Day_05.Part_2(input, pretty); break;
                case (6, 1): Day_06.Part_1(input, pretty); break;
                case (6, 2): Day_06.Part_2(input, pretty); break;
                case (7, 1): Day_07.Part_1(input, pretty); break;
                case (7, 2): Day_07.Part_2(input, pretty); break;
                case (8, 1): Day_08.Part_1(input, pretty); break;
                case (8, 2): Day_08.Part_2(input, pretty); break;
                case (9, 1): Day_09.Part_1(input, pretty); break;
                case (9, 2): Day_09.Part_2(input, pretty); break;
                case (10, 1): Day_10.Part_1(input, pretty); break;
                case (10, 2): Day_10.Part_2(input, pretty); break;
                case (11, 1): Day_11.Part_1(input, pretty); break;
                case (11, 2): Day_11.Part_2(input, pretty); break;
                case (12, 2): Day_12.Part_2(input, pretty); break;
                case (12, 1): Day_12.Part_1(input, pretty); break;

                default:
                    Console.WriteLine("Unexpected day " + args[0]);
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
