using System;
using System.Collections.Generic;
using System.Linq;
using advent_of_code_common.input_reader;

namespace advent_of_code_2024.days
{
    internal class day_01
    {
        internal static (int[], int[]) parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            List<int> first = new List<int>();
            List<int> second = new List<int>();

            while (input_reader.has_more_lines())
            {
                string input_line = input_reader.read_line();

                string[] tokens = input_line.Split(' ');

                int first_number = int.Parse(tokens.First());
                int second_number = int.Parse(tokens.Last());

                first.Add(first_number);
                second.Add(second_number);
            }

            return (first.ToArray(), second.ToArray());
        }

        public static void part_1(
            string input,
            bool pretty)
        {
            (int[] first, int[] second) = parse_input(input, pretty);

            Array.Sort(first);
            Array.Sort(second);

            int result = 0;

            for (int i = 0; i < first.Length; i++)
            {
                int difference = Math.Abs(first[i] - second[i]);
                result += difference;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", result);
            Console.ResetColor();
        }

        public static void part_2(
            string input,
            bool pretty)
        {
            (int[] first, int[] second) = parse_input(input, pretty);

            int result = 0;

            for (int i = 0; i < first.Length; i++)
            {
                int value = first[i];

                int count = second.Where(item => item == value).Count();

                result += value * count;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", result);
            Console.ResetColor();
        }
    }
}
