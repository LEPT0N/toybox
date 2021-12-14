using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace advent_of_code_2021.Days
{
    internal class Day_14
    {
        internal static void parse_input_1(
            string input,
            out LinkedList<char> polymer,
            out Dictionary<(char, char), char> insertion_rules)
        {
            c_input_reader input_reader = new c_input_reader(input);

            polymer = new LinkedList<char>();

            foreach (char polymer_input in input_reader.read_line())
            {
                polymer.AddLast(polymer_input);
            }

            input_reader.read_line();

            insertion_rules = new Dictionary<(char, char), char>();

            while (input_reader.has_more_lines())
            {
                string input_line = input_reader.read_line();

                string[] parsed_input_line = input_line.Split(" -> ");

                char pair_first = parsed_input_line[0][0];
                char pair_second = parsed_input_line[0][1];
                char insertion = parsed_input_line[1][0];

                insertion_rules[(pair_first, pair_second)] = insertion;
            }
        }

        internal static void step_polymer(
            LinkedList<char> polymer,
            Dictionary<(char, char), char> insertion_rules)
        {
            LinkedListNode<char> first = polymer.First;
            LinkedListNode<char> second = first.Next;

            while (second != null)
            {
                char new_char = insertion_rules[(first.Value, second.Value)];

                polymer.AddAfter(first, new_char);

                first = second;
                second = second.Next;
            }
        }

        internal static void print_polymer(
            LinkedList<char> polymer,
            bool generated)
        {
            bool highlighted = false;

            foreach (char value in polymer)
            {
                if (highlighted)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                }

                Console.Write(value);

                if (generated)
                {
                    highlighted = !highlighted;
                }
            }

            Console.WriteLine();
            Console.ResetColor();
        }

        public static void Part_1(string input, bool pretty)
        {
            // Parse input

            LinkedList<char> polymer;
            Dictionary<(char, char), char> insertion_rules;
            parse_input_1(input, out polymer, out insertion_rules);

            print_polymer(polymer, false);

            // Run the insertion steps

            for (int step = 0; step < 10; step++)
            {
                step_polymer(polymer, insertion_rules);
                print_polymer(polymer, true);
            }

            // Compute results

            Dictionary<char, int> result_counts = new Dictionary<char, int>();

            foreach (char value in polymer)
            {
                if (!result_counts.ContainsKey(value))
                {
                    result_counts.Add(value, 1);
                }
                else
                {
                    result_counts[value]++;
                }
            }

            int max_count = result_counts.Aggregate((max, current) => max.Value > current.Value ? max : current).Value;
            int min_count = result_counts.Aggregate((min, current) => min.Value < current.Value ? min : current).Value;

            int result = max_count - min_count;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", result);
            Console.ResetColor();
        }

        internal static void parse_input_2(
            string input,
            out string polymer_initial,
            out Dictionary<(char, char), char> insertion_rules)
        {
            c_input_reader input_reader = new c_input_reader(input);

            polymer_initial = input_reader.read_line();
            input_reader.read_line();

            insertion_rules = new Dictionary<(char, char), char>();

            while (input_reader.has_more_lines())
            {
                string input_line = input_reader.read_line();

                string[] parsed_input_line = input_line.Split(" -> ");

                char pair_first = parsed_input_line[0][0];
                char pair_second = parsed_input_line[0][1];
                char insertion = parsed_input_line[1][0];

                insertion_rules[(pair_first, pair_second)] = insertion;
            }
        }

        public static void Part_2(string input, bool pretty)
        {
            // Read the input

            string polymer_initial;
            Dictionary<(char, char), char> insertion_rules;
            parse_input_2(input, out polymer_initial, out insertion_rules);
            
            // Construct polymer_current, which lists the number of times each (char, char) pair exists in the polymer.

            Dictionary<(char, char), UInt64> polymer_current = new Dictionary<(char, char), UInt64>();
            Dictionary<(char, char), UInt64> polymer_next = new Dictionary<(char, char), UInt64>();

            foreach((char, char) insertion_rule_key in insertion_rules.Keys)
            {
                polymer_current.Add(insertion_rule_key, 0UL);
                polymer_next.Add(insertion_rule_key, 0UL);
            }

            for (int i = 0; i < polymer_initial.Length - 1; i++)
            {
                polymer_current[(polymer_initial[i], polymer_initial[i + 1])]++;
            }

            // Step through each insertion loop

            for (int step = 0; step < 40; step++)
            {
                foreach ((char, char) polymer_pair in polymer_current.Keys)
                {
                    UInt64 polymer_pair_count = polymer_current[polymer_pair];
                    if (polymer_pair_count > 0UL)
                    {
                        char intermediate = insertion_rules[polymer_pair];

                        polymer_next[(polymer_pair.Item1, intermediate)]+= polymer_pair_count;
                        polymer_next[(intermediate, polymer_pair.Item2)]+= polymer_pair_count;
                    }
                }

                // Swap polymer_current with polymer_next, and clear polymer_next

                Dictionary<(char, char), UInt64> polymer_temp = polymer_current;
                polymer_current = polymer_next;
                polymer_next = polymer_temp;

                foreach ((char, char) polymer_pair in polymer_next.Keys)
                {
                    polymer_next[polymer_pair] = 0UL;
                }
            }

            // Now count up how many times each character shows up in the final polymer_current.

            Dictionary<char, UInt64> result_counts = new Dictionary<char, UInt64>();

            foreach ((char, char) polymer_pair in polymer_current.Keys)
            {
                if (!result_counts.ContainsKey(polymer_pair.Item1))
                {
                    result_counts.Add(polymer_pair.Item1, 0);
                }
                result_counts[polymer_pair.Item1] += polymer_current[polymer_pair];

                if (!result_counts.ContainsKey(polymer_pair.Item2))
                {
                    result_counts.Add(polymer_pair.Item2, 0);
                }
                result_counts[polymer_pair.Item2] += polymer_current[polymer_pair];
            }

            // Note: all above values were doubled, so cut them in half.
            // Note: However, the first and last chars in the polymer are NOT doubled, so add 1 for each of them first.
            // Note: We know the first and last chars because they're unchanged from the initial input.

            result_counts[polymer_initial[0]]++;
            result_counts[polymer_initial[polymer_initial.Length - 1]]++;

            UInt64 max_count = result_counts.Aggregate((max, current) => max.Value > current.Value ? max : current).Value / 2;
            UInt64 min_count = result_counts.Aggregate((min, current) => min.Value < current.Value ? min : current).Value / 2;

            UInt64 result = max_count - min_count;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", result);
            Console.ResetColor();
        }
    }
}
