using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace advent_of_code_2021.Days
{
    internal class Day_18
    {
        [DebuggerDisplay("{DebuggerDisplay,nq}", Type = "number pair")]
        internal class c_snailfish_number
        {
            private enum e_snailfish_number_type
            {
                pair,
                raw_number,
            }

            private e_snailfish_number_type type;
            private c_snailfish_number parent;
            private c_snailfish_number left;
            private c_snailfish_number right;
            private UInt64 value;

            private string DebuggerDisplay
            {
                get
                {
                    switch (type)
                    {
                        case e_snailfish_number_type.raw_number:
                            return value.ToString();

                        case e_snailfish_number_type.pair:
                            return String.Format("[{0},{1}]", left.DebuggerDisplay, right.DebuggerDisplay);

                        default:
                            return "???";
                    }
                }
            }

            private c_snailfish_number parse_pair_element(
                string input,
                int start_index,
                out int end_index)
            {
                c_snailfish_number result;

                if (input[start_index] == '[')
                {
                    int left_bracket_depth = 1;
                    for (end_index = start_index; left_bracket_depth > 0; end_index++)
                    {
                        switch (input[end_index + 1])
                        {
                            case '[': left_bracket_depth++; break;
                            case ']': left_bracket_depth--; break;
                        }
                    }

                    result = new c_snailfish_number(this, input.Substring(start_index, end_index - start_index + 1));
                }
                else
                {
                    end_index = start_index;
                    while (input[end_index + 1] != ',' && input[end_index + 1] != ']')
                    {
                        end_index++;
                    }

                    result = new c_snailfish_number(this, input.Substring(start_index, end_index - start_index + 1));
                }

                return result;
            }

            public c_snailfish_number(
                string input)
                : this(null, input)
            {
            }

            private c_snailfish_number(
                c_snailfish_number p,
                string input)
            {
                parent = p;

                if (input[0] == '[')
                {
                    type = e_snailfish_number_type.pair;

                    int left_start_index = 1;
                    int left_end_index;

                    left = parse_pair_element(input, left_start_index, out left_end_index);

                    int right_start_index = left_end_index + 2;
                    int right_end_index;

                    right = parse_pair_element(input, right_start_index, out right_end_index);
                }
                else
                {
                    type = e_snailfish_number_type.raw_number;

                    value = UInt64.Parse(input);
                }
            }

            private c_snailfish_number(
                c_snailfish_number l,
                c_snailfish_number r,
                c_snailfish_number p)
            {
                type = e_snailfish_number_type.pair;
                parent = p;

                left = l.clone();
                left.parent = this;

                right = r.clone();
                right.parent = this;
            }

            private c_snailfish_number(
                UInt64 v,
                c_snailfish_number p)
            {
                type = e_snailfish_number_type.raw_number;
                parent = p;

                value = v;
            }

            public void display(bool pretty, int num_parents = 0)
            {
                if (!pretty) return;

                switch (type)
                {
                    case e_snailfish_number_type.raw_number:
                        {
                            ConsoleColor color = value >= 10 ? ConsoleColor.Yellow : ConsoleColor.White;
                            Console.ForegroundColor = color;
                            Console.Write(value);
                            Console.ResetColor();
                        }
                        break;

                    case e_snailfish_number_type.pair:
                        {
                            ConsoleColor color = num_parents >= 4 ? ConsoleColor.Red : ConsoleColor.White;

                            Console.ForegroundColor = color;
                            Console.Write('[');
                            Console.ResetColor();

                            left.display(pretty, num_parents + 1);

                            Console.ForegroundColor = color;
                            Console.Write(',');
                            Console.ResetColor();

                            right.display(pretty, num_parents + 1);

                            Console.ForegroundColor = color;
                            Console.Write(']');
                            Console.ResetColor();
                        }
                        break;
                }
            }

            static void validate(c_snailfish_number number, c_snailfish_number parent = null)
            {
                Debug.Assert(number.parent == parent);

                if (number.left != null)
                {
                    validate(number.left, number);
                }

                if (number.right != null)
                {
                    validate(number.right, number);
                }
            }

            public c_snailfish_number plus(
                bool pretty,
                c_snailfish_number other)
            {
                if (pretty)
                {
                    Console.WriteLine();
                    Console.WriteLine("adding these:");
                    display(pretty);
                    Console.WriteLine();
                    other.display(pretty);
                    Console.WriteLine();
                }

                validate(this);
                validate(other);

                c_snailfish_number result = new c_snailfish_number(this, other, null);

                validate(result);

                if (pretty)
                {
                    Console.WriteLine("equals:");
                    result.display(pretty);
                    Console.WriteLine();
                }

                bool reduced;
                do
                {
                    reduced = result.try_explode(pretty);

                    if (!reduced)
                    {
                        reduced = result.try_split(pretty);
                    }

                    if (pretty && reduced)
                    {
                        Console.WriteLine("reduced to:");
                        result.display(pretty);
                        Console.WriteLine();
                    }

                    validate(result);

                } while (reduced);

                return result;
            }

            private void explode_left_value(
                bool pretty)
            {
                Debug.Assert(type == e_snailfish_number_type.pair);
                Debug.Assert(left.type == e_snailfish_number_type.raw_number);

                UInt64 exploding_value = left.value;

                c_snailfish_number current = this;
                bool searching_up = true;

                while (searching_up)
                {
                    searching_up = (current.parent != null && current.parent.left == current);
                    current = current.parent;
                }

                if (current != null)
                {
                    current = current.left;

                    while (current.type == e_snailfish_number_type.pair)
                    {
                        current = current.right;
                    }

                    UInt64 exploding_result = exploding_value + current.value;

                    if (pretty)
                    {
                        Console.WriteLine("    {0} + {1} = {2}", exploding_value, current.value, exploding_result);
                    }

                    current.value = exploding_result;
                }
                else
                {
                    if (pretty)
                    {
                        Console.WriteLine("    Discarding {0}", exploding_value);
                    }
                }
            }

            private void explode_right_value(
                bool pretty)
            {
                Debug.Assert(type == e_snailfish_number_type.pair);
                Debug.Assert(right.type == e_snailfish_number_type.raw_number);

                UInt64 exploding_value = right.value;

                c_snailfish_number current = this;
                bool searching_up = true;

                while (searching_up)
                {
                    searching_up = (current.parent != null && current.parent.right == current);
                    current = current.parent;
                }

                if (current != null)
                {
                    current = current.right;

                    while (current.type == e_snailfish_number_type.pair)
                    {
                        current = current.left;
                    }

                    UInt64 exploding_result = exploding_value + current.value;

                    if (pretty)
                    {
                        Console.WriteLine("    {0} + {1} = {2}", exploding_value, current.value, exploding_result);
                    }

                    current.value = exploding_result;
                }
                else
                {
                    if (pretty)
                    {
                        Console.WriteLine("    Discarding {0}", exploding_value);
                    }
                }
            }

            public bool try_explode(
                bool pretty)
            {
                return try_explode(pretty, 0);
            }

            private bool try_explode(
                bool pretty,
                int num_parents)
            {
                bool result = false;

                if (type == e_snailfish_number_type.pair)
                {
                    result = left.try_explode(pretty, num_parents + 1);

                    if (!result &&
                        num_parents >= 4)
                    {
                        if (pretty)
                        {
                            Console.WriteLine("exploding:");
                            display(pretty);
                            Console.WriteLine();
                        }

                        explode_left_value(pretty);
                        explode_right_value(pretty);

                        type = e_snailfish_number_type.raw_number;
                        left = null;
                        right = null;
                        value = 0;

                        if (pretty)
                        {
                            Console.WriteLine("into:");
                            display(pretty);
                            Console.WriteLine();
                        }

                        result = true;
                    }

                    if (!result)
                    {
                        result = right.try_explode(pretty, num_parents + 1);
                    }
                }

                return result;
            }

            public bool try_split(
                bool pretty)
            {
                bool result = false;

                if (type == e_snailfish_number_type.pair)
                {
                    result = left.try_split(pretty);

                    if (!result)
                    {
                        result = right.try_split(pretty);
                    }
                }
                else  if (type == e_snailfish_number_type.raw_number &&
                    value >= 10)
                {
                    if (pretty)
                    {
                        Console.WriteLine("splitting:");
                        display(pretty);
                        Console.WriteLine();
                    }

                    type = e_snailfish_number_type.pair;
                    left = new c_snailfish_number(value / 2, this);
                    right = new c_snailfish_number(value / 2 + value % 2, this);
                    value = 0;

                    if (pretty)
                    {
                        Console.WriteLine("into:");
                        display(pretty);
                        Console.WriteLine();
                    }

                    result = true;
                }

                return result;
            }

            public c_snailfish_number clone()
            {
                switch (type)
                {
                    case e_snailfish_number_type.pair:
                        return new c_snailfish_number(left.clone(), right.clone(), parent);

                    case e_snailfish_number_type.raw_number:
                        return new c_snailfish_number(value, parent);

                    default:
                        return null;
                }
            }

            public UInt64 magnitude()
            {
                switch (type)
                {
                    case e_snailfish_number_type.pair:
                        return 3 * left.magnitude() + 2 * right.magnitude();

                    case e_snailfish_number_type.raw_number:
                        return value;

                    default:
                        return 0;
                }
            }
        }

        internal static c_snailfish_number[] parse_input(
            string input,
            bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            List<c_snailfish_number> numbers = new List<c_snailfish_number>();

            while (input_reader.has_more_lines())
            {
                string input_line = input_reader.read_line();

                numbers.Add(new c_snailfish_number(input_line));
            }

            if (pretty)
            {
                Console.WriteLine("input:");
                foreach (c_snailfish_number number in numbers)
                {
                    number.display(pretty);
                    Console.WriteLine();
                }
            }

            return numbers.ToArray();
        }

        public static void Part_1(
            string input,
            bool pretty)
        {
            c_snailfish_number[] numbers = parse_input(input, pretty);

            c_snailfish_number sum = numbers[0];

            for (int i = 1; i < numbers.Length; i++)
            {
                sum = sum.plus(pretty, numbers[i]);
            }

            Console.WriteLine();
            Console.WriteLine("final sum:");
            sum.display(true);
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Magnitude = {0}", sum.magnitude());
            Console.ResetColor();
        }

        public static void Part_2(
            string input,
            bool pretty)
        {
            c_snailfish_number[] numbers = parse_input(input, pretty);

            UInt64 max_magnitude = UInt64.MinValue;

            for (int i = 0; i < numbers.Length; i++)
            {
                for (int j = 0; j < numbers.Length; j++)
                {
                    if (i != j)
                    {
                        c_snailfish_number sum = numbers[i].plus(pretty, numbers[j]);

                        UInt64 magnitude = sum.magnitude();

                        if (pretty)
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine();
                            Console.WriteLine("Magnitude = {0}", magnitude);
                            Console.ResetColor();
                        }

                        max_magnitude = Math.Max(max_magnitude, magnitude);
                    }
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Max Magnitude = {0}", max_magnitude);
            Console.ResetColor();
        }
    }
}
