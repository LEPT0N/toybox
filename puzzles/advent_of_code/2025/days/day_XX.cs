using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.input_reader;

namespace advent_of_code_2025.days
{
    internal class day_XX
    {
        [DebuggerDisplay("todo", Type = "c_type")]
        internal class c_type
        {
#pragma warning disable 0169
            int unused;
#pragma warning restore 0169
        }

        internal static void parse_input(
            in c_input_reader input_reader,
            in bool pretty)
        {
            while (input_reader.has_more_lines())
            {
                input_reader.read_line();
            }
        }

        public static void part_1(
            c_input_reader input_reader,
            bool pretty)
        {
            parse_input(input_reader, pretty);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {0}");
            Console.ResetColor();
        }

        public static void part_2(
            c_input_reader input_reader,
            bool pretty)
        {
            // parse_input(input_reader, pretty);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {0}");
            Console.ResetColor();
        }
    }
}
