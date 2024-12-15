using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using advent_of_code_common.display_helpers;
using advent_of_code_common.input_reader;
using advent_of_code_common.int_math;

namespace advent_of_code_2024.days
{
    internal class day_15
    {
        internal static readonly c_vector k_up = new c_vector(0, -1);
        internal static readonly c_vector k_down = new c_vector(0, 1);
        internal static readonly c_vector k_left = new c_vector(-1, 0);
        internal static readonly c_vector k_right = new c_vector(1, 0);

        internal enum e_cell_state
        {
            empty,
            wall,
            box,
            box_left,
            box_right,
            robot,
        }

        internal static void display_cell(e_cell_state cell)
        {
            switch (cell)
            {
                case e_cell_state.empty:
                    Console.Write(' ');
                    break;

                case e_cell_state.wall:
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write('#');
                    break;

                case e_cell_state.box:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write('O');
                    break;

                case e_cell_state.box_left:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write('[');
                    break;

                case e_cell_state.box_right:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write(']');
                    break;

                case e_cell_state.robot:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write('@');
                    break;
            }
        }

        internal static (c_vector, e_cell_state[][], c_vector[]) parse_input(
            in c_input_reader input_reader,
            in bool wide,
            in bool pretty)
        {
            c_vector robot = new c_vector();

            List<e_cell_state[]> cells = new List<e_cell_state[]>();
            int row = 0;

            while (!string.IsNullOrEmpty(input_reader.peek_line()))
            {
                List<e_cell_state> cell_row = new List<e_cell_state>();
                int col = 0;

                foreach (char input_char in input_reader.read_line())
                {
                    e_cell_state cell_state;

                    switch (input_char)
                    {
                        case '.':
                            cell_state = e_cell_state.empty;
                            break;

                        case '#':
                            cell_state = e_cell_state.wall;
                            break;

                        case 'O':
                            cell_state = e_cell_state.box;
                            break;

                        case '@':
                            cell_state = e_cell_state.robot;
                            robot.x = col;
                            robot.y = row;
                            break;

                        default:
                            throw new Exception($"Bad cell input {input_char}");
                    }

                    if (wide)
                    {
                        switch (cell_state)
                        {
                            case e_cell_state.empty:
                                cell_row.Add(e_cell_state.empty);
                                cell_row.Add(e_cell_state.empty);
                                break;

                            case e_cell_state.wall:
                                cell_row.Add(e_cell_state.wall);
                                cell_row.Add(e_cell_state.wall);
                                break;

                            case e_cell_state.box:
                                cell_row.Add(e_cell_state.box_left);
                                cell_row.Add(e_cell_state.box_right);
                                break;

                            case e_cell_state.robot:
                                cell_row.Add(e_cell_state.robot);
                                cell_row.Add(e_cell_state.empty);
                                break;
                        }

                        col++;
                    }
                    else
                    {
                        cell_row.Add(cell_state);
                    }

                    col++;
                }

                cells.Add(cell_row.ToArray());

                row++;
            }

            input_reader.read_line();

            List<c_vector> directions = new List<c_vector>();

            while (input_reader.has_more_lines())
            {
                foreach (char input_char in input_reader.read_line())
                {
                    c_vector direction;

                    switch (input_char)
                    {
                        case '^': direction = k_up; break;
                        case 'v': direction = k_down; break;
                        case '<': direction = k_left; break;
                        case '>': direction = k_right; break;

                        default: throw new Exception($"Bad direction input {input_char}");
                    }

                    directions.Add(direction);
                }
            }

            return (robot, cells.ToArray(), directions.ToArray());
        }

        internal static int calculate_result(e_cell_state[][] cells)
        {
            int result = 0;

            for (int row = 0; row < cells.Length; row++)
            {
                for (int col = 0; col < cells[row].Length; col++)
                {
                    if (cells[row][col] == e_cell_state.box ||
                        cells[row][col] == e_cell_state.box_left)
                    {
                        result += 100 * row + col;
                    }
                }
            }

            return result;
        }

        internal static void create_bitmap(
            e_cell_state[][] cells,
            string file_name)
        {
            Bitmap bitmap = cells.create_bitmap(10, cell_state =>
            {
                Color cell_color = Color.Black;

                switch (cell_state)
                {
                    case e_cell_state.wall: cell_color = Color.White; break;
                    case e_cell_state.box: cell_color = Color.DarkRed; break;
                    case e_cell_state.box_left: cell_color = Color.DarkRed; break;
                    case e_cell_state.box_right: cell_color = Color.DarkRed; break;
                    case e_cell_state.robot: cell_color = Color.Green; break;
                }

                return cell_color;
            });

            Directory.CreateDirectory("output");

            bitmap.Save($"output\\{file_name}");
        }

        public static void part_1(
            c_input_reader input_reader,
            bool pretty)
        {
            (c_vector robot, e_cell_state[][] cells, c_vector[] directions) = parse_input(input_reader, false, pretty);

            if (pretty)
            {
                Console.WriteLine("Initial state:");
                cells.display(cell => display_cell(cell));
            }

            foreach (c_vector direction in directions)
            {
                c_vector start_position = new c_vector(robot);
                c_vector end_position = start_position.add(direction);

                // Scan to the first non-box item.

                while (cells[end_position.y][end_position.x] == e_cell_state.box)
                {
                    end_position = end_position.add(direction);
                }

                if (cells[end_position.y][end_position.x] == e_cell_state.wall)
                {
                    // We hit a wall. No move is possible.
                }
                else
                {
                    if (cells[end_position.y][end_position.x] != e_cell_state.empty)
                    {
                        throw new Exception("invalid state sliding boxes");
                    }

                    // We found a non-negative number of boxes followed by an empty space.
                    // Move each box forward one position.

                    while (!end_position.equal_to(start_position))
                    {
                        c_vector previous_position = end_position.subtract(direction);

                        cells[end_position.y][end_position.x] = cells[previous_position.y][previous_position.x];

                        end_position = previous_position;
                    }

                    // Then move the robot.

                    cells[start_position.y][start_position.x] = e_cell_state.empty;
                    robot = start_position.add(direction);
                }

                if (pretty)
                {
                    cells.display(cell => display_cell(cell));
                }
            }

            if (!pretty)
            {
                cells.display(cell => display_cell(cell));
            }

            if (main.options.Contains("bmp"))
            {
                create_bitmap(cells, "day_15_part_1.bmp");
            }

            int result = calculate_result(cells);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }

        internal static bool can_move_box(
            e_cell_state[][] cells,
            c_vector start_position,
            c_vector direction)
        {
            e_cell_state box_state = cells[start_position.y][start_position.x];

            if (box_state == e_cell_state.empty)
            {
                // This is an empty space. It can 'move out of the way'.

                return true;
            }
            else if (box_state == e_cell_state.wall)
            {
                // This is a wall. It can not move.

                return false;
            }

            // The current box can move as long as whatever is past it can move.

            if ((box_state == e_cell_state.box_left && direction == k_right) ||
                (box_state == e_cell_state.box_right && direction == k_left))
            {
                // Something is past this box. Check if it can move.

                c_vector next_position = start_position.add(direction.scale(2));

                return can_move_box(cells, next_position, direction);
            }
            else if ((box_state == e_cell_state.box_left || box_state == e_cell_state.box_right) &&
                (direction == k_up || direction == k_down))
            {
                c_vector left_box_position = box_state == e_cell_state.box_left ?
                    start_position :
                    start_position.add(k_left);
                c_vector right_box_position = left_box_position.add(k_right);

                c_vector next_left_position = left_box_position.add(direction);
                c_vector next_right_position = right_box_position.add(direction);

                e_cell_state next_left_state = cells[next_left_position.y][next_left_position.x];
                e_cell_state next_right_state = cells[next_right_position.y][next_right_position.x];

                if (next_left_state == e_cell_state.box_left && next_right_state == e_cell_state.box_right)
                {
                    // Another box is past this box. Check if it can move.

                    return can_move_box(cells, next_left_position, direction);
                }
                else
                {
                    // Something is past this box. Check if they can move.

                    return can_move_box(cells, next_left_position, direction) &&
                        can_move_box(cells, next_right_position, direction);
                }
            }
            else
            {
                throw new Exception("Invalid box move");
            }
        }

        internal static void move_box(
            e_cell_state[][] cells,
            c_vector start_position,
            c_vector direction)
        {
            // Assumes the move can be made.

            e_cell_state box_state = cells[start_position.y][start_position.x];

            if (box_state == e_cell_state.empty || box_state == e_cell_state.wall)
            {
                // This is not a box. There is nothing to move.

                return;
            }

            c_vector left_box_position = box_state == e_cell_state.box_left ?
                start_position :
                start_position.add(k_left);
            c_vector right_box_position = left_box_position.add(k_right);

            c_vector next_left_position = left_box_position.add(direction);
            c_vector next_right_position = right_box_position.add(direction);

            if ((box_state == e_cell_state.box_left && direction == k_right) ||
                (box_state == e_cell_state.box_right && direction == k_left))
            {
                // Past this box is something else. Move that first.

                c_vector next_position = start_position.add(direction.scale(2));

                move_box(cells, next_position, direction);
            }
            else if ((box_state == e_cell_state.box_left || box_state == e_cell_state.box_right) &&
                (direction == k_up || direction == k_down))
            {
                e_cell_state next_left_state = cells[next_left_position.y][next_left_position.x];
                e_cell_state next_right_state = cells[next_right_position.y][next_right_position.x];

                if (next_left_state == e_cell_state.box_left && next_right_state == e_cell_state.box_right)
                {
                    // Past this box is a single box. Move that first.

                    move_box(cells, next_left_position, direction);
                }
                else
                {
                    // Past this box is something else. Move them first.

                    move_box(cells, next_left_position, direction);
                    move_box(cells, next_right_position, direction);
                }
            }
            else
            {
                throw new Exception("Invalid box move");
            }

            // Move the current box

            cells[left_box_position.y][left_box_position.x] = e_cell_state.empty;
            cells[right_box_position.y][right_box_position.x] = e_cell_state.empty;

            cells[next_left_position.y][next_left_position.x] = e_cell_state.box_left;
            cells[next_right_position.y][next_right_position.x] = e_cell_state.box_right;
        }

        public static void part_2(
            c_input_reader input_reader,
            bool pretty)
        {
            (c_vector robot, e_cell_state[][] cells, c_vector[] directions) = parse_input(input_reader, true, pretty);

            if (pretty)
            {
                Console.WriteLine("Initial state:");
                cells.display(cell => display_cell(cell));
            }

            foreach (c_vector direction in directions)
            {
                c_vector start_position = new c_vector(robot);
                c_vector end_position = start_position.add(direction);

                if (cells[end_position.y][end_position.x] == e_cell_state.wall)
                {
                    // No move possible, go to the next direction
                }
                else if (cells[end_position.y][end_position.x] == e_cell_state.empty)
                {
                    // Empty space. Trivial move.

                    cells[start_position.y][start_position.x] = e_cell_state.empty;
                    cells[end_position.y][end_position.x] = e_cell_state.robot;
                    robot = end_position;
                }
                else
                {
                    if (cells[end_position.y][end_position.x] != e_cell_state.box_left &&
                        cells[end_position.y][end_position.x] != e_cell_state.box_right)
                    {
                        throw new Exception("invalid state sliding boxes");
                    }

                    // Hit a box. Can we move?

                    if (can_move_box(cells, end_position, direction))
                    {
                        // We can! Move the box (and recursively everything past it), then move the robot.

                        move_box(cells, end_position, direction);

                        cells[start_position.y][start_position.x] = e_cell_state.empty;
                        cells[end_position.y][end_position.x] = e_cell_state.robot;
                        robot = end_position;
                    }
                }

                if (pretty)
                {
                    cells.display(cell => display_cell(cell));
                }
            }

            if (!pretty)
            {
                cells.display(cell => display_cell(cell));
            }

            if (main.options.Contains("bmp"))
            {
                create_bitmap(cells, "day_15_part_2.bmp");
            }

            int result = calculate_result(cells);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }
    }
}
