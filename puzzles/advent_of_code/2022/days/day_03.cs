using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using advent_of_code_common.input_reader;

namespace advent_of_code_2022.days
{
    internal class day_03
    {
        internal static int get_letter_score(char input)
        {
            if (input >= 'a' && input <= 'z')
            {
                return (input - 'a') + 1;
            }
            else if (input >= 'A' && input <= 'Z')
            {
                return (input - 'A') + 27;
            }
            else
            {
                throw new Exception($"Bad scored letter {input}");
            }
        }

        [DebuggerDisplay("{left} - {right}", Type = "c_rucksack")]
        internal class c_rucksack
        {
            public readonly string total;
            public readonly string left;
            public readonly string right;

            public c_rucksack(string input)
            {
                total = input;
                left = input.Substring(0, input.Length / 2);
                right = input.Substring(input.Length / 2);
            }

            public char[] get_common()
            {
                return left.Where(l => right.Contains(l)).Distinct().ToArray();
            }

            public int get_score()
            {
                char[] common = get_common();

                if (common.Length == 0)
                {
                    return 0;
                }
                else
                {
                    return get_letter_score(common[0]);
                }
            }
        }

        [DebuggerDisplay("[{one}] [{two}] [{three}]", Type = "c_group")]
        internal class c_group
        {
            public readonly c_rucksack one;
            public readonly c_rucksack two;
            public readonly c_rucksack three;

            public c_group(string input_one, string input_two, string input_three)
            {
                one = new c_rucksack(input_one);
                two = new c_rucksack(input_two);
                three = new c_rucksack(input_three);
            }

            public char[] get_common()
            {
                return one.total.Where(r => two.total.Contains(r)).Where(r => three.total.Contains(r)).Distinct().ToArray();
            }

            public int get_score()
            {
                char[] common = get_common();

                if (common.Length == 0)
                {
                    return 0;
                }
                else
                {
                    return get_letter_score(common[0]);
                }
            }
        }

        internal static c_rucksack[] parse_input_1(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            List<c_rucksack> rucksacks = new List<c_rucksack>();

            while (input_reader.has_more_lines())
            {
                rucksacks.Add(new c_rucksack(input_reader.read_line()));
            }

            return rucksacks.ToArray();
        }

        public static void part_1(
            string input,
            bool pretty)
        {
            c_rucksack[] rucksacks = parse_input_1(input, pretty);

            char[][] common = rucksacks.Select(r => r.get_common()).ToArray();

            int[] scores = rucksacks.Select(r => r.get_score()).ToArray();

            int score = scores.Sum();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", score);
            Console.ResetColor();
        }

        internal static c_group[] parse_input_2(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            List<c_group> groups = new List<c_group>();

            while (input_reader.has_more_lines())
            {
                groups.Add(new c_group(
                    input_reader.read_line(),
                    input_reader.read_line(),
                    input_reader.read_line()));
            }

            return groups.ToArray();
        }

        public static void part_2(
            string input,
            bool pretty)
        {
            c_group[] groups = parse_input_2(input, pretty);

            char[][] badges = groups.Select(g => g.get_common()).ToArray();

            int[] scores = groups.Select(g => g.get_score()).ToArray();

            int score = scores.Sum();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", score);
            Console.ResetColor();
        }
    }
}
