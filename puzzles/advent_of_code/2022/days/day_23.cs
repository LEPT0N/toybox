using System;
using System.Diagnostics;
using advent_of_code_common.display_helpers;
using advent_of_code_common.extensions;
using advent_of_code_common.input_reader;

namespace advent_of_code_2022.days
{
    internal class day_23
    {
        public enum e_consideration_direction
        {
            none,
            up,
            down,
            left,
            right,
            many,
        }

        [DebuggerDisplay("filled = {filled} considered_by = {considered_by}", Type = "s_position")]
        internal struct s_position
        {
            public bool filled;
            public e_consideration_direction considered_by;

            public s_position()
            {
                filled = false;
                considered_by = e_consideration_direction.none;
            }

            public void display()
            {
                if (filled)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write('#');
                }
                else
                {
                    switch (considered_by)
                    {
                        case e_consideration_direction.none:
                            Console.Write(' ');
                            break;

                        case e_consideration_direction.up:
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write('v');
                            break;

                        case e_consideration_direction.down:
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write('^');
                            break;

                        case e_consideration_direction.left:
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write('>');
                            break;

                        case e_consideration_direction.right:
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write('<');
                            break;

                        case e_consideration_direction.many:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write('X');
                            break;

                        default:
                            throw new Exception($"Unable to display position with considered_by = {considered_by}");
                    }
                }
            }
        }

        internal static s_position[,] parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            string[] input_lines = input_reader.read_all_lines();

            s_position[,] positions = new s_position[input_lines.Length, input_lines[0].Length];

            for (int row = 0; row < positions.GetLength(0); row++)
            {
                for (int col = 0; col < positions.GetLength(1); col++)
                {
                    positions[row, col] = new s_position() { filled = (input_lines[row][col] == '#') };
                }
            }

            return positions;
        }

        internal static bool has_filled_edge(s_position[,] positions)
        {
            for (int row = 0; row < positions.GetLength(0); row++)
            {
                if (positions[row, 0].filled || positions[row, positions.GetLength(1) - 1].filled)
                {
                    return true;
                }
            }

            for (int col = 0; col < positions.GetLength(1); col++)
            {
                if (positions[0, col].filled || positions[positions.GetLength(0) - 1, col].filled)
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool any_neighbors_filled(
            s_position[,] positions,
            int row, int col,
            e_consideration_direction direction)
        {
            switch (direction)
            {
                case e_consideration_direction.none:
                    return positions[row - 1, col - 1].filled
                        || positions[row - 1, col    ].filled
                        || positions[row - 1, col + 1].filled

                        || positions[row    , col - 1].filled
                        || positions[row    , col + 1].filled

                        || positions[row + 1, col - 1].filled
                        || positions[row + 1, col    ].filled
                        || positions[row + 1, col + 1].filled;

                case e_consideration_direction.up:
                    return positions[row - 1, col - 1].filled
                        || positions[row - 1, col    ].filled
                        || positions[row - 1, col + 1].filled;

                case e_consideration_direction.down:
                    return positions[row + 1, col - 1].filled
                        || positions[row + 1, col    ].filled
                        || positions[row + 1, col + 1].filled;

                case e_consideration_direction.left:
                    return positions[row - 1, col - 1].filled
                        || positions[row    , col - 1].filled
                        || positions[row + 1, col - 1].filled;

                case e_consideration_direction.right:
                    return positions[row - 1, col + 1].filled
                        || positions[row    , col + 1].filled
                        || positions[row + 1, col + 1].filled;

                default:
                    throw new Exception($"'{direction}' is not a valid neighbor direction");
            }
        }

        internal static void set_consideration(
            s_position[,] positions,
            int row, int col,
            e_consideration_direction direction)
        {
            int considered_row = row;
            int considered_col = col;
            e_consideration_direction opposite_direction;

            switch (direction)
            {
                case e_consideration_direction.up:
                    considered_row--;
                    opposite_direction = e_consideration_direction.down;
                    break;

                case e_consideration_direction.down:
                    considered_row++;
                    opposite_direction = e_consideration_direction.up;
                    break;

                case e_consideration_direction.left:
                    considered_col--;
                    opposite_direction = e_consideration_direction.right;
                    break;

                case e_consideration_direction.right:
                    considered_col++;
                    opposite_direction = e_consideration_direction.left;
                    break;

                default:
                    throw new Exception($"'{direction}' is not a valid direction to set consideration");
            }

            if (positions[considered_row, considered_col].considered_by == e_consideration_direction.none)
            {
                positions[considered_row, considered_col].considered_by = opposite_direction;
            }
            else
            {
                positions[considered_row, considered_col].considered_by = e_consideration_direction.many;
            }
        }

        internal static e_consideration_direction get_consideration_direction_from_index(int index)
        {
            switch (index % 4)
            {
                case 0: return e_consideration_direction.up;
                case 1: return e_consideration_direction.down;
                case 2: return e_consideration_direction.left;
                case 3: return e_consideration_direction.right;

                default: throw new Exception($"Bad consideration direction index {index}");
            }
        }

        internal static bool commit_considerations(s_position[,] positions, int row, int col)
        {
            int source_row = row;
            int source_col = col;

            switch (positions[row, col].considered_by)
            {
                case e_consideration_direction.none:
                    return false;

                case e_consideration_direction.many:
                    return false;

                case e_consideration_direction.up:
                    source_row--;
                    break;

                case e_consideration_direction.down:
                    source_row++;
                    break;

                case e_consideration_direction.left:
                    source_col--;
                    break;

                case e_consideration_direction.right:
                    source_col++;
                    break;

                default:
                    throw new Exception("Invalid direction.");

            }

            positions[source_row, source_col].filled = false;
            positions[row, col].filled = true;

            return true;
        }

        internal static int get_unfilled_count(s_position[,] positions)
        {
            int min_row = positions.GetLength(0);
            int max_row = 0;

            int min_col = positions.GetLength(1);
            int max_col = 0;

            for (int row = 0; row < positions.GetLength(0); row++)
            {
                for (int col = 0; col < positions.GetLength(1); col++)
                {
                    if (positions[row, col].filled)
                    {
                        min_row = Math.Min(min_row, row);
                        max_row = Math.Max(max_row, row);

                        min_col = Math.Min(min_col, col);
                        max_col = Math.Max(max_col, col);
                    }
                }
            }

            int result = 0;

            for (int row = min_row; row <= max_row; row++)
            {
                for (int col = min_col; col <= max_col; col++)
                {
                    if (!positions[row, col].filled)
                    {
                        result++;
                    }
                }
            }

            return result;
        }

        public static void part_1(
            string input,
            bool pretty)
        {
            s_position[,] positions = parse_input(input, pretty);

            int first_consideration_index = 0;

            Console.WriteLine("Initial position:");
            positions.display(p => p.display());

            // Loop for N turns
            for (int turn = 1; turn <= 10; turn++)
            {
                Console.WriteLine("------- ------- ------- ------- ------- ------- -------");

                // Expand the grid if we need to.
                if (has_filled_edge(positions))
                {
                    positions = positions.add_border(1);
                }

                // Clear considerations
                // positions.for_each(p => p.considered_by = e_consideration_direction.none);
                for (int row = 0; row < positions.GetLength(0); row++)
                {
                    for (int col = 0; col < positions.GetLength(1); col++)
                    {
                        positions[row, col].considered_by = e_consideration_direction.none;
                    }
                }

                // Loop through each position on the grid to look for movement considerations.
                for (int row = 1; row < positions.GetLength(0) - 1; row++)
                {
                    for (int col = 1; col < positions.GetLength(1) - 1; col++)
                    {
                        // Only consider moving filled positions
                        if (positions[row, col].filled)
                        {
                            // If any neighbors are filled, then start considering a move.
                            if (any_neighbors_filled(positions, row, col, e_consideration_direction.none))
                            {
                                bool considered_any = false;

                                for (int i = 0; i < 4 && !considered_any; i++)
                                {
                                    e_consideration_direction consideration_direction = get_consideration_direction_from_index(first_consideration_index + i);

                                    if (!any_neighbors_filled(positions, row, col, consideration_direction))
                                    {
                                        set_consideration(positions, row, col, consideration_direction);

                                        considered_any = true;
                                    }
                                }
                            }
                        }
                    }
                }

                Console.WriteLine($"Considerations:");
                positions.display(p => p.display());

                // Loop through each consideration and make valid moves
                for (int row = 0; row < positions.GetLength(0); row++)
                {
                    for (int col = 0; col < positions.GetLength(1); col++)
                    {
                        commit_considerations(positions, row, col);
                    }
                }

                Console.WriteLine($"After turn {turn}:");
                positions.display(p => p.display());

                // Increment the starting consideration index
                first_consideration_index = (first_consideration_index + 1) % 4;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", get_unfilled_count(positions));
            Console.ResetColor();
        }

        public static void part_2(
            string input,
            bool pretty)
        {
            s_position[,] positions = parse_input(input, pretty);

            int first_consideration_index = 0;

            Console.WriteLine("Initial position:");
            positions.display(p => p.display());

            int turn = 0;
            bool someone_moved = true;

            // Loop as long as someone moved
            while(someone_moved)
            {
                turn++;
                someone_moved = false;

                if (pretty)
                {
                    Console.WriteLine("------- ------- ------- ------- ------- ------- -------");
                }

                // Expand the grid if we need to.
                if (has_filled_edge(positions))
                {
                    positions = positions.add_border(1);
                }

                // Clear considerations
                // positions.for_each(p => p.considered_by = e_consideration_direction.none);
                for (int row = 0; row < positions.GetLength(0); row++)
                {
                    for (int col = 0; col < positions.GetLength(1); col++)
                    {
                        positions[row, col].considered_by = e_consideration_direction.none;
                    }
                }

                // Loop through each position on the grid to look for movement considerations.
                for (int row = 1; row < positions.GetLength(0) - 1; row++)
                {
                    for (int col = 1; col < positions.GetLength(1) - 1; col++)
                    {
                        // Only consider moving filled positions
                        if (positions[row, col].filled)
                        {
                            // If any neighbors are filled, then start considering a move.
                            if (any_neighbors_filled(positions, row, col, e_consideration_direction.none))
                            {
                                bool considered_any = false;

                                for (int i = 0; i < 4 && !considered_any; i++)
                                {
                                    e_consideration_direction consideration_direction = get_consideration_direction_from_index(first_consideration_index + i);

                                    if (!any_neighbors_filled(positions, row, col, consideration_direction))
                                    {
                                        set_consideration(positions, row, col, consideration_direction);

                                        considered_any = true;
                                    }
                                }
                            }
                        }
                    }
                }

                if (pretty)
                {
                    Console.WriteLine($"Considerations:");
                    positions.display(p => p.display());
                }

                // Loop through each consideration and make valid moves
                for (int row = 0; row < positions.GetLength(0); row++)
                {
                    for (int col = 0; col < positions.GetLength(1); col++)
                    {
                        if (commit_considerations(positions, row, col))
                        {
                            someone_moved = true;
                        }
                    }
                }

                if (pretty)
                {
                    Console.WriteLine($"After turn {turn}:");
                    positions.display(p => p.display());
                }

                // Increment the starting consideration index
                first_consideration_index = (first_consideration_index + 1) % 4;
            }

            Console.WriteLine("Final position:");
            positions.display(p => p.display());

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", turn);
            Console.ResetColor();
        }
    }
}
