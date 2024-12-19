using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.extensions;
using advent_of_code_common.input_reader;

namespace advent_of_code_2024.days
{
    internal class day_19
    {
        [DebuggerDisplay("{stripes}", Type = "c_towel")]
        internal class c_towel
        {
            public readonly string stripes;

            public c_towel(
                string s)
            {
                stripes = s;
            }

            private static Int64 count_builds_from(
                string stripes,
                c_towel[] patterns,
                Dictionary<string, Int64> cached_results)
            {
                if (cached_results.ContainsKey(stripes))
                {
                    return cached_results[stripes];
                }

                Int64 result = 0;

                for (int i = 0; i < patterns.Length; i++)
                {
                    c_towel pattern = patterns[i];

                    if (pattern.stripes == stripes)
                    {
                        result++;
                    }
                    else if (stripes.common_prefix(pattern.stripes) == pattern.stripes)
                    {
                        result += count_builds_from(stripes.Substring(pattern.stripes.Length), patterns, cached_results);
                    }
                }

                cached_results[stripes] = result;

                return result;
            }

            public Int64 count_builds_from(
                c_towel[] patterns)
            {
                Dictionary<string, Int64> cached_results = new Dictionary<string, Int64>();

                return count_builds_from(stripes, patterns, cached_results);
            }
        }

        internal static (c_towel[], c_towel[]) parse_input(
            in c_input_reader input_reader,
            in bool pretty)
        {
            c_towel[] patterns = input_reader
                .read_line()
                .Split(", ")
                .Select(s => new c_towel(s))
                .ToArray();

            input_reader.read_line();

            List<c_towel> designs = new List<c_towel>();

            while (input_reader.has_more_lines())
            {
                designs.Add(new c_towel(input_reader.read_line()));
            }

            return (patterns, designs.ToArray());
        }

        public static void part_1(
            c_input_reader input_reader,
            bool pretty)
        {
            (c_towel[] patterns, c_towel[] designs) = parse_input(input_reader, pretty);

            int result = 0;

            foreach (c_towel design in designs)
            {
                Int64 result_count = design.count_builds_from(patterns);

                if (result_count > 0)
                {
                    result++;
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }

        public static void part_2(
            c_input_reader input_reader,
            bool pretty)
        {
            (c_towel[] patterns, c_towel[] designs) = parse_input(input_reader, pretty);

            Int64 result = 0;

            foreach (c_towel design in designs)
            {
                result += design.count_builds_from(patterns);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }
    }
}
