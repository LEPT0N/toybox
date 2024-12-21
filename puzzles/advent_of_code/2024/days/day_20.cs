using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.extensions;
using advent_of_code_common.grid;
using advent_of_code_common.input_reader;
using advent_of_code_common.int_math;

namespace advent_of_code_2024.days
{
    internal class day_20
    {
        [DebuggerDisplay("", Type = "c_racetrack")]
        internal class c_racetrack : c_grid<c_grid_cell>
        {
            public c_racetrack(
                c_input_reader input_reader)
            {
                initialize(input_reader);
            }

            protected override c_grid_cell create_cell(
                char input_char,
                c_vector position)
            {
                c_grid_cell result = new c_grid_cell(position, input_char == '#');

                switch (input_char)
                {
                    case '#':
                        break;

                    case 'S':
                        start = result;
                        break;

                    case 'E':
                        end = result;
                        break;

                    case '.':
                        break;

                    default:
                        throw new Exception($"Invalid cell input '{input_char}'");
                }

                return result;
            }

            public Dictionary<int, int> find_cheats(
                int max_cheat_distance)
            {
                Dictionary<int, int> cheats = new Dictionary<int, int>();

                cells.for_each(cell =>
                {
                    if (cell.blocked)
                    {
                        return;
                    }

                    // Find all valid neighors we could cheat to get to.

                    c_vector offset = new c_vector();
                    for (offset.row = -max_cheat_distance; offset.row <= max_cheat_distance; offset.row++)
                    {
                        int col_radius = max_cheat_distance - Math.Abs(offset.row);

                        for (offset.col = -col_radius; offset.col <= col_radius; offset.col++)
                        {
                            c_grid_cell neighbor = cells.try_get_index(cell.position.add(offset));

                            if (neighbor != null &&
                                !neighbor.blocked)
                            {
                                int distance_to_neighbor = cell.position.taxi_distance(neighbor.position);

                                if (distance_to_neighbor >= 2 &&
                                    distance_to_neighbor <= max_cheat_distance)
                                {
                                    // See if it's adventageous to cheat to get there.
                                    int cheating_distance = neighbor.distance_to_end + distance_to_neighbor;

                                    if (cheating_distance < cell.distance_to_end)
                                    {
                                        // Make note of this cheat.

                                        int distance_saved = cell.distance_to_end - cheating_distance;

                                        if (!cheats.ContainsKey(distance_saved))
                                        {
                                            cheats[distance_saved] = 0;
                                        }

                                        cheats[distance_saved]++;
                                    }
                                }
                            }
                        }
                    }
                });

                return cheats;
            }
        }

        internal static void part_worker(
            c_input_reader input_reader,
            bool pretty,
            int part,
            int max_cheat_distance)
        {
            c_racetrack racetrack = new c_racetrack(input_reader);

            if (pretty)
            {
                racetrack.display();
            }

            racetrack.compute_distances_to_end();

            if (pretty)
            {
                racetrack.display();
            }

            if (main.options.Contains("png"))
            {
                racetrack.create_picture($"day_20_part_{part}");
            }

            Dictionary<int, int> cheats = racetrack.find_cheats(max_cheat_distance);

            if (pretty)
            {
                foreach (int savings in cheats.Keys.Order())
                {
                    Console.WriteLine($"There are {cheats[savings]} cheats that save {savings} picoseconds.");
                }
            }

            int result = cheats.Where(e => e.Key >= 100).Select(e => e.Value).Sum();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }

        public static void part_1(
            c_input_reader input_reader,
            bool pretty)
        {
            part_worker(input_reader, pretty, 1, 2);
        }

        public static void part_2(
            c_input_reader input_reader,
            bool pretty)
        {
            part_worker(input_reader, pretty, 2, 20);
        }
    }
}
