using advent_of_code_common.int_math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace advent_of_code_common.extensions
{
    public static class extensions
    {
        public static bool test_bit(this int bit_field, int index)
        {
            return (0 != (bit_field & (1 << index)));
        }

        public static bool test_bit(this uint bit_field, int index)
        {
            return (0U != (bit_field & (1U << index)));
        }

        public static void set_bit(ref this int bit_field, int index)
        {
            bit_field |= (1 << index);
        }

        public static void set_bit(ref this uint bit_field, int index)
        {
            bit_field |= (1U << index);
        }

        public static T[,] flip<T>(this T[,] grid)
        {
            T[,] new_grid = new T[grid.GetLength(0), grid.GetLength(1)];

            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    new_grid[i, j] = grid[grid.GetLength(0) - 1 - i, j];
                }
            }

            return new_grid;
        }

        public static T[][] flip<T>(this T[][] grid)
        {
            T[][] new_grid = new T[grid.Length][];

            for (int i = 0; i < grid.Length; i++)
            {
                new_grid[i] = new T[grid[i].Length];

                for (int j = 0; j < grid[i].Length; j++)
                {
                    new_grid[i][j] = grid[grid.Length - 1 - i][j];
                }
            }

            return new_grid;
        }

        public static T[,] rotate<T>(this T[,] grid)
        {
            T[,] new_grid = new T[grid.GetLength(1), grid.GetLength(0)];

            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    new_grid[j, i] = grid[grid.GetLength(0) - 1 - i, j];
                }
            }

            return new_grid;
        }

        public static T[][] rotate<T>(this T[][] grid)
        {
            T[][] new_grid = new T[grid[0].Length][];

            for (int i = 0; i < grid[0].Length; i++)
            {
                new_grid[i] = new T[grid.Length];
            }

            for (int i = 0; i < grid.Length; i++)
            {
                for (int j = 0; j < grid[i].Length; j++)
                {
                    new_grid[j][i] = grid[grid.Length - 1 - i][j];
                }
            }

            return new_grid;
        }

        public static void for_each<T>(this T[] array, Action<T> action)
        {
            for (int index = 0; index < array.Length; index++)
            {
                action(array[index]);
            }
        }

        public static void for_each<T>(this IEnumerable<T> array, Action<T> action)
        {
            foreach (T element in array)
            {
                action(element);
            }
        }

        public static void for_each<T>(this T[][] array, Action<T> action)
        {
            for (int i = 0; i < array.Length; i++)
            {
                for (int j = 0; j < array[i].Length; j++)
                {
                    action(array[i][j]);
                }
            }
        }

        public static void for_each<T>(this T[,] array, Action<T> action)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    action(array[i, j]);
                }
            }
        }

        public static void fill<T>(this T[] array, T value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = value;
            }
        }

        public static void fill<T>(this T[][] array, T value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                for (int j = 0; j < array[i].Length; j++)
                {
                    array[i][j] = value;
                }
            }
        }

        public static void fill<T>(this T[,] array, T value)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    array[i, j] = value;
                }
            }
        }

        public static int count<T>(this T[,] array, Func<T, bool> predicate)
        {
            int sum = 0;

            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    if (predicate(array[i, j]))
                    {
                        sum++;
                    }
                }
            }

            return sum;
        }

        public static T[,] to_2d_array<T>(this List<List<T>> array)
        {
            if (array.Count == 0 || array[0].Count == 0)
            {
                return new T[0, 0];
            }

            T[,] result = new T[array.Count, array[0].Count];

            for (int i = 0; i < array.Count; i++)
            {
                for (int j = 0; j < array[0].Count; j++)
                {
                    result[i, j] = array[i][j];
                }
            }

            return result;
        }

        public static Int64 sum(this Int64[] array)
        {
            return array.Aggregate((a, b) => a + b);
        }

        public static Int64 sum(this IEnumerable<Int64> array)
        {
            return array.Aggregate((a, b) => a + b);
        }

        public static UInt64 sum(this UInt64[] array)
        {
            return array.Aggregate((a, b) => a + b);
        }

        public static UInt64 sum(this IEnumerable<UInt64> array)
        {
            return array.Aggregate((a, b) => a + b);
        }

        public static void swap_elements<T>(this T[] list, int i, int j)
        {
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        public static void reverse_elements<T>(this T[] list, int i, int j)
        {
            while (i < j)
            {
                list.swap_elements(i, j);
                i++;
                j--;
            }
        }
        public static T[] copy<T>(this T[] list)
        {
            T[] result = new T[list.Length];

            for (int i = 0; i < list.Length; i++)
            {
                result[i] = list[i];
            }

            return result;
        }

        // Copied from my nonogram solver
        public static int[][] get_all_permutations(this int[] permutation)
        {
            List<int[]> permutations = new List<int[]>();

            do
            {
                permutations.Add(permutation.copy());
            } while (permutation.get_next_permutation());

            return permutations.ToArray();
        }

        // Copied from my nonogram solver
        //
        // If there is another possible permutation, return 'true' and edit the input to be the next permutation
        // Written by me after understanding this explanation of std:next_permutation:
        //      https://stackoverflow.com/questions/11483060/stdnext-permutation-implementation-explanation
        // To use this, the first permutation is in ascending order, and the last permutation is in descending order.
        public static bool get_next_permutation(this int[] list)
        {
            int first = 0;
            int last = list.Length - 1;

            int i = last - 1;

            // Scan from the end of the list to find the first element not in descending order.
            while (i >= first)
            {
                if (list[i] < list[i + 1])
                {
                    // list[i + 1, last] is in descending order, but list[i] doesn't follow the trend.

                    // find the entry in list[i + 1, last] that is the next largest after list[i]
                    // We're guaranteed to find something since list[i] < list[i + 1]
                    // The first one we find is what we want, since list[i + 1, last] is in descending order
                    int next_largest = last;
                    while (list[i] >= list[next_largest])
                    {
                        next_largest--;
                    }

                    // put the next largest element into list[i] by swapping it with list[i]
                    // Note: list[i + 1, last] is still in descending order after this!
                    list.swap_elements(i, next_largest);

                    // Now that the next-largest element is in list[i], reverse list[i + 1, last] so that it's in ascending order.
                    list.reverse_elements(i + 1, last);

                    return true;
                }

                i--;
            }

            // The entire list is in descending order. No more permutations!
            return false;
        }

        public static void copy_to<T>(this T[,] source, T[,] destination,
            int source_start_0, int destination_start_0, int length_0,
            int source_start_1, int destination_start_1, int length_1)
        {
            for (int index_0 = 0; index_0 < length_0; index_0++)
            {
                for (int index_1 = 0; index_1 < length_1; index_1++)
                {
                    destination[destination_start_0 + index_0, destination_start_1 + index_1]
                        = source[source_start_0 + index_0, source_start_1 + index_1];
                }
            }
        }

        public static void copy_to<T>(this T[,,] source, T[,,] destination,
            int source_start_0, int destination_start_0, int length_0,
            int source_start_1, int destination_start_1, int length_1,
            int source_start_2, int destination_start_2, int length_2)
        {
            for (int index_0 = 0; index_0 < length_0; index_0++)
            {
                for (int index_1 = 0; index_1 < length_1; index_1++)
                {
                    for (int index_2 = 0; index_2 < length_2; index_2++)
                    {
                        destination[destination_start_0 + index_0, destination_start_1 + index_1, destination_start_2 + index_2]
                            = source[source_start_0 + index_0, source_start_1 + index_1, source_start_2 + index_2];
                    }
                }
            }
        }

        public static T[,] add_border<T>(this T[,] source, int border_size)
        {
            T[,] destination = new T[source.GetLength(0) + border_size * 2, source.GetLength(1) + border_size * 2];

            source.copy_to(
                destination,
                0, border_size, source.GetLength(0),
                0, border_size, source.GetLength(1));

            return destination;
        }

        public static bool is_valid_index<T>(this T[][] data, c_vector index)
        {
            return index.x >= 0 && index.x < data.Length
                && index.y >= 0 && index.y < data[index.x].Length;
        }

        public static bool is_valid_index<T>(this T[,] data, c_vector index)
        {
            return index.x >= 0 && index.x < data.GetLength(0)
                && index.y >= 0 && index.y < data.GetLength(1);
        }
    }
}
