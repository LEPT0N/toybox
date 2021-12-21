using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace advent_of_code_2021.Days
{
    internal class Day_XX
    {
        [DebuggerDisplay("todo", Type = "c_type")]
        internal class c_type
        {
        }

        internal static void parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            while (input_reader.has_more_lines())
            {
                input_reader.read_line();
            }
        }

        public static void Part_1(
            string input,
            bool pretty)
        {
            parse_input(input, pretty);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", 0);
            Console.ResetColor();
        }

        public static void Part_2(
            string input,
            bool pretty)
        {
            // parse_input(input, pretty);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", 0);
            Console.ResetColor();
        }
    }
}
