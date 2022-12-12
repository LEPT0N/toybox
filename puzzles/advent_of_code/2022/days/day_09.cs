using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.display_helpers;
using advent_of_code_common.input_reader;
using advent_of_code_common.int_math;

namespace advent_of_code_2022.days
{
    internal class day_09
    {

        [DebuggerDisplay("{direction} {distance}", Type = "c_movement")]
        internal class c_movement
        {
            public e_direction direction;
            public int distance;

            public c_movement(string input)
            {
                string[] inputs = input.Split(' ');

                switch (inputs[0])
                {
                    case "U":
                        direction = e_direction.up;
                        break;

                    case "D":
                        direction = e_direction.down;
                        break;

                    case "L":
                        direction = e_direction.left;
                        break;

                    case "R":
                        direction = e_direction.right;
                        break;

                    default:
                        throw new ArgumentException($"Bad direction {inputs[0]}");
                }

                distance = int.Parse(inputs[1]);
            }
        }

        internal static Dictionary<e_direction, c_vector> k_movements = new Dictionary<e_direction, c_vector>
        {
            {  e_direction.up, new c_vector(0, 1) },
            {  e_direction.down, new c_vector(0, -1) },
            {  e_direction.left, new c_vector(-1, 0) },
            {  e_direction.right, new c_vector(1, 0) },
        };

        internal static c_movement[] parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            List<c_movement> movements = new List<c_movement>();

            while (input_reader.has_more_lines())
            {
                movements.Add(new c_movement(input_reader.read_line()));
            }

            return movements.ToArray();
        }

        internal static void print_character_1(char c)
        {
            switch (c)
            {
                case 'H':
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(c);
                    break;

                case 'T':
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write(c);
                    break;

                case '#':
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(c);
                    break;

                default:
                    Console.Write(' ');
                    break;
            }
        }

        internal static void display_1(
            c_vector head,
            c_vector tail,
            HashSet<c_vector> tail_trail,
            c_rectangle bounds,
            bool pretty)
        {
            if (!pretty)
            {
                return;
            }

            char[,] output = new char[bounds.width + 1, bounds.height + 1];

            foreach (c_vector p in tail_trail)
            {
                output[p.x - bounds.min.x, p.y - bounds.min.y] = '#';
            }

            output[tail.x - bounds.min.x, tail.y - bounds.min.y] = 'T';
            output[head.x - bounds.min.x, head.y - bounds.min.y] = 'H';

            output.display(c => print_character_1(c));
        }

        public static void part_1(
            string input,
            bool pretty)
        {
            c_movement[] movements = parse_input(input, pretty);

            c_vector head = new c_vector();
            c_vector tail = new c_vector();

            HashSet<c_vector> tail_trail = new HashSet<c_vector>(new c_vector_comparer()) { tail };
            c_rectangle bounds = new c_rectangle(head, tail);

            foreach (c_movement movement in movements)
            {
                for (int i = 0; i < movement.distance; i++)
                {
                    head = head.add(k_movements[movement.direction]);
                    bounds.expand_to_fit(head);

                    if (!head.adjacent(tail))
                    {
                        c_vector difference = new c_vector(head.x - tail.x, head.y - tail.y);
                        difference.normalize();

                        tail = tail.add(difference);

                        tail_trail.Add(tail);
                    }

                    // display(head, tail, tail_trail, bounds, pretty);
                }
            }

            display_1(head, tail, tail_trail, bounds, pretty);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", tail_trail.Count());
            Console.ResetColor();
        }

        internal static void print_character_2(char c)
        {
            if (c == '0')
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write('H');
            }
            else if (c >= '1' && c <= '9')
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write(c);
            }
            else if (c == '#')
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(c);
            }
            else
            {
                Console.Write(' ');
            }
        }

        internal static void display_2(
            c_vector[] knots,
            HashSet<c_vector> tail_trail,
            c_rectangle bounds,
            bool pretty)
        {
            if (!pretty)
            {
                return;
            }

            char[,] output = new char[bounds.width + 1, bounds.height + 1];

            // Fill the output with '#'s for the trail
            foreach (c_vector p in tail_trail)
            {
                output[p.x - bounds.min.x, p.y - bounds.min.y] = '#';
            }

            // Fill in the knots
            for (int i = knots.Length - 1; i >= 0; i--)
            {
                output[knots[i].x - bounds.min.x, knots[i].y - bounds.min.y] = (char)('0' + i);
            }

            output.display(c => print_character_2(c));
        }

        public static void part_2(
            string input,
            bool pretty)
        {
            c_movement[] movements = parse_input(input, pretty);

            c_vector[] knots = new c_vector[10];
            Array.Fill(knots, new c_vector());

            HashSet<c_vector> tail_trail = new HashSet<c_vector>(new c_vector_comparer()) { knots[knots.Length - 1] };
            c_rectangle bounds = new c_rectangle(knots[0], knots[1]);

            foreach (c_movement movement in movements)
            {
                for (int m = 0; m < movement.distance; m++)
                {
                    knots[0] = knots[0].add(k_movements[movement.direction]);
                    bounds.expand_to_fit(knots[0]);

                    for (int i = 1; i < knots.Length; i++)
                    {
                        c_vector previous = knots[i - 1];
                        c_vector current = knots[i];

                        if (!previous.adjacent(current))
                        {
                            c_vector difference = new c_vector(previous.x - current.x, previous.y - current.y);
                            difference.normalize();

                            current = current.add(difference);

                            knots[i] = current;

                            if (i == knots.Length - 1)
                            {
                                tail_trail.Add(current);
                            }
                        }
                    }

                    // display_2(knots, tail_trail, bounds, pretty);
                }

                // display_2(knots, tail_trail, bounds, pretty);
            }

            display_2(knots, tail_trail, bounds, pretty);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", tail_trail.Count());
            Console.ResetColor();
        }
    }
}
