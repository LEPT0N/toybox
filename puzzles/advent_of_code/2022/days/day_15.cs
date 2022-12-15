using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using advent_of_code_common.input_reader;
using advent_of_code_common.int_math;

namespace advent_of_code_2022.days
{
    internal class day_15
    {
        [DebuggerDisplay("({position}) ({nearest_beacon}) {distance_to_nearest_beacon}", Type = "c_sensor")]
        internal class c_sensor
        {
            public readonly c_vector position;
            public readonly c_vector nearest_beacon;
            public readonly int distance_to_nearest_beacon;

            public c_sensor(c_vector p, c_vector n)
            {
                position = p;
                nearest_beacon = n;
                distance_to_nearest_beacon = position.taxi_distance(nearest_beacon);
            }

            public c_vector get_last_hit_below(c_vector coordinate)
            {
                int x_diff = Math.Abs(coordinate.x - position.x);
                int y_diff = coordinate.y - position.y;

                return new c_vector(coordinate.x, coordinate.y + distance_to_nearest_beacon - x_diff - y_diff);
            }
        }

        internal static Regex input_regex = new Regex(@"^Sensor at x=(-?\d+), y=(-?\d+): closest beacon is at x=(-?\d+), y=(-?\d+)$");

        internal static c_sensor[] parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            List<c_sensor> sensors = new List<c_sensor>();

            while (input_reader.has_more_lines())
            {
                string input_line = input_reader.read_line();
                Match match = input_regex.Match(input_line);

                if (!match.Success)
                {
                    throw new Exception("Bad input");
                }

                sensors.Add(new c_sensor(
                    new c_vector(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value)),
                    new c_vector(int.Parse(match.Groups[3].Value), int.Parse(match.Groups[4].Value))));
            }

            return sensors.ToArray();
        }

        public static void part_1(
            string input,
            bool pretty)
        {
            c_sensor[] sensors = parse_input(input, pretty);

            int min_x = sensors.Min(sensor => sensor.position.x - sensor.distance_to_nearest_beacon);
            int max_x = sensors.Max(sensor => sensor.position.x + sensor.distance_to_nearest_beacon);

            int result = 0;

            for (c_vector coordinate = new c_vector(min_x, 2000000); coordinate.x <= max_x; coordinate.x++)
            {
                if (sensors.Any(sensor => sensor.position.taxi_distance(coordinate) <= sensor.distance_to_nearest_beacon) &&
                    !sensors.Any(sensor => sensor.nearest_beacon.equal_to(coordinate)))
                {
                    result++;
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", result);
            Console.ResetColor();
        }

        public static void part_2(
            string input,
            bool pretty)
        {
            c_sensor[] sensors = parse_input(input, pretty);

            c_vector result_vector = null;

            int max = 4000000;

            for (c_vector coordinate = new c_vector(0, 0); coordinate.x <= max && result_vector == null; coordinate.x++)
            {
                for (coordinate.y = 0; coordinate.y <= max && result_vector == null; coordinate.y++)
                {
                    // Brute force. Works, but too slow.
                    //if (!sensors.Any(sensor => sensor.position.taxi_distance(coordinate) <= sensor.distance_to_nearest_beacon))
                    //{
                    //    result_vector = new c_vector(coordinate);
                    //}

                    IEnumerable<c_sensor> sensors_hitting_coordinate = sensors.Where(sensor => sensor.position.taxi_distance(coordinate) <= sensor.distance_to_nearest_beacon);

                    if (sensors_hitting_coordinate.Any())
                    {
                        // If there was overlap, skip ahead to the next potentially-available spot.
                        coordinate.y = sensors_hitting_coordinate.Select(sensor => sensor.get_last_hit_below(coordinate)).Max(position => position.y);
                    }
                    else
                    {
                        result_vector = new c_vector(coordinate);
                    }
                }
            }

            UInt64 result = (UInt64)result_vector.x;
            result *= 4000000;
            result += (UInt64)result_vector.y;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", result);
            Console.ResetColor();
        }
    }
}
