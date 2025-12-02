using advent_of_code_common.extensions;
using advent_of_code_common.input_reader;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace advent_of_code_2025.days
{
    internal class day_02
    {
        [DebuggerDisplay("[{low}-{high}]", Type = "c_range")]
        internal class c_range
        {
            public readonly UInt64 low;
            public readonly UInt64 high;

            public c_range(string input)
            {
                IEnumerable<UInt64> inputs = input.Split('-').Select(i => UInt64.Parse(i));

                low = inputs.First();
                high = inputs.Last();
            }

            // n => 10^n
            private UInt64 pow_10(UInt64 digits)
            {
                UInt64 result = 1;

                for (UInt64 i = 1; i < digits; i++)
                {
                    result = result * 10;
                }

                return result;
            }

            // xyz => xyzxyz
            private UInt64 double_number(UInt64 number, UInt64 digit_count)
            {
                UInt64 result = number;

                for (UInt64 i = 0; i < digit_count; i++)
                {
                    result = result * 10;
                }

                return result + number;
            }

            // xyz, n => xyzxyz...xyz (repeated n times)
            private UInt64 repeat_number(UInt64 number, UInt64 digit_count, UInt64 repeat_count)
            {
                UInt64 result = number;

                for (UInt64 r = 1; r < repeat_count; r++)
                {
                    for (UInt64 i = 0; i < digit_count; i++)
                    {
                        result = result * 10;
                    }

                    result += number;
                }

                return result;
            }

            // Find all numbers of the form a1a2...ana1a2...an
            public UInt64 sum_doubles()
            {
                UInt64 result = 0;

                UInt64 min_digits = low.count_digits();
                if (min_digits % 2 != 0)
                {
                    min_digits++;
                }

                UInt64 max_digits = high.count_digits();
                if (max_digits % 2 != 0)
                {
                    max_digits--;
                }

                for (UInt64 digits = min_digits; digits <= max_digits; digits += 2)
                {
                    UInt64 min = pow_10(digits / 2);
                    UInt64 max = pow_10(digits / 2 + 1) - 1;

                    for (UInt64 id = min; id <= max; id++)
                    {
                        UInt64 number = double_number(id, digits / 2);

                        if (number > high)
                        {
                            break;
                        }
                        else if (number >= low)
                        {
                            result += number;
                        }
                    }
                }

                return result;
            }

            // Find all numbers in the range that are made entirely of repeated smaller numbers.
            // Ex: aaaaaaa, ababababab, abcabc, abcabcabcabc, abcdabcdabcdabcd, etc
            public UInt64 sum_repeats()
            {
                // Numbers like 222222 could be found multiple times (2,2,2,2,2,2 or 22,22,22 or 222,222)
                HashSet<UInt64> results = new HashSet<UInt64>();

                // Find the range of digit counts we want to look for.
                // Ex: [11,333333] means we want to check from 2 digit numbers to 6 digit numbers.
                UInt64 min_digits = low.count_digits();
                UInt64 max_digits = high.count_digits();

                // Loop through each possible digit count
                // Ex: [11,333333] means look at 2, 3, 4, 5, and 6 digit numbers.
                for (UInt64 digits = min_digits; digits <= max_digits; digits++)
                {
                    // Loop through each possible set of repeats
                    // Ex: if 'digits' is 6, we want to check repeats that are 1, 2, or 3 digits long.
                    for (UInt64 repeat_size = 1; repeat_size <= digits / 2; repeat_size++)
                    {
                        // Only check repeat sizes that could result in our digit counts.
                        // Ex: if digits is 9, we would could fit repeats of size 3, but not 2 or 4.
                        if (digits % repeat_size == 0)
                        {
                            // Loop through all numbers in this repeat group
                            // Ex: if repeat_size was 3, we want to check from 100 to 999.

                            UInt64 min = pow_10(repeat_size);
                            UInt64 max = pow_10(repeat_size + 1) - 1;

                            for (UInt64 id = min; id <= max; id++)
                            {
                                // From our repeat size and id, build a number.
                                // Ex: id = 144, digits = 6, repeat_size = 3, number = 144144
                                UInt64 number = repeat_number(id, repeat_size, digits / repeat_size);

                                // Add any built repeated number if it's within [low, high]
                                if (number > high)
                                {
                                    break;
                                }
                                else if (number >= low)
                                {
                                    results.Add(number);
                                }
                            }
                        }
                    }
                }

                return results.sum();
            }
        }

        internal static c_range[] parse_input(
            in c_input_reader input_reader,
            in bool pretty)
        {
            return input_reader.read_line().Split(',').Select(input => new c_range(input)).ToArray();
        }

        // Given a range of numbers, find all numbers that are formed entirely of a smaller number repeated twice.
        // Ex: the range [998-1012] contains 1010.
        public static void part_1(
            c_input_reader input_reader,
            bool pretty)
        {
            c_range[] ranges = parse_input(input_reader, pretty);

            UInt64 result = ranges.Select(r => r.sum_doubles()).sum();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }

        // Given a range of numbers, find all numbers that are formed entirely of repeated smaller numbers.
        // Ex: the range [998-1012] contains 999 and 1010.
        public static void part_2(
            c_input_reader input_reader,
            bool pretty)
        {
            c_range[] ranges = parse_input(input_reader, pretty);

            UInt64 result = ranges.Select(r => r.sum_repeats()).sum();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }
    }
}
