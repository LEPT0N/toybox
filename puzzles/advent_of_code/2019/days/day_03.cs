using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.input_reader;
using advent_of_code_common.int_math;
using advent_of_code_common.display_helpers;
using advent_of_code_common.extensions;

namespace advent_of_code_2019.days
{
    internal class day_03
    {
        [DebuggerDisplay("{position} (dist = {total_distance})", Type = "c_intersection")]
        internal class c_intersection
        {
            public readonly c_vector position;
            public readonly int total_distance;

            public c_intersection(c_vector p, int t_d)
            {
                position = p;
                total_distance = t_d;
            }
        }

        [DebuggerDisplay("{size} (start = {start}) (dist = {base_distance})", Type = "c_line_segment")]
        internal class c_line_segment
        {
            public c_vector start;
            public int base_distance;
            public c_rectangle size;

            public c_line_segment(c_vector s, int d, c_vector e)
            {
                start = s;
                base_distance = d;
                size = new c_rectangle(s, e);
            }

            public c_intersection[] get_intersections(c_line_segment other)
            {
                c_rectangle intersection = size.get_intersection(other.size);
                if (intersection != null)
                {
                    return intersection.to_int_vectors().Select(i => new c_intersection(i, base_distance + i.taxi_distance(start) + other.base_distance + i.taxi_distance(other.start))).ToArray();
                }
                else
                {
                    return new c_intersection[0];
                }
            }
        }

        internal static c_line_segment[] parse_path(string input_path)
        {
            List<c_line_segment> results = new List<c_line_segment>();
            c_vector previous_position = new c_vector();
            int total_distance = 0;

            foreach (string input_direction in input_path.Split(","))
            {
                c_vector new_position = new c_vector(previous_position);

                int input_direction_magnitude = int.Parse(input_direction.Substring(1));

                switch (input_direction[0])
                {
                    case 'L': new_position.x -= input_direction_magnitude; break;
                    case 'R': new_position.x += input_direction_magnitude; break;
                    case 'D': new_position.y -= input_direction_magnitude; break;
                    case 'U': new_position.y += input_direction_magnitude; break;
                }

                results.Add(new c_line_segment(previous_position, total_distance, new_position));

                total_distance += new_position.taxi_distance(previous_position);
                previous_position = new_position;
            }

            return results.ToArray();
        }

        internal static (c_line_segment[], c_line_segment[]) parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);
            string first_input_path = input_reader.read_line();
            string second_input_path = input_reader.read_line();

            return (parse_path(first_input_path), parse_path(second_input_path));
        }

        internal static c_intersection[] find_intersections(
            c_line_segment[] first_path,
            c_line_segment[] second_path)
        {
            List<c_intersection> results = new List<c_intersection>();

            foreach (c_line_segment first_line in first_path)
            {
                foreach (c_line_segment second_line in second_path)
                {
                    results.AddRange(first_line.get_intersections(second_line));
                }
            }

            return results.ToArray();
        }

        internal static void display(c_line_segment[] line_segments)
        {
            c_rectangle bounds = new c_rectangle();

            foreach (c_line_segment line_segment in line_segments)
            {
                bounds.expand_to_fit(line_segment.size);
            }

            char[,] output = new char[bounds.width + 1, bounds.height + 1];
            output.fill(' ');

            foreach (c_line_segment line_segment in line_segments)
            {
                foreach (c_vector point in line_segment.size.to_int_vectors())
                {
                    output[point.x - bounds.min.x, point.y - bounds.min.y] = '-';
                }
            }

            output[0 - bounds.min.x, 0 - bounds.min.y] = 'O';

            output.display();
        }

        public static void part_1(
            string input,
            bool pretty)
        {
            (c_line_segment[] first_path, c_line_segment[] second_path) = parse_input(input, pretty);

            if (pretty)
            {
                display(first_path.Concat(second_path).ToArray());
            }

            c_intersection[] intersections = find_intersections(first_path, second_path);

            int[] distances = intersections.Select(i => i.position.taxi_magnitude()).OrderBy(x => x).ToArray();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", distances[1]);
            Console.ResetColor();
        }

        public static void part_2(
            string input,
            bool pretty)
        {
            (c_line_segment[] first_path, c_line_segment[] second_path) = parse_input(input, pretty);

            if (pretty)
            {
                display(first_path.Concat(second_path).ToArray());
            }

            c_intersection[] intersections = find_intersections(first_path, second_path);

            int[] distances = intersections.Select(i => i.total_distance).OrderBy(x => x).ToArray();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", distances[1]);
            Console.ResetColor();
        }
    }
}
