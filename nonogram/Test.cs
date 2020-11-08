using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Nonogram
{
    public static class Test
    {
        // x!
        private static long Factorial(this int x)
        {
            Debug.Assert(x >= 0);

            long total = 1;

            for (long i = 2; i <= x; i++)
            {
                total *= i;
            }

            return total;
        }

        // 10^x
        // Math.Pow but I don't want to convert to/from floating point numbers, and I only care about 10^x.
        private static long Ten_To_The(int x)
        {
            Debug.Assert(x >= 0);

            long result = 1;

            for (int i = 0; i < x; i++)
            {
                result *= 10;
            }

            return result;
        }

        // [1, 9, 8, 5] = 1985
        // Silly way for me to make it easy to both visualize a list and verify that an array of arrays is sorted.
        // elements must be in [0, 9]
        // No, I don't want to hear how this is stupid.
        private static long List_As_Int(this int[] list)
        {
            int last = list.Length - 1;
            long result = 0;

            for (int i = last; i >= 0; i--)
            {
                Debug.Assert(list[i] >= 0 && list[i] <= 9);

                result += list[i] * Ten_To_The(last - i);
            }

            return result;
        }

        // Run through Get_Next_Permutation until exhausted, and verify the results.
        private static long Validate_Get_Next_Permutation_For_List(int[] list)
        {
            // Keep track of all permutations we find.
            List<long> permutations_as_numbers = new List<long>();

            // Compute all permutations
            permutations_as_numbers.Add(list.List_As_Int());
            while (list.Get_Next_Permutation())
            {
                permutations_as_numbers.Add(list.List_As_Int());
            }

            // Print all permutations
            Console.Out.WriteLine("");
            foreach (long permutation in permutations_as_numbers)
            {
                Console.Out.WriteLine(permutation);
            }

            // Verify that the permutations returned were 'sorted'.
            for (int i = 0; i < permutations_as_numbers.Count - 1; i++)
            {
                Debug.Assert(permutations_as_numbers[i] < permutations_as_numbers[i + 1]);
            }

            // We also need to verify that the set of elements in each permutation are equal to the set of elements in the starting list,
            // but since the only operations on the list are Swap() (and I'm lazy), 'no, I don't think I will.'

            return permutations_as_numbers.Count;
        }

        // Basic test of Utilities.Get_Next_Permutation
        public static void Validate_Get_Next_Permutation()
        {
            Console.Out.WriteLine("");
            Console.Out.WriteLine("Test lists of size 0 to lists of size 8 with unique elements.");

            for (int list_length = 0; list_length <= 8; list_length++)
            {
                // Create a list of length 'list_length' with non-zero single-digit elements in ascending order.
                int[] list = new int[list_length];
                for (int i = 0; i < list_length; i++)
                {
                    list[i] = i + 1;
                }

                // We expect to find 'list_length!' permutations
                long permutation_count = Validate_Get_Next_Permutation_For_List(list);
                long expected_permutation_count = list_length.Factorial();
                Debug.Assert(permutation_count == expected_permutation_count);
            }

            Console.Out.WriteLine("");
            Console.Out.WriteLine("Test lists but with only '1's and '2's as elements.");

            for (int list_length = 2; list_length <= 15; list_length++)
            {
                for (int count_of_1s = 0; count_of_1s <= list_length; count_of_1s++)
                {
                    // Build a list of length 'list_length' containing 'count_of_1s' elements that are '1' and the rest '2'.
                    int[] list = new int[list_length];
                    for (int i = 0; i < list_length; i++)
                    {
                        list[i] = i < count_of_1s ? 1 : 2;
                    }

                    // We expect to find 'list_length! / (count_of_1s! * count_of_2s!)' permutations
                    long permutation_count = Validate_Get_Next_Permutation_For_List(list);
                    long expected_permutation_count = list_length.Factorial() / (count_of_1s.Factorial() * (list_length - count_of_1s).Factorial());
                    Debug.Assert(permutation_count == expected_permutation_count);
                }
            }
            
            Console.WriteLine("LGTM");
        }
    }
}
