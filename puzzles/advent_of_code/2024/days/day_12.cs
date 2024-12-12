using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.extensions;
using advent_of_code_common.input_reader;
using advent_of_code_common.int_math;

namespace advent_of_code_2024.days
{
    internal class day_12
    {
        [DebuggerDisplay("{plant_type} {position} {grouped}", Type = "c_plot")]
        internal class c_plot
        {
            public readonly char plant_type;
            public readonly c_vector position;

            public bool grouped;
            public bool[] fences;

            public c_plot(
                char t,
                c_vector p)
            {
                plant_type = t;
                position = new c_vector(p);
                grouped = false;
                fences = Enumerable.Repeat(false, 4).ToArray();
            }
        }

        internal static c_plot[][] parse_input(
            in c_input_reader input_reader,
            in bool pretty)
        {
            List<c_plot[]> plots = new List<c_plot[]>();

            c_vector position = new c_vector();

            while (input_reader.has_more_lines())
            {
                List<c_plot> plot_row = new List<c_plot>();

                position.col = 0;

                foreach (char c in input_reader.read_line())
                {
                    plot_row.Add(new c_plot(c, position));

                    position.col++;
                }

                plots.Add(plot_row.ToArray());

                position.row++;
            }

            return plots.ToArray();
        }

        internal static readonly c_vector[] k_neighbor_directions =
        {
            new c_vector(-1, 0),
            new c_vector(1, 0),
            new c_vector(0, -1),
            new c_vector(0, 1),
        };

        internal static (int, int) measure_group_1(
            c_plot[][] plots,
            c_vector position)
        {
            int area = 1;
            int perimeter = 0;

            plots[position.row][position.col].grouped = true;

            foreach (c_vector neighbor_direction in k_neighbor_directions)
            {
                c_vector neighbor_position = position.add(neighbor_direction);

                if (plots.is_valid_index(neighbor_position) &&
                    plots[position.row][position.col].plant_type == plots[neighbor_position.row][neighbor_position.col].plant_type)
                {
                    if (!plots[neighbor_position.row][neighbor_position.col].grouped)
                    {
                        (int neighbor_area, int neighbor_perimeter) = measure_group_1(plots, neighbor_position);

                        area += neighbor_area;
                        perimeter += neighbor_perimeter;
                    }
                }
                else
                {
                    perimeter++;
                }
            }


            return (area, perimeter);
        }

        public static void part_1(
            c_input_reader input_reader,
            bool pretty)
        {
            c_plot[][] plots = parse_input(input_reader, pretty);

            c_vector position = new c_vector();

            int result = 0;

            for (position.row = 0; position.row < plots.Length; position.row++)
            {
                for (position.col = 0; position.col < plots[position.row].Length; position.col++)
                {
                    if (!plots[position.row][position.col].grouped)
                    {
                        (int area, int perimeter) = measure_group_1(plots, position);

                        result += area * perimeter;
                    }
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }

        internal class c_neighbor
        {
            public readonly c_vector neighbor_direction;
            public readonly c_vector fence_direction;

            public c_neighbor(
                c_vector n,
                c_vector d)
            {
                neighbor_direction = n;
                fence_direction = d;
            }
        }

        internal static readonly c_neighbor[] k_neighbors =
        {
            new c_neighbor(
                new c_vector(-1, 0),
                new c_vector(0, 1)),

            new c_neighbor(
                new c_vector(1, 0),
                new c_vector(0, 1)),

            new c_neighbor(
                new c_vector(0, -1),
                new c_vector(1, 0)),

            new c_neighbor(
                new c_vector(0, 1),
                new c_vector(1, 0)),
        };

        internal static (int, int) measure_group_2(
            c_plot[][] plots,
            c_vector position)
        {
            int area = 1;
            int perimeter = 0;

            plots[position.row][position.col].grouped = true;

            for (int neighbor_index = 0; neighbor_index < k_neighbors.Length; neighbor_index++)
            {
                c_vector neighbor_position = position.add(k_neighbors[neighbor_index].neighbor_direction);

                if (plots.is_valid_index(neighbor_position) &&
                    plots[position.row][position.col].plant_type == plots[neighbor_position.row][neighbor_position.col].plant_type)
                {
                    if (!plots[neighbor_position.row][neighbor_position.col].grouped)
                    {
                        (int neighbor_area, int neighbor_perimeter) = measure_group_2(plots, neighbor_position);

                        area += neighbor_area;
                        perimeter += neighbor_perimeter;
                    }
                }
                else
                {
                    if (plots[position.row][position.col].fences[neighbor_index] == false)
                    {
                        // If no fence has been marked at this boundary, then mark the fence as far as we can in either direction and increment perimeter by 1.

                        c_vector fence_position = new c_vector(position);

                        while (plots.is_valid_index(fence_position))
                        {
                            c_vector outsider_position = fence_position.add(k_neighbors[neighbor_index].neighbor_direction);

                            if (plots[position.row][position.col].plant_type == plots[fence_position.row][fence_position.col].plant_type &&
                                (!plots.is_valid_index(outsider_position) || plots[position.row][position.col].plant_type != plots[outsider_position.row][outsider_position.col].plant_type))
                            {
                                plots[fence_position.row][fence_position.col].fences[neighbor_index] = true;

                                fence_position = fence_position.add(k_neighbors[neighbor_index].fence_direction);
                            }
                            else
                            {
                                break;
                            }
                        }

                        fence_position = new c_vector(position);

                        while (plots.is_valid_index(fence_position))
                        {
                            c_vector outsider_position = fence_position.add(k_neighbors[neighbor_index].neighbor_direction);

                            if (plots[position.row][position.col].plant_type == plots[fence_position.row][fence_position.col].plant_type &&
                                (!plots.is_valid_index(outsider_position) || plots[position.row][position.col].plant_type != plots[outsider_position.row][outsider_position.col].plant_type))
                            {
                                plots[fence_position.row][fence_position.col].fences[neighbor_index] = true;

                                fence_position = fence_position.subtract(k_neighbors[neighbor_index].fence_direction);
                            }
                            else
                            {
                                break;
                            }
                        }

                        perimeter++;
                    }
                }
            }


            return (area, perimeter);
        }

        public static void part_2(
            c_input_reader input_reader,
            bool pretty)
        {
            c_plot[][] plots = parse_input(input_reader, pretty);

            c_vector position = new c_vector();

            int result = 0;

            for (position.row = 0; position.row < plots.Length; position.row++)
            {
                for (position.col = 0; position.col < plots[position.row].Length; position.col++)
                {
                    if (!plots[position.row][position.col].grouped)
                    {
                        (int area, int perimeter) = measure_group_2(plots, position);

                        result += area * perimeter;
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
