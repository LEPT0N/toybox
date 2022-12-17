using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.extensions;
using advent_of_code_common.input_reader;
using advent_of_code_common.int_math;

namespace advent_of_code_2022.days
{
    internal class day_17
    {
        internal enum e_spot_type
        {
            blank = 0,
            h_line,
            plus,
            angle,
            l_line,
            box,
        }

        internal enum e_movement_type
        {
            wind = 0,
            gravity,
        }

        internal class c_piece
        {
            public e_spot_type type { get; set; }
            public c_vector[] positions { get; set; }

            public c_piece(e_spot_type t, c_vector[] p)
            {
                type = t;
                positions = p;
            }

            public c_piece(c_piece spawn, c_vector spawn_point)
            {
                type = spawn.type;
                positions = new c_vector[spawn.positions.Length];

                for (int i = 0; i < positions.Length; i++)
                {
                    positions[i] = new c_vector(spawn.positions[i].add(spawn_point));
                }
            }

            public c_piece(c_piece piece, e_direction direction)
            {
                type = piece.type;
                positions = new c_vector[piece.positions.Length];

                c_vector offset = new c_vector(0, 0);

                switch (direction)
                {
                    case e_direction.left: offset.x = -1; break;
                    case e_direction.right: offset.x = 1; break;
                    case e_direction.down: offset.y = -1; break;
                }

                for (int i = 0; i < positions.Length; i++)
                {
                    positions[i] = new c_vector(piece.positions[i].add(offset));
                }
            }
        }

        [DebuggerDisplay("highest_nonempty_spot = {highest_nonempty_spot}", Type = "c_type")]
        internal class c_board
        {
            private e_direction[] wind_directions;
            private int current_wind_direction;

            private c_piece[] piece_spawns;
            private int current_spawn_piece;
            private c_vector spawn_point;

            private int[] highest_nonempty_spots;
            private int highest_nonempty_spot;
            private UInt64 rows_deleted;
            private c_piece current_piece;
            private e_movement_type current_movement_type;
            private e_spot_type[,] spots;

            private static readonly int k_height_increment = 10;

            public c_board(
                int width,
                e_direction[] directions,
                c_piece[] spawns,
                c_vector spawn)
            {
                wind_directions = directions;
                current_wind_direction = 0;

                piece_spawns = spawns;
                current_spawn_piece = 0;
                spawn_point = spawn;

                highest_nonempty_spots = new int[width];
                highest_nonempty_spot = 0;
                rows_deleted = 0;
                current_piece = null;

                spots = new e_spot_type[width, k_height_increment + 1];

                for (int x = 0; x < width; x++)
                {
                    spots[x, 0] = e_spot_type.h_line;
                }
            }

            public UInt64 get_highest_nonempty_spot()
            {
                return (UInt64)highest_nonempty_spot + rows_deleted;
            }

            internal void set_piece(c_piece piece, e_spot_type type)
            {
                foreach (c_vector spot in piece.positions)
                {
                    spots[spot.x, spot.y] = type;
                }
            }

            internal bool valid(c_piece piece)
            {
                return piece.positions.All(position =>
                    position.x >= 0 &&
                    position.x < spots.GetLength(0) &&
                    spots[position.x, position.y] == e_spot_type.blank);
            }

            public void move_until_settled(bool pretty)
            {
                bool first = true;
                do
                {
                    move();

                    if (pretty && first)
                    {
                        first = false;
                        display();
                    }
                } while (current_piece != null);
            }

            public void move()
            {
                if (current_piece == null)
                {
                    // Spawn a new piece

                    current_piece = new c_piece(
                        piece_spawns[current_spawn_piece],
                        spawn_point.add(new c_vector(0, highest_nonempty_spot)));
                    current_spawn_piece = (current_spawn_piece + 1) % piece_spawns.Length;
                    current_movement_type = e_movement_type.wind;

                    int current_piece_highest_position = current_piece.positions.Max(position => position.y);

                    if (current_piece_highest_position >= spots.GetLength(1))
                    {
                        e_spot_type[,] new_spots = new e_spot_type[spots.GetLength(0), spots.GetLength(1) + k_height_increment];

                        spots.copy_to(new_spots,
                            0, 0, spots.GetLength(0),
                            0, 0, spots.GetLength(1));

                        spots = new_spots;
                    }

                    set_piece(current_piece, current_piece.type);
                }
                else
                {
                    // Find the moved location of the current piece

                    e_direction direction;

                    if (current_movement_type == e_movement_type.wind)
                    {
                        direction = wind_directions[current_wind_direction];
                        current_wind_direction = (current_wind_direction + 1) % wind_directions.Length;

                        current_movement_type = e_movement_type.gravity;
                    }
                    else
                    {
                        direction = e_direction.down;

                        current_movement_type = e_movement_type.wind;
                    }

                    c_piece new_current_piece = new c_piece(current_piece, direction);

                    // Try to move the current piece

                    set_piece(current_piece, e_spot_type.blank);

                    if (valid(new_current_piece))
                    {
                        // Successfully moved the current piece

                        current_piece = new_current_piece;
                        set_piece(current_piece, current_piece.type);
                    }
                    else
                    {
                        // Current piece couldn't move.

                        set_piece(current_piece, current_piece.type);

                        if (direction == e_direction.down)
                        {
                            // If the current piece was trying to move down, record the new high water marks...

                            highest_nonempty_spot = Math.Max(
                                highest_nonempty_spot,
                                current_piece.positions.Max(position => position.y));

                            current_piece.positions.for_each(position =>
                            {
                                highest_nonempty_spots[position.x] = Math.Max(highest_nonempty_spots[position.x], position.y);
                            });

                            // And shorten the board if we can...

                            int new_floor = highest_nonempty_spots.Min();
                            if (new_floor > k_height_increment)
                            {
                                e_spot_type[,] new_spots = new e_spot_type[spots.GetLength(0), spots.GetLength(1)];

                                spots.copy_to(new_spots,
                                    0, 0, spots.GetLength(0),
                                    new_floor + 1, 1, spots.GetLength(1) - new_floor - 1);

                                for (int x = 0; x < spots.GetLength(0); x++)
                                {
                                    new_spots[x, 0] = e_spot_type.h_line;
                                }

                                spots = new_spots;

                                highest_nonempty_spots = highest_nonempty_spots.Select(spot => spot - new_floor).ToArray();
                                highest_nonempty_spot -= new_floor;
                                rows_deleted += (UInt64)new_floor;
                            }

                            // And drop the current piece.

                            current_piece = null;
                        }
                    }
                }
            }

            internal void display_spot(e_spot_type spot_type)
            {
                switch (spot_type)
                {
                    case e_spot_type.blank:
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write('.');
                        break;

                    case e_spot_type.h_line:
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write('#');
                        break;

                    case e_spot_type.plus:
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write('#');
                        break;

                    case e_spot_type.angle:
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write('#');
                        break;

                    case e_spot_type.l_line:
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        Console.Write('#');
                        break;

                    case e_spot_type.box:
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.Write('#');
                        break;
                }

                Console.ResetColor();
            }

            public void display()
            {
                for (int y = spots.GetLength(1) - 1; y > 0; y--)
                {
                    Console.Write("|");

                    for (int x = 0; x < spots.GetLength(0); x++)
                    {
                        display_spot(spots[x, y]);
                    }

                    Console.Write("|");

                    UInt64 real_row = rows_deleted + (UInt64)y;
                    if (real_row % 10 == 0)
                    {
                        Console.Write($" {real_row}");
                    }

                    Console.WriteLine();
                }

                Console.Write("+");

                for (int x = 0; x < spots.GetLength(0); x++)
                {
                    Console.Write("-");
                }

                Console.WriteLine("+");
                Console.WriteLine();
            }
        }

        internal static e_direction[] parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            return input_reader
                .read_line()
                .ToCharArray()
                .Select(c => (c == '<' ? e_direction.left : e_direction.right))
                .ToArray();
        }

        public static void part_worker(
            string input,
            UInt64 piece_count,
            bool pretty)
        {
            e_direction[] directions = parse_input(input, pretty);

            int width = 7;
            c_vector spawn_point = new c_vector(2, 4);

            c_piece[] spawns = new c_piece[]
            {
                new c_piece(
                    e_spot_type.h_line,
                    new c_vector[]
                    {
                        new c_vector(0, 0),
                        new c_vector(1, 0),
                        new c_vector(2, 0),
                        new c_vector(3, 0),
                    }),
                new c_piece(
                    e_spot_type.plus,
                    new c_vector[]
                    {
                        new c_vector(1, 0),
                        new c_vector(0, 1),
                        new c_vector(1, 1),
                        new c_vector(1, 2),
                        new c_vector(2, 1),
                    }),
                new c_piece(
                    e_spot_type.angle,
                    new c_vector[]
                    {
                        new c_vector(0, 0),
                        new c_vector(1, 0),
                        new c_vector(2, 0),
                        new c_vector(2, 1),
                        new c_vector(2, 2),
                    }),
                new c_piece(
                    e_spot_type.l_line,
                    new c_vector[]
                    {
                        new c_vector(0, 0),
                        new c_vector(0, 1),
                        new c_vector(0, 2),
                        new c_vector(0, 3),
                    }),
                new c_piece(
                    e_spot_type.box,
                    new c_vector[]
                    {
                        new c_vector(0, 0),
                        new c_vector(1, 0),
                        new c_vector(0, 1),
                        new c_vector(1, 1),
                    }),
            };

            c_board board = new c_board(
                width,
                directions,
                spawns,
                spawn_point);

            if (pretty)
            {
                board.display();
            }

            DateTime previous = DateTime.UtcNow;
            TimeSpan display_period = TimeSpan.FromSeconds(1);

            for (UInt64 i = 0; i < piece_count; i++)
            {
                board.move_until_settled(pretty);

                if (pretty)
                {
                    board.display();
                }

                DateTime now = DateTime.UtcNow;
                if (now - previous >= display_period)
                {
                    previous = now;

                    Console.WriteLine($"Placed {i} / {piece_count}...");
                }
            }

            board.display();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", board.get_highest_nonempty_spot());
            Console.ResetColor();
        }

        public static void part_1(
            string input,
            bool pretty)
        {
            part_worker(input, 2022, pretty);
        }

        public static void part_2(
            string input,
            bool pretty)
        {
            part_worker(input, 1000000000000, pretty);
        }
    }
}
