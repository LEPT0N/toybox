using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using advent_of_code_common.extensions;
using advent_of_code_common.input_reader;
using advent_of_code_common.int_math;

namespace advent_of_code_2022.days
{
    internal class day_18
    {
        internal enum e_cube_type
        {
            empty = 0,
            filled,
            exposed,
        }

        internal static e_cube_type[,,] parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            List<c_vector> cubes = new List<c_vector>();
            c_rectangle cube_bounds = new c_rectangle();

            while (input_reader.has_more_lines())
            {
                int[] input_values = input_reader.read_line().Split(',').Select(i => int.Parse(i)).ToArray();

                c_vector cube = new c_vector(input_values[0], input_values[1], input_values[2]);

                cube_bounds.expand_to_fit(cube);
                cubes.Add(cube);
            }

            if (pretty)
            {
                Console.WriteLine($"Found {cubes.Count} cubes");
            }

            // Pad an extra cube around all sides so that there's no edge cases in surface area checking lolololol
            // Update: also gives me somewhere to start the 'exposed' cubes for part 2.
            e_cube_type[,,] grid = new e_cube_type[cube_bounds.width + 3, cube_bounds.height + 3, cube_bounds.depth + 3];

            foreach(c_vector cube in cubes)
            {
                grid[cube.x + 1, cube.y + 1, cube.z + 1] = e_cube_type.filled;
            }

            return grid;
        }

        internal static c_vector[] k_adjascent_cubes =
        {
            new c_vector(1, 0, 0),
            new c_vector(-1, 0, 0),
            new c_vector(0, 1, 0),
            new c_vector(0, -1, 0),
            new c_vector(0, 0, 1),
            new c_vector(0, 0, -1),
        };

        internal static int compute_surface_area(
            e_cube_type[,,] grid,
            e_cube_type neighbor_type,
            bool pretty)
        {
            int checked_cubes = 0;
            int checked_sides = 0;
            int surface_area = 0;

            for (int x = 1; x < grid.GetLength(0) - 1; x++)
            {
                for (int y = 1; y < grid.GetLength(1) - 1; y++)
                {
                    for (int z = 1; z < grid.GetLength(2) - 1; z++)
                    {
                        if (grid[x, y, z] == e_cube_type.filled)
                        {
                            checked_cubes++;

                            k_adjascent_cubes.for_each(n =>
                            {
                                checked_sides++;

                                if (grid[x + n.x, y + n.y, z + n.z] == neighbor_type)
                                {
                                    surface_area++;
                                }
                            });
                        }
                    }
                }
            }

            if (pretty)
            {
                Console.WriteLine($"Checked {checked_cubes} cubes");
                Console.WriteLine($"Checked {checked_sides} sides (should be {checked_cubes * 6})");
            }

            return surface_area;
        }

        public static void part_1(
            string input,
            bool pretty)
        {
            e_cube_type[,,] grid = parse_input(input, pretty);

            int surface_area = compute_surface_area(grid, e_cube_type.empty, pretty);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", surface_area);
            Console.ResetColor();
        }

        internal static void fill_exposed(
            e_cube_type[,,] grid,
            c_vector p)
        {
            if (grid[p.x, p.y, p.z] == e_cube_type.empty)
            {
                Queue<c_vector> work_queue = new Queue<c_vector>();

                grid[p.x, p.y, p.z] = e_cube_type.exposed;
                work_queue.Enqueue(p);

                while (work_queue.Any())
                {
                    c_vector c = work_queue.Dequeue();

                    k_adjascent_cubes.for_each(o =>
                    {
                        c_vector n = c.add(o);

                        if (n.x >= 0 && n.x < grid.GetLength(0) &&
                            n.y >= 0 && n.y < grid.GetLength(1) &&
                            n.z >= 0 && n.z < grid.GetLength(2) &&
                            grid[n.x, n.y, n.z] == e_cube_type.empty)
                        {
                            grid[n.x, n.y, n.z] = e_cube_type.exposed;
                            work_queue.Enqueue(n);
                        }
                    });
                }
            }
        }

        public static void part_2(
            string input,
            bool pretty)
        {
            e_cube_type[,,] grid = parse_input(input, pretty);

            // Mark all the outside cubes (and neighbors) as 'exposed' to the outside air.

            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    fill_exposed(grid, new c_vector(x, y, 0));
                    fill_exposed(grid, new c_vector(x, y, grid.GetLength(2) - 1));
                }

                for (int z = 0; z < grid.GetLength(2); z++)
                {
                    fill_exposed(grid, new c_vector(x, 0, z));
                    fill_exposed(grid, new c_vector(x, grid.GetLength(1) - 1, z));
                }
            }

            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int z = 0; z < grid.GetLength(2); z++)
                {
                    fill_exposed(grid, new c_vector(0, y, z));
                    fill_exposed(grid, new c_vector(grid.GetLength(0) - 1, y, z));
                }
            }

            // Compute the exposed surface area.

            int surface_area = compute_surface_area(grid, e_cube_type.exposed, pretty);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", surface_area);
            Console.ResetColor();
        }
    }
}
