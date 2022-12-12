using System;
using System.Collections.Generic;
using System.Diagnostics;
using advent_of_code_common.display_helpers;
using advent_of_code_common.input_reader;
using advent_of_code_common.int_math;

namespace advent_of_code_2022.days
{
    internal class day_12
    {
        internal enum e_tile_type
        {
            start,
            end,
            path,
            other,
        }

        [DebuggerDisplay("{elevation}", Type = "c_tile")]
        internal class c_tile
        {
            public char elevation { get; set; }
            public e_tile_type type { get; set; } = e_tile_type.other;
            public c_tile neighbor_up { get; set; } = null;
            public c_tile neighbor_down { get; set; } = null;
            public c_tile neighbor_left { get; set; } = null;
            public c_tile neighbor_right { get; set; } = null;
            public int distance_to_end { get; set; } = int.MaxValue;
            public e_direction direction_to_end { get; set; } = e_direction.none;
            public c_tile neighbor_to_end { get; set; } = null;

            private void consider_direction(c_tile neighbor, e_direction direction)
            {
                // Is it possible to move towards neighbor?
                if (elevation >= neighbor.elevation - 1)
                {
                    // Is travelling through neighbor faster than my current best?
                    if (distance_to_end > neighbor.distance_to_end + 1)
                    {
                        distance_to_end = neighbor.distance_to_end + 1;
                        direction_to_end = direction;
                        neighbor_to_end = neighbor;

                        find_neighbors_distance_to_me();
                    }
                }
            }

            public void find_neighbors_distance_to_me()
            {
                neighbor_up?.consider_direction(this, e_direction.down);
                neighbor_down?.consider_direction(this, e_direction.up);
                neighbor_left?.consider_direction(this, e_direction.right);
                neighbor_right?.consider_direction(this, e_direction.left);
            }

            public void highlight_path()
            {
                if (type == e_tile_type.other)
                {
                    type = e_tile_type.path;
                }

                neighbor_to_end?.highlight_path();
            }

            private void set_display_color()
            {
                // cyan, green, yellow, red
                // 26 | 13, 13 | 6, 7, 6, 7
                // a b c d e f
                // g h i j k l m
                // n o p q r s
                // t u v q x y z

                ConsoleColor color;

                if (type == e_tile_type.start || type == e_tile_type.end)
                {
                    color = ConsoleColor.White;
                }
                else if (elevation <= 'f')
                {
                    color = (type == e_tile_type.other ? ConsoleColor.DarkCyan : ConsoleColor.Cyan);
                }
                else if (elevation <= 'm')
                {
                    color = (type == e_tile_type.other ? ConsoleColor.DarkGreen : ConsoleColor.Green);
                }
                else if (elevation <= 's')
                {
                    color = (type == e_tile_type.other ? ConsoleColor.DarkYellow : ConsoleColor.Yellow);
                }
                else
                {
                    color = (type == e_tile_type.other ? ConsoleColor.DarkRed : ConsoleColor.Red);
                }

                Console.ForegroundColor = color;
            }

            public void display_elevation()
            {
                set_display_color();

                switch (type)
                {
                    case e_tile_type.start:
                        Console.Write($"S");
                        break;

                    case e_tile_type.end:
                        Console.Write($"E");
                        break;

                    case e_tile_type.other:
                    case e_tile_type.path:
                        Console.Write($"{elevation}");
                        break;
                }

                Console.ResetColor();
            }

            public void display_direction()
            {
                set_display_color();

                switch (type)
                {
                    case e_tile_type.start:
                        Console.Write($"S");
                        break;

                    case e_tile_type.end:
                        Console.Write($"E");
                        break;

                    default:
                        switch (direction_to_end)
                        {
                            case e_direction.up:
                                Console.Write("^");
                                break;

                            case e_direction.down:
                                Console.Write("v");
                                break;

                            case e_direction.left:
                                Console.Write("<");
                                break;

                            case e_direction.right:
                                Console.Write(">");
                                break;

                            case e_direction.none:
                                Console.Write("?");
                                break;
                        }
                        break;
                }
            }
        }

        internal static (c_tile[][], c_tile, c_tile) parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            List<c_tile[]> tiles_list = new List<c_tile[]>();
            c_tile start = null;
            c_tile end = null;

            // Read in the input
            while (input_reader.has_more_lines())
            {
                List<c_tile> tile_row = new List<c_tile>();

                foreach(char input_char in input_reader.read_line().ToCharArray())
                {
                    c_tile tile = new c_tile();

                    if (input_char == 'S')
                    {
                        tile.elevation = 'a';
                        tile.type = e_tile_type.start;
                        start = tile;
                    }
                    else if (input_char == 'E')
                    {
                        tile.elevation = 'z';
                        tile.type = e_tile_type.end;
                        end = tile;
                    }
                    else
                    {
                        tile.elevation = input_char;
                    }

                    tile_row.Add(tile);
                }

                tiles_list.Add(tile_row.ToArray());
            }

            c_tile[][] tiles = tiles_list.ToArray();

            // Link up neighbors
            for (int y = 0; y < tiles.Length; y++)
            {
                for (int x = 0; x < tiles[y].Length; x++)
                {
                    if (y > 0)
                    {
                        tiles[y][x].neighbor_up = tiles[y - 1][x];
                    }

                    if (y < tiles.Length - 1)
                    {
                        tiles[y][x].neighbor_down = tiles[y + 1][x];
                    }

                    if (x > 0)
                    {
                        tiles[y][x].neighbor_left = tiles[y][x - 1];
                    }

                    if (x < tiles[y].Length - 1)
                    {
                        tiles[y][x].neighbor_right = tiles[y][x + 1];
                    }
                }
            }

            return (tiles, start, end);
        }

        public static void part_1(
            string input,
            bool pretty)
        {
            (c_tile[][] tiles, c_tile start, c_tile end) = parse_input(input, pretty);

            // Calculate distances to the end tile
            end.distance_to_end = 0;
            end.find_neighbors_distance_to_me();

            if (pretty)
            {
                start.highlight_path();
                tiles.display(t => t.display_elevation());
                tiles.display(t => t.display_direction());
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", start.distance_to_end);
            Console.ResetColor();
        }

        public static void part_2(
            string input,
            bool pretty)
        {
            (c_tile[][] tiles, c_tile start, c_tile end) = parse_input(input, pretty);

            // Calculate the distances to the end tile
            end.distance_to_end = 0;
            end.find_neighbors_distance_to_me();

            // Search for a better start
            c_tile best_start = start;

            foreach (c_tile[] tile_row in tiles)
            {
                foreach (c_tile tile in tile_row)
                {
                    if (tile.elevation == 'a' && tile.distance_to_end < best_start.distance_to_end)
                    {
                        best_start = tile;
                    }
                }
            }

            best_start.type = e_tile_type.start;

            if (pretty)
            {
                best_start.highlight_path();
                tiles.display(t => t.display_elevation());
                tiles.display(t => t.display_direction());
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", best_start.distance_to_end);
            Console.ResetColor();
        }
    }
}
