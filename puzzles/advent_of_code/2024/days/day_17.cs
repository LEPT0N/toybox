using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Transactions;
using advent_of_code_common.input_reader;
using advent_of_code_common.extensions;

namespace advent_of_code_2024.days
{
    internal class day_17
    {
        [DebuggerDisplay("IP = {IP} A={A} B={B} C={C}", Type = "c_registers")]
        internal class c_registers
        {
            public int IP = 0;

            public Int64 A = 0;
            public Int64 B = 0;
            public Int64 C = 0;

            public List<Int64> output = new List<Int64>();

            public c_registers make_copy()
            {
                c_registers copy = new c_registers();
                copy.IP = this.IP;
                copy.A = this.A;
                copy.B = this.B;
                copy.C = this.C;

                // output is new

                return copy;
            }
        }

        internal abstract class i_instruction
        {
            public readonly int operand;

            protected i_instruction(int o)
            {
                operand = o;
            }

            protected string combo
            {
                get
                {
                    switch (operand)
                    {
                        case 0: return "0";
                        case 1: return "1";
                        case 2: return "2";
                        case 3: return "3";

                        case 4: return "A";
                        case 5: return "B";
                        case 6: return "C";

                        default: throw new Exception($"Invalid combo value {operand}");
                    }
                }
            }

            protected Int64 get_combo_value(c_registers registers)
            {
                switch (operand)
                {
                    case 0: return 0;
                    case 1: return 1;
                    case 2: return 2;
                    case 3: return 3;

                    case 4: return registers.A;
                    case 5: return registers.B;
                    case 6: return registers.C;

                    default: throw new Exception($"Invalid combo value {operand}");
                }
            }

            public abstract bool execute(c_registers registers);
        }

        [DebuggerDisplay("adv {combo,nq} => [ A = A / (1 << {combo,nq}) ]", Type = "c_adv_instruction")]
        internal class c_adv_instruction : i_instruction
        {
            public c_adv_instruction(int o) : base(o) { }

            public override bool execute(c_registers registers)
            {
                registers.A = registers.A / (1L << (int)get_combo_value(registers));

                return true;
            }
        }

        [DebuggerDisplay("bxl {operand} => [ B = B xor {operand} ]", Type = "c_bxl_instruction")]
        internal class c_bxl_instruction : i_instruction
        {
            public c_bxl_instruction(int o) : base(o) { }

            public override bool execute(c_registers registers)
            {
                registers.B = registers.B ^ operand;

                return true;
            }
        }

        [DebuggerDisplay("bst {combo,nq} => [ B = {combo,nq} mod 8 ]", Type = "c_bst_instruction")]
        internal class c_bst_instruction : i_instruction
        {
            public c_bst_instruction(int o) : base(o) { }

            public override bool execute(c_registers registers)
            {
                registers.B = get_combo_value(registers) % 8;

                return true;
            }
        }

        [DebuggerDisplay("jnz {operand} = [ IP = (A != 0) ? {operand} : IP + 2 ]", Type = "c_jnz_instruction")]
        internal class c_jnz_instruction : i_instruction
        {
            public c_jnz_instruction(int o) : base(o) { }

            public override bool execute(c_registers registers)
            {
                if (registers.A == 0)
                {
                    return true;
                }
                else
                {
                    registers.IP = operand;

                    return false;
                }
            }
        }

        [DebuggerDisplay("bxc => [ B = B xor C ]", Type = "c_bxc_instruction")]
        internal class c_bxc_instruction : i_instruction
        {
            public c_bxc_instruction(int o) : base(o) { }

            public override bool execute(c_registers registers)
            {
                registers.B = registers.B ^ registers.C;

                return true;
            }
        }

        [DebuggerDisplay("out {combo,nq} => [ outputs {combo,nq} mod 8 ]", Type = "c_out_instruction")]
        internal class c_out_instruction : i_instruction
        {
            public c_out_instruction(int o) : base(o) { }

            public override bool execute(c_registers registers)
            {
                registers.output.Add(get_combo_value(registers) % 8);

                return true;
            }
        }

        [DebuggerDisplay("bdv {combo,nq} => [ B = A / (1 << {combo,nq}) ]", Type = "c_bdv_instruction")]
        internal class c_bdv_instruction : i_instruction
        {
            public c_bdv_instruction(int o) : base(o) { }

            public override bool execute(c_registers registers)
            {
                registers.B = registers.A / (1L << (int)get_combo_value(registers));

                return true;
            }
        }

        [DebuggerDisplay("cdv {combo,nq} => [ C = A / (1 << {combo,nq}) ]", Type = "c_cdv_instruction")]
        internal class c_cdv_instruction : i_instruction
        {
            public c_cdv_instruction(int o) : base(o) { }

            public override bool execute(c_registers registers)
            {
                registers.C = registers.A / (1L << (int)get_combo_value(registers));

                return true;
            }
        }

        internal static (c_registers, i_instruction[], string) parse_input(
            in c_input_reader input_reader,
            in bool pretty)
        {
            c_registers registers = new c_registers();
            registers.A = int.Parse(input_reader.read_line().Substring("Register A: ".Length));
            registers.B = int.Parse(input_reader.read_line().Substring("Register B: ".Length));
            registers.C = int.Parse(input_reader.read_line().Substring("Register C: ".Length));

            input_reader.read_line();

            List<i_instruction> instructions = new List<i_instruction>();

            string input_string = input_reader
                .read_line()
                .Substring("Program: ".Length);

            int[] input = input_string
                .Split(',')
                .Select(s => int.Parse(s))
                .ToArray();

            for (int i = 0; i <input.Length; i += 2)
            {
                int opcode = input[i];
                int operand = input[i + 1];

                i_instruction instruction;

                switch (opcode)
                {
                    case 0: instruction = new c_adv_instruction(operand); break;
                    case 1: instruction = new c_bxl_instruction(operand); break;
                    case 2: instruction = new c_bst_instruction(operand); break;
                    case 3: instruction = new c_jnz_instruction(operand); break;
                    case 4: instruction = new c_bxc_instruction(operand); break;
                    case 5: instruction = new c_out_instruction(operand); break;
                    case 6: instruction = new c_bdv_instruction(operand); break;
                    case 7: instruction = new c_cdv_instruction(operand); break;

                    default: throw new Exception("Invalid opcode {opcode}");
                }

                instructions.Add(instruction);
                instructions.Add(null);
            }

            return (registers, instructions.ToArray(), input_string);
        }

        public static void part_1(
            c_input_reader input_reader,
            bool pretty)
        {
            (c_registers registers, i_instruction[] instructions, string input) = parse_input(input_reader, pretty);
            
            // Just execute the program and display the output.
            while (registers.IP < instructions.Length)
            {
                if (instructions[registers.IP].execute(registers))
                {
                    registers.IP += 2;
                }
            }

            string result = string.Join(',', registers.output);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }

        // Display a register as an array of 3-bit values.
        public static string output_register(Int64 register)
        {
            string output = "";

            while (register != 0)
            {
                Int64 entry = register % 8;
                register = register / 8;

                output = entry + output;
            }

            return output;
        }

        public static void part_2(
            c_input_reader input_reader,
            bool pretty)
        {
            (c_registers initial_registers, i_instruction[] instructions, string input) = parse_input(input_reader, pretty);

            // Register 'A' is interpreted as an array of 3-bit values.
            // Each loop of the program pops the 3 lower bits off of A.
            // The desired output is 16 values, so A needs to be 48 bits.
            Int64 register_A_base = 0x100000000000;

            // Keep track of interesting subsets of initial_register_A that generate long matches between input and output.
            // I somewhat arbitrarily chose buckets of FFF because that's 12 bits, which maps to 4 entries in the 'A' array of 3-bit values.
            HashSet<Int64> interesting_bits_1 = new HashSet<long>();
            Int64 interesting_bit_mask_1 = 0xFFF;

            HashSet<Int64> interesting_bits_2 = new HashSet<long>();
            Int64 interesting_bit_mask_2 = 0xFFF000;

            HashSet<Int64> interesting_bits_3 = new HashSet<long>();
            Int64 interesting_bit_mask_3 = 0xFFF000000;

            HashSet<Int64> interesting_bits_4 = new HashSet<long>();
            Int64 interesting_bit_mask_4 = 0xFFF000000000;


            // The code I got is basically:
            // 
            //     while (a != 0)
            //     {
            //         b = a % 8
            //         b = b ^ 3
            //         c = a / (1 << b)
            //         b = b ^ 5
            //         a = a / 8
            //         b = b ^ c
            //         output b
            //     }
            // 
            // Which means we're treating 'A' like an array of 3-bit values and operating on them from lowest bit to highest.
            // However the part that writes to 'C' is also looking forwards in the 'A' array and doing math in it, so I'm not sure
            // how to figure this out at first glance.
            // 
            // So instead, I'm going to run for awhile until the bottom bits of 'A' stabilize on a (hopefully) small set of possibilities.
            // Once I have that, I can set those values in stone as the only values worth searching for in that 0xXXX band of bits, and only start
            // searching through the next set of 3 bits.
            // 
            // So, run these one at a time until interesting_bits_N seems to stabilize. Collect all the combinations of stabilized interesting bits to generate the next set.

            // -------

            // No known interesting bits, just start at our base and increment by 1 each time.

            //Int64[] register_A_set = { 0 };
            //Int64 register_A_increment = 0x1;
            //Int64 common_prefix_length = 10;

            // -------

            // We have our set of interesting 0xXXX bits, so set those in stone and only start looking for higher bits that give us a better match.

            //Int64[] register_A_set =
            //{
            //    0xABF,
            //    0xE6D,
            //    0xAA0,
            //    0xEBF,
            //};
            //Int64 register_A_increment = 0x1000;
            //Int64 common_prefix_length = 18;

            // -------

            // We have our set of interesting 0xXXXXXX bits, so set those in stone and only start looking for higher bits that give us a better match.

            //Int64[] register_A_set =
            //{
            //    0x515e6d,
            //    0x515ebf,
            //    0x535e6d,
            //    0x535ebf,
            //    0xd15e6d,
            //    0xd15ebf,
            //    0xd35e6d,
            //    0xd35ebf,
            //    0xd25e6d,
            //    0xd25ebf,
            //    0xa94e6d,
            //    0xa94ebf,
            //    0x294e6d,
            //    0x294ebf,
            //};
            //Int64 register_A_increment = 0x1000000;
            //Int64 common_prefix_length = 22;

            // -------

            // We have our set of interesting 0xXXXXXXXXX bits, so set those in stone and only start looking for higher bits that give us a better match.

            // This finds our solution fairly quickly!

            Int64[] register_A_set =
            {
                0x3b0a94e6d,
                0x3b0a94ebf,
                0x370a94e6d,
                0x370a94ebf,
                0xb70a94e6d,
                0xb70a94ebf,
                0x330a94e6d,
                0x330a94ebf,
                0xb30a94e6d,
                0xb30a94ebf,
            };
            Int64 register_A_increment = 0x1000000000;
            Int64 common_prefix_length = 26;

            // -------

            bool found = false;
            string output = "";

            // Set register A to our first value worth checking.
            Int64 register_A_set_index = 0;
            Int64 initial_register_A = register_A_base + register_A_set[register_A_set_index];

            // Loop until we find out solution.
            while (!found)
            {
                // Get a fresh copy of our initial register state.
                c_registers registers = initial_registers.make_copy();
                registers.A = initial_register_A;

                // Execute the program.
                while (registers.IP < instructions.Length)
                {
                    if (instructions[registers.IP].execute(registers))
                    {
                        registers.IP += 2;
                    }
                }

                // Get the program's output.
                output = string.Join(',', registers.output);

                // See how common the output is to the input.
                string common_prefix = input.common_prefix(output);

                if (common_prefix.Length >= common_prefix_length)
                {
                    // If it's common enough, make note of what unique sets of three bytes showed up.

                    interesting_bits_1.Add(initial_register_A & interesting_bit_mask_1);
                    interesting_bits_2.Add(initial_register_A & interesting_bit_mask_2);
                    interesting_bits_3.Add(initial_register_A & interesting_bit_mask_3);
                    interesting_bits_4.Add(initial_register_A & interesting_bit_mask_4);

                    if (pretty)
                    {
                        Console.WriteLine($"{input} [A = {initial_register_A} = {initial_register_A:X} = {output_register(initial_register_A)} (interesting set = {interesting_bits_4.Count}.{interesting_bits_3.Count}.{interesting_bits_2.Count}.{interesting_bits_1.Count})] = {common_prefix}");
                    }
                }

                if (input == output)
                {
                    // Success!

                    found = true;
                }
                else
                {
                    // move to the next interesting number in our set.
                    register_A_set_index++;

                    // If we checked everything in the set, increment the next highest bit and go back to the beginning of our set.
                    if (register_A_set_index >= register_A_set.Length)
                    {
                        register_A_set_index = 0;
                        register_A_base += register_A_increment;
                    }

                    // Combine to the next input to check.
                    initial_register_A = register_A_base + register_A_set[register_A_set_index];
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"{input} [A = {initial_register_A}] = {output}");
            Console.ResetColor();
        }
    }
}
