using System;
using System.Collections.Generic;
using System.Diagnostics;
using advent_of_code_common.input_reader;

namespace advent_of_code_2020.Days
{
    internal class Day_17
    {
        [DebuggerDisplay("{DebuggerDisplay,nq}", Type = "c_grid")]
        internal class c_grid3
        {
            private bool[][][] data;

            private string DebuggerDisplay
            {
                get
                {
                    int x_size = data?.Length ?? 0;
                    int y_size = data?[0].Length ?? 0;
                    int z_size = data?[0][0].Length ?? 0;

                    return string.Format("{0} x {1} x {2}", x_size, y_size, z_size);
                }
            }

            public c_grid3(bool[][][] d)
            {
                data = d;
            }

            public c_grid3(int row_length, int column_length, int z_length)
            {
                data = new bool[row_length][][];

                for (int row = 0; row < row_length; row++)
                {
                    data[row] = new bool[column_length][];

                    for (int column = 0; column < column_length; column++)
                    {
                        data[row][column] = new bool[z_length];
                    }
                }
            }

            public void print(bool pretty, string title)
            {
                if (pretty)
                {
                    Console.WriteLine(title);
                    Console.WriteLine();

                    for (int z = 0; z < data[0][0].Length; z++)
                    {
                        Console.WriteLine("z = {0}", z);

                        for (int row = 0; row < data.Length; row++)
                        {
                            for (int column = 0; column < data[0].Length; column++)
                            {
                                Console.Write(data[row][column][z] ? '#' : '.');
                            }

                            Console.WriteLine();
                        }

                        Console.WriteLine();
                    }
                }
            }

            private bool is_position_alive(int row, int column, int z)
            {
                if (row < 0 || row >= data.Length ||
                    column < 0 || column >= data[row].Length ||
                    z < 0 || z >= data[row][column].Length)
                {
                    return false;
                }

                return data[row][column][z];
            }

            private bool step_position(int center_row, int center_column, int center_z)
            {
                bool currently_alive = is_position_alive(center_row, center_column, center_z);

                int neighbors_alive = 0;

                for (int row = center_row - 1; row <= center_row + 1; row++)
                {
                    for (int column = center_column - 1; column <= center_column + 1; column++)
                    {
                        for (int z = center_z - 1; z <= center_z + 1; z++)
                        {
                            if (row != center_row || column != center_column || z != center_z)
                            {
                                if (is_position_alive(row, column, z))
                                {
                                    neighbors_alive++;
                                }
                            }
                        }
                    }
                }

                bool result;

                if (currently_alive)
                {
                    result = (2 <= neighbors_alive && neighbors_alive <= 3);
                }
                else
                {
                    result = (neighbors_alive == 3);
                }

                return result;
            }

            public c_grid3 step()
            {
                c_grid3 new_grid = new c_grid3(data.Length + 2, data[0].Length + 2, data[0][0].Length + 2);

                for (int row = -1; row <= data.Length; row++)
                {
                    for (int column = -1; column <= data[0].Length; column++)
                    {
                        for (int z = -1; z <= data[0][0].Length; z++)
                        {
                            bool new_value = step_position(row, column, z);

                            new_grid.data[row + 1][column + 1][z + 1] = new_value;
                        }
                    }
                }

                return new_grid;
            }

            public int get_total_alive()
            {
                int result = 0;

                for (int row = 0; row < data.Length; row++)
                {
                    for (int column = 0; column < data[row].Length; column++)
                    {
                        for (int z = 0; z < data[row][column].Length; z++)
                        {
                            if (data[row][column][z])
                            {
                                result++;
                            }
                        }
                    }
                }

                return result;
            }
        }

        internal static c_grid3 parse_input3(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            List<bool[][]> grid_rows = new List<bool[][]>();

            while (input_reader.has_more_lines())
            {
                List<bool[]> grid_row = new List<bool[]>();

                foreach (char input_char in input_reader.read_line())
                {
                    bool active = (input_char == '#');

                    grid_row.Add(new bool[]{ active });
                }

                grid_rows.Add(grid_row.ToArray());
            }

            return new c_grid3(grid_rows.ToArray());
        }

        public static void Part_1(
            string input,
            bool pretty)
        {
            c_grid3 grid = parse_input3(input, pretty);

            grid.print(pretty, "Before any cycles:");

            for (int i = 0; i < 6; i++)
            {
                grid = grid.step();
                grid.print(pretty, "After " + i + " Cycles:");
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", grid.get_total_alive());
            Console.ResetColor();
        }

        [DebuggerDisplay("{DebuggerDisplay,nq}", Type = "c_grid")]
        internal class c_grid4
        {
            private bool[][][][] data;

            private string DebuggerDisplay
            {
                get
                {
                    int x_size = data?.Length ?? 0;
                    int y_size = data?[0].Length ?? 0;
                    int z_size = data?[0][0].Length ?? 0;
                    int w_size = data?[0][0][0].Length ?? 0;

                    return string.Format("{0} x {1} x {2} x {3}", x_size, y_size, z_size, w_size);
                }
            }

            public c_grid4(bool[][][][] d)
            {
                data = d;
            }

            public c_grid4(int row_length, int column_length, int z_length, int w_length)
            {
                data = new bool[row_length][][][];

                for (int row = 0; row < row_length; row++)
                {
                    data[row] = new bool[column_length][][];

                    for (int column = 0; column < column_length; column++)
                    {
                        data[row][column] = new bool[z_length][];

                        for (int z = 0; z < z_length; z++)
                        {
                            data[row][column][z] = new bool[w_length];
                        }
                    }
                }
            }

            public void print(bool pretty, string title)
            {
                if (pretty)
                {
                    Console.WriteLine(title);
                    Console.WriteLine();

                    for (int z = 0; z < data[0][0].Length; z++)
                    {
                        for (int w = 0; w < data[0][0][0].Length; w++)
                        {
                            Console.WriteLine("z = {0}, w = {1}", z, w);

                            for (int row = 0; row < data.Length; row++)
                            {
                                for (int column = 0; column < data[0].Length; column++)
                                {
                                    Console.Write(data[row][column][z][w] ? '#' : '.');
                                }

                                Console.WriteLine();
                            }

                            Console.WriteLine();
                        }
                    }
                }
            }

            private bool is_position_alive(int row, int column, int z, int w)
            {
                if (row < 0 || row >= data.Length ||
                    column < 0 || column >= data[row].Length ||
                    z < 0 || z >= data[row][column].Length ||
                    w < 0 || w >= data[row][column][z].Length)
                {
                    return false;
                }

                return data[row][column][z][w];
            }

            private bool step_position(int center_row, int center_column, int center_z, int center_w)
            {
                bool currently_alive = is_position_alive(center_row, center_column, center_z, center_w);

                int neighbors_alive = 0;

                for (int row = center_row - 1; row <= center_row + 1; row++)
                {
                    for (int column = center_column - 1; column <= center_column + 1; column++)
                    {
                        for (int z = center_z - 1; z <= center_z + 1; z++)
                        {
                            for (int w = center_w - 1; w <= center_w + 1; w++)
                            {
                                if (row != center_row || column != center_column || z != center_z || w != center_w)
                                {
                                    if (is_position_alive(row, column, z, w))
                                    {
                                        neighbors_alive++;
                                    }
                                }
                            }
                        }
                    }
                }

                bool result;

                if (currently_alive)
                {
                    result = (2 <= neighbors_alive && neighbors_alive <= 3);
                }
                else
                {
                    result = (neighbors_alive == 3);
                }

                return result;
            }

            public c_grid4 step()
            {
                c_grid4 new_grid = new c_grid4(data.Length + 2, data[0].Length + 2, data[0][0].Length + 2, data[0][0][0].Length + 2);

                for (int row = -1; row <= data.Length; row++)
                {
                    for (int column = -1; column <= data[0].Length; column++)
                    {
                        for (int z = -1; z <= data[0][0].Length; z++)
                        {
                            for (int w = -1; w <= data[0][0][0].Length; w++)
                            {
                                bool new_value = step_position(row, column, z, w);

                                new_grid.data[row + 1][column + 1][z + 1][w + 1] = new_value;
                            }
                        }
                    }
                }

                return new_grid;
            }

            public int get_total_alive()
            {
                int result = 0;

                for (int row = 0; row < data.Length; row++)
                {
                    for (int column = 0; column < data[row].Length; column++)
                    {
                        for (int z = 0; z < data[row][column].Length; z++)
                        {
                            for (int w = 0; w < data[row][column][z].Length; w++)
                            {
                                if (data[row][column][z][w])
                                {
                                    result++;
                                }
                            }
                        }
                    }
                }

                return result;
            }
        }

        internal static c_grid4 parse_input4(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            List<bool[][][]> grid_rows = new List<bool[][][]>();

            while (input_reader.has_more_lines())
            {
                List<bool[][]> grid_row = new List<bool[][]>();

                foreach (char input_char in input_reader.read_line())
                {
                    bool active = (input_char == '#');

                    bool[][] value = new bool[1][];
                    value[0] = new bool[] { active };

                    grid_row.Add(value);
                }

                grid_rows.Add(grid_row.ToArray());
            }

            return new c_grid4(grid_rows.ToArray());
        }

        public static void Part_2(
            string input,
            bool pretty)
        {
            c_grid4 grid = parse_input4(input, pretty);

            grid.print(pretty, "Before any cycles:");

            for (int i = 0; i < 6; i++)
            {
                grid = grid.step();
                grid.print(pretty, "After " + i + " Cycles:");
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", grid.get_total_alive());
            Console.ResetColor();
        }
    }
}
