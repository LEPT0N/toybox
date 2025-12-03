using System;
using System.Collections.Generic;
using System.Diagnostics;
using advent_of_code_common.extensions;
using advent_of_code_common.input_reader;

namespace advent_of_code_2025.days
{
    internal class day_03
    {
        internal static string[] parse_input(
            in c_input_reader input_reader,
            in bool pretty)
        {
            List<string> banks = new List<string>();

            while (input_reader.has_more_lines())
            {
                banks.Add(input_reader.read_line());
            }

            return banks.ToArray();
        }

        public static void part_1(
            c_input_reader input_reader,
            bool pretty)
        {
            string[] banks = parse_input(input_reader, pretty);

            int result = 0;

            foreach (string bank in banks)
            {
                int max = 0;

                // Just brute force finding every pair to get the max.
                for (int i = 0; i < bank.Length - 1; i++)
                {
                    for (int j = i + 1; j < bank.Length; j++)
                    {
                        int value = (bank[i] - '0') * 10 + (bank[j] - '0');

                        max = Math.Max(max, value);
                    }
                }

                result += max;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }

        private static UInt64 get_max(
            string bank,
            int start_index,
            int recursion_depth,
            Dictionary<(int, int), UInt64> library)
        {
            // Optimization: If we already know the result of this subtree, just return the previous value.
            // Without this it takes too long to run.
            if (library.TryGetValue((start_index, recursion_depth), out UInt64 result))
            {
                return result;
            }

            UInt64 max = 0;

            // Get the digit at the current index of the input string.
            UInt64 current = (UInt64)(bank[start_index] - '0');

            // If we're at the end of the string or we can't recurse anymore, just return the current digit.
            if (start_index == bank.Length - 1 || recursion_depth == 0)
            {
                max = current;
            }
            else
            {
                // Find the max for each subtree when the next character chosen is at index 'i'.
                // Don't iterate 'i' to the end since we know we have to fit 'recursion_depth' more characters later.
                for (int i = start_index + 1; i < bank.Length - recursion_depth + 1; i++)
                {
                    UInt64 total = get_max(bank, i, recursion_depth - 1, library);

                    if (total > max)
                    {
                        max = total;
                    }
                }

                // Prepend the current digit onto the max result.
                UInt64 digits = max.count_digits();

                for (UInt64 pow = 0; pow < digits; pow++)
                {
                    current *= 10;
                }

                max += current;
            }

            // Cache the result and return.
            library[(start_index, recursion_depth)] = max;

            return max;
        }

        public static void part_2(
            c_input_reader input_reader,
            bool pretty)
        {
            string[] banks = parse_input(input_reader, pretty);

            UInt64 result = 0;

            foreach (string bank in banks)
            {
                Dictionary<(int, int), UInt64> library = new Dictionary<(int, int), ulong>();

                UInt64 max = 0;

                // Check the max for starting at each character in the bank, saving the largest one.
                for (int i = 0; i < bank.Length - 11; i++)
                {
                    UInt64 current = get_max(bank, i, 11, library);

                    if (current > max)
                    {
                        max = current;
                    }
                }

                result += max;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }
    }
}
