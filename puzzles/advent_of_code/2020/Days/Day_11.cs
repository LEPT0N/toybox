using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.input_reader;

namespace advent_of_code_2020.Days
{
    internal class Day_11
    {
        internal enum e_tile_state
        {
            floor,
            occupied,
            empty,
        }

        [DebuggerDisplay("{state} {num_occupied_neighbors}", Type = "c_tile")]
        internal class c_tile
        {
            public e_tile_state state;
            public bool changed;
            public int num_occupied_neighbors;
        }

        internal static c_tile[][] parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            List<c_tile[]> tiles = new List<c_tile[]>();

            while (input_reader.has_more_lines())
            {
                List<c_tile> tile_row = new List<c_tile>();

                foreach (char input_char in input_reader.read_line())
                {
                    e_tile_state tile_state;

                    switch (input_char)
                    {
                        case '.': tile_state = e_tile_state.floor; break;
                        case 'L': tile_state = e_tile_state.empty; break;
                        case '#': tile_state = e_tile_state.occupied; break;
                        default: throw new Exception(String.Format("Bad input character '{0}'", input_char));
                    }

                    tile_row.Add(new c_tile { state = tile_state });
                }

                tiles.Add(tile_row.ToArray());
            }

            return tiles.ToArray();
        }

        internal static void display_tiles(string title, c_tile[][] tiles)
        {
            Console.WriteLine(title);

            for (int row = 0; row < tiles.Length; row++)
            {
                for (int column = 0; column < tiles[row].Length; column++)
                {
                    c_tile current_tile = tiles[row][column];

                    switch (current_tile.state, current_tile.changed)
                    {
                        case (e_tile_state.empty, true):
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write('L');
                            break;

                        case (e_tile_state.empty, false):
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.Write('L');
                            break;

                        case (e_tile_state.occupied, true):
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write('#');
                            break;

                        case (e_tile_state.occupied, false):
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.Write('#');
                            break;

                        case (e_tile_state.floor, true):
                        case (e_tile_state.floor, false):
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.Write('.');
                            break;

                        default:
                            throw new Exception(String.Format("tiles[{0}][{1}].state = {2}", row, column, current_tile.state));
                    }
                }

                Console.WriteLine();
            }

            Console.ResetColor();
            Console.WriteLine();
        }

        internal static bool update_tiles(c_tile[][] tiles, int overcrowding_limit)
        {
            bool changed = false;

            for (int row = 0; row < tiles.Length; row++)
            {
                for (int column = 0; column < tiles[row].Length; column++)
                {
                    c_tile current_tile = tiles[row][column];
                    current_tile.changed = false;

                    if (current_tile.state == e_tile_state.empty && current_tile.num_occupied_neighbors == 0)
                    {
                        changed = true;
                        current_tile.changed = true;
                        current_tile.state = e_tile_state.occupied;
                    }
                    else if (current_tile.state == e_tile_state.occupied && current_tile.num_occupied_neighbors >= overcrowding_limit)
                    {
                        changed = true;
                        current_tile.changed = true;
                        current_tile.state = e_tile_state.empty;
                    }
                }
            }

            return changed;
        }

        public static void Part_1(
            string input,
            bool pretty)
        {
            c_tile[][] tiles = parse_input(input, pretty);

            if (pretty)
            {
                display_tiles("input:", tiles);
            }

            int step_count = 0;
            bool changed = true;

            while (changed)
            {
                for (int row = 0; row < tiles.Length; row++)
                {
                    for (int column = 0; column < tiles[row].Length; column++)
                    {
                        c_tile current_tile = tiles[row][column];
                        current_tile.num_occupied_neighbors = 0;

                        for (int neighbor_row = row - 1; neighbor_row <= row + 1; neighbor_row++)
                        {
                            if (neighbor_row >= 0 && neighbor_row < tiles.Length)
                            {
                                for (int neighbor_column = column - 1; neighbor_column <= column + 1; neighbor_column++)
                                {
                                    if (neighbor_column >= 0 && neighbor_column < tiles[neighbor_row].Length)
                                    {
                                        if ((neighbor_row != row || neighbor_column != column) &&
                                            tiles[neighbor_row][neighbor_column].state == e_tile_state.occupied)
                                        {
                                            current_tile.num_occupied_neighbors++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                changed = update_tiles(tiles, 4);

                step_count++;

                if (pretty)
                {
                    display_tiles(String.Format("After Step {0}:", step_count), tiles);
                }
            }

            int num_occupied = tiles.Sum(tiles_row => tiles_row.Sum(tile => (tile.state == e_tile_state.occupied ? 1 : 0)));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", num_occupied);
            Console.ResetColor();
        }

        internal static bool can_see_occupied_seat(c_tile[][] tiles, int current_row, int current_column, int neighbor_row_offset, int neighbor_column_offset)
        {
            int row = current_row + neighbor_row_offset;
            int column = current_column + neighbor_column_offset;

            while (row >= 0 && row < tiles.Length && column >= 0 && column < tiles[row].Length)
            {
                if (tiles[row][column].state == e_tile_state.occupied)
                {
                    return true;
                }
                else if (tiles[row][column].state == e_tile_state.empty)
                {
                    return false;
                }

                row += neighbor_row_offset;
                column += neighbor_column_offset;
            }

            return false;
        }

        internal static (int, int)[] k_neighbor_offests =
        {
            (-1, -1),
            (-1, 0),
            (-1, 1),

            (0, -1),
            (0, 1),

            (1, -1),
            (1, 0),
            (1, 1),
        };

        public static void Part_2(
            string input,
            bool pretty)
        {
            c_tile[][] tiles = parse_input(input, pretty);
            if (pretty)
            {
                display_tiles("input:", tiles);
            }

            int step_count = 0;
            bool changed = true;

            while (changed)
            {
                for (int row = 0; row < tiles.Length; row++)
                {
                    for (int column = 0; column < tiles[row].Length; column++)
                    {
                        c_tile current_tile = tiles[row][column];
                        current_tile.num_occupied_neighbors = 0;

                        foreach((int, int) neighbor_offset in k_neighbor_offests)
                        {
                            if (can_see_occupied_seat(tiles, row, column, neighbor_offset.Item1, neighbor_offset.Item2))
                            {
                                current_tile.num_occupied_neighbors++;
                            }
                        }
                    }
                }

                changed = update_tiles(tiles, 5);

                step_count++;

                if (pretty)
                {
                    display_tiles(String.Format("After Step {0}:", step_count), tiles);
                }
            }

            int num_occupied = tiles.Sum(tiles_row => tiles_row.Sum(tile => (tile.state == e_tile_state.occupied ? 1 : 0)));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", num_occupied);
            Console.ResetColor();
        }
    }
}
