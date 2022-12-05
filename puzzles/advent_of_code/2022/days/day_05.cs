using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.extensions;
using advent_of_code_common.input_reader;

namespace advent_of_code_2022.days
{
    internal class day_05
    {
        internal class c_crate
        {
            public char name { get; set; }

            public int last_modified { get; set; }

            public c_crate(char n)
            {
                name = n;
                last_modified = -1;
            }

            public void print(int current_move)
            {
                if (last_modified == current_move)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }

                Console.Write($"[{name}] ");

                Console.ResetColor();
            }
        }

        internal class c_pile
        {
            List<c_crate> crates = new List<c_crate>();

            int last_removed_from = -1;
            int previous_height = -1;

            public int get_crate_count(int current_move)
            {
                int result = crates.Count;

                if (current_move == last_removed_from)
                {
                    result = previous_height;
                }

                return result;
            }

            public void print_crate_at(int index, int current_move)
            {
                if (index < crates.Count)
                {
                    crates[index].print(current_move);
                }
                else if(current_move == last_removed_from && index < previous_height)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("[ ] ");
                    Console.ResetColor();
                }
                else
                {
                    Console.Write("    ");
                }
            }

            public void add_crate_to_bottom(c_crate crate)
            {
                crates.Insert(0, crate);
            }

            public c_crate remove_crate(int current_move)
            {
                c_crate crate = crates[crates.Count - 1];

                if (current_move > last_removed_from)
                {
                    last_removed_from = current_move;
                    previous_height = crates.Count;
                }
                crate.last_modified = current_move;

                crates.RemoveAt(crates.Count - 1);

                return crate;
            }

            public void add_crate(c_crate crate)
            {
                crates.Add(crate);
            }

            public c_crate peek_top_crate()
            {
                return crates[crates.Count - 1];
            }

            public c_crate[] remove_crates(int amount, int current_move)
            {
                c_crate[] removed_crates = crates.GetRange(crates.Count - amount, amount).ToArray();

                if (current_move > last_removed_from)
                {
                    last_removed_from = current_move;
                    previous_height = crates.Count;
                }
                removed_crates.for_each(crate => crate.last_modified = current_move);

                crates.RemoveRange(crates.Count - amount, amount);

                return removed_crates;
            }

            public void add_crates(c_crate[] added_crates)
            {
                foreach (c_crate crate in added_crates)
                {
                    crates.Add(crate);
                }
            }
        }

        [DebuggerDisplay("move {amount} from {source} to {destination}", Type = "s_movement")]
        internal struct s_movement
        {
            public int amount { get; set; }
            public int source { get; set; }
            public int destination { get; set; }
        }

        internal static (c_pile[], s_movement[]) parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            c_pile[] piles = null;

            // Read each line while we're parsing piles.
            for (bool reading_piles = true; reading_piles;)
            {
                string line = input_reader.read_line();

                // After reading the first line we can figure out how many piles we have.
                if (piles == null)
                {
                    piles = new c_pile[(line.Length + 1) / 4];

                    for (int i = 0; i < piles.Length; i++)
                    {
                        piles[i] = new c_pile();
                    }
                }

                // Loop through each pile in the inputted line.
                for (int pile_index = 0; pile_index < piles.Length; pile_index++)
                {
                    char line_char = line[1 + 4 * pile_index];

                    if (line_char == ' ')
                    {
                        // we're over the top of the current pile.
                        continue;
                    }
                    else if (line_char < 'A' || line_char > 'Z')
                    {
                        // we read a non-pile line, stop reading piles.
                        reading_piles = false;
                        break;
                    }
                    else
                    {
                        // Add a crate to a pile.
                        piles[pile_index].add_crate_to_bottom(new c_crate(line_char));
                    }
                }
            }

            input_reader.read_line();

            List<s_movement> movements = new List<s_movement>();

            // Read in the movements.
            while (input_reader.has_more_lines())
            {
                string[] line = input_reader.read_line().Split(' ');

                movements.Add(new s_movement
                {
                    amount = int.Parse(line[1]),
                    source = int.Parse(line[3]),
                    destination = int.Parse(line[5])
                });
            }

            return (piles, movements.ToArray());
        }

        internal static void print_piles(c_pile[] piles, int current_move, bool pretty)
        {
            if (!pretty)
            {
                return;
            }

            Console.WriteLine();

            int max_pile_height = piles.Max(pile => pile.get_crate_count(current_move));

            for (int crate_index = max_pile_height - 1; crate_index >=0; crate_index--)
            {
                piles.for_each(pile => pile.print_crate_at(crate_index, current_move));
                Console.WriteLine();
            }

            for (int i = 1; i <= piles.Length; i++)
            {
                Console.Write($" {i}  ");
            }
            Console.WriteLine();
        }

        public static void part_1(
            string input,
            bool pretty)
        {
            (c_pile[] piles, s_movement[] movements) = parse_input(input, pretty);

            int move_count = 0;
            print_piles(piles, move_count, pretty);

            foreach (s_movement movement in movements)
            {
                move_count++;

                for (int i = 0; i < movement.amount; i++)
                {
                    c_crate crate = piles[movement.source - 1].remove_crate(move_count);
                    piles[movement.destination - 1].add_crate(crate);
                }

                print_piles(piles, move_count, pretty);
            }

            string result = "";
            foreach (c_pile pile in piles)
            {
                result += pile.peek_top_crate().name;
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
            (c_pile[] piles, s_movement[] movements) = parse_input(input, pretty);

            int move_count = 0;
            print_piles(piles, move_count, pretty);

            foreach (s_movement movement in movements)
            {
                move_count++;

                c_crate[] crates = piles[movement.source - 1].remove_crates(movement.amount, move_count);
                piles[movement.destination - 1].add_crates(crates);

                print_piles(piles, move_count, pretty);
            }

            string result = "";
            foreach (c_pile pile in piles)
            {
                result += pile.peek_top_crate().name;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", result);
            Console.ResetColor();
        }
    }
}
