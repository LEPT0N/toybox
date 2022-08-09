using advent_of_code_common.display_helpers;
using advent_of_code_common.extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.input_reader;

namespace advent_of_code_2020.Days
{
    public enum e_grid_value
    {
        on,
        off,
        monster,
    }

    public static class grid_value_extensions
    {
        public static void display(this e_grid_value value)
        {
            switch (value)
            {
                case e_grid_value.on:
                    Console.Write("#");
                    break;

                case e_grid_value.off:
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    Console.Write(".");
                    Console.ResetColor();
                    break;

                case e_grid_value.monster:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("O");
                    Console.ResetColor();
                    break;
            }
        }
    }

    internal class Day_20
    {
        [DebuggerDisplay("{id}: {neighbors.Count} neighbors", Type = "c_tile")]
        internal class c_tile
        {
            public readonly int id;
            private List<c_tile> neighbors = new List<c_tile>();
            private bool[,] grid = new bool[10, 10];
            private int[] edge_ids = new int[8];

            public c_tile top_neighbor { get; private set; } = null;
            public c_tile left_neighbor { get; private set; } = null;
            public c_tile right_neighbor { get; private set; } = null;
            public c_tile bottom_neighbor { get; private set; } = null;

            static readonly private ((int, int), (int, int))[] k_edge_traverals =
            {
                ((0, 0), (1, 0)),
                ((0, 0), (0, 1)),
                ((9, 0), (-1, 0)),
                ((9, 0), (0, 1)),
                ((0, 9), (0, -1)),
                ((0, 9), (1, 0)),
                ((9, 9), (-1, 0)),
                ((9, 9), (0, -1)),
            };

            static readonly private int k_edge_index_left = 0;
            static readonly private int k_edge_index_top = 1;
            static readonly private int k_edge_index_bottom = 3;
            static readonly private int k_edge_index_right = 5;

            public int neighbor_count()
            {
                return neighbors.Count;
            }

            private int unaligned_neighbor_count()
            {
                int unaligned_neighbors = neighbor_count();

                if (top_neighbor != null) unaligned_neighbors--;
                if (left_neighbor != null) unaligned_neighbors--;
                if (right_neighbor != null) unaligned_neighbors--;
                if (bottom_neighbor != null) unaligned_neighbors--;

                return unaligned_neighbors;
            }

            public bool get_grid_position(int row, int column)
            {
                return grid[row, column];
            }

            public c_tile(c_input_reader input_reader)
            {
                // Read the tile's ID number.

                string id_string = input_reader.read_line();
                id_string = id_string.Substring(5, id_string.Length - 6);

                id = int.Parse(id_string);

                // Read the tile's grid.

                int row = 0;
                string input_line;
                while (input_reader.has_more_lines())
                {
                    input_line = input_reader.read_line();

                    if (string.IsNullOrEmpty(input_line))
                    {
                        break;
                    }

                    for (int column = 0; column < input_line.Length; column++)
                    {
                        grid[row, column] = input_line[column] == '#';
                    }

                    row++;
                }

                // Compute the tile's edge ID numbers.

                for (int edge_index = 0; edge_index < k_edge_traverals.Length; edge_index++)
                {
                    edge_ids[edge_index] = compute_edge_id(edge_index);
                }
            }

            private int compute_edge_id(int edge_index)
            {
                int edge_id = 0;

                (int, int) grid_position = k_edge_traverals[edge_index].Item1;
                (int, int) grid_increment = k_edge_traverals[edge_index].Item2;

                for (int i = 0; i < 10; i++, grid_position.Item1 += grid_increment.Item1, grid_position.Item2 += grid_increment.Item2)
                {
                    edge_id *= 2;

                    if (grid[grid_position.Item1, grid_position.Item2])
                    {
                        edge_id++;
                    }
                }

                return edge_id;
            }

            public static void check_for_neighbors(c_tile a, c_tile b)
            {
                // If two tiles have matching edge ID numbers, mark them as neighbors.

                for (int i = 0; i < k_edge_traverals.Length; i++)
                {
                    for (int j = 0; j < k_edge_traverals.Length; j++)
                    {
                        if (a.edge_ids[i] == b.edge_ids[j])
                        {
                            a.neighbors.Add(b);
                            b.neighbors.Add(a);
                            return;
                        }
                    }
                }
            }

            private void flip()
            {
                grid = grid.flip();
            }

            private void rotate()
            {
                grid = grid.rotate();
            }

            private bool try_align(c_tile neighbor)
            {
                if (this.compute_edge_id(k_edge_index_top) == neighbor.compute_edge_id(k_edge_index_bottom))
                {
                    this.top_neighbor = neighbor;
                    neighbor.bottom_neighbor = this;

                    return true;
                }

                if (this.compute_edge_id(k_edge_index_bottom) == neighbor.compute_edge_id(k_edge_index_top))
                {
                    this.bottom_neighbor = neighbor;
                    neighbor.top_neighbor = this;

                    return true;
                }

                if (this.compute_edge_id(k_edge_index_left) == neighbor.compute_edge_id(k_edge_index_right))
                {
                    this.left_neighbor = neighbor;
                    neighbor.right_neighbor = this;

                    return true;
                }

                if (this.compute_edge_id(k_edge_index_right) == neighbor.compute_edge_id(k_edge_index_left))
                {
                    this.right_neighbor = neighbor;
                    neighbor.left_neighbor = this;

                    return true;
                }

                return false;
            }

            public void align_neighbors()
            {
                foreach (c_tile neighbor in neighbors)
                {
                    if (try_align(neighbor)) continue;

                    neighbor.rotate();
                    if (try_align(neighbor)) continue;

                    neighbor.rotate();
                    if (try_align(neighbor)) continue;

                    neighbor.rotate();
                    if (try_align(neighbor)) continue;

                    neighbor.flip();
                    if (try_align(neighbor)) continue;

                    neighbor.rotate();
                    if (try_align(neighbor)) continue;

                    neighbor.rotate();
                    if (try_align(neighbor)) continue;

                    neighbor.rotate();
                    if (try_align(neighbor)) continue;

                    throw new Exception("Neighbor alignment failed");
                }

                foreach (c_tile neighbor in neighbors)
                {
                    if (neighbor.unaligned_neighbor_count() > 0)
                    {
                        neighbor.align_neighbors();
                    }
                }
            }
        }

        internal abstract class c_search_target
        {
            public (int, int)[] positions;
            public int max_row;
            public int max_column;
        }

        internal class c_sea_monster_search_target : c_search_target
        {
            public c_sea_monster_search_target()
            {
                List<(int, int)> positions_list = new List<(int, int)>();

                //             1111111111
                //   01234567890123456789
                // 0                   # 
                // 1 #    ##    ##    ###
                // 2  #  #  #  #  #  #   

                positions_list.Add((0, 18));

                positions_list.Add((1, 0));
                positions_list.Add((1, 5));
                positions_list.Add((1, 6));
                positions_list.Add((1, 11));
                positions_list.Add((1, 12));
                positions_list.Add((1, 17));
                positions_list.Add((1, 18));
                positions_list.Add((1, 19));

                positions_list.Add((2, 1));
                positions_list.Add((2, 4));
                positions_list.Add((2, 7));
                positions_list.Add((2, 10));
                positions_list.Add((2, 13));
                positions_list.Add((2, 16));

                positions = positions_list.ToArray();

                max_row = 2;
                max_column = 19;
            }
        }

        internal class c_combined_tile
        {
            private e_grid_value[][] grid;

            public c_combined_tile(c_tile source)
            {
                // go to the top left
                while (source.top_neighbor != null)
                {
                    source = source.top_neighbor;
                }
                while (source.left_neighbor != null)
                {
                    source = source.left_neighbor;
                }

                List<e_grid_value[]> grid_list = new List<e_grid_value[]>();

                // Loop through each row of tiles
                for (c_tile row_start_tile = source;
                    row_start_tile != null;
                    row_start_tile = row_start_tile.bottom_neighbor)
                {
                    // In each row of tiles, loop through each row of grid positions
                    for (int row = 1; row < 9; row++)
                    {
                        List<e_grid_value> grid_row = new List<e_grid_value>();

                        // Loop through each tile in this row of tiles
                        for (c_tile current = row_start_tile;
                            current != null;
                            current = current.right_neighbor)
                        {
                            // In each tile, loop through each column of grid positions
                            for (int column = 1; column < 9; column++)
                            {
                                e_grid_value value = current.get_grid_position(row, column) ? e_grid_value.on : e_grid_value.off;

                                grid_row.Add(value);
                            }
                        }

                        grid_list.Add(grid_row.ToArray());
                    }

                }

                grid = grid_list.ToArray();
            }

            private bool try_search(c_search_target search_target)
            {
                bool found_target = false;

                for (int row = 0; row < grid.Length - search_target.max_row; row++)
                {
                    for (int column = 0; column < grid[row].Length - search_target.max_column; column++)
                    {
                        if (search_target.positions.All(search_position => grid[row + search_position.Item1][column + search_position.Item2] != e_grid_value.off))
                        {
                            foreach ((int, int) search_position in search_target.positions)
                            {
                                grid[row + search_position.Item1][column + search_position.Item2] = e_grid_value.monster;
                            }

                            found_target = true;
                        }
                    }
                }

                return found_target;
            }

            public void search_for(c_search_target search_target)
            {
                if (try_search(search_target)) return;

                grid = grid.rotate();
                if (try_search(search_target)) return;

                grid = grid.rotate();
                if (try_search(search_target)) return;

                grid = grid.rotate();
                if (try_search(search_target)) return;

                grid = grid.flip();
                if (try_search(search_target)) return;

                grid = grid.rotate();
                if (try_search(search_target)) return;

                grid = grid.rotate();
                if (try_search(search_target)) return;

                grid = grid.rotate();
                if (try_search(search_target)) return;
            }

            public int get_untargeted_count()
            {
                return grid.Aggregate(0, (sum, grid_row) => sum + grid_row.Aggregate(0, (sum, grid_position) => sum + (grid_position == e_grid_value.on ? 1 : 0)));
            }

            public void display()
            {
                grid.display(value => value.display());
            }
        }

        internal static c_tile[] parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            List<c_tile> tiles = new List<c_tile>();

            while (input_reader.has_more_lines())
            {
                tiles.Add(new c_tile(input_reader));
            }

            return tiles.ToArray();
        }

        public static void Part_1(
            string input,
            bool pretty)
        {
            c_tile[] tiles = parse_input(input, pretty);

            // Compute each tile's neighbor count

            for (int i = 0; i < tiles.Length - 1; i++)
            {
                for (int j = i + 1; j < tiles.Length; j++)
                {
                    c_tile.check_for_neighbors(tiles[i], tiles[j]);
                }
            }

            // Find the sum of the IDs of the tiles in the corners of the map.
            /// The corners are the only tiles with exactly two neighbors.

            UInt64 result = tiles
                .Where(tile => tile.neighbor_count() == 2)
                .Select(tile => (UInt64)tile.id)
                .Aggregate((x, y) => x * y);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", result);
            Console.ResetColor();
        }

        public static void Part_2(
            string input,
            bool pretty)
        {
            c_tile[] tiles = parse_input(input, pretty);

            // Compute each tile's neighbor count

            for (int i = 0; i < tiles.Length - 1; i++)
            {
                for (int j = i + 1; j < tiles.Length; j++)
                {
                    c_tile.check_for_neighbors(tiles[i], tiles[j]);
                }
            }

            tiles[0].align_neighbors();

            c_combined_tile combined_tile = new c_combined_tile(tiles[0]);

            combined_tile.display();

            combined_tile.search_for(new c_sea_monster_search_target());

            combined_tile.display();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", combined_tile.get_untargeted_count());
            Console.ResetColor();
        }
    }
}
