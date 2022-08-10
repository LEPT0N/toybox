using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.input_reader;

namespace advent_of_code_2019.days
{
    internal class day_01
    {
        internal static int[] parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);
            List<int> result = new List<int>();

            while (input_reader.has_more_lines())
            {
                result.Add(int.Parse(input_reader.read_line()));
            }

            return result.ToArray();
        }

        internal static int calculate_fuel(int mass)
        {
            return (mass / 3) - 2;
        }

        public static void part_1(
            string input,
            bool pretty)
        {
            int[] mass_list = parse_input(input, pretty);

            int[] fuel_list = mass_list.Select(mass => calculate_fuel(mass)).ToArray();

            int total_fuel = fuel_list.Sum();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", total_fuel);
            Console.ResetColor();
        }

        internal static int calculate_recursive_fuel(int mass)
        {
            int fuel = Math.Max(0, calculate_fuel(mass));

            if (fuel > 0)
            {
                fuel += calculate_recursive_fuel(fuel);
            }

            return fuel;
        }

        public static void part_2(
            string input,
            bool pretty)
        {
            int[] mass_list = parse_input(input, pretty);

            int[] fuel_list = mass_list.Select(mass => calculate_recursive_fuel(mass)).ToArray();

            int total_fuel = fuel_list.Sum();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", total_fuel);
            Console.ResetColor();
        }
    }
}
