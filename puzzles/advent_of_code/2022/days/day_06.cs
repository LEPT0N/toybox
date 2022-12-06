using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.input_reader;

namespace advent_of_code_2022.days
{
    internal class day_06
    {
        internal static int find_first_substring_with_no_duplicates(string input, int substring_length)
        {
            for (int i = 0; i < input.Length - substring_length; i++)
            {
                if (input.Substring(i, substring_length).Distinct().Count() == substring_length)
                {
                    return i + substring_length;
                }
            }

            return -1;
        }

        public static void part_1(
            string input,
            bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            string input_line = input_reader.read_line();

            int result = find_first_substring_with_no_duplicates(input_line, 4);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", result);
            Console.ResetColor();
        }

        public static void part_2(
            string input,
            bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            string input_line = input_reader.read_line();

            int result = find_first_substring_with_no_duplicates(input_line, 14);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", result);
            Console.ResetColor();
        }
    }
}
