using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
