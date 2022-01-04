using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace advent_of_code_2020.Days
{
    internal class Day_15
    {
        internal static int[] parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            return input_reader.read_line().Split(",").Select(x => int.Parse(x)).ToArray();
        }

        public static void Day_15_Worker(
            string input,
            bool pretty,
            int max_numbers_spoken)
        {
            int[] starting_numbers = parse_input(input, pretty);

            int previous_number_spoken = -1;
            int numbers_spoken = 0;
            Dictionary<int, int> previous_time_spoken = new Dictionary<int, int>();

            foreach (int starting_number in starting_numbers)
            {
                if (previous_number_spoken >= 0)
                {
                    previous_time_spoken[previous_number_spoken] = numbers_spoken;
                }

                numbers_spoken++;

                if (pretty)
                {
                    Console.WriteLine("Turn {0} = {1}", numbers_spoken, starting_number);
                }

                previous_number_spoken = starting_number;
            }

            while (numbers_spoken < max_numbers_spoken)
            {
                int current_number_spoken = 0;

                if (previous_time_spoken.ContainsKey(previous_number_spoken))
                {
                    current_number_spoken = numbers_spoken - previous_time_spoken[previous_number_spoken];
                }

                previous_time_spoken[previous_number_spoken] = numbers_spoken;

                numbers_spoken++;

                if (pretty)
                {
                    Console.WriteLine("Turn {0} = {1}", numbers_spoken, current_number_spoken);
                }

                previous_number_spoken = current_number_spoken;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", previous_number_spoken);
            Console.ResetColor();
        }

        public static void Part_1(
            string input,
            bool pretty)
        {
            Day_15_Worker(input, pretty, 2020);
        }

        public static void Part_2(
            string input,
            bool pretty)
        {
            Day_15_Worker(input, pretty, 30000000);
        }
    }
}
