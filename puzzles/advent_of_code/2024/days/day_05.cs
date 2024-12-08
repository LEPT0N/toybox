using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.extensions;
using advent_of_code_common.input_reader;

namespace advent_of_code_2024.days
{
    internal class day_05
    {
        [DebuggerDisplay("[{index}] = {value}", Type = "s_page")]
        internal struct s_page
        {
            public readonly int value;
            public int index;
            public bool invalid;

            public s_page(int v, int i)
            {
                value = v;
                index = i;
                invalid = false;
            }
        }

        [DebuggerDisplay("{first}|{last}", Type = "s_rule")]
        internal struct s_rule
        {
            public readonly int first;
            public readonly int last;

            public s_rule(int[] components)
            {
                first = components[0];
                last = components[1];
            }

            public bool verify(s_page[] page_list)
            {
                // Find all the indicies where 'first' shows up in 'page_list'
                int first_value = first;
                int[] first_indices = page_list
                    .Where(page => page.value == first_value)
                    .Select(page => page.index)
                    .ToArray();


                // Find all the indicies where 'last' shows up in 'page_list'
                int last_value = last;
                int[] last_indices = page_list
                    .Where(page => page.value == last_value)
                    .Select(page => page.index)
                    .ToArray();

                // If we found both, then the rule is satisfied if all 'first's cam before all 'lasts's.
                return first_indices.Length == 0
                    || last_indices.Length == 0
                    || first_indices.Max() < last_indices.Min();
            }

            public void swap(s_page[] page_list)
            {
                // If page_list is already verified then there's nothing to swap and you shouldn't be calling this.
                if (verify(page_list))
                {
                    throw new Exception("tried to swap a verified page list!");
                }

                // Swap the offending-est pair we find. Naming is silly by design.

                // Find the last index where 'first' shows up in 'page_list'.
                int first_value = first;
                int last_first_index = page_list
                    .Where(page => page.value == first_value)
                    .Select(page => page.index)
                    .Last();

                // Find the first index where 'last' shows up in 'page_list'.
                int last_value = last;
                int first_last_index = page_list
                    .Where(page => page.value == last_value)
                    .Select(page => page.index)
                    .First();

                // Swap them.
                page_list.swap_elements(last_first_index, first_last_index);
                page_list[last_first_index].index = last_first_index;
                page_list[first_last_index].index = first_last_index;
            }
        }

        internal static (s_rule[], s_page[][]) parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            // Read the rules

            List<s_rule> rules = new List<s_rule>();

            while (!string.IsNullOrEmpty(input_reader.peek_line()))
            {
                rules.Add(new s_rule(input_reader.read_line()
                    .Split('|')
                    .Select(s => int.Parse(s))
                    .ToArray()));
            }

            // Discard the empty line.
            input_reader.read_line();

            // Read the page lists

            List<s_page[]> page_lists = new List<s_page[]>();

            while (input_reader.has_more_lines())
            {
                page_lists.Add(input_reader.read_line()
                    .Split(',')
                    .Select(s => int.Parse(s))
                    .ToArray()
                    .Select((v, i) => new s_page(v, i))
                    .ToArray());
            }

            return (rules.ToArray(), page_lists.ToArray());
        }

        internal static void mark_invalid_pages(s_page[] page_list, s_rule[] failing_rules)
        {
            // Mark all pages that failed a rule.

            foreach (s_rule failing_rule in failing_rules)
            {
                for (int index = 0; index < page_list.Length; index++)
                {
                    if (page_list[index].value == failing_rule.first || page_list[index].value == failing_rule.last)
                    {
                        page_list[index].invalid = true;
                    }
                }
            }
        }

        internal static void display_invalid_page_list(s_page[] page_list)
        {
            // Invalid page lists are gray with the pages that failed a test marked as red.

            for (int index = 0; index < page_list.Length; index++)
            {
                Console.ForegroundColor = (page_list[index].invalid ? ConsoleColor.DarkRed : ConsoleColor.DarkGray);
                Console.Write($"{page_list[index].value} ");
            }
        }

        internal static void display_valid_page_list(s_page[] page_list)
        {
            // Valid page lists are bright white and the middle-page-number is green.

            int middle_index = page_list.Length / 2;

            for (int index = 0; index < page_list.Length; index++)
            {
                Console.ForegroundColor = (index == middle_index ? ConsoleColor.Green : ConsoleColor.White);
                Console.Write($"{page_list[index].value} ");
            }
        }

        internal static void display_ignored_page_list(s_page[] page_list)
        {
            // Ignored lists are all gray.

            Console.ForegroundColor = ConsoleColor.DarkGray;

            for (int index = 0; index < page_list.Length; index++)
            {
                Console.Write($"{page_list[index].value} ");
            }
        }

        public static void part_1(
            string input,
            bool pretty)
        {
            (s_rule[] rules, s_page[][] page_lists) = parse_input(input, pretty);

            // filter to only page lists that satisfy all rules,
            // Then find the middle value in each one,
            // Then sum all of those values.

            int result = page_lists
                .Where(page_list => rules.All(rule => rule.verify(page_list)))
                .Select(page_list => page_list[page_list.Length / 2].value)
                .Sum();

            if (pretty)
            {
                // Print each valid and invalid list.

                foreach (s_page[] page_list in page_lists)
                {
                    s_rule[] failing_rules = rules.Where(rule => !rule.verify(page_list)).ToArray();

                    if (failing_rules.Any())
                    {
                        // If any rules failed, mark the failed pages and display the invalid list.

                        mark_invalid_pages(page_list, failing_rules);

                        display_invalid_page_list(page_list);
                    }
                    else
                    {
                        // If no rules failed, display the valid list.

                        display_valid_page_list(page_list);
                    }

                    Console.WriteLine();
                }

                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }

        internal static void fix_page_lists(s_rule[] rules, s_page[][] invalid_page_lists)
        {
            foreach (s_page[] page_list in invalid_page_lists)
            {
                // As long as we find a failing rule for this page list, correct it.
                // Keep going until this page list doesn't break any rules.
                // I think the nature of the dataset means that we won't end up in an infinite loop?

                while (true)
                {
                    s_rule[] failing_rules = rules.Where(rule => !rule.verify(page_list)).ToArray();

                    if (failing_rules.Any())
                    {
                        failing_rules.First().swap(page_list);
                    }
                    else
                    {
                        break;
                    }

                }
            }
        }

        internal static void copy_page_lists(s_page[][] source, ref s_page[][] destination)
        {
            destination = new s_page[source.Length][];

            for (int index = 0; index < source.Length; index++)
            {
                destination[index] = new s_page[source[index].Length];

                source[index].CopyTo(destination[index], 0);
            }
        }

        // other option: is there one true ordering?

        public static void part_2(
            string input,
            bool pretty)
        {
            (s_rule[] rules, s_page[][] page_lists) = parse_input(input, pretty);

            s_page[][] pretty_page_lists = null;
            s_page[][] pretty_fixed_page_lists = null;

            if (pretty)
            {
                // If displaying output, make one copy of the original, and another to fix up.

                copy_page_lists(page_lists, ref pretty_page_lists);
                copy_page_lists(page_lists, ref pretty_fixed_page_lists);
            }

            // filter to only page lists that do not satisfy some number of rules.

            s_page[][] invalid_page_lists = page_lists
                .Where(page_list => rules.Any(rule => !rule.verify(page_list)))
                .ToArray();

            // Fix each of those invalid page lists.

            fix_page_lists(rules, invalid_page_lists);

            // Find the middle value in each (once invalid) valid page list,
            // Then sum all of those values.

            int result = invalid_page_lists
                .Select(page_list => page_list[page_list.Length / 2].value)
                .Sum();

            if (pretty)
            {
                // Wipe (but don't delete) all valid page lists.

                pretty_fixed_page_lists = pretty_fixed_page_lists
                    .Select(page_list => rules.Any(rule => !rule.verify(page_list)) ? page_list : new s_page[0])
                    .ToArray();

                // Fix all the invalid page lists.

                fix_page_lists(rules, pretty_fixed_page_lists);

                // Print each page list.

                for (int index = 0; index < pretty_page_lists.Length; index++)
                {
                    if (pretty_fixed_page_lists[index].Length == 0)
                    {
                        // Print each valid list as ignored.

                        display_ignored_page_list(pretty_page_lists[index]);
                    }
                    else
                    {
                        // Print each invalid list first as invalid then as fixed.

                        s_rule[] failing_rules = rules.Where(rule => !rule.verify(pretty_page_lists[index])).ToArray();
                        mark_invalid_pages(pretty_page_lists[index], failing_rules);

                        display_invalid_page_list(pretty_page_lists[index]);

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(" ->  ");

                        display_valid_page_list(pretty_fixed_page_lists[index]);
                    }

                    Console.WriteLine();
                }

                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }
    }
}
