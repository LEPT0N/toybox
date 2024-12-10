using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.display_helpers;
using advent_of_code_common.input_reader;
using advent_of_code_common.int_math;

namespace advent_of_code_2024.days
{
    internal class day_10
    {
        public const int k_invalid_height = -1;
        public const int k_min_height = 0;
        public const int k_max_height = 9;

        [DebuggerDisplay("[{row}, {col}] = {height}", Type = "c_cell")]
        internal class c_cell
        {
            public readonly int row;
            public readonly int col;

            public readonly int height;
            public HashSet<c_cell> reachable_tops;
            public int valid_paths_to_a_top;

            public c_cell(int r, int c, int h)
            {
                row = r;
                col = c;
                height = h;
                reachable_tops = new HashSet<c_cell>(new c_cell_comparer());
                valid_paths_to_a_top = 0;

                if (h == k_max_height)
                {
                    reachable_tops.Add(this);
                    valid_paths_to_a_top = 1;
                }
            }

            public ConsoleColor display_color
            {
                get
                {
                    switch (height)
                    {
                        case k_invalid_height: return ConsoleColor.White;

                        case 0: return ConsoleColor.White;
                        case 1: return ConsoleColor.DarkBlue;
                        case 2: return ConsoleColor.Blue;
                        case 3: return ConsoleColor.Cyan;
                        case 4: return ConsoleColor.DarkGreen;
                        case 5: return ConsoleColor.Green;
                        case 6: return ConsoleColor.DarkYellow;
                        case 7: return ConsoleColor.Yellow;
                        case 8: return ConsoleColor.Red;
                        case 9: return ConsoleColor.DarkRed;

                        default: throw new Exception($"Invalid height {height}");
                    }
                }
            }
        }

        internal class c_cell_comparer : IEqualityComparer<c_cell>
        {
            public bool Equals(c_cell a, c_cell b)
            {
                return a.row == b.row
                    && a.col == b.col;
            }

            public int GetHashCode(c_cell c)
            {
                return HashCode.Combine(c.row, c.col);
            }
        }

        internal static c_cell[][] parse_input(
            in c_input_reader input_reader,
            in bool pretty)
        {
            int row = 0;
            List<c_cell[]> result = new List<c_cell[]>();

            while (input_reader.has_more_lines())
            {
                int col = 0;
                List<c_cell> result_row = new List<c_cell>();

                foreach (char c in input_reader.read_line())
                {
                    result_row.Add(new c_cell(row, col, c == '.' ? k_invalid_height : c - '0'));

                    col++;
                }

                result.Add(result_row.ToArray());

                row++;
            }

            return result.ToArray();
        }

        public static c_cell[][] part_worker(
            c_input_reader input_reader,
            bool pretty,
            Action<c_cell> display_action)
        {
            c_cell[][] board = parse_input(input_reader, pretty);

            if (pretty)
            {
                board.display(cell =>
                {
                    Console.ForegroundColor = cell.display_color;

                    if (cell.height == k_invalid_height)
                    {
                        Console.Write(' ');
                    }
                    else
                    {
                        Console.Write(cell.height);
                    }
                });

                board.display(display_action);
            }

            // Scan through each height from top to bottom.

            for (int height = k_max_height - 1; height >= k_min_height; height--)
            {
                // For a given height, look through each cell on the board at that height.

                for (c_vector position = c_vector.k_vector_zero; position.row < board.Length; position.row++)
                {
                    for (position.col = 0; position.col < board[position.row].Length; position.col++)
                    {
                        if (board[position.row][position.col].height == height)
                        {
                            // Look through all eight neighbors to that cell.
                            c_vector neighbor = new c_vector();

                            for (c_vector offset = new c_vector(-1, 0); offset.row <= 1; offset.row++)
                            {
                                neighbor.row = position.row + offset.row;

                                if (neighbor.row >= 0 && neighbor.row < board.Length)
                                {
                                    for (offset.col = -1; offset.col <= 1; offset.col++)
                                    {
                                        neighbor.col = position.col + offset.col;

                                        if (neighbor.col >= 0 && neighbor.col < board[neighbor.row].Length
                                            && (offset.row == 0 || offset.col == 0))
                                        {
                                            // If that neighbor's height is one more, then add it's reachable tops to ours.

                                            if (board[neighbor.row][neighbor.col].height == height + 1)
                                            {
                                                board[position.row][position.col].reachable_tops.UnionWith(
                                                    board[neighbor.row][neighbor.col].reachable_tops);

                                                board[position.row][position.col].valid_paths_to_a_top += board[neighbor.row][neighbor.col].valid_paths_to_a_top;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (pretty)
                {
                    board.display(display_action);
                }
            }

            return board;
        }

        public static void part_1(
            c_input_reader input_reader,
            bool pretty)
        {
            Action<c_cell> display_action = cell =>
            {
                Console.ForegroundColor = cell.display_color;

                if (cell.reachable_tops.Any())
                {
                    Console.Write(cell.reachable_tops.Count);
                }
                else
                {
                    Console.Write(' ');
                }
            };

            c_cell[][] board = part_worker(input_reader, pretty, display_action);

            int result = board
                .Sum(row => row
                    .Where(cell => cell.height == 0)
                    .Sum(cell => cell.reachable_tops.Count));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }

        public static void part_2(
            c_input_reader input_reader,
            bool pretty)
        {
            Action<c_cell> display_action = cell =>
            {
                Console.ForegroundColor = cell.display_color;

                if (cell.height == k_invalid_height)
                {
                    Console.Write(' ');
                }
                else if (cell.valid_paths_to_a_top > 9)
                {
                    Console.Write('!');
                }
                else if (cell.valid_paths_to_a_top > 0)
                {
                    Console.Write(cell.valid_paths_to_a_top);
                }
                else
                {
                    Console.Write(' ');
                }
            };

            c_cell[][] board = part_worker(input_reader, pretty, display_action);

            int result = board
                .Sum(row => row
                    .Where(cell => cell.height == 0)
                    .Sum(cell => cell.valid_paths_to_a_top));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }
    }
}
