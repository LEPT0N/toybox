using System;
using System.Collections.Generic;
using System.Linq;
using advent_of_code_common.extensions;
using advent_of_code_common.input_reader;

namespace advent_of_code_2024.days
{
    internal class day_11
    {
        internal static LinkedList<Int64> parse_input_1(
            in c_input_reader input_reader,
            in bool pretty)
        {
            LinkedList<Int64> stones = new LinkedList<Int64>();

            while (input_reader.has_more_lines())
            {
                input_reader
                    .read_line()
                    .Split()
                    .Select(c => Int64.Parse(c))
                    .for_each(i => stones.AddLast(i));
            }

            return stones;
        }

        internal static void display_stones(LinkedList<Int64> stones)
        {
            for (LinkedListNode<Int64> current = stones.First; current != null; current = current.Next)
            {
                Console.Write($"{current.Value} ");
            }

            Console.WriteLine();
        }

        public static void part_1(
            c_input_reader input_reader,
            bool pretty)
        {
            LinkedList<Int64> stones = parse_input_1(input_reader, pretty);

            if (pretty)
            {
                Console.WriteLine("Initial arrangement:");
                display_stones(stones);
                Console.WriteLine();
            }

            int blink_count = 25;

            for (int i = 0; i < blink_count; i++)
            {
                for (LinkedListNode<Int64> current = stones.First; current != null; current = current.Next)
                {
                    string current_string_value = current.Value.ToString();

                    if (current.Value == 0)
                    {
                        current.Value = 1;
                    }
                    else if (current_string_value.Length % 2 == 0)
                    {
                        string left_value = current_string_value.Substring(0, current_string_value.Length / 2);
                        string right_value = current_string_value.Substring(current_string_value.Length / 2);

                        stones.AddBefore(current, Int64.Parse(left_value));

                        current.Value = int.Parse(right_value);

                    }
                    else
                    {
                        current.Value *= 2024;
                    }
                }

                if (pretty)
                {
                    Console.WriteLine($"After {i} blink(s):");
                    display_stones(stones);
                    Console.WriteLine();
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {stones.Count}");
            Console.ResetColor();
        }

        internal static void insert_values(
            Dictionary<Int64, Int64> stones,
            Int64 value,
            Int64 count)
        {
            if (stones.ContainsKey(value))
            {
                stones[value] += count;
            }
            else
            {
                stones.Add(value, count);
            }
        }

        internal static Dictionary<Int64, Int64> parse_input_2(
            in c_input_reader input_reader,
            in bool pretty)
        {
            Dictionary<Int64, Int64> stones = new Dictionary<Int64, Int64>();

            while (input_reader.has_more_lines())
            {
                foreach (string s in input_reader.read_line().Split())
                {
                    Int64 value = Int64.Parse(s);

                    insert_values(stones, value, 1);
                }
            }

            return stones;
        }

        internal static void display_stones(
            Dictionary<Int64, Int64> stones)
        {
            foreach (Int64 value in stones.Keys)
            {
                if (stones[value] == 1)
                {
                    Console.Write($"{value} ");
                }
                else
                {
                    Console.Write($"{value}x{stones[value]} ");
                }
            }

            Console.WriteLine();
        }

        public static void part_2(
            c_input_reader input_reader,
            bool pretty)
        {
            // stones.Keys => the numbers on each stone.
            // stones.Values => the count of stones with that number.

            Dictionary<Int64, Int64> stones = parse_input_2(input_reader, pretty);

            if (pretty)
            {
                Console.WriteLine("Initial arrangement:");
                display_stones(stones);
                Console.WriteLine();
            }

            int blink_count = 75;

            for (int i = 0; i < blink_count; i++)
            {
                Dictionary<Int64, Int64> new_stones = new Dictionary<Int64, Int64>();

                foreach (Int64 value in stones.Keys)
                {
                    string string_value = value.ToString();

                    if (value == 0)
                    {
                        insert_values(new_stones, 1, stones[value]);
                    }
                    else if (string_value.Length % 2 == 0)
                    {
                        string left_value = string_value.Substring(0, string_value.Length / 2);
                        string right_value = string_value.Substring(string_value.Length / 2);

                        insert_values(new_stones, Int64.Parse(left_value), stones[value]);
                        insert_values(new_stones, Int64.Parse(right_value), stones[value]);
                    }
                    else
                    {
                        insert_values(new_stones, value * 2024, stones[value]);
                    }
                }

                stones = new_stones;

                if (pretty)
                {
                    Console.WriteLine($"After {i} blink(s):");
                    display_stones(stones);
                    Console.WriteLine();
                }
            }

            Int64 result = stones.Keys.Select(key => stones[key]).Sum();

            Console.WriteLine($"Achieved with only {stones.Count} elements in the dictionary.");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }
    }
}
