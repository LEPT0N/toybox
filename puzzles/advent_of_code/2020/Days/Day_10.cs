using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace advent_of_code_2020.Days
{
    internal class Day_10
    {
        internal static int[] parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            List<int> numbers = new List<int>();

            numbers.Add(0);

            while (input_reader.has_more_lines())
            {
                numbers.Add(int.Parse(input_reader.read_line()));
            }

            return numbers.OrderBy(x => x).ToArray();
        }

        public static void Part_1(
            string input,
            bool pretty)
        {
            int[] numbers = parse_input(input, pretty);

            int[] diff_counts = new int[4];
            diff_counts[3]++;

            for (int i = 1; i < numbers.Length; i++)
            {
                diff_counts[numbers[i] - numbers[i-1]]++;
            }

            for(int i = 1; i < diff_counts.Length; i++)
            {
                Console.WriteLine("diff_counts[{0}] = {1}", i, diff_counts[i]);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", diff_counts[3] * diff_counts[1]);
            Console.ResetColor();
        }

        internal static void populate_arrangement_counts(int current_index, int[] numbers, UInt64[] arrangement_counts)
        {
            UInt64 result = 0;

            int current_number = numbers[current_index];

            for (int index = current_index + 1; index < numbers.Length && (numbers[index] - current_number) <= 3; index++)
            {
                result += arrangement_counts[index];
            }

            arrangement_counts[current_index] = result;
        }

        public static void Part_2(
            string input,
            bool pretty)
        {
            int[] numbers = parse_input(input, pretty);

            UInt64[] arrangement_counts = new ulong[numbers.Length];
            arrangement_counts[arrangement_counts.Length - 1] = 1;

            for (int i = numbers.Length - 2; i >= 0;  i--)
            {
                populate_arrangement_counts(i, numbers, arrangement_counts);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", arrangement_counts[0]);
            Console.ResetColor();
        }
    }
}
