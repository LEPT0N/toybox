using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using advent_of_code_common.display_helpers;
using advent_of_code_common.input_reader;
using advent_of_code_common.int_math;
using advent_of_code_common.min_heap;

namespace advent_of_code_2024.days
{
    internal class day_16
    {
        internal enum e_cell_state
        {
            empty,
            wall,
            start,
            end,
        }

        [DebuggerDisplay("{direction} {weight}", Type = "c_weighted_direction")]
        public struct c_weighted_direction
        {
            public e_direction direction;
            public int weight;

            public c_weighted_direction(
                e_direction d,
                int w)
            {
                direction = d;
                weight = w;
            }
        }

        [DebuggerDisplay("{state} {is_on_best_path}", Type = "c_cell")]
        internal class c_cell
        {
            public e_cell_state state;
            public Dictionary<e_direction, int> distance_to_end;
            public bool is_on_best_path;

            public c_cell(
                e_cell_state s)
            {
                state = s;
                is_on_best_path = false;

                if (state != e_cell_state.wall)
                {
                    distance_to_end = new Dictionary<e_direction, int>();

                    if (state == e_cell_state.end)
                    {
                        distance_to_end[e_direction.up] = 0;
                        distance_to_end[e_direction.down] = 0;
                        distance_to_end[e_direction.left] = 0;
                        distance_to_end[e_direction.right] = 0;
                    }
                    else
                    {
                        distance_to_end[e_direction.up] = int.MaxValue;
                        distance_to_end[e_direction.down] = int.MaxValue;
                        distance_to_end[e_direction.left] = int.MaxValue;
                        distance_to_end[e_direction.right] = int.MaxValue;
                    }
                }
            }

            public void mark_best_path(
                c_cell[][] cells,
                c_vector position,
                e_direction previous_direction)
            {
                is_on_best_path = true;

                if (distance_to_end.Values.Min() == 0)
                {
                    return;
                }

                // Assign weights to the different ways we could exit this cell.
                c_weighted_direction[] weighted_directions =
                {
                    new c_weighted_direction(previous_direction, 0),
                    new c_weighted_direction(c_int_math.rotate(previous_direction, e_angle.angle_90), 1000),
                    new c_weighted_direction(c_int_math.rotate(previous_direction, e_angle.angle_270), 1000),
                };

                // Find the actual weight to each direction.
                c_weighted_direction[] actual_weighted_directions = weighted_directions.Select(weighted_direction =>
                {
                    // Find the next cell for each weighted direction.
                    c_vector next_position = position.add(weighted_direction.direction);
                    c_cell next_cell = cells[next_position.row][next_position.col];

                    int best_next_weight = int.MaxValue;

                    if (next_cell.state != e_cell_state.wall)
                    {
                        // Assign weights to the different ways we could exit the next cell.
                        c_weighted_direction[] next_weighted_directions =
                        {
                            new c_weighted_direction(weighted_direction.direction, weighted_direction.weight),
                            new c_weighted_direction(c_int_math.rotate(weighted_direction.direction, e_angle.angle_90), 1000 + weighted_direction.weight),
                            new c_weighted_direction(c_int_math.rotate(weighted_direction.direction, e_angle.angle_270), 1000 + weighted_direction.weight),
                        };

                        // Find the cheapest way to exit the next cell.
                        best_next_weight = next_weighted_directions.Min(next_weighted_direction =>
                        {
                            return next_cell.distance_to_end[next_weighted_direction.direction] + next_weighted_direction.weight;
                        });
                    }

                    return new c_weighted_direction(weighted_direction.direction, best_next_weight);
                }).ToArray();

                // Find the directions with the minimum weight.
                int min_weight = actual_weighted_directions.Select(derp => derp.weight).Min();

                e_direction[] min_weighted_directions = actual_weighted_directions
                    .Where(weighted_direction => weighted_direction.weight == min_weight)
                    .Select(weighted_direction => weighted_direction.direction)
                    .ToArray();

                // Recurse to each of those.
                foreach (e_direction direction in min_weighted_directions)
                {
                    c_vector next_position = position.add(direction);
                    c_cell next_cell = cells[next_position.row][next_position.col];

                    next_cell.mark_best_path(cells, next_position, direction);
                }
            }

            public void display()
            {
                switch (state)
                {
                    case e_cell_state.empty:

                        if (is_on_best_path)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write('O');
                        }
                        else
                        {
                            int min_distance = distance_to_end.Values.Min();

                            if (min_distance == int.MaxValue)
                            {
                                Console.Write(' ');
                            }
                            else
                            {
                                e_direction direction = (distance_to_end.Keys.First(key => distance_to_end[key] == min_distance));

                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                Console.Write(direction.to_char());
                            }
                        }

                        break;

                    case e_cell_state.wall:
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write('#');
                        break;

                    case e_cell_state.start:
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write('S');
                        break;

                    case e_cell_state.end:
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write('E');
                        break;
                }
            }
        }

        [DebuggerDisplay("{position} {direction}", Type = "c_cell_search_state")]
        public class c_cell_search_state
        {
            public c_vector position;
            public e_direction direction;

            public c_cell_search_state(
                c_vector p,
                e_direction d)
            {
                position = p;
                direction = d;
            }
        }

        [DebuggerDisplay("", Type = "c_cell_comparer")]
        public class c_cell_comparer : IComparer<c_cell_search_state>
        {
            private c_cell[][] cells;

            public c_cell_comparer(
                c_cell[][] c)
            {
                cells = c;
            }

            public int Compare(
                c_cell_search_state a,
                c_cell_search_state b)
            {
                return cells[a.position.row][a.position.col].distance_to_end[a.direction].CompareTo(
                    cells[b.position.row][b.position.col].distance_to_end[b.direction]);
            }
        }

        internal static (c_vector, c_vector, c_cell[][]) parse_input(
            in c_input_reader input_reader,
            in bool pretty)
        {
            c_vector start = new c_vector();
            c_vector end = new c_vector();
            List<c_cell[]> cells = new List<c_cell[]>();
            int row = 0;

            while (input_reader.has_more_lines())
            {
                List<c_cell> cell_row = new List<c_cell>();
                int col = 0;

                foreach (char c in input_reader.read_line())
                {
                    e_cell_state state;

                    switch (c)
                    {
                        case '.': state = e_cell_state.empty; break;
                        case '#': state = e_cell_state.wall; break;
                        case 'S': state = e_cell_state.start; break;
                        case 'E': state = e_cell_state.end; break;

                        default: throw new Exception($"Invalid cell state '{c}'");
                    }

                    cell_row.Add(new c_cell(state));

                    if (state == e_cell_state.start)
                    {
                        start.row = row;
                        start.col = col;
                    }
                    else if (state == e_cell_state.end)
                    {
                        end.row = row;
                        end.col = col;
                    }

                    col++;
                }

                cells.Add(cell_row.ToArray());
                row++;
            }

            return (start, end, cells.ToArray());
        }

        internal static void run_search(
            c_vector end,
            c_cell[][] cells)
        {
            c_min_heap<c_cell_search_state> min_heap = new c_min_heap<c_cell_search_state>(new c_cell_comparer(cells));
            min_heap.add(new c_cell_search_state(end, e_direction.up));
            min_heap.add(new c_cell_search_state(end, e_direction.down));
            min_heap.add(new c_cell_search_state(end, e_direction.left));
            min_heap.add(new c_cell_search_state(end, e_direction.right));

            while (!min_heap.empty())
            {
                c_cell_search_state current_state = min_heap.remove();
                c_cell current_cell = cells[current_state.position.row][current_state.position.col];

                // Moved forward to get here
                {
                    c_vector previous_position = current_state.position.subtract(current_state.direction);
                    c_cell previous_cell = cells[previous_position.row][previous_position.col];

                    if (previous_cell.state != e_cell_state.wall &&
                        previous_cell.distance_to_end[current_state.direction]
                        > current_cell.distance_to_end[current_state.direction] + 1)
                    {
                        previous_cell.distance_to_end[current_state.direction] =
                            current_cell.distance_to_end[current_state.direction] + 1;

                        min_heap.add(new c_cell_search_state(previous_position, current_state.direction));
                    }
                }

                // Turned to get here
                foreach (e_angle angle in new e_angle[] { e_angle.angle_90, e_angle.angle_270 })
                {
                    e_direction previous_direction = c_int_math.rotate(current_state.direction, angle);

                    if (current_cell.distance_to_end[previous_direction]
                        > current_cell.distance_to_end[current_state.direction] + 1000)
                    {
                        current_cell.distance_to_end[previous_direction] =
                            current_cell.distance_to_end[current_state.direction] + 1000;

                        min_heap.add(new c_cell_search_state(current_state.position, previous_direction));
                    }
                }
            }
        }

        public static void part_1(
            c_input_reader input_reader,
            bool pretty)
        {
            (c_vector start, c_vector end, c_cell[][] cells) = parse_input(input_reader, pretty);

            if (pretty)
            {
                cells.display(cell => cell.display());
            }

            run_search(end, cells);

            if (pretty)
            {
                cells.display(cell => cell.display());
            }

            int result = cells[start.row][start.col].distance_to_end[e_direction.right];

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }

        public static void part_2(
            c_input_reader input_reader,
            bool pretty)
        {
            (c_vector start, c_vector end, c_cell[][] cells) = parse_input(input_reader, pretty);

            if (pretty)
            {
                cells.display(cell => cell.display());
            }

            run_search(end, cells);

            cells[start.row][start.col].mark_best_path(cells, start, e_direction.right);

            if (pretty)
            {
                cells.display(cell => cell.display());
            }

            if (main.options.Contains("png"))
            {
                Bitmap bitmap = cells.create_bitmap(10, cell =>
                {
                    switch (cell.state)
                    {
                        case e_cell_state.wall: return Color.White;
                        case e_cell_state.start: return Color.DarkGreen;
                        case e_cell_state.end: return Color.DarkRed;

                        default: return cell.is_on_best_path ? Color.DarkKhaki : Color.Black;
                    }
                });

                Directory.CreateDirectory("output");

                bitmap.Save($"output\\day_16_part_2.png", ImageFormat.Png);
            }

            int result = cells.Select(cell_row => cell_row.Where(cell => cell.is_on_best_path).Count()).Sum();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }
    }
}
