using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace advent_of_code_2020.Days
{
    internal class Day_18
    {
        internal interface i_value
        {
            public UInt64 compute();
            public UInt64 advanced_compute();
        }

        [DebuggerDisplay("{value}", Type = "c_literal_value")]
        internal class c_literal_value : i_value
        {
            public readonly UInt64 value;

            public c_literal_value(string input)
            {
                value = UInt64.Parse(input);
            }

            public c_literal_value(UInt64 input)
            {
                value = input;
            }

            public UInt64 compute()
            {
                return value;
            }

            public UInt64 advanced_compute()
            {
                return value;
            }

            public override string ToString()
            {
                return value.ToString();
            }
        }

        [DebuggerDisplay("{DebuggerDisplay,nq}", Type = "c_value_group")]
        internal class c_value_group : i_value
        {
            private static Regex number_scraper = new Regex(@"^(\d+).*");

            private i_value[] values;
            private e_operator[] operators;

            private string DebuggerDisplay
            {
                get { return ToString(); }
            }

            public override string ToString()
            {
                string result = "";

                if (values.Length == 0)
                {
                    result = "<empty>";
                }
                else
                {
                    result = "(";

                    result += values[0].ToString();

                    for (int i = 1; i < values.Length; i++)
                    {
                        switch (operators[i - 1])
                        {
                            case e_operator.add:
                                result += " + ";
                                break;

                            case e_operator.multiply:
                                result += " * ";
                                break;
                        }

                        result += values[i].ToString();
                    }

                    result += ")";
                }

                return result;
            }

            public c_value_group(string input, ref int index)
            {
                List<i_value> values_list = new List<i_value>();
                List<e_operator> operators_list = new List<e_operator>();

                for (; index < input.Length; index++)
                {
                    if (input[index] == ' ')
                    {
                        continue;
                    }
                    else if (input[index] == '+')
                    {
                        operators_list.Add(e_operator.add);
                    }
                    else if (input[index] == '*')
                    {
                        operators_list.Add(e_operator.multiply);
                    }
                    else if (input[index] >= '0' && input[index] <= '9')
                    {
                        Match number_match = number_scraper.Match(input.Substring(index));

                        values_list.Add(new c_literal_value(number_match.Groups[1].Value));
                    }
                    else if(input[index] == '(')
                    {
                        index++;

                        values_list.Add(new c_value_group(input, ref index));
                    }
                    else if (input[index] == ')')
                    {
                        break;
                    }
                    else
                    {
                        throw new Exception(string.Format("Unexpected character '{0}' at index '{1}' of input '{2}'", input[index], index, input));
                    }
                }

                values = values_list.ToArray();
                operators = operators_list.ToArray();
            }

            public UInt64 compute()
            {
                UInt64 result = values[0].compute();

                for (int i = 1; i < values.Length; i++)
                {
                    UInt64 new_value = values[i].compute();

                    switch (operators[i - 1])
                    {
                        case e_operator.add:
                            result += new_value;
                            break;

                        case e_operator.multiply:
                            result *= new_value;
                            break;
                    }
                }

                return result;
            }

            public UInt64 advanced_compute()
            {
                List<i_value> values_list = values.ToList();
                List<e_operator> operators_list = operators.ToList();

                // First perform additions
                for (int i = 0; i < operators_list.Count; i++)
                {
                    if (operators_list[i] == e_operator.add)
                    {
                        UInt64 new_value = values_list[i].advanced_compute() + values_list[i + 1].advanced_compute();

                        values_list[i] = new c_literal_value(new_value);
                        values_list.RemoveAt(i + 1);
                        operators_list.RemoveAt(i);
                        i--;
                    }
                }

                // Then perform multiplications
                UInt64 result = values_list[0].advanced_compute();

                for (int i = 0; i < operators_list.Count; i++)
                {
                    UInt64 new_value = values_list[i + 1].advanced_compute();

                    if (operators_list[i] == e_operator.multiply)
                    {
                        result *= new_value;
                    }
                    else
                    {
                        throw new Exception("everything should be multiply here.");
                    }
                }

                return result;
            }
        }

        internal enum e_operator
        {
            add,
            multiply,
        }

        internal static c_value_group[] parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            List<c_value_group> value_groups = new List<c_value_group>();

            while (input_reader.has_more_lines())
            {
                int index = 0;
                value_groups.Add(new c_value_group(input_reader.read_line(), ref index));
            }

            return value_groups.ToArray();
        }

        public static void Part_1(
            string input,
            bool pretty)
        {
            c_value_group[] value_groups = parse_input(input, pretty);

            UInt64[] results = value_groups.Select(x => x.compute()).ToArray();

            if (pretty)
            {
                for (int i = 0; i < value_groups.Length; i++)
                {
                    Console.WriteLine("{0} = {1}", value_groups[i], results[i]);
                }
            }

            UInt64 result = results.Aggregate((x, y) => x + y);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", result);
            Console.ResetColor();
        }

        public static void Part_2(
            string input,
            bool pretty)
        {
            c_value_group[] value_groups = parse_input(input, pretty);

            UInt64[] results = value_groups.Select(x => x.advanced_compute()).ToArray();

            if (pretty)
            {
                for (int i = 0; i < value_groups.Length; i++)
                {
                    Console.WriteLine("{0} = {1}", value_groups[i], results[i]);
                }
            }

            UInt64 result = results.Aggregate((x, y) => x + y);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", result);
            Console.ResetColor();
        }
    }
}
