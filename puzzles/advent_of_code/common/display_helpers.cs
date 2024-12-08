using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace advent_of_code_common.display_helpers
{
    public static class special_characters
    {
        // https://www.fileformat.info/info/unicode/block/box_drawing/images.htm

        public static char k_box_icon_left_and_right = '\u2500';
        public static char k_box_icon_up_and_down = '\u2502';

        public static char k_box_icon_down_and_right = '\u250C';
        public static char k_box_icon_down_and_left = '\u2510';
        public static char k_box_icon_up_and_right = '\u2514';
        public static char k_box_icon_up_and_left = '\u2518';

        public static char k_box_icon_all_but_up = '\u252C';
        public static char k_box_icon_all_but_down = '\u2534';
        public static char k_box_icon_all_but_left = '\u251C';
        public static char k_box_icon_all_but_right = '\u2524';

        public static char k_box_icon_up = '\u2575';
        public static char k_box_icon_down = '\u2577';
        public static char k_box_icon_left = '\u2574';
        public static char k_box_icon_right = '\u2576';

        public static char k_box_icon_all_four = '\u253C';
        public static char k_box_icon_none = ' ';
    }

    public static class display_utilities
    {
        public static void display_grid_top(int width)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(special_characters.k_box_icon_down_and_right);
            for (int column = 0; column < width; column++)
            {
                Console.Write(special_characters.k_box_icon_left_and_right);
            }
            Console.WriteLine(special_characters.k_box_icon_down_and_left);
            Console.ResetColor();
        }

        public static void display_grid_side()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(special_characters.k_box_icon_up_and_down);
            Console.ResetColor();
        }

        public static void display_grid_bottom(int width)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(special_characters.k_box_icon_up_and_right);
            for (int column = 0; column < width; column++)
            {
                Console.Write(special_characters.k_box_icon_left_and_right);
            }
            Console.WriteLine(special_characters.k_box_icon_up_and_left);
            Console.ResetColor();
        }
    }

    public static class extensions
    {
        public static void display<T>(this T[,] grid, Action<T> value_display_function = null)
        {
            display_utilities.display_grid_top(grid.GetLength(1));

            for (int row = 0; row < grid.GetLength(0); row++)
            {
                display_utilities.display_grid_side();

                for (int column = 0; column < grid.GetLength(1); column++)
                {
                    if (value_display_function != null)
                    {
                        value_display_function(grid[row, column]);
                    }
                    else
                    {
                        Console.Write(grid[row, column].ToString());
                    }
                }

                display_utilities.display_grid_side();
                Console.WriteLine();
            }

            display_utilities.display_grid_bottom(grid.GetLength(1));
        }

        public static void display<T>(this T[][] grid, Action<T> value_display_function = null)
        {
            display_utilities.display_grid_top(grid[0].Length);

            for (int row = 0; row < grid.Length; row++)
            {
                display_utilities.display_grid_side();

                for (int column = 0; column < grid[row].Length; column++)
                {
                    if (value_display_function != null)
                    {
                        value_display_function(grid[row][column]);
                    }
                    else
                    {
                        Console.Write(grid[row][column].ToString());
                    }
                }

                display_utilities.display_grid_side();
                Console.WriteLine();
            }

            display_utilities.display_grid_bottom(grid[0].Length);
        }
    }
}
