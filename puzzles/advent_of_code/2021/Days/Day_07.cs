using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace advent_of_code_2021.Days
{
    internal class Day_07
    {
        private static void print_costs(ref int[] costs, int target)
        {
            if (costs.Length > 100)
            {
                return;
            }

            Console.WriteLine();

            for (int i = 0; i < costs.Length; i++)
            {
                Console.ForegroundColor = (i == target) ? ConsoleColor.Blue : ConsoleColor.Gray;

                Console.Write(" {0}", costs[i]);
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" [ Total = {0} ]", costs.Sum());

            Console.ResetColor();
        }

        private static int calculate_cost(ref int[] positions, int target)
        {
            int[] incremental_costs = new int[positions.Length];

            {
                int lower_moved = 0;

                for (int i = 0; i < target; i++)
                {
                    lower_moved += positions[i];

                    incremental_costs[i] = lower_moved;
                }
            }

            {
                int upper_moved = 0;

                for (int i = positions.Length - 1; i > target; i--)
                {
                    upper_moved += positions[i];

                    incremental_costs[i] = upper_moved;
                }
            }

            print_costs(ref incremental_costs, target);

            return incremental_costs.Sum();
        }

        private static int sum_one_to_n(int n)
        {
            return n * (n + 1) / 2;
        }

        private static int calculate_better_cost(ref int[] positions, int target)
        {
            int[] incremental_costs = new int[positions.Length];

            for (int i = 0; i < positions.Length; i++)
            {
                int distance = Math.Abs(target - i);
                int cost_per_crab = sum_one_to_n(distance);
                int incremental_cost = cost_per_crab * positions[i];

                incremental_costs[i] = incremental_cost;
            }

            print_costs(ref incremental_costs, target);

            return incremental_costs.Sum();
        }

        public static void Day_7_Worker(string input, bool use_better_cost)
        {
            c_input_reader input_reader = new c_input_reader(input);

            List<int> starting_positions = input_reader.read_line().Split(',').Select(x => int.Parse(x)).ToList();

            int max_position = starting_positions.Max();

            int[] positions = new int[max_position + 1];

            foreach (int starting_position in starting_positions)
            {
                positions[starting_position]++;
            }

            Console.WriteLine("Starting Positions = {0}", string.Join(", ", positions));

            int[] costs = new int[max_position + 1];
            for (int i = 0; i < positions.Length; i++)
            {
                if (use_better_cost)
                {
                    costs[i] = calculate_better_cost(ref positions, i);
                }
                else
                {
                    costs[i] = calculate_cost(ref positions, i);
                }
            }

            Console.WriteLine();
            Console.WriteLine("Costs = {0}", string.Join(", ", costs));

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Lowest Cost = {0}", costs.Min());

            Console.ResetColor();
        }

        public static void Part_1(string input)
        {
            Day_7_Worker(input, false);
        }

        public static void Part_2(string input)
        {
            Day_7_Worker(input, true);
        }
    }
}
