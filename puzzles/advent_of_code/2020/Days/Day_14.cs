using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using advent_of_code_common.input_reader;

namespace advent_of_code_2020.Days
{
    internal class Day_14
    {
        static Regex k_mem_input_regex = new Regex(@"mem\[(\d+)\]");

        public static void Part_1(
            string input,
            bool pretty)
        {
            Dictionary<UInt64, UInt64> memory = new Dictionary<UInt64, UInt64>();

            // Has a '1' for each '1' in the current mask. The rest are '0'.
            UInt64 bitmask_1s = 0;

            // Has a '0' for each '0' in the current mask. The rest are '1'.
            UInt64 bitmask_0s = UInt64.MaxValue;

            c_input_reader input_reader = new c_input_reader(input);

            // Loop through each input line
            while (input_reader.has_more_lines())
            {
                string[] input_line = input_reader.read_line().Split(" = ");

                // Parse a 'mask' line
                if (input_line[0] == "mask")
                {
                    string mask_string = input_line[1];

                    bitmask_1s = 0;
                    bitmask_0s = UInt64.MaxValue;

                    // Read in each char in the mask
                    UInt64 mask_bit = 1;
                    for (int i = mask_string.Length - 1; i >= 0; i--)
                    {
                        char mask_char = mask_string[i];

                        // Update the bitmasks as necessary
                        if (mask_char == '1')
                        {
                            bitmask_1s |= mask_bit;
                        }
                        else if (mask_char == '0')
                        {
                            bitmask_0s &= ~mask_bit;
                        }
                        else if (mask_char != 'X')
                        {
                            throw new Exception("bad mask");
                        }

                        mask_bit <<= 1;
                    }
                }
                // Parse a 'mem' line
                else
                {
                    Match match = k_mem_input_regex.Match(input_line[0]);

                    if (!match.Success)
                    {
                        throw new Exception("bad mem");
                    }

                    UInt64 mem_location = UInt64.Parse(match.Groups[1].Value);
                    UInt64 mem_value = UInt64.Parse(input_line[1]);

                    // Update the value based on the current mask
                    UInt64 masked_value = mem_value | bitmask_1s;
                    masked_value = masked_value & bitmask_0s;

                    // Save the value to memory.
                    memory[mem_location] = masked_value;
                }
            }

            // Sum all values in memory
            UInt64 sum_of_all_values = memory.Values.Aggregate((x, y) => x + y);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", sum_of_all_values);
            Console.ResetColor();
        }

        public static void Part_2(
            string input,
            bool pretty)
        {
            Dictionary<UInt64, UInt64> memory = new Dictionary<UInt64, UInt64>();

            // Has a '1' for each '1' in the current mask. The rest are '0'.
            UInt64 bitmask_1s = 0;

            // Has a '0' for each '0' in the current mask. The rest are '1'.
            UInt64 bitmask_0s = UInt64.MaxValue;

            // Has a '1' for each 'X' in the current mask. The rest are '0'.
            UInt64 bitmask_xs = 0;

            // Stores the value of each bit that was 'X' in the current mask.
            // Ex: 001XX0X => {1, 4, 8}, since bits 1, 4, and 8 are 'X' in the mask.
            UInt64[] x_bits = new UInt64[0];

            c_input_reader input_reader = new c_input_reader(input);

            // Loop through each input line
            while (input_reader.has_more_lines())
            {
                string[] input_line = input_reader.read_line().Split(" = ");

                // Parse a 'mask' line
                if (input_line[0] == "mask")
                {
                    string mask_string = input_line[1];

                    bitmask_1s = 0;
                    bitmask_0s = UInt64.MaxValue;
                    bitmask_xs = 0;
                    List<UInt64> x_bits_list = new List<UInt64>();

                    // Read in each char in the mask
                    UInt64 mask_bit = 1;
                    for (int i = mask_string.Length - 1; i >= 0; i--)
                    {
                        char mask_char = mask_string[i];

                        // Update the bitmasks as necessary
                        if (mask_char == '1')
                        {
                            bitmask_1s |= mask_bit;
                        }
                        else if (mask_char == '0')
                        {
                            bitmask_0s &= ~mask_bit;
                        }
                        else if (mask_char == 'X')
                        {
                            bitmask_xs |= mask_bit;
                            x_bits_list.Add(mask_bit);
                        }
                        else
                        {
                            throw new Exception("bad mask");
                        }

                        mask_bit <<= 1;
                    }

                    x_bits = x_bits_list.ToArray();
                }
                // Parse a 'mem' line
                else
                {
                    Match match = k_mem_input_regex.Match(input_line[0]);

                    if (!match.Success)
                    {
                        throw new Exception("bad mem");
                    }

                    // Get the current mem location and apply the current masks. Clear out bits where the mask was 'X' as well.
                    UInt64 mem_location = UInt64.Parse(match.Groups[1].Value);
                    UInt64 masked_location = mem_location | bitmask_1s;
                    // masked_location = masked_location & bitmask_0s; Instructions say to ignore '0's.
                    masked_location = masked_location & ~bitmask_xs;

                    UInt64 mem_value = UInt64.Parse(input_line[1]);

                    // Loop through all 2^(x_bits.Length) permuations of the 'X' bits.
                    int mask_permutations = 1 << x_bits.Length;
                    for (int mask_permutation = 0; mask_permutation < mask_permutations; mask_permutation++)
                    {
                        // Initialize the final memory address to the masked address with 'X' bits stripped out.
                        UInt64 final_location = masked_location;

                        // Loop through our x_bits and set any into final_destination based on the current permutation
                        // Ex:
                        //      x_bits = {4, 8, 32}
                        //      mask_permutation = 5 = 0b101
                        //  So mask_permutation has the first and third bits set,
                        //  So set the first and third values in x_bits into final_location
                        //  So Insert 4 and 32 into final_location.
                        for (int bit_index = 0; bit_index < x_bits.Length; bit_index++)
                        {
                            if (0 != (mask_permutation & (1 << bit_index)))
                            {
                                final_location |= x_bits[bit_index];
                            }
                        }

                        // Save the value to memory.
                        memory[final_location] = mem_value;
                    }
                }
            }

            // Sum all values in memory
            UInt64 sum_of_all_values = memory.Values.Aggregate((x, y) => x + y);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", sum_of_all_values);
            Console.ResetColor();
        }
    }
}
