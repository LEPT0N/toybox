using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.int_math;
using advent_of_code_common.input_reader;
using advent_of_code_common.display_helpers;

namespace advent_of_code_2024.days
{
    internal class day_06
    {
        internal enum e_cell_state
        {
            empty,
            blocked,
            visited,
            guard_up,
            guard_down,
            guard_left,
            guard_right,
        }

        [DebuggerDisplay("{state}", Type = "c_cell_state")]
        internal class c_cell_state
        {
            public e_cell_state state { get; set; }
            private bool visited_connects_up { get; set; } = false;
            private bool visited_connects_down { get; set; } = false;
            private bool visited_connects_left { get; set; } = false;
            private bool visited_connects_right { get; set; } = false;

            private bool guard_left_up { get; set; } = false;
            private bool guard_left_down { get; set; } = false;
            private bool guard_left_left { get; set; } = false;
            private bool guard_left_right { get; set; } = false;

            public c_cell_state(char c)
            {
                switch (c)
                {
                    case '^':
                    case '.':
                        state = e_cell_state.empty;
                        break;

                    case '#':
                        state = e_cell_state.blocked;
                        break;

                    default:
                        throw new Exception($"Unexpected input on the board: '{c}'");
                }
            }

            public c_cell_state(c_cell_state other_cell)
            {
                state = other_cell.state;

                visited_connects_up = other_cell.visited_connects_up;
                visited_connects_down = other_cell.visited_connects_down;
                visited_connects_left = other_cell.visited_connects_left;
                visited_connects_right = other_cell.visited_connects_right;

                guard_left_up = other_cell.guard_left_up;
                guard_left_down = other_cell.guard_left_down;
                guard_left_left = other_cell.guard_left_left;
                guard_left_right = other_cell.guard_left_right;
            }

            public void display()
            {
                ConsoleColor guard_color = ConsoleColor.Cyan;

                switch (state)
                {
                    case e_cell_state.empty:
                        Console.Write(' ');
                        break;

                    case e_cell_state.blocked:
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.DarkYellow;
                        Console.Write('#');
                        Console.ResetColor();
                        break;

                    case e_cell_state.visited:

                        char visited_char;

                        switch (visited_connects_up, visited_connects_down, visited_connects_left, visited_connects_right)
                        {
                            case (true, true, true, true): visited_char = special_characters.k_box_icon_all_four; break;

                            case (false, true, true, true): visited_char = special_characters.k_box_icon_all_but_up; break;
                            case (true, false, true, true): visited_char = special_characters.k_box_icon_all_but_down; break;
                            case (true, true, false, true): visited_char = special_characters.k_box_icon_all_but_left; break;
                            case (true, true, true, false): visited_char = special_characters.k_box_icon_all_but_right; break;

                            case (true, true, false, false): visited_char = special_characters.k_box_icon_up_and_down; break;
                            case (true, false, true, false): visited_char = special_characters.k_box_icon_up_and_left; break;
                            case (false, true, true, false): visited_char = special_characters.k_box_icon_down_and_left; break;
                            case (true, false, false, true): visited_char = special_characters.k_box_icon_up_and_right; break;
                            case (false, true, false, true): visited_char = special_characters.k_box_icon_down_and_right; break;
                            case (false, false, true, true): visited_char = special_characters.k_box_icon_left_and_right; break;

                            case (true, false, false, false): visited_char = 'o'; break;
                            case (false, true, false, false): visited_char = 'o'; break;
                            case (false, false, true, false): visited_char = 'o'; break;
                            case (false, false, false, true): visited_char = 'o'; break;

                            default:
                                throw new Exception("a visited cell wasn't visited?");
                        }

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(visited_char);
                        Console.ResetColor();
                        break;

                    case e_cell_state.guard_up:
                        Console.ForegroundColor = guard_color;
                        Console.Write('^');
                        break;

                    case e_cell_state.guard_down:
                        Console.ForegroundColor = guard_color;
                        Console.Write('v');
                        break;

                    case e_cell_state.guard_left:
                        Console.ForegroundColor = guard_color;
                        Console.Write('<');
                        break;

                    case e_cell_state.guard_right:
                        Console.ForegroundColor = guard_color;
                        Console.Write('>');
                        break;

                    default:
                        throw new Exception($"Invalid cell state: '{state}'");
                }
            }

            public void guard_leaves(e_direction direction)
            {
                state = e_cell_state.visited;

                switch (direction)
                {
                    case e_direction.up:
                        visited_connects_up = true;
                        guard_left_up = true;
                        break;

                    case e_direction.down:
                        visited_connects_down = true;
                        guard_left_down = true;
                        break;

                    case e_direction.left:
                        visited_connects_left = true;
                        guard_left_left = true;
                        break;

                    case e_direction.right:
                        visited_connects_right = true;
                        guard_left_right = true;
                        break;

                    default:
                        throw new Exception($"Unexpected direction: '{direction}'");
                }
            }

            public void guard_enters(e_direction direction)
            {
                switch (direction)
                {
                    case e_direction.up:
                        state = e_cell_state.guard_up;
                        visited_connects_down = true;
                        break;

                    case e_direction.down:
                        state = e_cell_state.guard_down;
                        visited_connects_up = true;
                        break;

                    case e_direction.left:
                        state = e_cell_state.guard_left;
                        visited_connects_right = true;
                        break;

                    case e_direction.right:
                        state = e_cell_state.guard_right;
                        visited_connects_left = true;
                        break;

                    default:
                        throw new Exception($"Unexpected direction: '{direction}'");
                }
            }

            public bool visited_already(e_direction direction)
            {
                switch (direction)
                {
                    case e_direction.up: return guard_left_up;
                    case e_direction.down: return guard_left_down;
                    case e_direction.left: return guard_left_left;
                    case e_direction.right: return guard_left_right;

                    default:
                        throw new Exception($"Unexpected direction: '{direction}'");
                }
            }
        }

        [DebuggerDisplay("{guard_position} {guard_direction}", Type = "c_type")]
        internal class c_board
        {
            private c_cell_state[][] cells;

            private c_vector guard_position;
            private e_direction guard_direction;

            public int rows { get { return cells.Length; } }
            public int cols { get { return cells[0].Length; } }

            public e_cell_state get_cell_state(int row, int col)
            {
                return cells[row][col].state;
            }

            public void set_cell_state(int row, int col, e_cell_state cell_state)
            {
                cells[row][col].state = cell_state;
            }

            public c_board(c_input_reader input_reader)
            {
                List<c_cell_state[]> cell_rows = new List<c_cell_state[]>();

                for (int row = 0; input_reader.has_more_lines(); row++)
                {
                    List<c_cell_state> cell_row = new List<c_cell_state>();

                    string input_line = input_reader.read_line();

                    for (int col = 0; col < input_line.Length; col++)
                    {
                        c_cell_state cell_state = new c_cell_state(input_line[col]);

                        if (input_line[col] == '^')
                        {
                            cell_state.state = e_cell_state.guard_up;
                            guard_direction = e_direction.up;
                            guard_position = new c_vector(col, row);
                        }

                        cell_row.Add(cell_state);
                    }

                    cell_rows.Add(cell_row.ToArray());
                }

                cells = cell_rows.ToArray();
            }

            public c_board(c_board other_board)
            {
                guard_position = new c_vector(other_board.guard_position);
                guard_direction = other_board.guard_direction;

                cells = new c_cell_state[other_board.cells.Length][];

                for (int row = 0; row < cells.Length; row++)
                {
                    cells[row] = new c_cell_state[other_board.cells[row].Length];

                    for (int col = 0; col < cells.Length; col++)
                    {
                        cells[row][col] = new c_cell_state(other_board.cells[row][col]);
                    }
                }
            }

            private bool guard_exists()
            {
                return guard_position != null;
            }

            private c_vector increment(c_vector position, e_direction direction)
            {
                c_vector result = new c_vector(position);

                switch (direction)
                {
                    case e_direction.up:
                        result.y--;
                        break;

                    case e_direction.down:
                        result.y++;
                        break;

                    case e_direction.left:
                        result.x--;
                        break;

                    case e_direction.right:
                        result.x++;
                        break;
                }

                return result;
            }

            private bool is_valid_position(c_vector position)
            {
                return position.y >= 0
                    && position.y < cells.Length
                    && position.x >= 0
                    && position.x < cells[0].Length;
            }

            private bool is_empty_position(c_vector position)
            {
                return is_valid_position(position)
                    && (cells[position.y][position.x].state == e_cell_state.empty
                        || cells[position.y][position.x].state == e_cell_state.visited);
            }

            private e_cell_state direction_to_cell_state(e_direction direction)
            {
                switch (direction)
                {
                    case e_direction.up: return e_cell_state.guard_up;
                    case e_direction.down: return e_cell_state.guard_down;
                    case e_direction.left: return e_cell_state.guard_left;
                    case e_direction.right: return e_cell_state.guard_right;

                    default:
                        throw new Exception($"Invalid direction {direction}");
                }
            }

            private void walk_guard_straight_ahead()
            {
                c_vector new_position;

                // Walk forward as far as we can.

                for (new_position = increment(guard_position, guard_direction);
                    is_empty_position(new_position);
                    new_position = increment(guard_position, guard_direction))
                {
                    cells[new_position.y][new_position.x].guard_enters(guard_direction);
                    cells[guard_position.y][guard_position.x].guard_leaves(guard_direction);

                    guard_position = new_position;
                }

                if (is_valid_position(new_position))
                {
                    // We hit a block, so turn.

                    guard_direction = c_int_math.rotate(guard_direction, e_angle.angle_270);

                    cells[guard_position.y][guard_position.x].state = direction_to_cell_state(guard_direction);
                }
                else
                {
                    // We ran off the board.

                    cells[guard_position.y][guard_position.x].guard_leaves(guard_direction);
                    guard_position = null;
                }
            }

            public bool try_walk_guard_to_exit()
            {
                while (guard_exists())
                {
                    walk_guard_straight_ahead();

                    if (guard_exists() && cells[guard_position.y][guard_position.x].visited_already(guard_direction))
                    {
                        // If the guard still exists and he has already walked forward from the current spot, then we hit a loop.

                        return false;
                    }

                    // display();
                }

                // The guard ran off the board and is gone.

                return true;
            }

            public int get_visited_cell_count()
            {
                return cells
                    .Sum(cell_row => cell_row
                        .Where(cell => cell.state != e_cell_state.empty && cell.state != e_cell_state.blocked)
                        .Count());
            }

            public void display()
            {
                cells.display(display_cell);
            }

            private void display_cell(c_cell_state cell_state)
            {
                cell_state.display();
            }
        }

        public static void part_1(
            string input,
            bool pretty)
        {
            c_board board = new c_board(new c_input_reader(input));

            if (pretty)
            {
                board.display();
            }

            board.try_walk_guard_to_exit();

            if (pretty)
            {
                board.display();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {board.get_visited_cell_count()}");
            Console.ResetColor();
        }

        public static void part_2(
            string input,
            bool pretty)
        {
            c_board starting_board = new c_board(new c_input_reader(input));

            // Generate the initial path to the exit. The visited spots here are where we can try putting the new blocks.

            c_board initial_walked_board = new c_board(starting_board);
            initial_walked_board.try_walk_guard_to_exit();

            if (pretty)
            {
                Console.WriteLine("Initial board:");
                starting_board.display();

                Console.WriteLine("initial walked board:");
                initial_walked_board.display();
            }

            int result = 0;

            if (pretty)
            {
                Console.WriteLine("infinite loop boards:");
            }

            // Try putting a block on each visited spot in the first journey.

            for (int row = 0; row < initial_walked_board.rows; row++)
            {
                for (int col = 0; col < initial_walked_board.cols; col++)
                {
                    if (initial_walked_board.get_cell_state(row, col) == e_cell_state.visited)
                    {
                        c_board walked_board = new c_board(starting_board);
                        walked_board.set_cell_state(row, col, e_cell_state.blocked);

                        // If a walk doesn't make it to the exit, then we found a loop.

                        if (!walked_board.try_walk_guard_to_exit())
                        {
                            result++;

                            if (pretty)
                            {
                                walked_board.display();
                            }
                        }
                    }
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }
    }
}
