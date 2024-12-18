using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using advent_of_code_common.display_helpers;
using advent_of_code_common.extensions;
using advent_of_code_common.input_reader;
using advent_of_code_common.int_math;
using advent_of_code_common.min_heap;

namespace advent_of_code_2024.days
{
    internal class day_18
    {
        [DebuggerDisplay("{position} {blocked} {distance_to_end} {on_best_path}", Type = "c_cell")]
        internal class c_cell
        {
            public c_vector position;
            public bool blocked = false;

            public int distance_to_end = int.MaxValue;
            public e_direction direction_to_end = e_direction.none;
            public bool on_best_path = false;

            public void display()
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

            public Color get_png_color()
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

        [DebuggerDisplay("", Type = "c_cell_comparer")]
        public class c_cell_comparer : IComparer<c_cell>
        {
            public int Compare(
                c_cell a,
                c_cell b)
            {
                return a.distance_to_end.CompareTo(b.distance_to_end);
            }
        }

        internal static (int, c_vector[], c_cell[,], c_cell, c_cell) parse_input(
            in c_input_reader input_reader,
            in bool pretty)
        {
            int[] bounds_input = input_reader
                .read_line()
                .Split(',')
                .Select(s => int.Parse(s))
                .ToArray();
            c_rectangle bounds = new c_rectangle(
                new c_vector(),
                new c_vector(bounds_input[1], bounds_input[0]));

            int count = int.Parse(input_reader.read_line());

            input_reader.read_line();

            List<c_vector> blocks = new List<c_vector>();

            while (input_reader.has_more_lines())
            {
                int[] block_input = input_reader
                    .read_line()
                    .Split(',')
                    .Select(s => int.Parse(s))
                    .ToArray();

                blocks.Add(new c_vector(block_input[1], block_input[0]));
            }

            c_cell[,] grid = new c_cell[bounds.height + 1, bounds.width + 1];

            for (int row = 0; row <= bounds.height; row++)
            {
                for (int col = 0; col <= bounds.width; col++)
                {
                    grid[row, col] = new c_cell();
                    grid[row, col].position = new c_vector(row, col);
                }
            }

            c_cell start = grid[0, 0];
            c_cell end = grid[bounds.height, bounds.width];

            if (pretty)
            {
                grid.display(cell => cell.display());
            }

            return (count, blocks.ToArray(), grid, start, end);
        }

        internal static e_direction[] k_cell_neighbors =
        {
            e_direction.up,
            e_direction.down,
            e_direction.left,
            e_direction.right,
        };

        public static void compute_best_path(
            c_cell[,] grid,
            c_cell start,
            c_cell end)
        {
            grid.for_each(cell =>
            {
                cell.distance_to_end = int.MaxValue;
                cell.direction_to_end= e_direction.none;
                cell.on_best_path = false;
            });

            end.distance_to_end = 0;

            c_min_heap<c_cell> min_heap = new c_min_heap<c_cell>(new c_cell_comparer());
            min_heap.add(end);

            while (!min_heap.empty())
            {
                c_cell cell = min_heap.remove();

                foreach (e_direction direction in k_cell_neighbors)
                {
                    c_cell neighbor = grid.try_get_index(cell.position.add(direction));

                    if (neighbor != null &&
                        !neighbor.blocked &&
                        neighbor.distance_to_end > cell.distance_to_end + 1)
                    {
                        neighbor.distance_to_end = cell.distance_to_end + 1;
                        neighbor.direction_to_end = c_int_math.rotate(direction, e_angle.angle_180);
                        min_heap.add(neighbor);
                    }
                }
            }

            if (start.direction_to_end != e_direction.none)
            {
                c_cell path = start;
                while (path != end)
                {
                    path.on_best_path = true;

                    path = grid.try_get_index(path.position.add(path.direction_to_end));
                }
            }
        }

        internal static void display_result(
            c_cell[,] grid,
            c_cell start,
            c_cell end)
        {
                grid.display(cell =>
                {
                    if (cell == start)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write('S');
                    }
                    else if (cell == end)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write('E');
                    }
                    else
                    {
                        cell.display();
                    }
                });
        }

        internal static void create_result_png(
            c_cell[,] grid,
            c_cell start,
            c_cell end,
            int part)
        {
            if (main.options.Contains("png"))
            {
                Bitmap bitmap = grid.create_bitmap(10, cell =>
                {
                    if (cell == start || cell == end)
                    {
                        return Color.White;
                    }
                    else
                    {
                        return cell.get_png_color();
                    }
                });

                Directory.CreateDirectory("output");

                bitmap.Save($"output\\day_18_part_{part}.png", ImageFormat.Png);
            }
        }

        public static void part_1(
            c_input_reader input_reader,
            bool pretty)
        {
            (int initial_block_count, c_vector[] blocks, c_cell[,] grid, c_cell start, c_cell end) = parse_input(input_reader, pretty);

            for (int i = 0; i < initial_block_count; i++)
            {
                c_vector block = blocks[i];

                grid[block.row, block.col].blocked = true;
            }

            if (pretty)
            {
                grid.display(cell => cell.display());
            }

            compute_best_path(grid, start, end);

            if (pretty)
            {
                display_result(grid, start, end);
            }

            if (main.options.Contains("png"))
            {
                create_result_png(grid, start, end, 1);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {start.distance_to_end}");
            Console.ResetColor();
        }

        public static void part_2(
            c_input_reader input_reader,
            bool pretty)
        {
            (int initial_block_count, c_vector[] blocks, c_cell[,] grid, c_cell start, c_cell end) = parse_input(input_reader, pretty);

            int blocks_placed;

            for (blocks_placed = 0; blocks_placed < initial_block_count; blocks_placed++)
            {
                c_vector block = blocks[blocks_placed];

                grid[block.row, block.col].blocked = true;
            }

            if (pretty)
            {
                grid.display(cell => cell.display());
            }

            compute_best_path(grid, start, end);

            while (start.direction_to_end != e_direction.none)
            {
                c_vector block = blocks[blocks_placed];
                grid[block.row, block.col].blocked = true;
                blocks_placed++;

                compute_best_path(grid, start, end);

                if (pretty)
                {
                    Console.WriteLine($"Placed {blocks_placed} blocks");
                    display_result(grid, start, end);
                }
            }

            display_result(grid, start, end);

            if (main.options.Contains("png"))
            {
                create_result_png(grid, start, end, 2);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {blocks[blocks_placed - 1].col},{blocks[blocks_placed - 1].row}");
            Console.ResetColor();
        }

        // NOT 2920
    }
}
