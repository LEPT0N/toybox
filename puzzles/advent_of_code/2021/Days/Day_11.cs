using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace advent_of_code_2021.Days
{
    internal class Day_11
    {
        [DebuggerDisplay("[{row}][{col}] = {energy}", Type = "c_octopus")]
        internal class c_octopus
        {
            public readonly int row;
            public readonly int col;
            public int energy { get; private set; }

            public c_octopus(int r, int c, int initial_energy)
            {
                row = r;
                col = c;
                energy = initial_energy;
            }

            public void step()
            {
                energy = (energy + 1) % 10;
            }

            public void boost()
            {
                if (!flashed())
                {
                    step();
                }
            }

            public bool flashed()
            {
                return energy == 0;
            }
        }

        public static c_octopus[][] parse_input(string input)
        {
            c_input_reader input_reader = new c_input_reader(input);
            int row = 0;
            List<c_octopus[]> octopi_row_list = new List<c_octopus[]>();

            while (input_reader.has_more_lines())
            {
                string input_line = input_reader.read_line();

                List<c_octopus> octopi_list = new List<c_octopus>();

                int col = 0;
                foreach (int initial_energy in input_line.Select(x => x - '0'))
                {
                    octopi_list.Add(new c_octopus(row, col, initial_energy));
                    col++;
                }

                octopi_row_list.Add(octopi_list.ToArray());
                row++;
            }

            return octopi_row_list.ToArray();
        }

        public static void print_octopi(string title, ref c_octopus[][] octopi)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(title);

            foreach(c_octopus[] octopi_row in octopi)
            {
                foreach(c_octopus octopus in octopi_row)
                {
                    if (octopus.flashed())
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    }

                    Console.Write(octopus.energy);
                }

                Console.WriteLine();
            }

            Console.ResetColor();
        }

        public static int[,] k_neighbor_offsets =
        {
            {-1, -1},
            {-1, 0},
            {-1, 1},
            {0, -1},
            {0, 1},
            {1, -1},
            {1, 0},
            {1, 1},
        };

        public static List<c_octopus> get_neighbors(ref c_octopus[][] octopi, c_octopus octopus)
        {
            List<c_octopus> neighbors = new List<c_octopus>();

            for (int neighbor_index = 0; neighbor_index < k_neighbor_offsets.GetLength(0); neighbor_index++)
            {
                int neighbor_row = octopus.row + k_neighbor_offsets[neighbor_index, 0];
                int neighbor_col = octopus.col + k_neighbor_offsets[neighbor_index, 1];

                if (neighbor_row >= 0 && neighbor_row < octopi.Length &&
                    neighbor_col >= 0 && neighbor_col < octopi[neighbor_row].Length)
                {
                    neighbors.Add(octopi[neighbor_row][neighbor_col]);
                }
            }

            return neighbors;
        }

        public static int step_simulation(ref c_octopus[][] octopi)
        {
            int flash_count = 0;

            Queue<c_octopus> flashed = new Queue<c_octopus>();

            // First, the energy level of each octopus increases by 1.
            for (int row = 0; row < octopi.Length; row++)
            {
                for (int col = 0; col < octopi[row].Length; col++)
                {
                    octopi[row][col].step();

                    if (octopi[row][col].flashed())
                    {
                        flash_count++;
                        flashed.Enqueue(octopi[row][col]);
                    }
                }
            }

            // Then, any octopus with an energy level greater than 9 flashes. This
            // increases the energy level of all adjacent octopuses by 1, including
            // octopuses that are diagonally adjacent. If this causes an octopus to
            // have an energy level greater than 9, it also flashes.This process
            // continues as long as new octopuses keep having their energy level
            // increased beyond 9. (An octopus can only flash at most once per step.)

            while (flashed.Count > 0)
            {
                c_octopus octopus = flashed.Dequeue();

                // This octopus flashed, so look at all neighbors
                foreach (c_octopus neighbor in get_neighbors(ref octopi, octopus))
                {
                    // If the neighbor hasn't flashed yet, boost them.
                    if (!neighbor.flashed())
                    {
                        neighbor.boost();

                        if (neighbor.flashed())
                        {
                            flash_count++;
                            flashed.Enqueue(neighbor);
                        }
                    }
                }
            }

            return flash_count;
        }

        public static void Part_1(string input, bool pretty)
        {
            c_octopus[][] octopi = parse_input(input);

            print_octopi("Before any steps:", ref octopi);

            int flash_count = 0;

            // Run the simulation through each step
            for (int step = 1; step <= 100; step++)
            {
                flash_count += step_simulation(ref octopi);

                if (step % 10 == 0)
                {
                    print_octopi("After step " + step + ":", ref octopi);
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Total Flashes = {0}", flash_count);
            Console.ResetColor();
        }

        public static void Part_2(string input, bool pretty)
        {
            c_octopus[][] octopi = parse_input(input);

            print_octopi("Before any steps:", ref octopi);

            int total_octopi = octopi.Length * octopi[0].Length;
            int step_count = 0;
            int flash_count = 0;

            // Run the simulation through each step
            while (flash_count != total_octopi)
            {
                step_count++;
                flash_count = step_simulation(ref octopi);

                if (step_count % 10 == 0)
                {
                    print_octopi("After step " + step_count + ":", ref octopi);
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Total Steps for all octopi to flash = {0}", step_count);
            Console.ResetColor();

            print_octopi("Final:", ref octopi);
        }
    }
}
