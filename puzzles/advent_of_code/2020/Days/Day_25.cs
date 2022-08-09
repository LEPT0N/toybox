using System;
using System.Collections.Generic;
using advent_of_code_common.input_reader;

namespace advent_of_code_2020.Days
{
    internal class Day_25
    {
        internal static (UInt64, UInt64) parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            UInt64 first = UInt64.Parse(input_reader.read_line());
            UInt64 second = UInt64.Parse(input_reader.read_line());

            return (first, second);
        }

        internal static readonly UInt64 k_bound = 20201227;

        internal static (UInt64, UInt64)[] find_loop_size(UInt64 public_key)
        {
            List<(UInt64, UInt64)> results = new List<(UInt64, UInt64)>();

            for (UInt64 subject_number = 1; subject_number < k_bound; subject_number++)
            {
                UInt64 value = 1;
                UInt64 loop_size;

                for (loop_size = 0; value != public_key && loop_size < k_bound; loop_size++)
                {
                    UInt64 new_value = value * subject_number;
                    value = new_value % k_bound;
                }

                if (value == public_key)
                {
                    results.Add((subject_number, loop_size));
                }
            }

            return results.ToArray();
        }

        internal static UInt64 calculate_encryption_key(UInt64 subject_number, UInt64 loop_size)
        {
            UInt64 value = 1;

            for (UInt64 i = 0; i < loop_size; i++)
            {
                UInt64 new_value = value * subject_number;
                value = new_value % k_bound;
            }

            return value;
        }

        public static void Part_1(
            string input,
            bool pretty)
        {
            (UInt64 first_public_key, UInt64 second_public_key) = parse_input(input, pretty);

            bool success = false;
            UInt64 first_loop_size = 0;
            UInt64 second_loop_size = 0;
            UInt64 subject_number;

            // Loop through every possible subject number
            for (subject_number = 2; !success && subject_number < k_bound; subject_number++)
            {
                UInt64 value = 1;

                // For a given subject number, find a loop size that will generate the first public key
                for (first_loop_size = 0; value != first_public_key && first_loop_size < k_bound; first_loop_size++)
                {
                    UInt64 new_value = value * subject_number;
                    value = new_value % k_bound;
                }

                // If we find a (subject number, loop size) that generates the first public key,
                // Then see if that subject number can work for the second public key, too.
                if (value == first_public_key)
                {
                    value = 1;

                    // For the subject number, find a loop size that will generate the second public key
                    for (second_loop_size = 0; value != second_public_key && second_loop_size < k_bound; second_loop_size++)
                    {
                        UInt64 new_value = value * subject_number;
                        value = new_value % k_bound;
                    }

                    // If a given subject number has loop sizes that generate both public keys, then we are done.
                    if (value == second_public_key)
                    {
                        success = true;
                        break;
                    }
                }
            }

            if (success)
            {
                // These should be the same.
                UInt64 first_encryption_key = calculate_encryption_key(second_public_key, first_loop_size);
                UInt64 second_encryption_key = calculate_encryption_key(first_public_key, second_loop_size);

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"Subject Number = {subject_number}");
                Console.WriteLine("First");
                Console.WriteLine($"    Public Key = {first_public_key}");
                Console.WriteLine($"    Loop Size = {first_loop_size}");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"    Encryption Key = {first_encryption_key}");

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Second");
                Console.WriteLine($"    Public Key = {second_public_key}");
                Console.WriteLine($"    Loop Size = {second_loop_size}");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"    Encryption Key = {second_encryption_key}");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine();
                Console.WriteLine("Failure");
                Console.ResetColor();
            }
        }

        public static void Part_2(
            string input,
            bool pretty)
        {
            // parse_input(input, pretty);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", 0);
            Console.ResetColor();
        }
    }
}
