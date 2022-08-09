using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.input_reader;

namespace advent_of_code_2020.Days
{
    internal class Day_16
    {
        [DebuggerDisplay("{DebuggerDisplay,nq}", Type = "c_number_rule")]
        internal class c_number_range
        {
            public int min;
            public int max;

            public string DebuggerDisplay
            {
                get { return String.Format("[{0}, {1}]", min, max); }
            }

            public bool validate(int value)
            {
                return min <= value && value <= max;
            }
        }

        [DebuggerDisplay("{DebuggerDisplay,nq}", Type = "c_number_rule")]
        internal class c_number_rule
        {
            public string name;
            public c_number_range[] ranges;

            private string DebuggerDisplay
            {
                get { return String.Format("{0} = {1}", name, string.Join(" or ", ranges.Select(x => x.DebuggerDisplay))); }
            }

            public bool validate(int value)
            {
                return ranges.Any(x => x.validate(value));
            }
        }


        [DebuggerDisplay("{DebuggerDisplay,nq}", Type = "c_ticket")]
        internal class c_ticket
        {
            public int[] values;

            private string DebuggerDisplay
            {
                get { return String.Format("{0}", string.Join(",", values)); }
            }
        }

        internal static (c_number_rule[], c_ticket, c_ticket[]) parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            List<c_number_rule> number_rules = new List<c_number_rule>();

            // Parse the number rules
            while (input_reader.has_more_lines())
            {
                string input_line = input_reader.read_line();

                if (string.IsNullOrEmpty(input_line))
                {
                    break;
                }

                string[] parsed_input_line = input_line.Split(": ");

                c_number_rule number_rule = new c_number_rule();

                number_rule.name = parsed_input_line[0];

                List<c_number_range> ranges_list = new List<c_number_range>();

                string[] ranges_input = parsed_input_line[1].Split(" or ");
                foreach (string range_input in ranges_input)
                {
                    int[] range = range_input.Split("-").Select(x => int.Parse(x)).ToArray();

                    ranges_list.Add(new c_number_range { min = range[0], max = range[1] });
                }

                number_rule.ranges = ranges_list.ToArray();

                number_rules.Add(number_rule);
            }

            c_ticket my_ticket = new c_ticket();

            // Read my ticket
            {
                input_reader.read_line();
                string input_line = input_reader.read_line();

                my_ticket.values = input_line.Split(",").Select(x => int.Parse(x)).ToArray();
            }

            List<c_ticket> other_tickets = new List<c_ticket>();

            // Read other tickets
            {
                input_reader.read_line();
                input_reader.read_line();

                while (input_reader.has_more_lines())
                {
                    c_ticket other_ticket = new c_ticket();

                    other_ticket.values = input_reader.read_line().Split(",").Select(x => int.Parse(x)).ToArray();

                    other_tickets.Add(other_ticket);
                }
            }

            return (number_rules.ToArray(), my_ticket, other_tickets.ToArray());
        }

        public static void Part_1(
            string input,
            bool pretty)
        {
            (c_number_rule[] rules, c_ticket my_ticket, c_ticket[] other_tickets) = parse_input(input, pretty);

            List<int> invalid_values = new List<int>();

            foreach (c_ticket ticket in other_tickets)
            {
                foreach (int value in ticket.values)
                {
                    if (!rules.Any(rule => rule.validate(value)))
                    {
                        invalid_values.Add(value);
                        break;
                    }
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", invalid_values.Aggregate((x, y) => x + y));
            Console.ResetColor();
        }

        public static void Part_2(
            string input,
            bool pretty)
        {
            (c_number_rule[] rules, c_ticket my_ticket, c_ticket[] other_tickets) = parse_input(input, pretty);

            // Discard invalid tickets

            List<c_ticket> valid_tickets = new List<c_ticket>();
            valid_tickets.Add(my_ticket);

            foreach (c_ticket ticket in other_tickets)
            {
                if (ticket.values.All(value => rules.Any(rule => rule.validate(value))))
                {
                    valid_tickets.Add(ticket);
                }
            }

            // Build a list of all rules that match each set of values

            int value_count = my_ticket.values.Length;

            List<c_number_rule>[] matching_rules = new List<c_number_rule>[value_count];
            for (int value_index = 0; value_index < value_count; value_index++)
            {
                matching_rules[value_index] = rules.Where(
                    rule => valid_tickets.All(
                    valid_ticket => rule.validate(valid_ticket.values[value_index]))).ToList();
            }

            // Some values will still have multiple valid rules.
            // If any values only have one valid rule, then remove that rule as a possibility from all other values.
            // Hopefully this is enough and we don't have to get into Sudoku-level shenanigans.

            // Loop as long as any value has more than one matching rule.
            while (matching_rules.Any(x => x.Count > 1))
            {
                // Look at each value that has only one matching rule
                for (int value_index = 0; value_index < value_count; value_index++)
                {
                    if (matching_rules[value_index].Count == 1)
                    {
                        c_number_rule taken_rule = matching_rules[value_index].First();

                        // Loop through all other values and remove this rule from their list
                        for (int other_value_index = 0; other_value_index < value_count; other_value_index++)
                        {
                            if (value_index != other_value_index && matching_rules[other_value_index].Count > 1)
                            {
                                matching_rules[other_value_index] = matching_rules[other_value_index].Where(other_rule => other_rule != taken_rule).ToList();
                            }
                        }
                    }
                }
            }

            // Multiply all 'departure' values for our ticket.

            UInt64 result = 1UL;

            for (int value_index = 0; value_index < value_count; value_index++)
            {
                if (matching_rules[value_index].First().name.StartsWith("departure "))
                {
                    result *= (UInt64)my_ticket.values[value_index];
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", result);
            Console.ResetColor();
        }
    }
}
