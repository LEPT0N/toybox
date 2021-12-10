using System;
using advent_of_code_2021.Days;

namespace advent_of_code_2021
{
    // https://adventofcode.com/2021

    internal class Program
    {
        static void Main(string[] args)
        {
            int day = Int32.Parse(args[0]);
            int part = Int32.Parse(args[1]);
            string input = args[2];

            switch (day, part)
            {
                case (1, 1): Day_01.Part_1(input); break;
                case (1, 2): Day_01.Part_2(input); break;
                case (2, 1): Day_02.Part_1(input); break;
                case (2, 2): Day_02.Part_2(input); break;
                case (3, 1): Day_03.Part_1(input); break;
                case (3, 2): Day_03.Part_2(input); break;
                case (4, 1): Day_04.Part_1(input); break;
                case (4, 2): Day_04.Part_2(input); break;
                case (5, 1): Day_05.Part_1(input); break;
                case (5, 2): Day_05.Part_2(input); break;
                case (6, 1): Day_06.Part_1(input); break;
                case (6, 2): Day_06.Part_2(input); break;
                case (7, 1): Day_07.Part_1(input); break;
                case (7, 2): Day_07.Part_2(input); break;
                case (8, 1): Day_08.Part_1(input); break;
                case (8, 2): Day_08.Part_2(input); break;
                case (9, 1): Day_09.Part_1(input); break;
                case (9, 2): Day_09.Part_2(input); break;
                case (10, 1): Day_10.Part_1(input); break;
                case (10, 2): Day_10.Part_2(input); break;

                default:
                    Console.WriteLine("Unexpected day " + args[0]);
                    break;
            }
        }
    }
}
