using System;
using System.Collections.Generic;
using System.Diagnostics;
using advent_of_code_common.input_reader;

using advent_of_code_common.int_math;

namespace advent_of_code_2020.Days
{
    internal class Day_12
    {
        internal enum e_action
        {
            north,
            south,
            east,
            west,
            left,
            right,
            forward,
        }

        internal enum e_direction
        {
            north,
            south,
            east,
            west,
        }

        [DebuggerDisplay("{action} {value}", Type = "c_instruction")]
        internal class c_instruction
        {
            public e_action action;
            public int value;
        }

        internal static c_instruction[] parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            List<c_instruction> instructions = new List<c_instruction>();

            while (input_reader.has_more_lines())
            {
                string input_line = input_reader.read_line();

                char input_action = input_line[0];
                int input_value = int.Parse(input_line.Substring(1));

                e_action action;
                switch (input_action)
                {
                    case 'N': action = e_action.north; break;
                    case 'S': action = e_action.south; break;
                    case 'E': action = e_action.east; break;
                    case 'W': action = e_action.west; break;
                    case 'L': action = e_action.left; break;
                    case 'R': action = e_action.right; break;
                    case 'F': action = e_action.forward; break;

                    default:
                        throw new Exception(String.Format("Bad action in input line '{0}'", input_line));
                }

                instructions.Add(new c_instruction { action = action, value = input_value });
            }

            return instructions.ToArray();
        }

        public static void Part_1(
            string input,
            bool pretty)
        {
            c_instruction[] instructions = parse_input(input, pretty);

            e_direction ship_direction = e_direction.east;
            int distance_north = 0;
            int distance_east = 0;

            foreach (c_instruction instruction in instructions)
            {
                switch (instruction.action)
                {
                    case e_action.north: distance_north += instruction.value; break;
                    case e_action.south: distance_north -= instruction.value; break;
                    case e_action.east: distance_east += instruction.value; break;
                    case e_action.west: distance_east -= instruction.value; break;

                    case e_action.forward:
                        {
                            switch (ship_direction)
                            {
                                case e_direction.north: distance_north += instruction.value; break;
                                case e_direction.south: distance_north -= instruction.value; break;
                                case e_direction.east: distance_east += instruction.value; break;
                                case e_direction.west: distance_east -= instruction.value; break;
                            }
                        }
                        break;

                    case e_action.left:
                        {
                            switch (ship_direction, instruction.value)
                            {
                                case (e_direction.north, 90): ship_direction = e_direction.west; break;
                                case (e_direction.north, 180): ship_direction = e_direction.south; break;
                                case (e_direction.north, 270): ship_direction = e_direction.east; break;

                                case (e_direction.south, 90): ship_direction = e_direction.east; break;
                                case (e_direction.south, 180): ship_direction = e_direction.north; break;
                                case (e_direction.south, 270): ship_direction = e_direction.west; break;

                                case (e_direction.east, 90): ship_direction = e_direction.north; break;
                                case (e_direction.east, 180): ship_direction = e_direction.west; break;
                                case (e_direction.east, 270): ship_direction = e_direction.south; break;

                                case (e_direction.west, 90): ship_direction = e_direction.south; break;
                                case (e_direction.west, 180): ship_direction = e_direction.east; break;
                                case (e_direction.west, 270): ship_direction = e_direction.north; break;

                                default:
                                    throw new Exception("Bad (direction,turn_amount) value");
                            }
                        }
                        break;

                    case e_action.right:
                        {
                            switch (ship_direction, instruction.value)
                            {
                                case (e_direction.north, 90): ship_direction = e_direction.east; break;
                                case (e_direction.north, 180): ship_direction = e_direction.south; break;
                                case (e_direction.north, 270): ship_direction = e_direction.west; break;

                                case (e_direction.south, 90): ship_direction = e_direction.west; break;
                                case (e_direction.south, 180): ship_direction = e_direction.north; break;
                                case (e_direction.south, 270): ship_direction = e_direction.east; break;

                                case (e_direction.east, 90): ship_direction = e_direction.south; break;
                                case (e_direction.east, 180): ship_direction = e_direction.west; break;
                                case (e_direction.east, 270): ship_direction = e_direction.north; break;

                                case (e_direction.west, 90): ship_direction = e_direction.north; break;
                                case (e_direction.west, 180): ship_direction = e_direction.east; break;
                                case (e_direction.west, 270): ship_direction = e_direction.south; break;

                                default:
                                    throw new Exception("Bad (direction,turn_amount) value");
                            }
                        }
                        break;
                }
            }

            Console.WriteLine("Distance North = {0}", distance_north);
            Console.WriteLine("Distance East = {0}", distance_east);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", Math.Abs(distance_north) + Math.Abs(distance_east));
            Console.ResetColor();
        }

        // 5685 is too high

        public static void Part_2(
            string input,
            bool pretty)
        {
            c_matrix rotate_right_90 = c_matrix.rotate(e_axis.z, e_angle.angle_270);
            c_matrix rotate_180 = c_matrix.rotate(e_axis.z, e_angle.angle_180);
            c_matrix rotate_left_90 = c_matrix.rotate(e_axis.z, e_angle.angle_90);

            c_instruction[] instructions = parse_input(input, pretty);

            c_vector ship_position = new c_vector();

            c_vector waypoint_position = new c_vector(10, 1, 0);

            foreach (c_instruction instruction in instructions)
            {
                switch (instruction.action)
                {
                    case e_action.north: waypoint_position.y += instruction.value; break;
                    case e_action.south: waypoint_position.y -= instruction.value; break;
                    case e_action.east: waypoint_position.x += instruction.value; break;
                    case e_action.west: waypoint_position.x -= instruction.value; break;

                    case e_action.forward:
                        {
                            for (int i = 0; i < instruction.value; i++)
                            {
                                ship_position = ship_position.add(waypoint_position);
                            }
                        }
                        break;

                    case e_action.left:
                        {
                            switch (instruction.value)
                            {
                                case 270:
                                    waypoint_position = rotate_right_90.multiply(waypoint_position);
                                    break;

                                case 180:
                                    waypoint_position = rotate_180.multiply(waypoint_position);
                                    break;

                                case 90:
                                    waypoint_position = rotate_left_90.multiply(waypoint_position);
                                    break;

                                default:
                                    throw new Exception("Bad Rotation");
                            }
                        }
                        break;

                    case e_action.right:
                        {
                            switch (instruction.value)
                            {
                                case 270:
                                    waypoint_position = rotate_left_90.multiply(waypoint_position);
                                    break;

                                case 180:
                                    waypoint_position = rotate_180.multiply(waypoint_position);
                                    break;

                                case 90:
                                    waypoint_position = rotate_right_90.multiply(waypoint_position);
                                    break;

                                default:
                                    throw new Exception("Bad Rotation");
                            }
                        }
                        break;
                }
            }

            ship_position.print("Ship Distance");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", ship_position.taxi_magnitude());
            Console.ResetColor();
        }
    }
}
