using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace advent_of_code_2020.Days
{
    internal class Day_7
    {
        static Regex k_whole_line_pattern = new Regex(@"^([a-z\s]+) bags contain (.+)\.$");

        static Regex k_single_containee_pattern = new Regex(@"^(\d+) (.+) bags?$");

        static void parse_input(
            string input,
            out Dictionary<string, HashSet<Tuple<int, string>>> contains,
            out Dictionary<string, HashSet<string>> contained_by)
        {
            c_input_reader input_reader = new c_input_reader(input);

            // Given a bag as input, find out which bags it can contain.
            contains = new Dictionary<string, HashSet<Tuple<int, string>>>();

            // Given a bag as input, find out which bags can contain it.
            contained_by = new Dictionary<string, HashSet<string>>();

            while (input_reader.has_more_lines())
            {
                Match whole_line_match = k_whole_line_pattern.Match(input_reader.read_line());

                string container = whole_line_match.Groups[1].Value;

                string[] containee_inputs = whole_line_match.Groups[2].Value.Split(", ");

                contains.TryAdd(container, new HashSet<Tuple<int, string>>());

                foreach (string containee_input in containee_inputs)
                {
                    if (containee_input != "no other bags")
                    {
                        Match containee_match = k_single_containee_pattern.Match(containee_input);

                        int containee_count = int.Parse(containee_match.Groups[1].Value);

                        string containee_name = containee_match.Groups[2].Value;

                        Tuple<int, string> containee = new Tuple<int, string>(containee_count, containee_name);

                        contains[container].Add(containee);

                        contained_by.TryAdd(containee_name, new HashSet<string>());
                        contained_by[containee_name].Add(container);
                    }
                }
            }

            if (contains.Count < 100)
            {
                foreach (string container in contains.Keys)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("A [{0}] bag can contain [", container);

                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(string.Join(", ", contains[container]
                        .Select(x => x.Item1.ToString() + " " + x.Item2)
                        .ToList()));

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("]");

                    Console.ResetColor();
                }

                Console.WriteLine();

                foreach (string containee in contained_by.Keys)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("A [{0}] bag can be contained by [", containee);

                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(string.Join(", ", contained_by[containee].ToList()));

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("]");

                    Console.ResetColor();
                }
            }
        }

        public static void Part_1(string input)
        {
            Dictionary<string, HashSet<Tuple<int, string>>> contains;
            Dictionary<string, HashSet<string>> contained_by;

            parse_input(input, out contains, out contained_by);

            HashSet<string> outermost_bags = new HashSet<string>();

            HashSet<string> bags_to_consider = new HashSet<string>();
            bags_to_consider.Add("shiny gold");

            while (bags_to_consider.Count != 0)
            {
                HashSet<string> new_bags_to_consider = new HashSet<string>();

                foreach (string bag_to_consider in bags_to_consider)
                {
                    outermost_bags.Add(bag_to_consider);

                    if (contained_by.ContainsKey(bag_to_consider))
                    {
                        foreach (string new_bag_to_consider in contained_by[bag_to_consider])
                        {
                            if (!outermost_bags.Contains(new_bag_to_consider))
                            {
                                new_bags_to_consider.Add(new_bag_to_consider);
                            }
                        }
                    }
                }

                bags_to_consider = new_bags_to_consider;
            }

            outermost_bags.Remove("shiny gold");

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Possible outermost bags: ");

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(string.Join(", ", outermost_bags.ToList()));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Total possible outermost bags = {0}", outermost_bags.Count());

            Console.ResetColor();
        }

        public static void Part_2(string input)
        {
            Dictionary<string, HashSet<Tuple<int, string>>> contains;
            Dictionary<string, HashSet<string>> contained_by;

            parse_input(input, out contains, out contained_by);

            Dictionary<string, int> bags_within = new Dictionary<string, int>();

            Dictionary<string, int> bags_to_consider = new Dictionary<string, int>();
            bags_to_consider.Add("shiny gold", 1);

            while (bags_to_consider.Count != 0)
            {
                Dictionary<string, int> new_bags_to_consider = new Dictionary<string, int>();

                foreach (string bag_to_consider in bags_to_consider.Keys)
                {
                    int bag_to_consider_amount = bags_to_consider[bag_to_consider];

                    foreach(Tuple<int, string> contained in contains[bag_to_consider])
                    {
                        bags_within.TryAdd(contained.Item2, 0);
                        new_bags_to_consider.TryAdd(contained.Item2, 0);

                        bags_within[contained.Item2] = bags_within[contained.Item2] + contained.Item1 * bag_to_consider_amount;
                        new_bags_to_consider[contained.Item2] = new_bags_to_consider[contained.Item2] + contained.Item1 * bag_to_consider_amount;
                    }
                }

                bags_to_consider = new_bags_to_consider;
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("All contained bags: ");

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(string.Join(", ", bags_within
                .Select(x => x.Value.ToString() + " " + x.Key)
                .ToList()));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Total contained bags = {0}", bags_within.Sum(x => x.Value));

            Console.ResetColor();
        }
    }
}
