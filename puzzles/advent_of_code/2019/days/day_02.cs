using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.extensions;
using advent_of_code_common.input_reader;

namespace advent_of_code_2019.days
{
    internal class day_02
    {
        [DebuggerDisplay("current_index = {current_index}", Type = "c_computer")]
        internal class c_computer
        {
            private const int k_opcode_add = 1;
            private const int k_opcode_multiply = 2;
            private const int k_opcode_terminate = 99;

            [DebuggerDisplay("{value} {state}", Type = "c_opcode")]
            private class c_opcode
            {
                private enum e_opcode_state
                {
                    none,
                    read,
                    written,
                }

                private int value;
                private e_opcode_state state;

                public c_opcode(int input)
                {
                    value = input;
                    state = e_opcode_state.none;
                }

                public void reset_state()
                {
                    state = e_opcode_state.none;
                }

                public int read_value()
                {
                    state = e_opcode_state.read;
                    return value;
                }

                public void write_value(int input)
                {
                    state = e_opcode_state.written;
                    value = input;
                }

                public void print(int index, bool is_current_index = false, string value_override = null)
                {
                    ConsoleColor data_color = ConsoleColor.DarkGray;

                    switch (state)
                    {
                        case e_opcode_state.read:
                            data_color = ConsoleColor.Cyan;
                            break;

                        case e_opcode_state.written:
                            data_color = ConsoleColor.Yellow;
                            break;
                    }

                    Console.ForegroundColor = is_current_index ? ConsoleColor.DarkYellow : data_color;
                    Console.Write("[");

                    Console.ForegroundColor = data_color;
                    Console.Write("{0}:{1}", index, value_override != null ? value_override : value);

                    Console.ForegroundColor = is_current_index ? ConsoleColor.DarkYellow : data_color;
                    Console.Write("]");
                }
            }

            c_opcode[] opcodes;
            int current_index;

            public c_computer(int[] input)
            {
                current_index = 0;

                opcodes = input.Select(input_value => new c_opcode(input_value)).ToArray();
            }

            public void set_value(int index, int value)
            {
                opcodes[index].write_value(value);
            }

            public int get_value(int index)
            {
                return opcodes[index].read_value();
            }

            public int compute(bool pretty)
            {
                bool terminate = false;

                while (!terminate)
                {
                    if (pretty)
                    {
                        print();
                    }

                    opcodes.for_each(opcode => opcode.reset_state());

                    switch (opcodes[current_index].read_value())
                    {
                        case k_opcode_add:
                            compute_add(pretty);
                            break;

                        case k_opcode_multiply:
                            compute_multiply(pretty);
                            break;

                        case k_opcode_terminate:
                            compute_terminate(pretty);
                            terminate = true;
                            break;
                    }

                    current_index += 4;
                }

                return opcodes[0].read_value();
            }

            private void print()
            {
                for (int i = 0; i < opcodes.Length; i++)
                {
                    if (i % 4 == 0)
                    {
                        Console.WriteLine();
                    }

                    opcodes[i].print(i, i == current_index);
                    Console.Write(" ");
                }
            }

            private void compute_add(bool pretty)
            {
                int input_a_instruction_index = current_index + 1;
                int input_a_value_index = opcodes[input_a_instruction_index].read_value();
                int input_a_value = opcodes[input_a_value_index].read_value();

                int input_b_instruction_index = current_index + 2;
                int input_b_value_index = opcodes[input_b_instruction_index].read_value();
                int input_b_value = opcodes[input_b_value_index].read_value();

                int output_instruction_index = current_index + 3;
                int output_value_index = opcodes[output_instruction_index].read_value();
                int output_value = input_a_value + input_b_value;
                opcodes[output_value_index].write_value(output_value);

                if (pretty)
                {
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.Write("    ");

                    opcodes[current_index].print(current_index, true, "ADD");

                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(" : ");

                    opcodes[input_a_value_index].print(input_a_value_index);

                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(" + ");

                    opcodes[input_b_value_index].print(input_b_value_index);

                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(" = ");

                    opcodes[output_value_index].print(output_value_index);

                    Console.ResetColor();
                    Console.WriteLine();
                }
            }

            private void compute_multiply(bool pretty)
            {
                int input_a_instruction_index = current_index + 1;
                int input_a_value_index = opcodes[input_a_instruction_index].read_value();
                int input_a_value = opcodes[input_a_value_index].read_value();

                int input_b_instruction_index = current_index + 2;
                int input_b_value_index = opcodes[input_b_instruction_index].read_value();
                int input_b_value = opcodes[input_b_value_index].read_value();

                int output_instruction_index = current_index + 3;
                int output_value_index = opcodes[output_instruction_index].read_value();
                int output_value = input_a_value * input_b_value;
                opcodes[output_value_index].write_value(output_value);

                if (pretty)
                {
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.Write("    ");

                    opcodes[current_index].print(current_index, true, "MUL");

                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(" : ");

                    opcodes[input_a_value_index].print(input_a_value_index);

                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(" + ");

                    opcodes[input_b_value_index].print(input_b_value_index);

                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(" = ");

                    opcodes[output_value_index].print(output_value_index);

                    Console.ResetColor();
                    Console.WriteLine();
                }
            }

            private void compute_terminate(bool pretty)
            {
                if (pretty)
                {
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.Write("    ");

                    opcodes[current_index].print(current_index, true, "TRM");

                    Console.ResetColor();
                    Console.WriteLine();
                }
            }
        }

        internal static int[] parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            return input_reader.read_line().Split(",").Select(i => int.Parse(i)).ToArray();
        }

        public static void part_1(
            string input,
            bool pretty)
        {
            int[] values = parse_input(input, pretty);

            c_computer computer = new c_computer(values);

            computer.set_value(1, 12);
            computer.set_value(2, 2);

            int result = computer.compute(pretty);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", result);
            Console.ResetColor();
        }

        public static void part_2(
            string input,
            bool pretty)
        {
            int[] values = parse_input(input, pretty);

            int noun = 0;
            int verb = 0;
            int result = 0;
            bool success = false;

            for (noun = 0; noun <= 99 && !success; noun++)
            {
                for (verb = 0; verb <= 99 && !success; verb++)
                {
                    c_computer computer = new c_computer(values);
                    computer.set_value(1, noun);
                    computer.set_value(2, verb);

                    int output = computer.compute(pretty);

                    if (output == 19690720)
                    {
                        result = 100 * noun + verb;
                        success = true;
                    }
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = 100 * {0} + {1} = {2}", noun, verb, result);
            Console.ResetColor();
        }
    }
}
