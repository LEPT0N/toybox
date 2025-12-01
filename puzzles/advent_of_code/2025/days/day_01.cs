using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.input_reader;

namespace advent_of_code_2025.days
{
    internal class day_01
    {
        internal enum e_turn_direction
        {
            left,
            right,
        }

        [DebuggerDisplay("todo", Type = "c_turn")]
        internal class c_turn
        {
            public readonly e_turn_direction direction;
            public readonly int amount;

            public c_turn(
                e_turn_direction d,
                int a)
            {
                direction = d;
                amount = a;
            }

            public (int, int) apply_to(
                int max,
                int start)
            {
                int result = 0;

                int hits = amount / max;
                int turn_amount = amount % max;

                if (turn_amount > 0)
                {
                    if (direction == e_turn_direction.left)
                    {
                        turn_amount = -turn_amount;
                    }

                    result = start + turn_amount;

                    if (result == 0)
                    {
                        hits++;
                    }
                    else if (result < 0)
                    {
                        result += max;

                        if (start != 0)
                        {
                            hits++;
                        }
                    }
                    else if (result >= max)
                    {
                        result -= max;

                        if (start != 0)
                        {
                            hits++;
                        }
                    }
                }

                return (result, hits);
            }
        }

        internal static c_turn[] parse_input(
            in c_input_reader input_reader,
            in bool pretty)
        {
            List<c_turn> turns = new List<c_turn>();

            while (input_reader.has_more_lines())
            {
                string line = input_reader.read_line();

                e_turn_direction direction;
                switch (line[0])
                {
                    case 'R': direction = e_turn_direction.right; break;
                    case 'L': direction = e_turn_direction.left; break;
                    default: throw new ArgumentException($"Invalid direction {line[0]}");
                }

                int amount = Int32.Parse(line.Substring(1));

                turns.Add(new c_turn(direction, amount));
            }

            return turns.ToArray();
        }

        public static void part_1(
            c_input_reader input_reader,
            bool pretty)
        {
            c_turn[] turns = parse_input(input_reader, pretty);

            int dial = 50;

            int result = 0;

            foreach (c_turn turn in turns)
            {
                (dial, _) = turn.apply_to(100, dial);

                if (dial == 0)
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
            c_turn[] turns = parse_input(input_reader, pretty);

            int dial = 50;

            int result = 0;

            foreach (c_turn turn in turns)
            {
                (dial, int hits) = turn.apply_to(100, dial);

                result += hits;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }
    }
}
