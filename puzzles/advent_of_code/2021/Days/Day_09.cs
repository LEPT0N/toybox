using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace advent_of_code_2021.Days
{
    internal class Day_09
    {
        public static void Part_1(string input)
        {
            c_input_reader input_reader = new c_input_reader(input);

            int result = 0;

            List<int[]> heights_list = new List<int[]>();

            while (input_reader.has_more_lines())
            {
                string input_line = input_reader.read_line();

                heights_list.Add(input_line.Select(x => x - '0').ToArray());
            }

            int[][] heights = heights_list.ToArray();

            for (int row = 0; row < heights.Length; row++)
            {
                for (int col = 0; col < heights[row].Length; col++)
                {
                    int current_height = heights[row][col];

                    List<int> neighbor_heights = new List<int>();

                    if (row > 0)
                    {
                        neighbor_heights.Add(heights[row - 1][col]);
                    }
                    if (row < heights.Length - 1)
                    {
                        neighbor_heights.Add(heights[row + 1][col]);
                    }
                    if (col > 0)
                    {
                        neighbor_heights.Add(heights[row][col - 1]);
                    }
                    if (col < heights[row].Length - 1)
                    {
                        neighbor_heights.Add(heights[row][col + 1]);
                    }

                    if (neighbor_heights.All(neighbor_height => (current_height < neighbor_height)))
                    {
                        Console.WriteLine(
                            "Low point found: heights[{0}][{1}] = {2}",
                            row, col, current_height);

                        result += current_height + 1;
                    }
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", result);
            Console.ResetColor();
        }

        [DebuggerDisplay("[r {row}, c {col}] = {height}")]
        internal class c_location
        {
            public c_location(int r, int c, int h)
            {
                row = r;
                col = c;
                height = h;
            }

            public readonly int row;
            public readonly int col;
            public readonly int height;
        }

        public static List<c_location> get_neighbors(ref c_location[][] locations, int row, int col)
        {
            List<c_location> neighbors = new List<c_location>();

            if (row > 0)
            {
                neighbors.Add(locations[row - 1][col]);
            }
            if (row < locations.Length - 1)
            {
                neighbors.Add(locations[row + 1][col]);
            }
            if (col > 0)
            {
                neighbors.Add(locations[row][col - 1]);
            }
            if (col < locations[row].Length - 1)
            {
                neighbors.Add(locations[row][col + 1]);
            }

            return neighbors;
        }

        public static void Part_2(string input)
        {
            c_input_reader input_reader = new c_input_reader(input);

            // Read input

            List<c_location[]> locations_list = new List<c_location[]>();

            int input_row = 0;

            while (input_reader.has_more_lines())
            {
                string input_line = input_reader.read_line();

                List<c_location> location_array = new List<c_location>();

                int input_col = 0;

                foreach(char input_char in input_line)
                {
                    location_array.Add(new c_location(input_row, input_col, input_char - '0'));

                    input_col++;
                }

                locations_list.Add(location_array.ToArray());

                input_row++;
            }

            c_location[][] locations = locations_list.ToArray();

            // Find low points

            List<c_location> low_points = new List<c_location>();

            for (int row = 0; row < locations.Length; row++)
            {
                for (int col = 0; col < locations[row].Length; col++)
                {
                    c_location current = locations[row][col];

                    List<c_location> neighbors = get_neighbors(ref locations, row, col);

                    if (neighbors.All(neighbor => (current.height < neighbor.height)))
                    {
                        low_points.Add(current);
                    }
                }
            }

            // Find basins

            List<int> basins = new List<int>();

            foreach(c_location low_point in low_points)
            {
                HashSet<c_location> basin_locations = new HashSet<c_location>();
                basin_locations.Add(low_point);

                Queue<c_location> new_locations = new Queue<c_location>();
                new_locations.Enqueue(low_point);

                while (new_locations.Count > 0)
                {
                    c_location location = new_locations.Dequeue();

                    List<c_location> neighbors = get_neighbors(ref locations, location.row, location.col);

                    foreach(c_location neighbor in neighbors)
                    {
                        if (neighbor.height >= location.height &&
                            neighbor.height < 9 &&
                            !basin_locations.Contains(neighbor))
                        {
                            basin_locations.Add(neighbor);
                            new_locations.Enqueue(neighbor);
                        }
                    }
                }

                int basin_size = basin_locations.Count();

                basins.Add(basin_size);
            }

            // Output results

            basins.Sort((a, b) => b.CompareTo(a));

            int first = basins[0];
            int second = basins[1];
            int third = basins[2];

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0} + {1} + {2} = {3}",
                first, second, third,
                first * second * third);
            Console.ResetColor();
        }
    }
}
