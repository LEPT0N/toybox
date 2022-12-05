using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.input_reader;

namespace advent_of_code_2022.days
{
    internal class day_05
    {
        internal class c_pile
        {
            List<char> crates = new List<char>();

            public void add_crate_to_bottom(char crate)
            {
                crates.Insert(0, crate);
            }

            public char remove_crate()
            {
                char crate = crates[crates.Count - 1];

                crates.RemoveAt(crates.Count - 1);

                return crate;
            }

            public void add_crate(char crate)
            {
                crates.Add(crate);
            }

            public char peek_top_crate()
            {
                return crates[crates.Count - 1];
            }

            public char[] remove_crates(int amount)
            {
                char[] removed_crates = crates.GetRange(crates.Count - amount, amount).ToArray();

                crates.RemoveRange(crates.Count - amount, amount);

                return removed_crates;
            }

            public void add_crates(char[] added_crates)
            {
                foreach (char crate in added_crates)
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
                        piles[pile_index].add_crate_to_bottom(line_char);
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

        public static void part_1(
            string input,
            bool pretty)
        {
            (c_pile[] piles, s_movement[] movements) = parse_input(input, pretty);

            foreach (s_movement movement in movements)
            {
                for (int i = 0; i < movement.amount; i++)
                {
                    char crate = piles[movement.source - 1].remove_crate();
                    piles[movement.destination - 1].add_crate(crate);
                }
            }

            string result = "";
            foreach (c_pile pile in piles)
            {
                result += pile.peek_top_crate();
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

            foreach (s_movement movement in movements)
            {
                char[] crates = piles[movement.source - 1].remove_crates(movement.amount);
                piles[movement.destination - 1].add_crates(crates);
            }

            string result = "";
            foreach (c_pile pile in piles)
            {
                result += pile.peek_top_crate();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", result);
            Console.ResetColor();
        }
    }
}
