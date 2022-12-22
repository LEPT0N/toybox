using System;
using System.Collections.Generic;
using System.Diagnostics;
using advent_of_code_common.input_reader;

namespace advent_of_code_2022.days
{
    internal class day_21
    {
        internal abstract class c_monkey
        {
            public string name { get; private set; }
            public c_monkey parent { get; set; }
            public bool human_in_tree { get; private set; }

            public c_monkey(string n)
            {
                name = n;
                parent = null;
                human_in_tree = false;
            }

            public abstract Int64 compute();

            public abstract Int64 determine_human_value_for_target(Int64 target_value);

            public abstract Int64 determine_human_value();

            public void set_human()
            {
                for (c_monkey current = this; current != null; current = current.parent)
                {
                    current.human_in_tree = true;
                }
            }
        }

        [DebuggerDisplay("{name} = {value}", Type = "c_monkey_const")]
        internal class c_monkey_const : c_monkey
        {
            private Int64 value;

            public c_monkey_const(string n, Int64 v)
                : base(n)
            {
                value = v;
            }

            public override Int64 compute()
            {
                return value;
            }

            public override Int64 determine_human_value_for_target(Int64 target_value)
            {
                if (!human_in_tree)
                {
                    throw new Exception("I am not a human.");
                }

                return target_value;
            }

            public override Int64 determine_human_value()
            {
                throw new Exception("determine_human_value can't be called for a c_monkey_const");
            }
        }

        internal enum e_monkey_operator
        {
            add,
            subtract,
            multiply,
            divide,
        }

        [DebuggerDisplay("{name} = {left_name} {op} {right_name}", Type = "c_monkey_operator")]
        internal class c_monkey_operator : c_monkey
        {
            public string left_name { get; private set; }
            public c_monkey left { get; private set; }

            public e_monkey_operator op { get; private set; }

            public string right_name { get; private set; }
            public c_monkey right { get; private set; }

            public c_monkey_operator(string n, string l_n, e_monkey_operator o, string r_n)
                : base(n)
            {
                left_name = l_n;
                left = null;
                op = o;
                right_name = r_n;
                right = null;
            }

            public void set_children(c_monkey l, c_monkey r)
            {
                left = l;
                l.parent = this;

                right = r;
                r.parent = this;
            }

            public override Int64 compute()
            {
                switch (op)
                {
                    case e_monkey_operator.add:
                        return left.compute() + right.compute();

                    case e_monkey_operator.subtract:
                        return left.compute() - right.compute();

                    case e_monkey_operator.multiply:
                        return left.compute() * right.compute();

                    case e_monkey_operator.divide:
                        return left.compute() / right.compute();

                    default:
                        throw new Exception($"Invalid operation {op}");
                }
            }

            public override Int64 determine_human_value_for_target(Int64 target_value)
            {
                if (!human_in_tree)
                {
                    throw new Exception("I am not a human.");
                }

                c_monkey human_child = left;
                c_monkey const_child = right;

                if (right.human_in_tree)
                {
                    human_child = right;
                    const_child = left;
                }

                Int64 const_value = const_child.compute();

                Int64 child_target_value;

                switch (op, left.human_in_tree)
                {
                    case (e_monkey_operator.add, true):
                        // x + c = t
                        // x = t - c
                        child_target_value = target_value - const_value;
                        break;

                    case (e_monkey_operator.subtract, true):
                        // x - c = t
                        // x = t + c
                        child_target_value = target_value + const_value;
                        break;

                    case (e_monkey_operator.multiply, true):
                        // x * c = t
                        // x = t / c
                        child_target_value = target_value / const_value;
                        break;

                    case (e_monkey_operator.divide, true):
                        // x / c = t
                        // x = t * c
                        child_target_value = target_value * const_value;
                        break;

                    case (e_monkey_operator.add, false):
                        // c + x = t
                        // x = t - c
                        child_target_value = target_value - const_value;
                        break;

                    case (e_monkey_operator.subtract, false):
                        // c - x = t
                        // x = c - t
                        child_target_value = const_value - target_value;
                        break;

                    case (e_monkey_operator.multiply, false):
                        // c * x = t
                        // x = t / c
                        child_target_value = target_value / const_value;
                        break;

                    case (e_monkey_operator.divide, false):
                        // c / x = t
                        // x = c / t
                        child_target_value = const_value / target_value;
                        break;

                    default:
                        throw new Exception($"Invalid operation {op}");
                }

                return human_child.determine_human_value_for_target(child_target_value);
            }

            public override Int64 determine_human_value()
            {
                if (left.human_in_tree == right.human_in_tree)
                {
                    throw new Exception("Two humans. Not allowed.");
                }

                c_monkey human_child = left;
                c_monkey const_child = right;

                if (right.human_in_tree)
                {
                    human_child = right;
                    const_child = left;
                }

                Int64 const_value = const_child.compute();

                return human_child.determine_human_value_for_target(const_value);
            }
        }

        internal static Dictionary<string, c_monkey> parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            Dictionary<string, c_monkey> monkeys = new Dictionary<string, c_monkey>();

            while (input_reader.has_more_lines())
            {
                string[] input_line = input_reader.read_line().Split(": ");
                string monkey_name = input_line[0];
                string monkey_value = input_line[1];

                if (Int64.TryParse(monkey_value, out Int64 monkey_const_value))
                {
                    monkeys.Add(monkey_name, new c_monkey_const(monkey_name, monkey_const_value));
                }
                else
                {
                    string[] input_value = monkey_value.Split(' ');
                    string left_monkey_name = input_value[0];
                    string monkey_operator = input_value[1];
                    string right_monkey_name = input_value[2];

                    e_monkey_operator op;
                    switch (monkey_operator)
                    {
                        case "+": op = e_monkey_operator.add; break;
                        case "-": op = e_monkey_operator.subtract; break;
                        case "*": op = e_monkey_operator.multiply; break;
                        case "/": op = e_monkey_operator.divide; break;
                        default: throw new Exception($"Bad operator '{monkey_operator}'");
                    }

                    monkeys.Add(monkey_name, new c_monkey_operator(monkey_name, left_monkey_name, op, right_monkey_name));
                }
            }

            foreach (c_monkey monkey in monkeys.Values)
            {
                c_monkey_operator monkey_operator = monkey as c_monkey_operator;

                if (monkey_operator != null)
                {
                    monkey_operator.set_children(monkeys[monkey_operator.left_name], monkeys[monkey_operator.right_name]);
                }
            }

            return monkeys;
        }

        public static void part_1(
            string input,
            bool pretty)
        {
            Dictionary<string, c_monkey> monkeys = parse_input(input, pretty);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", monkeys["root"].compute());
            Console.ResetColor();
        }

        public static void part_2(
            string input,
            bool pretty)
        {
            Dictionary<string, c_monkey> monkeys = parse_input(input, pretty);

            monkeys["humn"].set_human();

            Int64 result = monkeys["root"].determine_human_value();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", result);
            Console.ResetColor();
        }
    }
}
