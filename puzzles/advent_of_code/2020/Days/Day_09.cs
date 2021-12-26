using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace advent_of_code_2020.Days
{
    internal class Day_09
    {
        internal static UInt64[] parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            List<UInt64> numbers = new List<UInt64>();

            while (input_reader.has_more_lines())
            {
                numbers.Add(UInt64.Parse(input_reader.read_line()));
            }

            return numbers.ToArray();
        }

        internal static readonly int k_preamble = 25;

        public static void Day_09_Worker(
            string input,
            bool pretty)
        {
            UInt64[] numbers = parse_input(input, pretty);

            UInt64 invalid_number = 0;

            for (int current = k_preamble; current < numbers.Length; current++)
            {
                int preamble_start = current - k_preamble;
                int preamble_end = current - 1;

                bool found_sum = false;

                for (int first = preamble_start; first < preamble_end && !found_sum; first++)
                {
                    for (int second = first + 1; second <= preamble_end && !found_sum; second++)
                    {
                        if (numbers[first] + numbers[second] == numbers[current])
                        {
                            if (pretty)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("numbers[{0}] ({1}) + numbers[{2}] ({3}) = numbers[{4}] ({5})",
                                    first, numbers[first],
                                    second, numbers[second],
                                    current, numbers[current]);
                            }

                            found_sum = true;
                        }
                    }
                }

                if (!found_sum)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("numbers[{0}] ({1}) has no sum in it's preamble", current, numbers[current]);

                    invalid_number = numbers[current];
                    break;
                }
            }

            for (int start = 0; start < numbers.Length; start++)
            {
                int end = start;

                UInt64 sum = numbers[start];

                while (sum < invalid_number)
                {
                    end++;
                    sum += numbers[end];
                }

                if (sum == invalid_number)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;

                    Console.WriteLine();

                    Console.WriteLine("Sum of numbers[{0} .. {1}] ({2} .. {3}) == {4}",
                        start, end,
                        numbers[start], numbers[end],
                        invalid_number);

                    UInt64 min = numbers[start..end].Min();
                    UInt64 max = numbers[start..end].Max();

                    Console.WriteLine("Min + max = {0} + {1} = {2}",
                        min, max,
                        min + max);

                    break;
                }
            }

            Console.ResetColor();
        }

        public static void Part_1(
            string input,
            bool pretty)
        {
            Day_09_Worker(input, pretty);
        }

        public static void Part_2(
            string input,
            bool pretty)
        {
            Day_09_Worker(input, pretty);
        }
    }
}
