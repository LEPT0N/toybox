
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using advent_of_code_common.display_helpers;
using advent_of_code_common.extensions;
using advent_of_code_common.input_reader;
using advent_of_code_common.int_math;
using advent_of_code_common.min_heap;

namespace advent_of_code_common.grid
{
    [DebuggerDisplay("{position} {blocked}", Type = "c_grid_cell")]
    public class c_grid_cell
    {
        public c_vector position;
        public bool blocked;

        public int distance_to_end = int.MaxValue;
        public e_direction direction_to_end = e_direction.none;
        public bool on_best_path = false;

        public c_grid_cell(
            c_vector p,
            bool b)
        {
            position = p;
            blocked = b;
        }

        public virtual void display()
        {
            ConsoleColor color = ConsoleColor.Black;
            char value = ' ';

            if (blocked)
            {
                color = ConsoleColor.Red;
                value = '#';
            }
            else if (direction_to_end != e_direction.none)
            {
                value = direction_to_end.to_char();

                color = on_best_path ? ConsoleColor.Green : ConsoleColor.DarkBlue;
            }

            Console.ForegroundColor = color;
            Console.Write(value);
        }

        public virtual Color get_picture_color()
        {
            if (blocked)
            {
                return Color.Red;
            }
            else if (on_best_path)
            {
                return Color.Green;
            }
            else if (direction_to_end != e_direction.none)
            {
                return Color.Blue;
            }
            else
            {
                return Color.Black;
            }
        }
    }

    [DebuggerDisplay("", Type = "c_grid_cell_comparer")]
    internal class c_grid_cell_comparer : IComparer<c_grid_cell>
    {
        public int Compare(
            c_grid_cell a,
            c_grid_cell b)
        {
            return a.distance_to_end.CompareTo(b.distance_to_end);
        }
    }

    [DebuggerDisplay("", Type = "c_grid")]
    public abstract class c_grid<T> where T : c_grid_cell
    {
        protected T[][] cells;

        protected T start;
        protected T end;

        protected void initialize(
            c_input_reader input_reader)
        {
            List<T[]> cell_rows = new List<T[]>();
            int row = 0;

            while (input_reader.has_more_lines())
            {
                List<T> cell_row = new List<T>();
                int col = 0;

                foreach (char input_char in input_reader.read_line())
                {
                    cell_row.Add(create_cell(input_char, new c_vector(row, col)));

                    col++;
                }

                cell_rows.Add(cell_row.ToArray());
                row++;
            }

            cells = cell_rows.ToArray();
        }

        protected abstract T create_cell(
            char input_char,
            c_vector position);

        protected static e_direction[] k_cell_neighbors =
        {
            e_direction.up,
            e_direction.down,
            e_direction.left,
            e_direction.right,
        };

        public void compute_distances_to_end()
        {
            cells.for_each(cell =>
            {
                cell.distance_to_end = int.MaxValue;
                cell.direction_to_end = e_direction.none;
                cell.on_best_path = false;
            });

            c_min_heap<T> min_heap = new c_min_heap<T>(new c_grid_cell_comparer());

            end.distance_to_end = 0;
            min_heap.add(end);

            while (!min_heap.empty())
            {
                T cell = min_heap.remove();

                foreach (e_direction direction in k_cell_neighbors)
                {
                    T neighbor = cells.try_get_index(cell.position.add(direction));

                    if (neighbor != null &&
                        !neighbor.blocked)
                    {
                        int distance_difference = distance_between(cell, neighbor);

                        if (neighbor.distance_to_end > cell.distance_to_end + distance_difference)
                        {
                            neighbor.distance_to_end = cell.distance_to_end + distance_difference;
                            neighbor.direction_to_end = c_int_math.rotate(direction, e_angle.angle_180);
                            min_heap.add(neighbor);
                        }
                    }
                }
            }

            if (start.direction_to_end != e_direction.none)
            {
                T best_path_cell = start;
                while (best_path_cell != end)
                {
                    best_path_cell.on_best_path = true;

                    best_path_cell = cells.try_get_index(best_path_cell.position.add(best_path_cell.direction_to_end));
                }
            }
        }

        protected virtual int distance_between(
            T a,
            T b)
        {
            return a.position.taxi_distance(b.position);
        }

        protected virtual void display_start()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write('S');
        }

        protected virtual void display_end()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write('E');
        }

        public void display()
        {
            cells.display(cell =>
            {
                if (cell == start)
                {
                    display_start();
                }
                else if (cell == end)
                {
                    display_end();
                }
                else
                {
                    cell.display();
                }
            });
        }

        protected virtual Color get_start_picture_color()
        {
            return Color.White;
        }

        protected virtual Color get_end_picture_color()
        {
            return Color.White;
        }

        public void create_picture(
            string filename)
        {
            Bitmap bitmap = cells.create_bitmap(10, cell =>
            {
                if (cell == start)
                {
                    return get_start_picture_color();
                }
                else if (cell == end)
                {
                    return get_end_picture_color();
                }
                else
                {
                    return cell.get_picture_color();
                }
            });

            Directory.CreateDirectory("output");

            bitmap.Save($"output\\{filename}.png", ImageFormat.Png);
        }
    }
}
