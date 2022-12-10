using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.input_reader;

namespace advent_of_code_2022.days
{
    internal class day_10
    {
        [DebuggerDisplay("x = {x}", Type = "c_registers")]
        internal static class c_registers
        {
            public static int x = 1;
        }

        internal interface i_command
        {
            public void execute();
            public void print();
        }

        [DebuggerDisplay("noop", Type = "c_command_noop")]
        internal class c_command_noop : i_command
        {
            private readonly bool hidden;

            public c_command_noop(bool h)
            {
                hidden = h;
            }

            public void execute() { }

            public void print()
            {
                if (hidden)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                }

                Console.WriteLine($"noop");
                Console.ResetColor();
            }
        }

        [DebuggerDisplay("addx = {argument}", Type = "c_command_addx")]
        internal class c_command_addx : i_command
        {
            private readonly int argument;

            public c_command_addx(int a)
            {
                argument = a;
            }

            public void execute()
            {
                c_registers.x += argument;
            }

            public void print()
            {
                Console.WriteLine($"addx {argument}\t\tx = {c_registers.x}");
            }
        }

        internal static i_command[] parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            List<i_command> commands = new List<i_command>();

            // Command '0' since the question says the first command is command '1' and arrays start at zero.
            commands.Add(new c_command_noop(true));

            while (input_reader.has_more_lines())
            {
                string[] input_line = input_reader.read_line().Split(' ');

                switch (input_line[0])
                {
                    case "addx":
                        commands.Add(new c_command_noop(true));
                        commands.Add(new c_command_addx(int.Parse(input_line[1])));
                        break;

                    case "noop":
                        commands.Add(new c_command_noop(false));
                        break;

                    default:
                        throw new ArgumentException($"Invalid command {input_line[0]}");
                }
            }

            return commands.ToArray();
        }

        public static void part_1(
            string input,
            bool pretty)
        {
            i_command[] commands = parse_input(input, pretty);

            List<int> signal_strengths = new List<int>();
            int next_signal_strength_index = 20;

            for (int i = 0; i < commands.Length; i++)
            {
                if (i == next_signal_strength_index)
                {
                    int signal_strength = i * c_registers.x;

                    signal_strengths.Add(signal_strength);

                    if (pretty)
                    {
                        Console.WriteLine($"\t\t --> signal strength = {signal_strength}");
                    }

                    next_signal_strength_index += 40;
                }
                commands[i].execute();

                if (pretty)
                {
                    Console.Write($"[{i}]\t");

                    commands[i].print();
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", signal_strengths.Sum());
            Console.ResetColor();
        }

        public static void part_2(
            string input,
            bool pretty)
        {
            i_command[] commands = parse_input(input, pretty);

            List<string> crt_grid = new List<string>();
            int current_crt_column = 0;
            string current_crt_row = "";

            for (int current_cycle = 1; current_cycle < commands.Length; current_cycle++)
            {
                if (Math.Abs(current_crt_column - c_registers.x) <= 1)
                {
                    current_crt_row += 'O';
                }
                else
                {
                    current_crt_row += ' ';
                }

                commands[current_cycle].execute();

                current_crt_column++;

                if (current_crt_column == 40)
                {
                    crt_grid.Add(current_crt_row);
                    current_crt_column = 0;
                    current_crt_row = "";
                }
            }

            foreach (string row in crt_grid)
            {
                Console.WriteLine(row);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", 0);
            Console.ResetColor();
        }
    }
}
