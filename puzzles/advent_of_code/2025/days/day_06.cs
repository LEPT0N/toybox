using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.extensions;
using advent_of_code_common.input_reader;

namespace advent_of_code_2025.days
{
    internal class day_06
    {
        internal enum e_operator
        {
            unknown,
            add,
            multiply,
        }

        [DebuggerDisplay("{op} {inputs[0]} ...", Type = "c_equation")]
        internal class c_equation
        {
            private e_operator op;
            private List<UInt64> inputs;

            public c_equation()
            {
                op = e_operator.unknown;
                inputs = new List<UInt64>();
            }

            public c_equation(char op_char)
            {
                switch (op_char)
                {
                    case '+': op = e_operator.add; break;
                    case '*': op = e_operator.multiply; break;
                    default: throw new ArgumentException($"invalid operator '{op_char}'");
                }

                inputs = new List<UInt64>();
            }

            public void set_operator(e_operator op_input)
            {
                op = op_input;
            }

            public void add_input(UInt64 input)
            {
                inputs.Add(input);
            }

            public UInt64 get_result()
            {
                switch(op)
                {
                    case e_operator.add:
                        return inputs.Aggregate(0UL, (first, second) => first + second);

                    case e_operator.multiply:
                        return inputs.Aggregate(1UL, (first, second) => first * second);

                    default: throw new ArgumentException($"invalid operator '{op}'");
                }
            }
        }

        internal static c_equation[] parse_input_1(
            in c_input_reader input_reader,
            in bool pretty)
        {
            string line;

            // Gather the input into rows of numbers and rows of operators.

            List<UInt64[]> input_rows = new List<UInt64[]>();
            List<char> operators = new List<char>();

            while (input_reader.has_more_lines())
            {
                line = input_reader.read_line().Trim();

                if (line[0] >= '0' && line[0] <= '9')
                {
                    input_rows.Add(line
                        .Split(' ')
                        .Where(s => !String.IsNullOrEmpty(s))
                        .Select(s => UInt64.Parse(s))
                        .ToArray());
                }
                else
                {
                    operators = line
                        .Split(' ')
                        .Where(s => !String.IsNullOrEmpty(s))
                        .Select(s => s[0])
                        .ToList();

                    break;
                }
            }

            // For each item in our arrays, build an equation and add to the list.

            List<c_equation> equations = new List<c_equation>();

            for (int i = 0; i < operators.Count; i++)
            {
                c_equation equation = new c_equation(operators[i]);

                for (int row = 0; row < input_rows.Count; row++)
                {
                    equation.add_input(input_rows[row][i]);
                }

                equations.Add(equation);
            }

            return equations.ToArray();
        }

        public static void part_1(
            c_input_reader input_reader,
            bool pretty)
        {
            c_equation[] equations = parse_input_1(input_reader, pretty);

            // Sum all results.

            UInt64[] results = equations.Select(e => e.get_result()).ToArray();

            UInt64 result = results.sum();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }

        internal static (string[], string) parse_input_2(
            in c_input_reader input_reader,
            in bool pretty)
        {
            List<string> input_rows = new List<string>();
            string operators = null;

            // Just build the rows of string data and return.

            while (input_reader.has_more_lines())
            {
                // load-bearing whitespace.
                string line = " " + input_reader.read_line();

                if (input_reader.has_more_lines())
                {
                    input_rows.Add(line);
                }
                else
                {
                    operators = line;
                }
            }

            return (input_rows.ToArray(), operators);
        }

        public static void part_2(
            c_input_reader input_reader,
            bool pretty)
        {
            (string[] input_rows, string operators) = parse_input_2(input_reader, pretty);

            List<c_equation> equations = new List<c_equation>();
            c_equation current_equation = new c_equation();

            // Loop through each column of input from right to left.

            for (int i = operators.Length - 1; i >= 0; i--)
            {
                // If we find a colum of all whitespace, add the finished equation to our list.

                if (operators[i] == ' ' && input_rows.All(row => row[i] == ' '))
                {
                    equations.Add(current_equation);
                    current_equation = new c_equation();
                }
                else
                {
                    // Otherwise, if we find an operator, set that into the current equation.

                    switch (operators[i])
                    {
                        case '+': current_equation.set_operator(e_operator.add); break;
                        case '*': current_equation.set_operator(e_operator.multiply); break;
                    }

                    // And build any number in this column, and set that into the equation.

                    UInt64 value = 0;
                    for (int row = 0; row < input_rows.Length; row++)
                    {
                        char input_char = input_rows[row][i];

                        if (input_char != ' ')
                        {
                            value = (value * 10UL) + (UInt64)(input_char - '0');
                        }
                    }

                    current_equation.add_input(value);
                }
            }

            // Sum all results.

            UInt64[] results = equations.Select(e => e.get_result()).ToArray();

            UInt64 result = results.sum();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }
    }
}
