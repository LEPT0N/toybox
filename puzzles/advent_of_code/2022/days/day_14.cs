using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.display_helpers;
using advent_of_code_common.extensions;
using advent_of_code_common.input_reader;
using advent_of_code_common.int_math;

namespace advent_of_code_2022.days
{
    internal enum e_cave_position
    {
        empty = 0,
        wall,
        sand,
        invalid,
    }

    internal class day_14
    {
        [DebuggerDisplay("{bounds} {expanding}", Type = "c_cave")]
        internal class c_cave
        {
            private c_vector start;
            private c_rectangle bounds;
            private bool expanding;

            // x and y are flipped to make displaying easier.
            private e_cave_position[,] positions;

            public c_cave(c_vector s, List<c_line> lines, bool e)
            {
                start = s;
                bounds = new c_rectangle(s);
                expanding = e;

                foreach (c_line line in lines)
                {
                    bounds.expand_to_fit(new c_rectangle(line.first, line.second));
                }

                if (expanding)
                {
                    bounds.max.y += 2;
                }
                else
                {
                    bounds.expand_by(2);
                }

                positions = new e_cave_position[bounds.height + 1, bounds.width + 1];

                foreach (c_line line in lines)
                {
                    foreach(c_vector wall in line.to_int_vectors())
                    {
                        set_position(wall, e_cave_position.wall);
                    }
                }

                if (expanding)
                {
                    for (c_vector bottom_wall = new c_vector(bounds.min.x, bounds.max.y);
                        bottom_wall.x <= bounds.max.x;
                        bottom_wall.x++)
                    {
                        set_position(bottom_wall, e_cave_position.wall);
                    }
                }
            }

            internal void set_position(c_vector v, e_cave_position position)
            {
                positions[v.y - bounds.min.y, v.x - bounds.min.x] = position;
            }

            internal e_cave_position get_position(c_vector v)
            {
                if (expanding)
                {
                    if (v.x < bounds.min.x)
                    {
                        e_cave_position[,] new_positions = new e_cave_position[positions.GetLength(0), positions.GetLength(1) + 1];

                        positions.copy_to(new_positions,
                            0, 0, positions.GetLength(0),
                            0, 1, positions.GetLength(1));

                        positions = new_positions;
                        bounds.min.x--;
                        set_position(new c_vector(v.x, bounds.max.y), e_cave_position.wall);
                    }
                    else if (v.x > bounds.max.x)
                    {
                        e_cave_position[,] new_positions = new e_cave_position[positions.GetLength(0), positions.GetLength(1) + 1];

                        positions.copy_to(new_positions,
                            0, 0, positions.GetLength(0),
                            0, 0, positions.GetLength(1));

                        positions = new_positions;
                        bounds.max.x++;
                        set_position(new c_vector(v.x, bounds.max.y), e_cave_position.wall);
                    }

                    return positions[v.y - bounds.min.y, v.x - bounds.min.x];
                }
                else
                {
                    if (bounds.contains(v))
                    {
                        return positions[v.y - bounds.min.y, v.x - bounds.min.x];
                    }
                    else
                    {
                        return e_cave_position.invalid;
                    }
                }
            }

            internal static readonly c_vector[] movements =
            {
                new c_vector(0, 1),
                new c_vector(-1, 1),
                new c_vector(1, 1),
            };

            public bool try_add_sand()
            {
                c_vector current = new c_vector(start);

                if (get_position(current) != e_cave_position.empty)
                {
                    return false;
                }

                bool moved = true;
                while (moved)
                {
                    moved = (null != movements.FirstOrDefault(movement =>
                    {
                        c_vector new_position = current.add(movement);

                        if (get_position(new_position) == e_cave_position.empty)
                        {
                            current = new_position;

                            return true;
                        }

                        return false;
                    }));
                }

                if (current.y == bounds.max.y)
                {
                    return false;
                }
                else
                {
                    set_position(current, e_cave_position.sand);

                    return true;
                }
            }

            public int number_of_matching_positions(e_cave_position p)
            {
                return positions.count(position => position == p);
            }

            internal void display_position(e_cave_position position)
            {
                switch (position)
                {
                    case e_cave_position.empty:
                        Console.Write(' ');
                        break;

                    case e_cave_position.wall:
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write('#');
                        Console.ResetColor();
                        break;

                    case e_cave_position.sand:
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write('o');
                        Console.ResetColor();
                        break;
                }
            }

            public void display()
            {
                positions.display(p => display_position(p));
            }
        }

        internal static c_vector parse_point(string input)
        {
            string[] split_input = input.Split(",");

            return new c_vector(
                int.Parse(split_input[0]),
                int.Parse(split_input[1]));
        }

        internal static readonly c_vector k_start_point = new c_vector(500, 0);

        internal static c_cave parse_input(
            in string input,
            bool expanding,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            List<c_line> lines = new List<c_line>();

            while (input_reader.has_more_lines())
            {
                string[] point_input = input_reader.read_line().Split(" -> ");
                c_vector previous_point = parse_point(point_input[0]);

                for (int i = 1; i < point_input.Length; i++)
                {
                    c_vector current_point = parse_point(point_input[i]);
                    c_line line = new c_line(previous_point, current_point);

                    lines.Add(line);

                    previous_point = current_point;
                }
            }

            return new c_cave(
                k_start_point,
                lines,
                expanding);
        }

        internal static void part_worker(
            string input,
            bool expanding,
            bool pretty)
        {
            c_cave cave = parse_input(input, expanding, pretty);

            if (pretty)
            {
                cave.display();
            }

            while (cave.try_add_sand())
            {
                if (pretty)
                {
                    cave.display();
                }
            }

            if (!pretty)
            {
                cave.display();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", cave.number_of_matching_positions(e_cave_position.sand));
            Console.ResetColor();
        }

        public static void part_1(
            string input,
            bool pretty)
        {
            part_worker(input, false, pretty);
        }

        public static void part_2(
            string input,
            bool pretty)
        {
            part_worker(input, true, pretty);
        }
    }
}
