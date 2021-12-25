using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace advent_of_code_2021.Days
{
    internal class Day_25
    {
        internal enum e_direction
        {
            east,
            south,
        }

        [DebuggerDisplay("[{column}, {row}]", Type = "s_coordinate")]
        internal struct s_coordinate
        {
            public int row;
            public int column;

            public s_coordinate add(s_coordinate other)
            {
                return new s_coordinate { row = this.row + other.row, column = this.column + other.column };
            }

            public bool equals(s_coordinate other)
            {
                return this.row == other.row && this.column == other.column;
            }
        }

        private static readonly s_coordinate k_east = new s_coordinate { row = 0, column = 1 };
        private static readonly s_coordinate k_south = new s_coordinate { row = 1, column = 0 };

        [DebuggerDisplay("p {position} v {velocity}", Type = "c_sea_cucumber")]
        internal class c_sea_cucumber
        {
            public s_coordinate position;
            public s_coordinate velocity;

            public char get_display_char()
            {
                if (velocity.equals(k_east))
                {
                    return '>';
                }
                else if (velocity.equals(k_south))
                {
                    return 'v';
                }
                else
                {
                    throw new Exception("unable to find sea cucumber display char");
                }
            }
        }

        internal class c_sea_floor
        {

            private List<c_sea_cucumber> sea_cucumbers;
            private c_sea_cucumber[][] positions;

            public c_sea_floor(c_input_reader input_reader)
            {
                sea_cucumbers = new List<c_sea_cucumber>();

                List<c_sea_cucumber[]> position_list = new List<c_sea_cucumber[]>();

                s_coordinate position = new s_coordinate { row = 0, column = 0 };
                while (input_reader.has_more_lines())
                {
                    List<c_sea_cucumber> position_row_list = new List<c_sea_cucumber>();

                    position.column = 0;
                    foreach(char input_char in input_reader.read_line())
                    {
                        c_sea_cucumber sea_cucumber = null;

                        switch (input_char)
                        {
                            case '>':
                                sea_cucumber = new c_sea_cucumber { position = position, velocity = k_east };
                                break;

                            case 'v':
                                sea_cucumber = new c_sea_cucumber { position = position, velocity = k_south };
                                break;

                            case '.':
                                break;

                            default:
                                throw new Exception("Invalid input");
                        }

                        if (sea_cucumber != null)
                        {
                            sea_cucumbers.Add(sea_cucumber);
                        }

                        position_row_list.Add(sea_cucumber);

                        position.column++;
                    }

                    position_list.Add(position_row_list.ToArray());
                    position.row++;
                }

                positions = position_list.ToArray();
            }

            private s_coordinate get_position_coordinates(s_coordinate position)
            {
                return new s_coordinate { row= position.row % positions.Length, column = position.column % positions[0].Length};
            }

            private bool step(s_coordinate step_velocity)
            {
                List<s_coordinate> cleared_positions = new List<s_coordinate>();

                foreach (c_sea_cucumber sea_cucumber in sea_cucumbers)
                {
                    if (sea_cucumber.velocity.equals(step_velocity))
                    {
                        s_coordinate new_position = get_position_coordinates(
                            sea_cucumber.position.add(sea_cucumber.velocity));

                        if (positions[new_position.row][new_position.column] == null)
                        {
                            cleared_positions.Add(sea_cucumber.position);

                            sea_cucumber.position = new_position;
                            positions[new_position.row][new_position.column] = sea_cucumber;
                        }
                    }
                }

                bool any_moved = (cleared_positions.Count > 0);

                foreach (s_coordinate cleared_position in cleared_positions)
                {
                    positions[cleared_position.row][cleared_position.column] = null;
                }

                return any_moved;
            }

            public bool step()
            {
                bool any_moved = false;

                if(step(k_east))
                {
                    any_moved = true;
                }

                if (step(k_south))
                {
                    any_moved = true;
                }

                return any_moved;
            }

            public void display(string title)
            {
                Console.WriteLine();
                Console.WriteLine(title);

                foreach(c_sea_cucumber[] position_row in positions)
                {
                    foreach(c_sea_cucumber sea_cucumber in position_row)
                    {
                        if (sea_cucumber == null)
                        {
                            Console.Write('.');
                        }
                        else
                        {
                            Console.Write(sea_cucumber.get_display_char());
                        }
                    }

                    Console.WriteLine();
                }
            }
        }

        internal static c_sea_floor parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            c_sea_floor sea_floor = new c_sea_floor(input_reader);

            return sea_floor;
        }

        public static void Part_1(
            string input,
            bool pretty)
        {
            c_sea_floor sea_floor = parse_input(input, pretty);
            if (pretty)
            {
                sea_floor.display("Initial state:");
            }

            int step_count = 0;

            while (sea_floor.step())
            {
                step_count++;

                if (pretty)
                {
                    sea_floor.display(string.Format(
                        "After {0} step{1}",
                        step_count,
                        step_count > 1 ? "s" : ""));
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", step_count + 1);
            Console.ResetColor();
        }

        public static void Part_2(
            string input,
            bool pretty)
        {
            // parse_input(input, pretty);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", 0);
            Console.ResetColor();
        }
    }
}
