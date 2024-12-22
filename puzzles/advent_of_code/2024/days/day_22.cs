using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.input_reader;

namespace advent_of_code_2024.days
{
    internal class day_22
    {
        [DebuggerDisplay("{first}, {second}, {third}, {fourth}", Type = "s_price_difference_history")]
        internal struct s_price_difference_history
        {
            public Int64 first;
            public Int64 second;
            public Int64 third;
            public Int64 fourth;
        }

        [DebuggerDisplay("", Type = "c_price_difference_history_comparer")]
        internal class c_price_difference_history_comparer : IEqualityComparer<s_price_difference_history>
        {
            public bool Equals(s_price_difference_history a, s_price_difference_history b)
            {
                return a.first == b.first
                    && a.second == b.second
                    && a.third == b.third
                    && a.fourth == b.fourth;
            }

            public int GetHashCode(s_price_difference_history c)
            {
                return HashCode.Combine(c.first, c.second, c.third, c.fourth);
            }
        }

        [DebuggerDisplay("{value}", Type = "c_secret_number")]
        internal class c_secret_number(Int64 i)
        {
            public readonly Int64 initial_value = i;
            public Int64 value { get; private set; } = i;

            private readonly Queue<Int64> price_difference_history = new();
            private Int64 previous_price { get; set; } = int.MinValue;

            private void mix(Int64 other)
            {
                value ^= other;
            }

            private void prune()
            {
                value %= 16777216;
            }

            public void next()
            {
                previous_price = current_price();

                mix(value * 64);
                prune();
                mix(value / 32);
                prune();
                mix(value * 2048);
                prune();

                Int64 price_difference = current_price() - previous_price;

                price_difference_history.Enqueue(price_difference);
                if (price_difference_history.Count > 4)
                {
                    price_difference_history.Dequeue();
                }
            }

            public Int64 current_price()
            {
                return value % 10;
            }

            public s_price_difference_history get_price_difference_history()
            {
                Int64[] history_array = [.. price_difference_history];

                s_price_difference_history history = new()
                {
                    first = history_array[0],
                    second = history_array[1],
                    third = history_array[2],
                    fourth = history_array[3],
                };

                return history;
            }
        }

        internal static c_secret_number[] parse_input(
            in c_input_reader input_reader)
        {
            List<c_secret_number> secret_numbers = [];

            while (input_reader.has_more_lines())
            {
                secret_numbers.Add(new c_secret_number(Int64.Parse(input_reader.read_line())));
            }

            return [.. secret_numbers];
        }

        public static void part_1(
            c_input_reader input_reader,
            bool pretty)
        {
            c_secret_number[] secret_numbers = parse_input(input_reader);

            const int k_iteration_count = 2000;

            // Calculate the final value for each secret number.
            foreach (c_secret_number secret_number in secret_numbers)
            {
                for (int i = 0; i < k_iteration_count; i++)
                {
                    secret_number.next();
                }

                if (pretty)
                {
                    Console.WriteLine($"{secret_number.initial_value}: {secret_number.value}");
                }
            }

            // Sum each final value.
            Int64 result = secret_numbers.Sum(s => s.value);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }

        public static void part_2(
            c_input_reader input_reader,
            bool pretty)
        {
            c_secret_number[] secret_numbers = parse_input(input_reader);

            const int k_iteration_count = 2000;

            // Keep track of the total price of each price history.
            Dictionary<s_price_difference_history, Int64> secret_number_total_prices = new (new c_price_difference_history_comparer());

            foreach (c_secret_number secret_number in secret_numbers)
            {
                // Keep track of the first instance of each price history for this secret number.
                Dictionary<s_price_difference_history, Int64> secret_number_prices = new (new c_price_difference_history_comparer());

                // Generate all of the values for this secret number.
                for (int i = 1; i <= k_iteration_count; i++)
                {
                    secret_number.next();

                    // If there have been enough values to generate a price history,
                    // and this is the first time that price history has shown up for this secret number,
                    // Then remember that price for the current history.
                    if (i >= 4)
                    {
                        s_price_difference_history price_difference_history = secret_number.get_price_difference_history();

                        if (!secret_number_prices.ContainsKey(price_difference_history))
                        {
                            secret_number_prices[price_difference_history] = secret_number.current_price();
                        }
                    }
                }

                // For each price history for this secret number,
                // add that price to the total price for this price history across all secret numbers.
                foreach (s_price_difference_history history in secret_number_prices.Keys)
                {
                    if (!secret_number_total_prices.ContainsKey(history))
                    {
                        secret_number_total_prices[history] = secret_number_prices[history];
                    }
                    else
                    {
                        secret_number_total_prices[history] += secret_number_prices[history];
                    }
                }

                if (pretty)
                {
                    Console.WriteLine($"{secret_number.initial_value}: {secret_number.value}");
                }
            }

            // Return the highest price among all the found price histories.
            Int64 result = secret_number_total_prices.Values.Max();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }
    }
}
