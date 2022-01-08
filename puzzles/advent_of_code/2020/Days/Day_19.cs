using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace advent_of_code_2020.Days
{
    internal class Day_19
    {
        internal abstract class c_rule
        {
            public readonly int id;

            protected c_rule(int input_id)
            {
                id = input_id;
            }

            public abstract (bool, int) try_match(string input, int index = 0);

            public abstract void fill_null_subrules(c_rule filler = null);
        }

        [DebuggerDisplay("{DebuggerDisplay,nq}", Type = "c_basic_rule")]
        internal class c_basic_rule : c_rule
        {
            private char value;

            public c_basic_rule(int input_id, char input) : base(input_id)
            {
                value = input;
            }

            public override (bool, int) try_match(string input, int index = 0)
            {
                if (index < input.Length && input[index] == value)
                {
                    return (true, index + 1);
                }
                else
                {
                    return (false, 0);
                }
            }

            public override void fill_null_subrules(c_rule filler = null)
            {
                return;
            }

            public override string ToString()
            {
                return String.Format("\"{0}\"", value);
            }

            private string DebuggerDisplay
            {
                get { return string.Format("{0}: {1}", id, ToString()); }
            }
        }

        [DebuggerDisplay("{DebuggerDisplay,nq}", Type = "c_multi_rule")]
        internal class c_multi_rule : c_rule
        {
            private c_rule[] sub_rules;

            public c_multi_rule(int input_id, c_rule[] input) : base(input_id)
            {
                sub_rules = input;
            }

            public override (bool, int) try_match(string input, int index = 0)
            {
                foreach (c_rule sub_rule in sub_rules)
                {
                    (bool sub_rule_matched, int new_index) = sub_rule.try_match(input, index);

                    if (sub_rule_matched)
                    {
                        index = new_index;
                    }
                    else
                    {
                        return (false, 0);
                    }
                }

                return (true, index);
            }

            public override void fill_null_subrules(c_rule filler = null)
            {
                if (filler == null)
                {
                    filler = this;
                }

                for (int i = 0; i < sub_rules.Length; i++)
                {
                    if (sub_rules[i] == null)
                    {
                        sub_rules[i] = filler;
                    }
                }
            }

            public override string ToString()
            {
                return string.Join(" ", sub_rules.Select(sub_rule => sub_rule.id));
            }

            private string DebuggerDisplay
            {
                get { return string.Format("{0}: {1}", id, ToString()); }
            }
        }

        [DebuggerDisplay("{DebuggerDisplay,nq}", Type = "c_choice_rule")]
        internal class c_choice_rule : c_rule
        {
            private c_rule[] sub_rules;

            public c_choice_rule(int input_id, c_rule[] input) : base(input_id)
            {
                sub_rules = input;
            }

            public override (bool, int) try_match(string input, int index = 0)
            {
                // TODO I need to search all, not just the first one that mathces.

                // I think I should try returning (bool, int)[] for matches found.

                foreach (c_rule sub_rule in sub_rules)
                {
                    (bool sub_rule_matched, int new_index) = sub_rule.try_match(input, index);

                    if (sub_rule_matched)
                    {
                        return (true, new_index);
                    }
                }

                return (false, 0);
            }

            public override void fill_null_subrules(c_rule filler = null)
            {
                if (filler == null)
                {
                    filler = this;
                }

                for (int i = 0; i < sub_rules.Length; i++)
                {
                    if (sub_rules[i] == null)
                    {
                        sub_rules[i] = filler;
                    }
                    else
                    {
                        sub_rules[i].fill_null_subrules(filler);
                    }
                }
            }

            public override string ToString()
            {
                return string.Join(" | ", sub_rules.Select(sub_rule => sub_rule.ToString()));
            }

            private string DebuggerDisplay
            {
                get { return string.Format("{0}: {1}", id, ToString()); }
            }
        }

        [DebuggerDisplay("Count = {rules.Count}", Type = "c_rule_collection")]
        internal class c_rule_collection
        {
            private readonly Dictionary<int, c_rule> rules = new Dictionary<int, c_rule>();

            public c_rule_collection(c_input_reader input_reader)
            {
                Dictionary<int, (int[], string)> pending_rules = new Dictionary<int, (int[], string)>();

                // Loop through lines of input to parse the rules
                for (string input = input_reader.read_line();
                    !string.IsNullOrEmpty(input);
                    input = input_reader.read_line())
                {
                    string[] split_input = input.Split(": ");

                    int rule_id = int.Parse(split_input[0]);

                    string rule_definition_input = split_input[1];

                    if (rule_definition_input[0] == '"')
                    {
                        // If a basic rule is found, add it to the final list of rules
                        rules[rule_id] = new c_basic_rule(rule_id, rule_definition_input[1]);
                    }
                    else
                    {
                        // If a complex rule is found, add it's id, sub rule ids, and definition string to a temporary collection

                        List<int> sub_rule_ids = new List<int>();

                        foreach (string sub_rule_id_input in rule_definition_input.Split(" "))
                        {
                            int sub_rule_id;
                            if (int.TryParse(sub_rule_id_input, out sub_rule_id))
                            {
                                sub_rule_ids.Add(sub_rule_id);
                            }
                        }

                        pending_rules[rule_id] = (sub_rule_ids.ToArray(), rule_definition_input);
                    }
                }

                // Loop as long as there are still rules in the temporary collection
                while (pending_rules.Count > 0)
                {
                    // Check against each rule
                    foreach (int pending_rule_id in pending_rules.Keys)
                    {
                        // Look for a pending rule which has all subrules in the final rule list.
                        if (pending_rules[pending_rule_id].Item1.All(sub_rule_id => sub_rule_id == pending_rule_id || rules.ContainsKey(sub_rule_id)))
                        {
                            /*bool weird = false;

                            if (pending_rules[pending_rule_id].Item1.Any(sub_rule_id => sub_rule_id == pending_rule_id))
                            {
                                weird = true;
                            }

                            if (weird)
                            {
                                continue;
                            }*/

                            string[] multi_rule_definitions = pending_rules[pending_rule_id].Item2.Split(" | ");

                            c_multi_rule[] multi_rules = new c_multi_rule[multi_rule_definitions.Length];

                            // Build a list of multi rules from the pending rule's definition string.
                            for (int i = 0; i < multi_rule_definitions.Length; i++)
                            {
                                //c_rule[] sub_rules = multi_rule_definitions[i].Split(" ").Select(x => rules?[int.Parse(x)]).ToArray();
                                c_rule[] sub_rules = multi_rule_definitions[i].Split(" ").Select(x => int.Parse(x)).Select(
                                    x => rules.ContainsKey(x) ? rules[x] : null).ToArray();

                                multi_rules[i] = new c_multi_rule(pending_rule_id, sub_rules);
                            }

                            // Save the new rule as either a choice rule or a multi rule based on how many groups of multi rules there were.
                            c_rule new_rule;

                            if (multi_rules.Length > 1)
                            {
                                new_rule = new c_choice_rule(pending_rule_id, multi_rules);
                            }
                            else
                            {
                                new_rule = multi_rules[0];
                            }

                            new_rule.fill_null_subrules();

                            rules[pending_rule_id] = new_rule;

                            // Remove this rule from the temporary list.
                            pending_rules.Remove(pending_rule_id);
                            break;
                        }
                    }
                }
            }

            public void display_rules()
            {
                foreach (c_rule rule in rules.Values)
                {
                    Console.WriteLine("{0}: {1}", rule.id, rule.ToString());
                }
            }

            public bool matches_rule(string message, int rule_id)
            {
                (bool match, int end_index) = rules[rule_id].try_match(message, 0);

                return match && end_index == message.Length;
            }
        }

        internal static (c_rule_collection, string[]) parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            c_rule_collection rule_collection = new c_rule_collection(input_reader);

            List<string> messages = new List<string>();

            while (input_reader.has_more_lines())
            {
                messages.Add(input_reader.read_line());
            }

            return (rule_collection, messages.ToArray());
        }

        public static void Part_1(
            string input,
            bool pretty)
        {
            (c_rule_collection rule_collection, string[] messages)  = parse_input(input, pretty);

            if (pretty)
            {
                rule_collection.display_rules();
            }

            int result = 0;

            foreach (string message in messages)
            {
                if (rule_collection.matches_rule(message, 0))
                {
                    result++;
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }

                if (pretty)
                {
                    Console.WriteLine(message);
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", result);
            Console.ResetColor();
        }

        public static void Part_2(
            string input,
            bool pretty)
        {
            // parse_input(input, pretty);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", 0);
            Console.ResetColor();
        }
    }
}
