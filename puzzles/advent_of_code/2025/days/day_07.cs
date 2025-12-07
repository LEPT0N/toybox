using advent_of_code_common.extensions;
using advent_of_code_common.grid;
using advent_of_code_common.input_reader;
using advent_of_code_common.int_math;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace advent_of_code_2025.days
{
    internal class day_07
    {
        [DebuggerDisplay("", Type = "c_tree_cell")]
        internal class c_tree_cell : c_grid_cell
        {
            public UInt64 beams;

            public c_tree_cell(
            c_vector p,
            bool b) : base(p, b)
            {
                beams = 0;
            }

            public override void display()
            {
                ConsoleColor color = ConsoleColor.Black;
                char value = ' ';

                if (blocked)
                {
                    color = ConsoleColor.Red;
                    value = '^';
                }
                else if (beams > 0)
                {
                    color = ConsoleColor.DarkGreen;
                    value = '|';
                }

                Console.ForegroundColor = color;
                Console.Write(value);
            }

            public override Color get_picture_color()
            {
                if (blocked)
                {
                    return Color.Red;
                }
                else if (beams > 0)
                {
                    return Color.Green;
                }
                else
                {
                    return Color.Black;
                }
            }
        }

        [DebuggerDisplay("", Type = "c_tree_grid")]
        internal class c_tree_grid : c_grid<c_tree_cell>
        {
            public c_tree_grid(
                c_input_reader input_reader)
            {
                initialize(input_reader);
            }

            protected override c_tree_cell create_cell(
                char input_char,
                c_vector position)
            {
                c_tree_cell cell = new c_tree_cell(position, input_char == '^');

                if (input_char == 'S')
                {
                    start = cell;
                }

                return cell;
            }

            protected override void display_start()
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write('S');
            }

            protected override Color get_start_picture_color()
            {
                return Color.Yellow;
            }

            public int fire_beam()
            {
                int splits = 0;

                // Have a queue of beams. The start position is our first beam. Loop as long as we have more beams to fire.

                Queue<c_vector> beams_to_fire = new Queue<c_vector>();
                beams_to_fire.Enqueue(start.position);

                while (beams_to_fire.Count > 0)
                {
                    c_vector beam = beams_to_fire.Dequeue();

                    // Once we have a beam, fire that beam downwards, marking each cell as we go.

                    while (cells.is_valid_index(beam) && cells[beam.row][beam.col].beams == 0)
                    {
                        cells[beam.row][beam.col].beams = 1;

                        // If we hit a spltter, stop this beam but possibly enqueue two more.

                        if (cells[beam.row][beam.col].blocked)
                        {
                            splits++;

                            if (cells[beam.row][beam.col - 1].beams == 0)
                            {
                                beams_to_fire.Enqueue(new c_vector(beam.row, beam.col - 1));
                            }
                            if (cells[beam.row][beam.col + 1].beams == 0)
                            {
                                beams_to_fire.Enqueue(new c_vector(beam.row, beam.col + 1));
                            }

                            break;
                        }

                        beam.row++;
                    }
                }

                return splits;
            }

            private void fire_one_quantum_beam(
                c_vector beam,
                UInt64 count)
            {
                // Fire a beam downwards until it's blocked.
                // Note we're actually firing many beams at once, specified by 'count'.

                while (cells.is_valid_index(beam))
                {
                    cells[beam.row][beam.col].beams += count;

                    if (cells[beam.row][beam.col].blocked)
                    {
                        break;
                    }

                    beam.row++;
                }
            }

            public UInt64 fire_quantum_beam()
            {
                start.blocked = true;
                start.beams = 1;

                // Loop row by row through the grid.

                for (int row = 0; row < cells.Length; row++)
                {
                    for (int col = 0; col < cells[row].Length; col++)
                    {
                        // If we find a blocked cell that has been beamed, fire two new beams off to either side.
                        // Note that each splitter is only processed once, which is how this stays performant.

                        if (cells[row][col].blocked && cells[row][col].beams > 0)
                        {
                            fire_one_quantum_beam(new c_vector(row, col - 1), cells[row][col].beams);
                            fire_one_quantum_beam(new c_vector(row, col + 1), cells[row][col].beams);
                        }
                    }
                }

                // Add up the number of beams we see along the bottom row.

                UInt64 total_beams = 0;

                for (int col = 0; col < cells[0].Length; col++)
                {
                    total_beams += cells[cells.Length - 1][col].beams;
                }

                return total_beams;
            }
        }

        public static void part_1(
            c_input_reader input_reader,
            bool pretty)
        {
            c_tree_grid grid = new c_tree_grid(input_reader);

            if (pretty)
            {
                grid.display();
            }

            int result = grid.fire_beam();

            if (pretty)
            {
                grid.display();
            }

            if (main.options.Contains("png"))
            {
                grid.create_picture($"day_7_part_1");
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }

        public static void part_2(
            c_input_reader input_reader,
            bool pretty)
        {
            c_tree_grid grid = new c_tree_grid(input_reader);

            if (pretty)
            {
                grid.display();
            }

            UInt64 result = grid.fire_quantum_beam();

            if (pretty)
            {
                grid.display();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }
    }
}
