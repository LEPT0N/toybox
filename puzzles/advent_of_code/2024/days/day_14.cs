using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using advent_of_code_common.display_helpers;
using advent_of_code_common.input_reader;
using advent_of_code_common.int_math;

namespace advent_of_code_2024.days
{
    internal class day_14
    {
        [DebuggerDisplay("{position} {velocity}", Type = "c_robot")]
        internal class c_robot
        {
            public c_vector position { get; private set; }
            private readonly c_vector velocity;

            public c_robot(
                c_vector p,
                c_vector v)
            {
                position = p;
                velocity = v;
            }

            public c_vector get_position(
                c_vector bounds,
                int seconds)
            {
                c_vector final_position = position.add(velocity.scale(seconds)).mod(bounds);

                // Mod doesn't do what I want (kinda) if the value negative, so give it a boost.
                return new c_vector(
                    final_position.x + (final_position.x >= 0 ? 0 : bounds.x),
                    final_position.y + (final_position.y >= 0 ? 0 : bounds.y));
            }

            public void step(
                c_vector bounds,
                int steps = 1)
            {
                position = get_position(bounds, steps);
            }
        }

        internal static c_robot[] parse_input(
            in c_input_reader input_reader,
            in bool pretty)
        {
            List<c_robot> robots = new List<c_robot>();

            while (input_reader.has_more_lines())
            {
                string[] input_line = input_reader
                    .read_line()
                    .Substring("p=".Length)
                    .Split(" v=");

                int[] position = input_line[0]
                    .Split(",")
                    .Select(s => int.Parse(s))
                    .ToArray();

                int[] velocity = input_line[1]
                    .Split(",")
                    .Select(s => int.Parse(s))
                    .ToArray();

                robots.Add(new c_robot(
                    new c_vector(position[0], position[1]),
                    new c_vector(velocity[0], velocity[1])));
            }

            return robots.ToArray();
        }

        public static void part_1(
            c_input_reader input_reader,
            bool pretty)
        {
            c_robot[] robots = parse_input(input_reader, pretty);
            int seconds = 100;

            // c_vector bounds = new c_vector(11, 7, 1);
            c_vector bounds = new c_vector(101, 103, 1);

            int[] results = new int[4];

            foreach(c_robot robot in robots)
            {
                c_vector position = robot.get_position(bounds, seconds);

                if (position.x < bounds.x / 2)
                {
                    if (position.y < bounds.y / 2)
                    {
                        results[0]++;
                    }
                    else if (position.y > bounds.y / 2)
                    {
                        results[1]++;
                    }
                }
                else if (position.x > bounds.x / 2)
                {
                    if (position.y < bounds.y / 2)
                    {
                        results[2]++;
                    }
                    else if (position.y > bounds.y / 2)
                    {
                        results[3]++;
                    }
                }
            }

            int result = results.Aggregate((a, b) => a * b);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }

        public static void part_2_display(
            int[,] picture,
            int seconds)
        {
            Console.WriteLine($"Robots after {seconds} seconds:");
            Console.WriteLine($"{seconds} mod {101} = {seconds % 101}");
            Console.WriteLine($"{seconds} mod {103} = {seconds % 103}");

            picture.display(v =>
            {
                if (v == 0)
                {
                    Console.Write(' ');
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(v);
                }
            });

            Console.WriteLine();
        }

        public static void save_to_bitmap(
            int[,] picture,
            int seconds,
            int scale = 1)
        {
            using (Bitmap bitmap = new Bitmap(picture.GetLength(1), picture.GetLength(0)))
            {
                for (int x = 0; x < picture.GetLength(0); x++)
                {
                    for (int y = 0; y < picture.GetLength(1); y++)
                    {
                        if (picture[x, y] > 0)
                        {
                            bitmap.SetPixel(y, x, Color.Green);
                        }
                    }
                }

                Directory.CreateDirectory("output");

                bitmap.Save($"output\\seconds_{seconds}.bmp");
            }
        }

        public static void part_2_search_mode(
            c_robot[] robots,
            bool pretty,
            c_vector bounds)
        {
            string latest_input = null;
            int seconds = 0;

            int step_count = pretty ? 1000 : 10;

            while (latest_input != "end")
            {
                if (!pretty)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("####################################################################################################");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("####################################################################################################");
                    Console.ResetColor();
                }
                Console.WriteLine("Generating...");
                Console.WriteLine();

                for (int i = 1; i <= step_count; i++)
                {
                    seconds++;

                    int[,] picture = new int[bounds.y, bounds.x];

                    foreach (c_robot robot in robots)
                    {
                        robot.step(bounds);

                        picture[robot.position.y, robot.position.x]++;
                    }

                    if (pretty)
                    {
                        save_to_bitmap(picture, seconds);
                    }
                    else
                    {
                        part_2_display(picture, seconds);
                    }
                }

                Console.WriteLine("Done. Press <Enter> to generate more.");
                latest_input = Console.ReadLine();
            }
        }

        public static void part_2_destroy_mode(
            c_robot[] robots,
            bool pretty,
            c_vector bounds,
            int mod_x_result,
            int mod_y_result)
        {
            int seconds = mod_x_result;

            while (seconds % bounds.y != mod_y_result)
            {
                seconds += bounds.x;
            }

            int[,] picture = new int[bounds.y, bounds.x];

            foreach (c_robot robot in robots)
            {
                robot.step(bounds, seconds);

                picture[robot.position.y, robot.position.x]++;
            }

            part_2_display(picture, seconds);

            if (pretty)
            {
                save_to_bitmap(picture, seconds);
            }
        }

        public static void part_2(
            c_input_reader input_reader,
            bool pretty)
        {
            c_robot[] robots = parse_input(input_reader, pretty);

            // c_vector bounds = new c_vector(11, 7, 1);
            c_vector bounds = new c_vector(101, 103, 1);

            if (main.options.Contains("search"))
            {
                part_2_search_mode(robots, pretty, bounds);
            }
            else if (main.options.Contains("destroy"))
            {
                // Using search mode, I found that there were hotizontal and vertical clusterings when the following were true:
                // seconds mod 101 == 72
                // seconds mod 103 == 31
                // Found from noticing seconds = 31, 72, 134, and 173.
                part_2_destroy_mode(robots, pretty, bounds, 72, 31);
            }
            else
            {
                throw new ArgumentException("No option specified");
            }
        }
    }
}
