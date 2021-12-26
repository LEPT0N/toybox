using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace advent_of_code_2020.Days
{
    internal class Day_08
    {
        internal enum e_instruction_type
        {
            nop,
            acc,
            jmp,
        }

        [DebuggerDisplay("t = {type} a = {argument} v = {visited}", Type = "c_instruction")]
        internal class c_instruction
        {
            public e_instruction_type type { get; set; }
            public bool visited { get; set; }
            public int argument { get; set; }

            public c_instruction(c_instruction other)
            {
                this.type = other.type;
                this.visited = other.visited;
                this.argument = other.argument;
            }

            public c_instruction(string input)
            {
                visited = false;

                string[] split_input = input.Split(' ');

                {
                    string type_string = split_input[0];

                    switch (type_string)
                    {
                        case "nop":
                            type = e_instruction_type.nop;
                            break;

                        case "acc":
                            type = e_instruction_type.acc;
                            break;

                        case "jmp":
                            type = e_instruction_type.jmp;
                            break;

                        default:
                            throw new Exception(string.Format("unrecognized instruction type '{0}'", type_string));
                    }
                }

                {
                    string argument_string = split_input[1];

                    if (argument_string[0] == '+')
                    {
                        argument_string = argument_string.Substring(1);
                    }

                    argument = int.Parse(argument_string);
                }
            }

            public c_instruction swap_type()
            {
                c_instruction result = new c_instruction(this);

                switch (this.type)
                {
                    case e_instruction_type.nop:
                        result.type = e_instruction_type.jmp;
                        break;

                    case e_instruction_type.jmp:
                        result.type = e_instruction_type.nop;
                        break;

                    default:
                        throw new Exception("Illecal swap");
                }

                return result;
            }

            public int get_accumulator_increment()
            {
                switch (type)
                {
                    case e_instruction_type.nop:
                    case e_instruction_type.jmp:
                        return 0;

                    case e_instruction_type.acc:
                        return argument;

                    default:
                        throw new Exception(string.Format("unrecognized instruction type '{0}'", type));
                }
            }

            public int get_next_instruction_offset()
            {
                switch (type)
                {
                    case e_instruction_type.nop:
                    case e_instruction_type.acc:
                        return 1;

                    case e_instruction_type.jmp:
                        return argument;

                    default:
                        throw new Exception(string.Format("unrecognized instruction type '{0}'", type));
                }
            }

            public void print()
            {
                switch (type)
                {
                    case e_instruction_type.nop:
                        Console.Write("nop ");
                        break;

                    case e_instruction_type.jmp:
                        Console.Write("jmp ");
                        break;


                    case e_instruction_type.acc:
                        Console.Write("acc ");
                        break;


                    default:
                        throw new Exception(string.Format("unrecognized instruction type '{0}'", type));
                }

                if (argument >= 0)
                {
                    Console.Write("+");
                }

                Console.WriteLine(argument);
            }
        }

        [DebuggerDisplay("ip = {instruction_pointer} acc = {accumulator}", Type = "c_evaluator")]
        internal class c_evaluator
        {
            private c_instruction[] instructions;
            private int instruction_pointer;
            public int accumulator { get; private set; }

            private int swapped_instruction_index;
            private c_instruction swapped_instruction;

            public c_evaluator(List<c_instruction> instruction_list)
            {
                instructions = instruction_list.ToArray();
                instruction_pointer = 0;
                accumulator = 0;
            }

            public int instruction_count()
            {
                return instructions.Length;
            }

            private bool current_instruction_visited()
            {
                return instructions[instruction_pointer].visited;
            }

            private void execute_single_instruction()
            {
                c_instruction current_instruction = instructions[instruction_pointer];
                current_instruction.visited = true;
                accumulator += current_instruction.get_accumulator_increment();
                instruction_pointer += current_instruction.get_next_instruction_offset();
            }

            public bool run_to_completion()
            {
                foreach(c_instruction instruction in instructions)
                {
                    instruction.visited = false;
                }

                if (swapped_instruction != null)
                {
                    swapped_instruction.visited = false;
                }

                accumulator = 0;
                instruction_pointer = 0;

                while (instruction_pointer < instructions.Length && !current_instruction_visited())
                {
                    execute_single_instruction();
                }

                return instruction_pointer == instructions.Length;
            }

            public e_instruction_type get_instruction_type(int instruction_index)
            {
                return instructions[instruction_index].type;
            }

            public void swap(int instruction_index_to_swap)
            {
                if (swapped_instruction != null)
                {
                    instructions[swapped_instruction_index] = swapped_instruction;
                }

                swapped_instruction_index = instruction_index_to_swap;
                swapped_instruction = instructions[swapped_instruction_index];

                instructions[swapped_instruction_index] = swapped_instruction.swap_type();
            }

            public void print(string title)
            {
                Console.WriteLine();
                Console.WriteLine(title);
                for (int i = 0; i < instructions.Length; i++)
                {
                    bool is_current = (i == this.instruction_pointer);
                    bool is_swapped = (swapped_instruction != null && i == this.swapped_instruction_index);
                    switch ((is_current, is_swapped))
                    {
                        case (true, true):
                            Console.ForegroundColor = ConsoleColor.DarkMagenta;
                            break;

                        case (true, false):
                            Console.ForegroundColor = ConsoleColor.Blue;
                            break;

                        case (false, true):
                            Console.ForegroundColor = ConsoleColor.Red;
                            break;

                        default:
                            Console.ForegroundColor = (instructions[i].visited ? ConsoleColor.White : ConsoleColor.DarkGray);
                            break;
                    }

                    instructions[i].print();
                }

                Console.ResetColor();
            }
        }

        internal static c_evaluator parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            List<c_instruction> instructions = new List<c_instruction>();

            while (input_reader.has_more_lines())
            {
                c_instruction instruction = new c_instruction(input_reader.read_line());

                instructions.Add(instruction);
            }

            c_evaluator evaluator = new c_evaluator(instructions);

            return evaluator;
        }

        public static void Part_1(
            string input,
            bool pretty)
        {
            c_evaluator evaluator = parse_input(input, pretty);

            evaluator.run_to_completion();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", evaluator.accumulator);
            Console.ResetColor();
        }

        public static void Part_2(
            string input,
            bool pretty)
        {
            c_evaluator evaluator = parse_input(input, pretty);
            if (pretty)
            {
                evaluator.print("input:");
            }

            for (int swapped_instruction_index = 0; swapped_instruction_index < evaluator.instruction_count(); swapped_instruction_index++)
            {
                if (evaluator.get_instruction_type(swapped_instruction_index) != e_instruction_type.acc)
                {
                    evaluator.swap(swapped_instruction_index);

                    if (pretty)
                    {
                        evaluator.print(String.Format("Swapped {0}", swapped_instruction_index));
                    }

                    if (evaluator.run_to_completion())
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Result = {0}", evaluator.accumulator);
                        Console.ResetColor();
                    }
                    else if (pretty)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Result = {0}", evaluator.accumulator);
                        Console.ResetColor();
                    }
                }
            }
        }
    }
}
