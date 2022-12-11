using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using advent_of_code_common.input_reader;

namespace advent_of_code_2022.days
{
    internal class day_11
    {
        internal interface i_operation
        {
            public UInt64 execute(UInt64 old);
        }

        [DebuggerDisplay("(old + {argument})", Type = "c_operation_add")]
        internal class c_operation_add : i_operation
        {
            private UInt64 argument;

            public c_operation_add(UInt64 a)
            {
                argument = a;
            }

            public UInt64 execute(UInt64 old)
            {
                return (old + argument);
            }
        }

        [DebuggerDisplay("(old * {argument})", Type = "c_operation_multiply")]
        internal class c_operation_multiply : i_operation
        {
            private UInt64 argument;

            public c_operation_multiply(UInt64 a)
            {
                argument = a;
            }

            public UInt64 execute(UInt64 old)
            {
                return (old * argument);
            }
        }

        [DebuggerDisplay("(old * old)", Type = "c_operation_square")]
        internal class c_operation_square : i_operation
        {
            public UInt64 execute(UInt64 old)
            {
                return (old * old);
            }
        }

        [DebuggerDisplay("(v % {divisor}) == 0 ? {true_result} : {false_result}", Type = "c_test")]
        internal class c_test
        {
            private UInt64 divisor;
            private UInt64 true_result;
            private UInt64 false_result;

            public c_test(UInt64 d, UInt64 t, UInt64 f)
            {
                divisor = d;
                true_result = t;
                false_result = f;
            }

            public UInt64 execute(UInt64 value)
            {
                if (value % divisor == 0)
                {
                    return true_result;
                }
                else
                {
                    return false_result;
                }
            }

            public UInt64 get_divisor()
            {
                return divisor;
            }
        }

        [DebuggerDisplay("{operation} -> {test}", Type = "c_monkey")]
        internal class c_monkey
        {
            private Queue<UInt64> held_items = new Queue<UInt64>();
            private i_operation operation;
            private c_test test;
            private UInt64 inspections_performed;

            public c_monkey(
                UInt64[] items,
                i_operation op,
                c_test t)
            {
                held_items = new Queue<UInt64>();
                foreach(UInt64 i in items)
                {
                    held_items.Enqueue(i);
                }

                operation = op;
                test = t;
                inspections_performed = 0;
            }

            public void hold(UInt64 item)
            {
                held_items.Enqueue(item);
            }

            public void execute(c_monkey[] monkeys, UInt64 worry_factor, UInt64 total_divisor)
            {
                while (held_items.Any())
                {
                    inspections_performed++;

                    UInt64 old_worry = held_items.Dequeue();

                    UInt64 new_worry = operation.execute(old_worry) / worry_factor;

                    UInt64 contained_new_worry = new_worry % total_divisor;

                    UInt64 target = test.execute(contained_new_worry);

                    monkeys[target].hold(contained_new_worry);
                }
            }

            public string print_held_items()
            {
                return string.Join(", ", held_items.ToArray().Select(n => n.ToString()));
            }

            public UInt64 get_inspection_count()
            {
                return inspections_performed;
            }

            public UInt64 get_test_divisor()
            {
                return test.get_divisor();
            }
        }

        internal static c_monkey[] parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            List<c_monkey> monkeys = new List<c_monkey>();

            while (input_reader.has_more_lines())
            {
                // Monkey N:
                input_reader.read_line();

                //   Starting items: N, N, N
                UInt64[] held_items = input_reader.read_line()
                    .Substring("  Starting items: ".Length)
                    .Split(", ")
                    .Select(n => UInt64.Parse(n))
                    .ToArray();

                //   Operation: new [op] [arg]
                string[] operation_input = input_reader.read_line()
                    .Substring("  Operation: new = old ".Length)
                    .Split(' ');

                i_operation operation;

                if (operation_input[0] == "+")
                {
                    operation = new c_operation_add(UInt64.Parse(operation_input[1]));
                }
                else
                {
                    if (operation_input[1] == "old")
                    {
                        operation = new c_operation_square();
                    }
                    else
                    {
                        operation = new c_operation_multiply(UInt64.Parse(operation_input[1]));
                    }
                }

                //   Test: divisible by [N]
                UInt64 test_divisor = UInt64.Parse(input_reader.read_line()
                    .Substring("  Test: divisible by ".Length));

                //     If true: throw to monkey 
                UInt64 test_true_result = UInt64.Parse(input_reader.read_line()
                    .Substring("    If true: throw to monkey ".Length));

                //     If true: throw to monkey 
                UInt64 test_false_result = UInt64.Parse(input_reader.read_line()
                    .Substring("    If false: throw to monkey ".Length));

                c_test test = new c_test(test_divisor, test_true_result, test_false_result);

                // Add to our result list
                monkeys.Add(new c_monkey(held_items, operation, test));

                // Read blank line
                if (input_reader.has_more_lines())
                {
                    input_reader.read_line();
                }
            }

            return monkeys.ToArray();
        }

        public static void display(bool pretty, int round, c_monkey[] monkeys)
        {
            if (!pretty)
            {
                return;
            }

            Console.WriteLine($"After round {round}, the monkeys are holding items with these worry levels:");

            for (int i = 0; i < monkeys.Length; i++)
            {
                Console.WriteLine("Monkey {0}: {1}", i, monkeys[i].print_held_items());
            }

            Console.WriteLine();
        }

        public static void display_inspection_counts(bool pretty, int round, c_monkey[] monkeys)
        {
            if (!pretty)
            {
                return;
            }

            Console.WriteLine($"== After round {round} ==");

            for (int i = 0; i < monkeys.Length; i++)
            {
                UInt64 inspection_count = monkeys[i].get_inspection_count();

                Console.WriteLine($"Monkey {i} inspected items {inspection_count} times.");
            }

            Console.WriteLine();
        }

        public static void part_1(
            string input,
            bool pretty)
        {
            c_monkey[] monkeys = parse_input(input, pretty);

            display(pretty, 0, monkeys);

            int round_count = 20;

            for (int i = 1; i <= round_count; i++)
            {
                foreach (c_monkey monkey in monkeys)
                {
                    monkey.execute(monkeys, 3, 1000000000);
                }

                display(pretty, i, monkeys);
            }

            display_inspection_counts(pretty, round_count, monkeys);

            UInt64[] results = monkeys
                .Select(m => m.get_inspection_count())
                .OrderByDescending(n => n)
                .ToArray();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", results[0] * results[1]);
            Console.ResetColor();
        }

        public static void part_2(
            string input,
            bool pretty)
        {
            c_monkey[] monkeys = parse_input(input, pretty);

            UInt64 total_divisor = monkeys.Select(m => m.get_test_divisor()).Aggregate(1UL, (m, n) => m * n);

            int round_count = 10000;

            for (int i = 1; i <= round_count; i++)
            {
                foreach (c_monkey monkey in monkeys)
                {
                    monkey.execute(monkeys, 1, total_divisor);
                }

                if (i == 1 || i == 20 || i % 1000 == 0)
                {
                    display_inspection_counts(pretty, i, monkeys);
                }
            }

            display_inspection_counts(pretty, round_count, monkeys);

            UInt64[] results = monkeys
                .Select(m => m.get_inspection_count())
                .OrderByDescending(n => n)
                .ToArray();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", results[0] * results[1]);
            Console.ResetColor();
        }
    }
}
