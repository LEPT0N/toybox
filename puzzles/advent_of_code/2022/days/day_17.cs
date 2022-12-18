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

            public void set_highest_nonempty_spot(UInt64 total_highest_nonempty_spot)
            {
                // Pretend to move the tower up/down by setting how many rows have been deleted
                // so that get_highest_nonempty_spot will match the input to this method.
                rows_deleted = total_highest_nonempty_spot - (UInt64)highest_nonempty_spot; 
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

            private class c_board_thumbprint
            {
                // Reference data
                public UInt64 pieces_settled_count { get; set; }
                public UInt64 highest_nonempty_spot { get; set; }

                // Equivalence data
                // The current wind, current piece, and shape of the top of the tower is probably enough to detect equivalent tower states.
                // An exact equivalence would replace highest_nonempty_spots with a more exact definition of the shape of the top of the tower.
                // But this worked.
                public int current_spawn_piece { get; set; }
                public int current_wind_direction { get; set; }

                public int[] highest_nonempty_spots { get; set; }
            }

            private class c_board_thumbprint_comparer : IEqualityComparer<c_board_thumbprint>
            {
                public bool Equals(c_board_thumbprint a, c_board_thumbprint b)
                {
                    if (a.current_spawn_piece != b.current_spawn_piece ||
                        a.current_wind_direction != b.current_wind_direction)
                    {
                        return false;
                    }

                    for (int i = 0; i < a.highest_nonempty_spots.Length; i++)
                    {
                        if (a.highest_nonempty_spots[i] != b.highest_nonempty_spots[i])
                        {
                            return false;
                        }
                    }

                    return true;
                }

                public int GetHashCode(c_board_thumbprint a)
                {
                    // Assume at least three columns or some of the pieces don't fit.
                    // It's just a hash, collisions are fine.
                    return HashCode.Combine(
                        a.current_spawn_piece,
                        a.current_wind_direction,
                        a.highest_nonempty_spots[0],
                        a.highest_nonempty_spots[1],
                        a.highest_nonempty_spots[2]);
                }
            }

            public (UInt64 loop_1_start, UInt64 loop_1_start_height, UInt64 loop_2_start, UInt64 loop_2_start_height) move_until_loop_detected(bool pretty)
            {
                // Run the game until a loop is detected (since the placed blocks and wind input is in a set pattern, then if we run the game long enough then
                // we will eventually loop back to a previous state, just with a higher tower.

                UInt64 pieces_settled_count = 0;

                // Keep track of all states we've seen
                HashSet<c_board_thumbprint> board_thumbprints = new HashSet<c_board_thumbprint>(new c_board_thumbprint_comparer());

                while (true)
                {
                    // Place a piece
                    move_until_settled(pretty);
                    pieces_settled_count++;

                    // Create a thumbprint for the current board state.
                    int[] highest_nonempty_spots_copy = new int[highest_nonempty_spots.Length];
                    Array.Copy(highest_nonempty_spots, highest_nonempty_spots_copy, highest_nonempty_spots.Length);

                    c_board_thumbprint new_thumbprint = new c_board_thumbprint
                    {
                        pieces_settled_count = pieces_settled_count,
                        highest_nonempty_spot = get_highest_nonempty_spot(),

                        current_spawn_piece = current_spawn_piece,
                        current_wind_direction = current_wind_direction,
                        highest_nonempty_spots = highest_nonempty_spots_copy,
                    };

                    if (board_thumbprints.TryGetValue(new_thumbprint, out c_board_thumbprint existing_thumbprint))
                    {
                        // Our new thumbprint is equivalent to an older one! We found a loop, so return.

                        return (
                            existing_thumbprint.pieces_settled_count,
                            existing_thumbprint.highest_nonempty_spot,
                            new_thumbprint.pieces_settled_count,
                            new_thumbprint.highest_nonempty_spot
                            );
                    }
                    else
                    {
                        // Add the current thumbprint to the collection.

                        board_thumbprints.Add(new_thumbprint);
                    }
                }
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

        internal static c_board parse_board(
            string input,
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

            return board;
        }

        public static void part_1(
            string input,
            bool pretty)
        {
            c_board board = parse_board(input, pretty);

            UInt64 piece_count = 2022;
            
            // Place {piece_count} pieces
            for (UInt64 i = 0; i < piece_count; i++)
            {
                board.move_until_settled(pretty);

                if (pretty)
                {
                    board.display();
                }
            }

            if (!pretty)
            {
                board.display();
            }

            // And display the total tower height.
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", board.get_highest_nonempty_spot());
            Console.ResetColor();
        }

        public static void part_2(
            string input,
            bool pretty)
        {
            // At first I figured I could solve this by chopping off the bottom of the board to wherever the board became inaccessible to newly
            // spawned pieces, and just keep track of how many rows had been sent to the shadow realm. This seemed to work! however...
            // By my estimates, using part_1 to place 1000000000000 pieces would take between a week and a month for this program to run.
            // So instead we need to do something clever.
            // Because the inputs (wind and pieces) are on set deterministic loops, eventually we will find that the relevant board state loops.
            // With this in mind, we can solve this problem by:
            //      1. Run the game up until the first loop.
            //      2. Quickly compute how many times the game will loop on the way to 1T pieces, and how tall the tower will be at that point.
            //      3. Continue running the game after the last loop and stop at 1T pieces placed.

            c_board board = parse_board(input, pretty);

            UInt64 total_piece_count = 1000000000000;

            // Run until we detect the first loop.
            var loop_detection_result = board.move_until_loop_detected(pretty);

            Console.WriteLine("Found loop from {0} (height {1}) to {2} (height {3})",
                loop_detection_result.loop_1_start,
                loop_detection_result.loop_1_start_height,
                loop_detection_result.loop_2_start,
                loop_detection_result.loop_2_start_height);

            // Figure out how many loops will run, and the state of the board will be after all those loops.
            UInt64 pieces_placed_before_loops = loop_detection_result.loop_1_start;
            UInt64 height_before_loops = loop_detection_result.loop_1_start_height;

            UInt64 pieces_placed_per_loop = (loop_detection_result.loop_2_start - loop_detection_result.loop_1_start);
            UInt64 height_per_loop = (loop_detection_result.loop_2_start_height - loop_detection_result.loop_1_start_height);

            UInt64 loop_count = (total_piece_count - pieces_placed_before_loops) / pieces_placed_per_loop;
            UInt64 placed_after_loops = pieces_placed_before_loops + (pieces_placed_per_loop * loop_count);
            UInt64 height_after_loops = height_before_loops + (height_per_loop * loop_count);

            // Push the board up to this new post-loop height
            board.set_highest_nonempty_spot(height_after_loops);

            // Finish running the game
            for (UInt64 i = placed_after_loops; i < total_piece_count; i++)
            {
                board.move_until_settled(pretty);

                if (pretty)
                {
                    board.display();
                }
            }

            if (!pretty)
            {
                board.display();
            }

            // And display the total tower height.
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", board.get_highest_nonempty_spot());
            Console.ResetColor();
        }
    }
}
