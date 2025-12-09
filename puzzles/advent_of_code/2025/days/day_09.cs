using advent_of_code_common.input_reader;
using advent_of_code_common.int_math;
using System;
using System.Collections.Generic;
using System.Linq;

namespace advent_of_code_2025.days
{
    internal class day_09
    {
        internal static c_vector[] parse_input(
            in c_input_reader input_reader,
            in bool pretty)
        {
            List<c_vector> result = new List<c_vector>();

            while (input_reader.has_more_lines())
            {
                IEnumerable<int> line = input_reader.read_line().Split(',').Select(s => int.Parse(s));

                result.Add(new c_vector(line.First(), line.Last()));
            }

            return result.ToArray();
        }

        internal static Int64 taxi_area(c_vector tile_a, c_vector tile_b)
        {
            // Special use case of taxi_area since we're dealing in int32 but the result can be int64.

            return (Math.Abs(tile_a.x - tile_b.x) + 1L)
                * (Math.Abs(tile_a.y - tile_b.y) + 1L)
                * (Math.Abs(tile_a.z - tile_b.z) + 1L);
        }

        public static void part_1(
            c_input_reader input_reader,
            bool pretty)
        {
            c_vector[] tiles = parse_input(input_reader, pretty);

            Int64 result = 0;

            // Loop through all tile pairs, finding the pair with the biggest area.

            for (int i = 0; i < tiles.Length - 1; i++)
            {
                for (int j = i + 1; j < tiles.Length; j++)
                {
                    Int64 distance = taxi_area(tiles[i], tiles[j]);

                    if (distance > result)
                    {
                        result = distance;
                    }
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }

        internal static c_rectangle[] get_bounding_lines(
            c_vector[] tiles)
        {
            List<c_rectangle> bounds = new List<c_rectangle>();

            for (int i = 0; i < tiles.Length - 1; i++)
            {
                bounds.Add(new c_rectangle(tiles[i], tiles[i + 1]));
            }

            bounds.Add(new c_rectangle(tiles[0], tiles[tiles.Length - 1]));

            return bounds.ToArray();
        }

        internal static bool any_lines_intersect(
            c_vector tile_a,
            c_vector tile_b,
            c_rectangle[] lines)
        {
            c_rectangle rectangle = new c_rectangle(tile_a, tile_b);

            return lines.Any(line =>
            {
                // Same as c_rectangle.strictly_intersects but only in two dimensions.

                return rectangle.min.x < line.max.x
                && rectangle.max.x > line.min.x
                && rectangle.min.y < line.max.y
                && rectangle.max.y > line.min.y;
            });
        }

        public static void part_2(
            c_input_reader input_reader,
            bool pretty)
        {
            c_vector[] tiles = parse_input(input_reader, pretty);

            // create rectangles for each bounding line of our meta shape.

            c_rectangle[] lines = get_bounding_lines(tiles);

            Int64 result = 0;

            // Loop through all tile pairs, finding the pair with the biggest area.

            for (int i = 0; i < tiles.Length - 1; i++)
            {
                for (int j = i + 1; j < tiles.Length; j++)
                {
                    // Ignore tile pairs who's rectangle overlaps with one of the bounding lines.

                    if (!any_lines_intersect(tiles[i], tiles[j], lines))
                    {
                        Int64 distance = taxi_area(tiles[i], tiles[j]);

                        if (distance > result)
                        {
                            result = distance;
                        }
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
