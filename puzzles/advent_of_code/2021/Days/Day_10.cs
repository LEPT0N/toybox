using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace advent_of_code_2021.Days
{
    internal class Day_10
    {
        [DebuggerDisplay("{DebuggerDisplay,nq}", Type = "c_chunk")]
        internal class c_chunk
        {
            public readonly char start_char;
            public readonly char end_char;

            public c_chunk(char input)
            {
                start_char = input;

                switch (input)
                {
                    case '(': end_char = ')'; break;
                    case '[': end_char = ']'; break;
                    case '{': end_char = '}'; break;
                    case '<': end_char = '>'; break;
                }
            }

            private string DebuggerDisplay
            {
                get { return String.Format("{0} {1}", start_char, end_char); }
            }
        }

        internal static int get_score(char input)
        {
            switch (input)
            {
                case ')': return 3;
                case ']': return 57;
                case '}': return 1197;
                case '>': return 25137;
            }

            return 0;
        }

        public static void Part_1(string input)
        {
            c_input_reader input_reader = new c_input_reader(input);

            int result = 0;

            while (input_reader.has_more_lines())
            {
                string input_line = input_reader.read_line();

                Stack<c_chunk> chunks = new Stack<c_chunk>();

                bool line_corrupt = false;

                foreach(char input_char in input_line)
                {
                    if (input_char == '(' ||
                        input_char == '[' ||
                        input_char == '{' ||
                        input_char == '<')
                    {
                        chunks.Push(new c_chunk(input_char));
                    }
                    else
                    {
                        c_chunk chunk = chunks.Pop();

                        if (input_char != chunk.end_char)
                        {
                            int score = get_score(input_char);

                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(
                                "Expected {0}, but found {1} instead. ({2} points)",
                                chunk.end_char,
                                input_char,
                                score);

                            result += score;

                            line_corrupt = true;
                            break;
                        }
                    }
                }

                if (!line_corrupt)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("Line valid.");
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", result);
            Console.ResetColor();
        }

        internal static UInt64 get_score2(char input)
        {
            switch (input)
            {
                case ')': return 1;
                case ']': return 2;
                case '}': return 3;
                case '>': return 4;
            }

            return 0;
        }

        public static void Part_2(string input)
        {
            c_input_reader input_reader = new c_input_reader(input);

            List<UInt64> scores = new List<UInt64>();

            while (input_reader.has_more_lines())
            {
                string input_line = input_reader.read_line();

                Stack<c_chunk> chunks = new Stack<c_chunk>();

                bool line_corrupt = false;

                foreach (char input_char in input_line)
                {
                    if (input_char == '(' ||
                        input_char == '[' ||
                        input_char == '{' ||
                        input_char == '<')
                    {
                        chunks.Push(new c_chunk(input_char));
                    }
                    else
                    {
                        c_chunk chunk = chunks.Pop();

                        if (input_char != chunk.end_char)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(
                                "Corrupt line. Expected {0}, but found {1} instead.",
                                chunk.end_char,
                                input_char);

                            line_corrupt = true;
                            break;
                        }
                    }
                }

                if (!line_corrupt)
                {
                    if (chunks.Count != 0)
                    {
                        UInt64 score = 0;
                        string completion = "";

                        while (chunks.Count != 0)
                        {
                            c_chunk chunk = chunks.Pop();

                            completion += chunk.end_char;

                            score *= 5;
                            score += get_score2(chunk.end_char);
                        }

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(
                            "Incomplete line. Complete by adding {0}. ({1} points)",
                            completion,
                            score);

                        scores.Add(score);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine("Line valid.");
                    }
                }
            }

            UInt64 result = scores.OrderBy(x => x).ElementAt(scores.Count / 2);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", result);
            Console.ResetColor();
        }
    }
}
