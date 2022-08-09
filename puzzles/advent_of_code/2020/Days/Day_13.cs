using System;
using System.Collections.Generic;
using advent_of_code_common.input_reader;

namespace advent_of_code_2020.Days
{
    internal class Day_13
    {
        const UInt64 k_invalid_bus_id = 1UL;

        internal static (UInt64, UInt64[]) parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            UInt64 start_time = UInt64.Parse(input_reader.read_line());

            List<UInt64> bus_ids = new List<UInt64>();

            string[] bus_ids_input = input_reader.read_line().Split(',');

            foreach (string bus_id_input in bus_ids_input)
            {
                UInt64 bus_id;
                if (UInt64.TryParse(bus_id_input, out bus_id))
                {
                    bus_ids.Add(bus_id);
                }
                else
                {
                    bus_ids.Add(k_invalid_bus_id);
                }
            }

            return (start_time, bus_ids.ToArray());
        }

        public static void Part_1(
            string input,
            bool pretty)
        {
            (UInt64 start_time, UInt64[] bus_ids) = parse_input(input, pretty);

            UInt64 best_bus_arrival_time = UInt64.MaxValue;
            UInt64 best_bus_id = 0;

            foreach (UInt64 bus_id in bus_ids)
            {
                if (bus_id != k_invalid_bus_id)
                {
                    UInt64 time_since_last_arrival = start_time % bus_id;
                    UInt64 bus_arrival_time = bus_id - time_since_last_arrival;

                    if (bus_arrival_time < best_bus_arrival_time)
                    {
                        best_bus_arrival_time = bus_arrival_time;
                        best_bus_id = bus_id;
                    }
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine(
                "Result = {0} * {1} = {2}",
                best_bus_id,
                best_bus_arrival_time,
                best_bus_id * best_bus_arrival_time);
            Console.ResetColor();
        }

        public static void Part_2(
            string input,
            bool pretty)
        {
            (UInt64 start_time, UInt64[] bus_ids) = parse_input(input, pretty);

            // Our answer (the 'least common multiple' of all inputs)
            UInt64 result = 0;

            // Our iterator on the result. Start iterating by 1.
            UInt64 total_product = 1;

            // Loop through each inputted bus id, skipping invalid entries.
            for (int i = 0; i < bus_ids.Length; i++)
            {
                if (bus_ids[i] != k_invalid_bus_id)
                {
                    // increase the result by 'total_product' until we find a number (plus 'i') where bus_id 'i' arrives.
                    while ((result + (UInt64)i) % bus_ids[i] != 0)
                    {
                        result += total_product;
                    }

                    // Increase total_product by the current bus id. Since we know the current result is a time where the
                    // current bus arrives 'i' timesteps later, then as long as result is incremented by some multiple of
                    // this bus id, then that bus will continue arriving at that same offset on any future result.
                    total_product *= bus_ids[i];
                }
            }

            Console.WriteLine();
            for (int i = 0; i < bus_ids.Length; i++)
            {
                if (bus_ids[i] != k_invalid_bus_id)
                {
                    Console.WriteLine("({0} + {1}) % {2} = {3}", result, i, bus_ids[i], ((result + (UInt64)i) % bus_ids[i]));
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("x = {0}", result);
            Console.ResetColor();
        }
    }
}
