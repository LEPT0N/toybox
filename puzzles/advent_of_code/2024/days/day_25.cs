using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.input_reader;

namespace advent_of_code_2024.days
{
    internal class day_25
    {
        [DebuggerDisplay("{one},{two},{three},{four},{five}", Type = "c_combination")]
        internal class c_combination
        {
            public int one = -1;
            public int two = -1;
            public int three = -1;
            public int four = -1;
            public int five = -1;

            public bool is_match(c_combination other)
            {
                return this.one + other.one <= 5 &&
                    this.two + other.two <= 5 &&
                    this.three + other.three <= 5 &&
                    this.four + other.four <= 5 &&
                    this.five + other.five <= 5;
            }
        }

        internal static (c_combination[], c_combination[]) parse_input(
            in c_input_reader input_reader)
        {
            List<c_combination> locks = [];
            List<c_combination> keys = [];

            while (input_reader.has_more_lines())
            {
                bool is_lock = input_reader.peek_line().Equals("#####");

                c_combination combination = new();

                while (!string.IsNullOrEmpty(input_reader.try_peek_line()))
                {
                    string input_line = input_reader.read_line();
                    if (input_line[0] == '#') { combination.one++; }
                    if (input_line[1] == '#') { combination.two++; }
                    if (input_line[2] == '#') { combination.three++; }
                    if (input_line[3] == '#') { combination.four++; }
                    if (input_line[4] == '#') { combination.five++; }
                }

                if (is_lock)
                {
                    locks.Add(combination);
                }
                else
                {
                    keys.Add(combination);
                }

                input_reader.try_read_line();
            }

            return ([..locks], [..keys]);
        }

        public static void part_1(
            c_input_reader input_reader,
            bool pretty)
        {
            (c_combination[] locks, c_combination[] keys) = parse_input(input_reader);

            int result = locks.Sum(l => keys.Where(k => l.is_match(k)).Count());

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }

        public static void part_2(
            c_input_reader input_reader,
            bool pretty)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {0}");
            Console.ResetColor();
        }
    }
}
