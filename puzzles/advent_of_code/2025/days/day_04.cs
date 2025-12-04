using advent_of_code_common.extensions;
using advent_of_code_common.grid;
using advent_of_code_common.input_reader;
using advent_of_code_common.int_math;
using System;
using System.Diagnostics;
using System.Drawing;

namespace advent_of_code_2025.days
{
    internal class day_04
    {
        internal class c_department_cell : c_grid_cell
        {
            public bool movable;
            public bool removed;

            public c_department_cell(
            c_vector p,
            bool b) : base(p, b)
            {
                movable = false;
            }

            public override void display()
            {
                ConsoleColor color = ConsoleColor.Black;
                char value = ' ';

                if (removed)
                {
                    color = ConsoleColor.DarkBlue;
                    value = 'x';
                }
                else if (movable)
                {
                    color = ConsoleColor.Blue;
                    value = 'X';
                }
                else if (blocked)
                {
                    color = ConsoleColor.Gray;
                    value = '@';
                }

                Console.ForegroundColor = color;
                Console.Write(value);
            }

            public override Color get_picture_color()
            {
                if (removed)
                {
                    return Color.DarkBlue;
                }
                else if (movable)
                {
                    return Color.Blue;
                }
                else if (blocked)
                {
                    return Color.Gray;
                }
                else
                {
                    return Color.Black;
                }
            }
        }

        [DebuggerDisplay("", Type = "c_department")]
        internal class c_department : c_grid<c_department_cell>
        {
            public c_department(
                c_input_reader input_reader)
            {
                initialize(input_reader);
            }

            protected override c_department_cell create_cell(
                char input_char,
                c_vector position)
            {
                return new c_department_cell(position, input_char == '@');
            }

            public void mark_movable_cells()
            {
                cells.for_each(cell =>
                {
                    if (cell.blocked)
                    {
                        int blocked_neighbors = 0;

                        c_vector neighbor = new c_vector();

                        for (neighbor.row = cell.position.row - 1; neighbor.row <= cell.position.row + 1; neighbor.row++)
                        {
                            for (neighbor.col = cell.position.col - 1; neighbor.col <= cell.position.col + 1; neighbor.col++)
                            {
                                if (cells.is_valid_index(neighbor) && cells[neighbor.row][neighbor.col].blocked)
                                {
                                    blocked_neighbors++;
                                }
                            }
                        }

                        if (blocked_neighbors <= 4)
                        {
                            cell.movable = true;
                        }
                    }
                });
            }

            public int movable_cell_count()
            {
                return cells.count(cell => cell.movable);
            }

            public void remove_movable()
            {
                cells.for_each(cell =>
                {
                    if (cell.movable)
                    {
                        cell.blocked = false;
                        cell.movable = false;
                        cell.removed = true;
                    }
                });
            }
        }

        public static void part_1(
            c_input_reader input_reader,
            bool pretty)
        {
            c_department department = new c_department(input_reader);

            if (pretty)
            {
                department.display();
            }

            department.mark_movable_cells();

            if (pretty)
            {
                department.display();
            }

            if (main.options.Contains("png"))
            {
                department.create_picture($"day_20_part_1");
            }

            int result = department.movable_cell_count();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }

        public static void part_2(
            c_input_reader input_reader,
            bool pretty)
        {
            c_department department = new c_department(input_reader);

            if (pretty)
            {
                department.display();
            }

            int total_removed = 0;
            bool any_removed = false;

            do
            {
                department.mark_movable_cells();

                int removed = department.movable_cell_count();

                total_removed += removed;

                any_removed = (removed != 0);

                if (pretty)
                {
                    department.display();
                }

                department.remove_movable();

            } while (any_removed);

            if (main.options.Contains("png"))
            {
                department.create_picture($"day_20_part_2");
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {total_removed}");
            Console.ResetColor();
        }
    }
}
