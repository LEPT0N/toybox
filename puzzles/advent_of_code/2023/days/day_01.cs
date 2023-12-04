using System;
using System.Collections.Generic;
using System.Linq;
using advent_of_code_common.input_reader;

namespace advent_of_code_2023.days
{
    internal class day_01
    {
        internal static char[][] parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            List<char[]> lines = new List<char[]>();

            while (input_reader.has_more_lines())
            {
                lines.Add(input_reader.read_line().ToArray());
            }

            return lines.ToArray();
        }

        public static void part_1(
            string input,
            bool pretty)
        {
            char[][] lines = parse_input(input, pretty);

            List<int> values = new List<int>();

            foreach (char[] line in lines)
            {
                int[] digits = line
                    .Where(c => c >= '0' && c <= '9')
                    .Select(c => c - '0').ToArray();

                int value = digits.First() * 10 + digits.Last();

                values.Add(value);
            }

            int sum = values.Sum();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", sum);
            Console.ResetColor();
        }

        public static void part_2(
            string input,
            bool pretty)
        {
            char[][] lines = parse_input(input, pretty);

            string[] target_values =
            {
                "0", "zero",
                "1", "one",
                "2", "two",
                "3", "three",
                "4", "four",
                "5", "five",
                "6", "six",
                "7", "seven",
                "8", "eight",
                "9", "nine",
			};

            int sum = 0;

			foreach (char[] line in lines)
            {
                string string_line = new string(line);

                int first_index = lines.Length;
				int first_value = -1;

				int last_index = -1;
				int last_value = -1;

                for (int target_index = 0; target_index < target_values.Length; target_index++)
                {
                    string target_value = target_values[target_index];

                    int index = string_line.IndexOf(target_value);
                    int value = target_index / 2;

                    if (index >= 0 && index < first_index)
                    {
                        first_index = index;
                        first_value = value;
                    }

					index = string_line.LastIndexOf(target_value);
					value = target_index / 2;

					if (index >= 0 && index > last_index)
					{
						last_index = index;
						last_value = value;
                    }
				}

                int line_value = first_value * 10 + last_value;

                sum += line_value;
			}

			Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", sum);
            Console.ResetColor();
        }
    }
}
