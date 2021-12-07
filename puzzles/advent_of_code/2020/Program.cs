using System;
using advent_of_code_2020.Days;

namespace advent_of_code_2020
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int day = Int32.Parse(args[0]);
            int part = Int32.Parse(args[1]);
            string input = args[2];

            switch (day, part)
            {
                case (1, 1): Day_1.Part_1(input); break;
                case (1, 2): Day_1.Part_2(input); break;
                case (2, 1): Day_2.Part_1(input); break;
                case (2, 2): Day_2.Part_2(input); break;
                case (3, 1): Day_3.Part_1(input); break;
                case (3, 2): Day_3.Part_2(input); break;
                case (4, 1): Day_4.Part_1(input); break;
                case (4, 2): Day_4.Part_2(input); break;
                case (5, 1): Day_5.Part_1(input); break;
                case (5, 2): Day_5.Part_2(input); break;
                case (6, 1): Day_6.Part_1(input); break;
                case (6, 2): Day_6.Part_2(input); break;

                default:
                    Console.WriteLine("Unexpected day " + args[0]);
                    break;
            }
        }
    }
}
