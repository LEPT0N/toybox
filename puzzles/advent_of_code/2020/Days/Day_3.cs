using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace advent_of_code_2020.Days
{
    internal class Day_3
    {
        public static void Part_1(string input)
        {
            string[] map = System.IO.File.ReadAllLines(input).ToArray();

            int col = 0;

            int trees_hit = 0;

            foreach (string map_row in map)
            {
                if (map_row[col] == '#')
                {
                    trees_hit++;
                }

                col = (col + 3) % map_row.Length;
            }

            Console.WriteLine("Trees hit = {0}", trees_hit);
        }

        public static void Part_2(string input)
        {
            string[] map = System.IO.File.ReadAllLines(input).ToArray();

            int[] slope_row_increment = { 1, 1, 1, 1, 2 };
            int[] slope_column_increment = { 1, 3, 5, 7, 1 };

            int result = 1;

            for (int slope_index = 0; slope_index < slope_row_increment.Length; slope_index++)
            {
                int column = 0;
                int trees_hit = 0;

                for (int row_index = 0; row_index < map.Length; row_index += slope_row_increment[slope_index])
                {
                    if (map[row_index][column] == '#')
                    {
                        trees_hit++;
                    }

                    column = (column + slope_column_increment[slope_index]) % map[row_index].Length;
                }

                Console.WriteLine("Trees hit for slope {0} = {1}", slope_index, trees_hit);
                result *= trees_hit;
            }

            Console.WriteLine("Answer = {0}", result);
        }
    }
}
