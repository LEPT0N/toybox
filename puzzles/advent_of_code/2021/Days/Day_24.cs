using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace advent_of_code_2021.Days
{
    internal class Day_24
    {
        [DebuggerDisplay("{DebuggerDisplay,nq}", Type = "c_value")]
        internal class c_value
        {
            public string name;
            public BigInteger value;

            private string DebuggerDisplay
            {
                get { return (name != null ? name : value.ToString()); }
            }
        }

        internal interface i_instruction
        {
            public void execute();
        }

        [DebuggerDisplay("inp {a}", Type = "c_input_instruction")]
        internal class c_input_instruction : i_instruction
        {
            public c_value a;
            public c_alu alu;

            public void execute()
            {
                a.value = alu.read_input_value();
            }
        }

        [DebuggerDisplay("add {a} {b}", Type = "c_input_instruction")]
        internal class c_add_instruction : i_instruction
        {
            public c_value a;
            public c_value b;

            public void execute()
            {
                a.value = a.value + b.value;
            }
        }

        [DebuggerDisplay("mul {a} {b}", Type = "c_multiply_instruction")]
        internal class c_multiply_instruction : i_instruction
        {
            public c_value a;
            public c_value b;

            public void execute()
            {
                a.value = a.value * b.value;
            }
        }

        [DebuggerDisplay("div {a} {b}", Type = "c_divide_instruction")]
        internal class c_divide_instruction : i_instruction
        {
            public c_value a;
            public c_value b;

            public void execute()
            {
                a.value = a.value / b.value;
            }
        }

        [DebuggerDisplay("mod {a} {b}", Type = "c_modulo_instruction")]
        internal class c_modulo_instruction : i_instruction
        {
            public c_value a;
            public c_value b;

            public void execute()
            {
                a.value = a.value % b.value;
            }
        }

        [DebuggerDisplay("eql {a} {b}", Type = "c_equals_instruction")]
        internal class c_equals_instruction : i_instruction
        {
            public c_value a;
            public c_value b;

            public void execute()
            {
                a.value = (a.value == b.value ? 1 : 0);
            }
        }

        [DebuggerDisplay("{DebuggerDisplay,nq}", Type = "c_value")]
        internal class c_alu
        {
            private Dictionary<char, c_value> registers;

            public int instruction_groups_count { get; private set; }
            private List<i_instruction>[] instruction_groups;

            private int[] input;
            int inputs_read;

            private string DebuggerDisplay
            {
                get
                {
                    return string.Format(
                        "w = {0} x = {1} y = {2} z = {3}",
                        registers['w'].value,
                        registers['x'].value,
                        registers['y'].value,
                        registers['z'].value);
                }
            }

            public c_alu()
            {
                registers = new Dictionary<char, c_value>();

                registers['w'] = new c_value { name = "w", value = 0 };
                registers['x'] = new c_value { name = "x", value = 0 };
                registers['y'] = new c_value { name = "y", value = 0 };
                registers['z'] = new c_value { name = "z", value = 0 };

                instruction_groups = new List<i_instruction>[14];
                instruction_groups_count = 0;
            }

            public void set_register_value(char name, BigInteger value)
            {
                registers[name].value = value;
            }

            public BigInteger get_register_value(char name)
            {
                return registers[name].value;
            }

            private c_value create_value(string input)
            {
                c_value result;

                if (registers.ContainsKey(input[0]))
                {
                    result = registers[input[0]];
                }
                else
                {
                    Int64 value;
                    if (Int64.TryParse(input, out value))
                    {
                        result = new c_value { value = Int64.Parse(input) };
                    }
                    else
                    {
                        throw new Exception(String.Format("Bad value input '{0}'", input));
                    }
                }

                return result;
            }

            public void add_instruction(string input)
            {
                string[] split_input = input.Split(' ');

                c_value a = create_value(split_input[1]);
                c_value b = null;

                if (split_input.Length > 2)
                {
                    b = create_value(split_input[2]);
                }

                i_instruction instruction;

                switch (split_input[0])
                {
                    case "inp":
                        instruction_groups[instruction_groups_count] = new List<i_instruction>();
                        instruction_groups_count++;
                        instruction = new c_input_instruction { a = a, alu = this };
                        break;

                    case "add": instruction = new c_add_instruction { a = a, b = b }; break;
                    case "mul": instruction = new c_multiply_instruction { a = a, b = b }; break;
                    case "div": instruction = new c_divide_instruction { a = a, b = b }; break;
                    case "mod": instruction = new c_modulo_instruction { a = a, b = b }; break;
                    case "eql": instruction = new c_equals_instruction { a = a, b = b }; break;

                    default:
                        throw new Exception(String.Format("Bad instruction input '{0}'", split_input[0]));
                }

                instruction_groups[instruction_groups_count - 1].Add(instruction);
            }

            public void set_input(int[] input_values)
            {
                input = input_values;
                inputs_read = 0;
            }

            public int read_input_value()
            {
                int result = input[inputs_read];
                inputs_read++;
                return result;
            }

            public void execute(int first_instruction_group = 0)
            {
                for (int instruction_group = first_instruction_group; instruction_group < instruction_groups_count; instruction_group++)
                {
                    foreach (i_instruction instruction in instruction_groups[instruction_group])
                    {
                        instruction.execute();
                    }
                }
            }

            public void execute_just_one_group(int instruction_group)
            {
                foreach (i_instruction instruction in instruction_groups[instruction_group])
                {
                    instruction.execute();
                }
            }

            /*public void execute()
            {
                foreach (i_instruction instruction in instructions)
                {
                    instruction.execute();
                }
            }*/
        }

        internal static c_alu parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            c_alu alu = new c_alu();

            while (input_reader.has_more_lines())
            {
                alu.add_instruction(input_reader.read_line());
            }

            return alu;
        }

        /*internal class c_something
        {
            int target_result;
        }*/

        public static void derp_each_once(
            c_alu alu)
        {
            int[] input_values = new int[1];

            for (int instruction_group = 13; instruction_group >= 13; instruction_group--)
            // for (int instruction_group = alu.instruction_groups_count - 1; instruction_group >= 0; instruction_group--)
            {
                Console.WriteLine();
                Console.WriteLine("---------------------------");
                Console.WriteLine("INSTRUCTION GROUP {0}", instruction_group);
                Console.WriteLine();

                BigInteger min_prev_minus_current_result = int.MaxValue;
                BigInteger max_prev_minus_current_result = int.MinValue;

                //for (int input_value = 1; input_value <= 9; input_value++)
                for (int input_value = 1; input_value <= 9; input_value++)
                {
                    BigInteger target_result = 0;

                    // for (BigInteger target_result = -26; target_result <= 26; target_result++)
                    {
                        input_values[0] = input_value;

                        for (BigInteger previous_result = -1000; previous_result <= 1000; previous_result++)
                        {
                            alu.set_register_value('z', previous_result);
                            alu.set_input(input_values.ToArray());
                            alu.execute_just_one_group(instruction_group);

                            BigInteger result = alu.get_register_value('z');
                            if (result == target_result)
                            {
                                BigInteger prev_minus_current_result = (previous_result - result) / 26;

                                Console.WriteLine(
                                    "z = {0} w = {1} result = {2} (prev - result = {3})",
                                    previous_result,
                                    input_value,
                                    result,
                                    prev_minus_current_result);

                                min_prev_minus_current_result = BigInteger.Min(min_prev_minus_current_result, prev_minus_current_result);
                                max_prev_minus_current_result = BigInteger.Max(max_prev_minus_current_result, prev_minus_current_result);
                            }
                        }
                    }
                }

                Console.WriteLine();
                Console.WriteLine("prev - result = [ {0}, {1} ]",
                    min_prev_minus_current_result,
                    max_prev_minus_current_result);
            }
        }

        public static void derp(
            c_alu alu,
            List<int> input_values,
            int instruction_group,
            BigInteger target_result)
        {
            input_values.Insert(0, 0);

            for (int input_value = 1; input_value <= 9; input_value++)
            {
                input_values[0] = input_value;

                BigInteger previous_result_min = target_result * 26 - 26 - 5;
                BigInteger previous_result_max = target_result * 26 + 26 + 5;

                if (instruction_group == 0)
                {
                    previous_result_min = 0;
                    previous_result_max = 0;
                }

                // for (BigInteger previous_result = target_result * 26 - 1; previous_result <= target_result * 26 + 26 + 5; previous_result++)
                for (BigInteger previous_result = previous_result_min; previous_result <= previous_result_max; previous_result++)
                {
                    alu.set_register_value('z', previous_result);
                    alu.set_input(input_values.ToArray());
                    alu.execute(instruction_group);

                    BigInteger output_value = alu.get_register_value('z');
                    if (output_value == target_result)
                    {
                        /*Console.WriteLine(
                            "{0} z = {1} w = {2} result = {3}",
                            new string(' ', instruction_group),
                            previous_result,
                            input_value,
                            output_value);*/

                        if (instruction_group == 0)
                        {
                            Console.WriteLine("neat");
                        }
                        else
                        {
                            derp(alu, input_values, instruction_group - 1, previous_result);
                        }

                        break;
                    }
                }
                
                /*previous_result_min = target_result - 26 - 5;
                previous_result_max = target_result + 26 + 5;

                if (instruction_group == 0)
                {
                    previous_result_min = 0;
                    previous_result_max = 0;
                }

                for (BigInteger previous_result = previous_result_min; previous_result <= previous_result_max; previous_result++)
                {
                    alu.set_register_value('z', previous_result);
                    alu.set_input(input_values.ToArray());
                    alu.execute(instruction_group);

                    BigInteger output_value = alu.get_register_value('z');
                    if (output_value == target_result)
                    {
                        //Console.WriteLine(
                        //    "{0} z = {1} w = {2} result = {3}",
                        //    new string(' ', instruction_group),
                        //    previous_result,
                        //    input_value,
                        //    output_value);

                        if (instruction_group == 0)
                        {
                            Console.WriteLine("neat");
                        }
                        else
                        {
                            derp(alu, input_values, instruction_group - 1, previous_result);
                        }

                        break;
                    }
                }*/

                previous_result_min = target_result/26 - 26 - 5;
                previous_result_max = target_result/26 + 26 + 5;

                if (instruction_group == 0)
                {
                    previous_result_min = 0;
                    previous_result_max = 0;
                }

                for (BigInteger previous_result = previous_result_min; previous_result <= previous_result_max; previous_result++)
                {
                    alu.set_register_value('z', previous_result);
                    alu.set_input(input_values.ToArray());
                    alu.execute(instruction_group);

                    BigInteger output_value = alu.get_register_value('z');
                    if (output_value == target_result)
                    {
                        //Console.WriteLine(
                        //    "{0} z = {1} w = {2} result = {3}",
                        //    new string(' ', instruction_group),
                        //    previous_result,
                        //    input_value,
                        //    output_value);

                        if (instruction_group == 0)
                        {
                            Console.WriteLine("neat");
                        }
                        else
                        {
                            derp(alu, input_values, instruction_group - 1, previous_result);
                        }

                        break;
                    }
                }
            }

            input_values.RemoveAt(0);
        }

        public static void derp_ultimate(
            c_alu alu)
        {
            BigInteger previous_result = 0;

            int[] input_values = new int[2];

            for (int first_input_value = 1; first_input_value <= 9; first_input_value++)
            {
                for (int second_input_value = 1; second_input_value <= 9; second_input_value++)
                {
                    input_values[0] = first_input_value;
                    input_values[1] = second_input_value;

                    alu.set_register_value('z', previous_result);
                    alu.set_input(input_values.ToArray());
                    alu.execute();

                    BigInteger output_value = alu.get_register_value('z');

                    if (output_value == 0)
                    {
                        Console.WriteLine("[{0}, {1}] = {2}", first_input_value, second_input_value, output_value);
                    }
                }
            }
        }

        public static void Part_1(
            string input,
            bool pretty)
        {
            c_alu alu = parse_input(input, pretty);

            /*int[] input_values = new int[14];
            for (int i = 0; i < input_values.Length; i++)
            {
                input_values[i] = 6;
            }

            for (int w_value = 1; w_value <= 9; w_value++)
            {
                input_values[0] = w_value;

                for (int z_value = -20000; z_value < 20000; z_value++)
                {
                    alu.set_register_value('z', z_value);
                    alu.set_input(input_values);
                    alu.execute();

                    int output_value = alu.get_register_value('z');
                    if (output_value == 0)
                    {
                        Console.WriteLine(
                            "z = {0} w = {1} result = {2}",
                            z_value,
                            w_value,
                            output_value);
                    }
                }
            }*/

            // List<int> input_values = new List<int>();

            {
                // Set input to just two groups to get the list of valid pairs:
                // derp_ultimate(alu);
            }

            // derp_each_once(alu);

            // alu.set_input(input_values);

            // Verifying results for part 1:
            {
                // Construct this from calls to derp_ultimate and choose the largest pairs:
                // 96979989692495

                int[] input_values = { 9, 6, 9, 7, 9, 9, 8, 9, 6, 9, 2, 4, 9, 5 };
                alu.set_input(input_values);

                alu.set_register_value('z', 0);
                alu.execute();
            }

            // Verifying results for part 2:
            {
                // Construct this from calls to derp_ultimate and choose the smallest pairs:
                // 51316214181141

                int[] input_values = { 5, 1, 3, 1, 6, 2, 1, 4, 1, 8, 1, 1, 4, 1 };
                alu.set_input(input_values);

                alu.set_register_value('z', 0);
                alu.execute();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", alu.get_register_value('z'));
            Console.ResetColor();
        }

        public static void Part_2(
            string input,
            bool pretty)
        {
            // c_alu alu = parse_input(input, pretty);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", 0);
            Console.ResetColor();
        }
    }
}
