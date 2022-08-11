using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.input_reader;

namespace advent_of_code_2019.days
{
    internal class day_04
    {
        internal static (int, int) parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            int[] output = input_reader.read_line().Split('-').Select(x => int.Parse(x)).ToArray();

            return (output[0], output[1]);
        }

        internal static int[] split_number(int n)
        {
            List<int> result = new List<int>();

            while (n > 0)
            {
                result.Add(n % 10);
                n = n / 10;
            }

            return result.ToArray();
        }

        internal static bool less_than_or_equal_to(int[] a, int[] b)
        {
            if (a.Length > b.Length)
            {
                return false;
            }

            for (int i = a.Length - 1; i >= 0; i--)
            {
                if (a[i] < b[i])
                {
                    return true;
                }
                else if (a[i] > b[i])
                {
                    return false;
                }
            }

            return true;
        }

        internal static bool is_valid(int[] a, bool require_naked_pair)
        {
            bool has_pair = false;
            bool has_naked_pair = false;

            for (int i = 0; i < a.Length - 1; i++)
            {
                if (a[i] < a[i + 1])
                {
                    return false;
                }
                else if (a[i] == a[i + 1])
                {
                    has_pair = true;

                    if ((i == 0 || a[i - 1] != a[i]) &&
                        (i == a.Length - 2 || a[i] != a[i + 2]))
                    {
                        has_naked_pair = true;
                    }
                }
            }

            if (require_naked_pair && !has_naked_pair)
            {
                return false;
            }

            return has_pair;
        }

        internal static void increment(ref int[] a)
        {
            for (int i = 0; i < a.Length; i++)
            {
                a[i]++;

                for (int j = i - 1; j >=0; j--)
                {
                    a[j] = a[i];
                }

                if (a[i] <= 9)
                {
                    break;
                }
            }
        }

        internal static void display(int[] a)
        {
            for (int i = a.Length - 1; i >= 0; i--)
            {
                Console.Write("{0}", a[i]);
            }

            Console.WriteLine();
        }

        internal static void worker(
            string input,
            bool pretty,
            bool require_naked_pair)
        {
            (int min, int max) = parse_input(input, pretty);

            int[] max_split = split_number(max);

            int num_valid_passwords = 0;

            for (int[] current = split_number(min);
                less_than_or_equal_to(current, max_split);
                increment(ref current))
            {
                if (is_valid(current, require_naked_pair))
                {
                    num_valid_passwords++;

                    if (pretty)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        display(current);
                    }
                }
                else if(pretty && require_naked_pair && is_valid(current, false))
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    display(current);
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", num_valid_passwords);
            Console.ResetColor();
        }

        public static void part_1(
            string input,
            bool pretty)
        {
            worker(input, pretty, false);
        }

        public static void part_2(
            string input,
            bool pretty)
        {
            worker(input, pretty, true);
        }
    }
}
