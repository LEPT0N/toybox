using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.input_reader;

namespace advent_of_code_2022.days
{
    internal class day_04
    {
        [DebuggerDisplay("{min}-{max}", Type = "s_range")]
        internal class s_range
        {
            public int min { get; set; }
            public int max { get; set; }

            public s_range(string input)
            {
                string[] inputs = input.Split('-');

                min = int.Parse(inputs[0]);
                max = int.Parse(inputs[1]);
            }

            public bool contains_or_contained_by(s_range other)
            {
                return min <= other.min && max >= other.max
                    || other.min <= min && other.max >= max;
            }

            public bool overlaps(s_range other)
            {
                return min <= other.max && other.min <= max;
            }
        }

        internal static s_range[][] parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            List<s_range[]> ranges = new List<s_range[]>();

            while (input_reader.has_more_lines())
            {
                string[] range_pair_input = input_reader.read_line().Split(',');

                s_range[] range_pair = range_pair_input.Select(input => new s_range(input)).ToArray();

                ranges.Add(range_pair);
            }

            return ranges.ToArray();
        }

        public static void part_1(
            string input,
            bool pretty)
        {
            s_range[][] ranges = parse_input(input, pretty);

            s_range[][] redundant_ranges = ranges.Where(range_pair => range_pair[0].contains_or_contained_by(range_pair[1])).ToArray();

            int num_redundant_ranges = redundant_ranges.Length;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", num_redundant_ranges);
            Console.ResetColor();
        }

        public static void part_2(
            string input,
            bool pretty)
        {
            s_range[][] ranges = parse_input(input, pretty);

            s_range[][] overlapping_ranges = ranges.Where(range_pair => range_pair[0].overlaps(range_pair[1])).ToArray();

            int num_overlapping_ranges = overlapping_ranges.Length;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", num_overlapping_ranges);
            Console.ResetColor();
        }
    }
}
