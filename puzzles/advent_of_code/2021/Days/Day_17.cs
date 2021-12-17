using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

namespace advent_of_code_2021.Days
{
    internal class Day_17
    {
        internal class c_square
        {
            public int min_x;
            public int max_x;
            public int min_y;
            public int max_y;

            public c_square()
            {
                min_x = max_x = min_y = max_y = 0;
            }

            public c_square(
                string input)
            {
                Regex input_regex = new Regex(@"^target area: x=(\-?\d+)\.\.(\-?\d+), y=(\-?\d+)\.\.(\-?\d+)$");

                Match match = input_regex.Match(input);

                min_x = int.Parse(match.Groups[1].Value);
                max_x = int.Parse(match.Groups[2].Value);

                min_y = int.Parse(match.Groups[3].Value);
                max_y = int.Parse(match.Groups[4].Value);
            }

            public c_square(
                c_square other)
            {
                min_x = other.min_x;
                max_x = other.max_x;

                min_y = other.min_y;
                max_y = other.max_y;
            }

            public int row_count()
            {
                return max_y - min_y + 1;
            }

            public int column_count()
            {
                return max_x - min_x + 1;
            }

            public void expand_to_include(
                List<Point> points)
            {
                min_x = points.Aggregate(min_x, (min, point) => Math.Min(min, point.X));
                max_x = points.Aggregate(max_x, (max, point) => Math.Max(max, point.X));

                min_y = points.Aggregate(min_y, (min, point) => Math.Min(min, point.Y));
                max_y = points.Aggregate(max_y, (max, point) => Math.Max(max, point.Y));
            }
        }

        internal class c_probe
        {
            public Point position;
            public Point velocity;

            public void step()
            {
                position.X += velocity.X;
                position.Y += velocity.Y;

                velocity.Y--;

                if (velocity.X > 0)
                {
                    velocity.X--;
                }
                else if(velocity.X < 0)
                {
                    velocity.X++;
                }
            }

            public bool hit(
                c_square target_area)
            {
                return position.X >= target_area.min_x
                    && position.X <= target_area.max_x
                    && position.Y >= target_area.min_y
                    && position.Y <= target_area.max_y;
            }

            public bool overshot(
                c_square target_area)
            {
                return position.Y < target_area.min_y
                    || position.X > target_area.max_x;
            }
        }

        internal static c_square parse_input(
            string input)
        {
            c_input_reader input_reader = new c_input_reader(input);

            string input_line = input_reader.read_line();

            c_square target_area = new c_square(input_line);

            return target_area;
        }

        internal static void display_output(
            List<Point> points,
            c_square target_area)
        {
            c_square output_area = new c_square(target_area);

            output_area.expand_to_include(points);

            // Build blank output array
            char[][] output = new char[output_area.row_count()][];
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = new char[output_area.column_count()];
                for(int j = 0; j < output[i].Length; j++)
                {
                    output[i][j] = ' ';
                }
            }

            // Mark each spot in the target area
            for (int row = target_area.min_y; row <= target_area.max_y; row++)
            {
                for (int column = target_area.min_x; column <= target_area.max_x; column++)
                {
                    output[row - output_area.min_y][column - output_area.min_x] = 'T';
                }
            }

            // Mark each point
            foreach (Point point in points)
            {
                output[point.Y - output_area.min_y][point.X - output_area.min_x] = '#';
            }

            // Display the results
            for (int row = output.Length - 1; row >= 0; row--)
            {
                for (int column = 0; column < output[row].Length; column++)
                {
                    Console.Write(output[row][column]);
                }

                Console.WriteLine();
            }
        }

        internal static bool fire_probe(
            Point initial_position,
            Point initial_velocity,
            c_square target_area,
            bool pretty)
        {
            c_probe probe = new c_probe();
            probe.position = initial_position;
            probe.velocity = initial_velocity;

            List<Point> points = new List<Point>();
            Point previous_point = new Point(0, 0);
            points.Add(previous_point);

            while (!probe.hit(target_area) && !probe.overshot(target_area))
            {
                probe.step();

                points.Add(probe.position);
            }

            bool hit = probe.hit(target_area);

            if (hit)
            {
                if (pretty) display_output(points, target_area);

                int highest_y = points.Aggregate(int.MinValue, (max, point) => Math.Max(max, point.Y));

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine();
                Console.WriteLine("Hit with Initial Velocity = ({0}, {1})", initial_velocity.X, initial_velocity.Y);
                Console.WriteLine("Highest Y = {0}", highest_y);
                Console.WriteLine();
                Console.ResetColor();
            }

            return hit;
        }

        public static void Part_1(
            string input,
            bool pretty)
        {
            c_square target_area = parse_input(input);

            Point initial_position = new Point(0, 0);
            Point initial_velocity = new Point(0, 0);

            // Loop through each initial X velocity
            while (true)
            {
                initial_velocity.X++;

                // X comes to a stopping point after it moved X * (X + 1) / 2 positions.
                int final_x = initial_position.X + (initial_velocity.X * (initial_velocity.X + 1)) / 2;

                if (final_x > target_area.max_x)
                {
                    // If X comes to a rest after the target area, stop looping.
                    break;
                }
                else if (final_x < target_area.min_x)
                {
                    // If X comes to a rest before the target area, continue to the next loop
                    continue;
                }
                else
                {
                    // Now that we know X comes to a stop within the target area's X coordinates,
                    // we need to find out just how high we can set our initial Y velocity and
                    // still land in the target area.
                    //
                    // Both the example input and real input put the target below the initial position.
                    // This means that the probe must go up, down, and at some point cross exactly at
                    // the initial Y position.
                    //
                    // Given this, the highest we could possibly go implies that when we're travelling
                    // downward and cross the initial position again, we want to be going down as fast
                    // as possible. The fastest downward speed possible that still lands in the target
                    // area has to mean that the very next step takes us directly to the bottom of the
                    // target area.

                    // Our final Y position will be at the bottom of the target area.
                    int final_position_y = target_area.min_y;

                    // The Y position of the previous step will be level with the initial position.
                    int previous_position_y = initial_position.Y;

                    // The Y velocity of the final step is the difference of the last two positions.
                    int final_velocity_y = final_position_y - previous_position_y;

                    // The previous y velocity is one less downward.
                    int previous_velocity_y = final_velocity_y + 1;

                    // The initial velocity is then the opposite of that.
                    initial_velocity.Y = -previous_velocity_y;

                    // Firing the probe will display the data we're after.
                    fire_probe(
                        initial_position,
                        initial_velocity,
                        target_area,
                        pretty);
                }
            }
        }

        public static void Part_2(
            string input,
            bool pretty)
        {
            c_square target_area = parse_input(input);

            Point initial_position = new Point(0, 0);

            // Put a bounds on what initial velocities we will check
            c_square initial_velocities = new c_square();
            {
                // Minimum initial x velocity -> smallest x velocity that can possibly make it to target_area.min_x
                initial_velocities.min_x = 1;
                while (true)
                {
                    // X comes to a stopping point after it moved X * (X + 1) / 2 positions.
                    int final_x = initial_position.X + (initial_velocities.min_x * (initial_velocities.min_x + 1)) / 2;

                    if (final_x >= target_area.min_x)
                    {
                        break;
                    }
                    else
                    {
                        initial_velocities.min_x++;
                    }
                }

                // Maximum initial x velocity -> single step goes right to target_area.max_x
                initial_velocities.max_x = target_area.max_x - initial_position.X;

                // Minimum initial y velocity -> single step goes right to target_area.min_y
                initial_velocities.min_y = target_area.min_y - initial_position.Y;

                // Maximum initial y velocity -> final step goes from initial_position.Y to target_position.min_Y
                initial_velocities.max_y = initial_position.Y - target_area.min_y - 1;
            }

            // Loop through each initial velocity and see if a fired probe hits the target.
            List<Point> successful_initial_velocities = new List<Point>();

            Point initial_velocity = new Point();
            for (initial_velocity.X = initial_velocities.min_x; initial_velocity.X <= initial_velocities.max_x; initial_velocity.X++)
            {
                for (initial_velocity.Y = initial_velocities.min_y; initial_velocity.Y <= initial_velocities.max_y; initial_velocity.Y++)
                {
                    if (fire_probe(
                        initial_position,
                        initial_velocity,
                        target_area,
                        pretty))
                    {
                        successful_initial_velocities.Add(initial_velocity);
                    }
                }
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine();
            Console.WriteLine("Results = {0}", string.Join(" ", successful_initial_velocities.Select(point => string.Format("({0}, {1})", point.X, point.Y))));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Total successful initial velocities = {0}", successful_initial_velocities.Count);
            Console.ResetColor();
        }
    }
}
