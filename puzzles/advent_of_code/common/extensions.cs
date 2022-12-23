using System;
using System.Collections.Generic;

namespace advent_of_code_common.extensions
{
    public static class extensions
    {
        public static bool test_bit(this int bit_field, int index)
        {
            return (0 != (bit_field & (1 << index)));
        }

        public static void set_bit(ref this int bit_field, int index)
        {
            bit_field |= (1 << index);
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

        public static T[,] add_border<T>(this T[,] source, int border_size)
        {
            T[,] destination = new T[source.GetLength(0) + border_size * 2, source.GetLength(1) + border_size * 2];

            source.copy_to(
                destination,
                0, border_size, source.GetLength(0),
                0, border_size, source.GetLength(1));

            return destination;
        }
    }
}
