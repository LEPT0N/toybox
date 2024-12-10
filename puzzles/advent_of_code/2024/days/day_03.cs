using System;
using System.Text.RegularExpressions;
using advent_of_code_common.input_reader;

namespace advent_of_code_2024.days
{
    internal class day_03
    {
        public static void part_1(
            c_input_reader input_reader,
            bool pretty)
        {
            Regex mul_format = new Regex(@"mul\((\d{1,3}),(\d{1,3})\)");

            int result = 0;

            while (input_reader.has_more_lines())
            {
                string input_line = input_reader.read_line();

                MatchCollection mul_matches = mul_format.Matches(input_line);

                foreach (Match mul_match in mul_matches)
                {
                    int first = int.Parse(mul_match.Groups[1].Value);
                    int second = int.Parse(mul_match.Groups[2].Value);

                    result += first * second;
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", result);
            Console.ResetColor();
        }

        public static void part_2(
            c_input_reader input_reader,
            bool pretty)
        {
            Regex mul_format = new Regex(@"mul\((\d{1,3}),(\d{1,3})\)|(do\(\))|(don't\(\))");

            int result = 0;
            bool enabled = true;

            while (input_reader.has_more_lines())
            {
                string input_line = input_reader.read_line();

                MatchCollection mul_matches = mul_format.Matches(input_line);

                foreach (Match mul_match in mul_matches)
                {
                    if (!string.IsNullOrEmpty(mul_match.Groups[1].Value))
                    {
                        if (enabled)
                        {
                            // Found a mul

                            int first = int.Parse(mul_match.Groups[1].Value);
                            int second = int.Parse(mul_match.Groups[2].Value);

                            result += first * second;
                        }
                    }
                    else if (!string.IsNullOrEmpty(mul_match.Groups[3].Value))
                    {
                        // Found a do
                        enabled = true;
                    }
                    else if (!string.IsNullOrEmpty(mul_match.Groups[4].Value))
                    {
                        // Found a don't
                        enabled = false;
                    }
                    else
                    {
                        throw new Exception("oops");
                    }
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", result);
            Console.ResetColor();
        }
    }
}

// not right: 28546082
