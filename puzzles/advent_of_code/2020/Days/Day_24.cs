using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.input_reader;

namespace advent_of_code_2020.Days
{
    internal class Day_24
    {
        internal enum e_hex_direction
        {
            east,
            west,
            north_east,
            north_west,
            south_east,
            south_west,
        }

        [DebuggerDisplay("({x}, {y})", Type = "s_hex_coordinate")]
        internal struct s_hex_coordinate
        {
            public int x;
            public int y;

            public void move(e_hex_direction direction)
            {
				// y == 0, x = 0 1 2 3 4
				// y == 1, x =  0 1 2 3 4
				// y == 2, x = 0 1 2 3 4
				// y == 3, x =  0 1 2 3 4

				switch (direction)
                {
                    case e_hex_direction.east:
                        x++;
                        break;

                    case e_hex_direction.west:
                        x--;
                        break;

                    case e_hex_direction.north_east:
                    case e_hex_direction.south_east:
                        x += (y % 2 == 0 ? 0 : 1);
                        break;

                    case e_hex_direction.north_west:
                    case e_hex_direction.south_west:
                        x -= (y % 2 == 0 ? 1 : 0);
                        break;
                }

                switch (direction)
                {
                    case e_hex_direction.north_east:
                    case e_hex_direction.north_west:
                        y++;
                        break;

                    case e_hex_direction.south_east:
                    case e_hex_direction.south_west:
                        y--;
                        break;
                }
            }
        }

        internal static e_hex_direction[][] parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            List<e_hex_direction[]> directions = new List<e_hex_direction[]>();

            while (input_reader.has_more_lines())
            {
                string input_line = input_reader.read_line();

                List<e_hex_direction> direction_line = new List<e_hex_direction>();

                for (int i = 0; i < input_line.Length; i++)
                {
                    switch (input_line[i])
                    {
                        case 'e':
                            direction_line.Add(e_hex_direction.east);
                            break;

                        case 'w':
                            direction_line.Add(e_hex_direction.west);
                            break;

                        case 'n':
                            {
                                i++;

                                switch (input_line[i])
                                {
                                    case 'e':
                                        direction_line.Add(e_hex_direction.north_east);
                                        break;

                                    case 'w':
                                        direction_line.Add(e_hex_direction.north_west);
                                        break;

                                    default:
                                        throw new Exception("bad input");
                                }
                            }
                            break;

                        case 's':
                            {
                                i++;

                                switch (input_line[i])
                                {
                                    case 'e':
                                        direction_line.Add(e_hex_direction.south_east);
                                        break;

                                    case 'w':
                                        direction_line.Add(e_hex_direction.south_west);
                                        break;

                                    default:
                                        throw new Exception("bad input");
                                }
                            }
                            break;

                        default:
                            throw new Exception("bad input");
                    }
                }

                directions.Add(direction_line.ToArray());
            }

            return directions.ToArray();
        }

        internal static s_hex_coordinate[] apply_movements(e_hex_direction[][] movements_list)
        {
            HashSet<s_hex_coordinate> flipped_coordinates = new HashSet<s_hex_coordinate>();

            foreach (e_hex_direction[] movements in movements_list)
            {
                s_hex_coordinate coordinate = new s_hex_coordinate { x = 0, y = 0 };

                foreach (e_hex_direction movement in movements)
                {
                    coordinate.move(movement);
                }

                if (flipped_coordinates.Contains(coordinate))
                {
                    flipped_coordinates.Remove(coordinate);
                }
                else
                {
                    flipped_coordinates.Add(coordinate);
                }
            }

            return flipped_coordinates.ToArray();
        }

        internal class c_hex_grid
        {
            private bool[][] data;

			public c_hex_grid(int x_size, int y_size)
			{
				data = new bool[x_size][];

				for (int x = 0; x < x_size; x++)
				{
					data[x] = new bool[y_size];
				}
			}

			public c_hex_grid(s_hex_coordinate[] coordinates)
            {
                int min_x = coordinates.Min(c => c.x);
				int max_x = coordinates.Max(c => c.x);
				int min_y = coordinates.Min(c => c.y);
				int max_y = coordinates.Max(c => c.y);

				// only shift y by an even number to keep neighbors the same.
				if (min_y % 2 != 0)
                {
                    min_y--;
                }

				data = new bool[max_x - min_x + 1][];
                for (int x = 0; x < data.Length; x++)
                {
                    data[x] = new bool[max_y - min_y + 1];
                }

                foreach (s_hex_coordinate coordinate in coordinates)
                {
                    int x = coordinate.x - min_x;
					int y = coordinate.y - min_y;

                    data[x][y] = true;
				}

			}

			public int get_total_set()
			{
				int result = 0;

				for (int x = 0; x < data.Length; x++)
                {
					for (int y = 0; y < data[x].Length; y++)
				    {
						if (data[x][y])
						{
							result++;
						}
					}
				}

				return result;
			}

			private bool is_position_set(int x, int y)
			{
				if (x < 0 || x >= data.Length ||
					y < 0 || y >= data[x].Length)
				{
					return false;
				}

				return data[x][y];
			}

            private int get_neighbors_set(int center_x, int center_y)
			{
				int neighbors_set = 0;

				e_hex_direction[] directions = (e_hex_direction[])Enum.GetValues(typeof(e_hex_direction));
				foreach (e_hex_direction direction in directions)
				{
					s_hex_coordinate coordinate = new s_hex_coordinate { x = center_x, y = center_y };
					coordinate.move(direction);

					if (is_position_set(coordinate.x, coordinate.y))
					{
						neighbors_set++;
					}
				}

                return neighbors_set;
			}

            private bool will_be_set(bool is_set, int neighbors_set)
			{
				bool result;

				if (is_set)
				{
					result = (neighbors_set == 1 || neighbors_set == 2);
				}
				else
				{
					result = (neighbors_set == 2);
				}

				return result;
			}

			private bool step_position(int center_x, int center_y)
			{
				bool is_set = is_position_set(center_x, center_y);

				int neighbors_set = get_neighbors_set(center_x, center_y);

                return will_be_set(is_set, neighbors_set);
			}

			public c_hex_grid step()
			{
                // New grid could possibly grow by 1 in each direction, but also scale the 'y' coordinate by
                // another 1 so we shift the y coordinates by an even number, keeping their neighbors the same.
				c_hex_grid new_grid = new c_hex_grid(data.Length + 2, data[0].Length + 3);

				for (int x = -1; x <= data.Length; x++)
				{
					for (int y = -1; y <= data[0].Length; y++)
					{
						bool new_value = step_position(x, y);

						new_grid.data[x + 1][y + 2] = new_value;
					}
				}

                // After a few steps, the largest possible board is usually much larger than what coordinates are actually set.
				return new_grid.trim();
			}

            private c_hex_grid trim()
            {
                int min_x = int.MaxValue;
                int max_x = int.MinValue;
                int min_y = int.MaxValue;
                int max_y = int.MinValue;

                for (int x = 0; x < data.Length; x++)
                {
                    for (int y = 0; y < data[0].Length; y++)
                    {
                        if (data[x][y])
                        {
                            min_x = Math.Min(min_x, x);
							max_x = Math.Max(max_x, x);

							min_y = Math.Min(min_y, y);
							max_y = Math.Max(max_y, y);
						}
                    }
                }

                if (min_x > max_x)
                {
                    min_x = 0;
                    max_x = 1;
                    min_y = 0;
                    max_y = 1;
                }

				// only shift y by an even number to keep neighbors the same.
				if (min_y % 2 != 0)
				{
					min_y--;
				}

				c_hex_grid new_grid = new c_hex_grid(max_x - min_x + 1, max_y - min_y + 1);

				for (int x = 0; x < data.Length; x++)
				{
					for (int y = 0; y < data[0].Length; y++)
					{
						if (data[x][y])
						{
                            new_grid.data[x - min_x][y - min_y] = true;
						}
					}
				}

				return new_grid;
			}

            public void print(bool pretty, string title)
			{
				Console.WriteLine("{0}: {1}", title, get_total_set());

				if (pretty)
				{
					for (int y = -1; y <= data[0].Length; y++)
                    {
                        if (y % 2 != 0)
                        {
                            Console.Write(" ");
                        }

                        for (int x = -1; x <= data.Length; x++)
						{
							bool is_set = is_position_set(x, y);
							int neighbors_set = get_neighbors_set(x, y);
                            bool will_change = (is_set != will_be_set(is_set, neighbors_set));

                            if (is_set)
                            {
                                // Red are dying.
								Console.ForegroundColor = will_change ? ConsoleColor.Red : ConsoleColor.White;
							}
                            else
							{
                                // Green will be born soon.
								Console.ForegroundColor = will_change ? ConsoleColor.DarkGreen : ConsoleColor.DarkGray;
							}

                            Console.Write($"{neighbors_set} ");
						}

						Console.WriteLine();
					}

                    Console.WriteLine();
                }
            }
        }

        public static void Part_1(
            string input,
            bool pretty)
        {
            e_hex_direction[][] movements_list = parse_input(input, pretty);

            s_hex_coordinate[] flipped_coordinates = apply_movements(movements_list);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", flipped_coordinates.Length);
            Console.ResetColor();
        }

        public static void Part_2(
            string input,
            bool pretty)
        {
            e_hex_direction[][] movements_list = parse_input(input, pretty);

            s_hex_coordinate[] flipped_coordinates = apply_movements(movements_list);

            c_hex_grid grid = new c_hex_grid(flipped_coordinates);
            grid.print(pretty, "Day 0");

            for (int i = 1; i <= 100; i++)
			{
				c_hex_grid new_grid = grid.step();
				new_grid.print(pretty, $"Day {i}");

				grid = new_grid;
			}

			Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", grid.get_total_set());
            Console.ResetColor();
        }
    }
}
