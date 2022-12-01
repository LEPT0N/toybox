using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.input_reader;

namespace advent_of_code_2022.days
{
    internal class day_01
    {
        internal static int[][] parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            List<int[]> all_groups = new List<int[]>();
            List<int> current_group = new List<int>();

            while (input_reader.has_more_lines())
            {
                string line = input_reader.read_line();

                if (string.IsNullOrEmpty(line))
                {
                    all_groups.Add(current_group.ToArray());
                    current_group = new List<int>();
                }
                else
                {
                    current_group.Add(int.Parse(line));
                }
            }

            if (current_group.Any())
            {
                all_groups.Add(current_group.ToArray());
            }

            return all_groups.ToArray();
        }

        public static void part_1(
            string input,
            bool pretty)
        {
            int[][] groups = parse_input(input, pretty);

            int max_group_sum = groups.Max(group => group.Sum());

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", max_group_sum);
            Console.ResetColor();
        }

        public static void part_2(
            string input,
            bool pretty)
        {
            int[][] groups = parse_input(input, pretty);

            int[] max_groups = groups.Select(group => group.Sum()).OrderByDescending(x => x).Take(3).ToArray();

            int max_groups_sum = max_groups.Sum();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", max_groups_sum);
            Console.ResetColor();
        }
    }
}
