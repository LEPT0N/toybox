using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.input_reader;

namespace advent_of_code_2022.days
{
    internal class day_02
    {
        internal enum e_hand
        {
            rock,
            paper,
            scissors,
        };

        internal enum e_result
        {
            win,
            tie,
            lose,
        }

        [DebuggerDisplay("{my_hand} , {enemy_hand}, {intended_result}", Type = "s_game")]
        internal struct s_game
        {
            public e_hand enemy_hand { get; set; }
            public e_hand my_hand { get; set; }
            public e_result intended_result { get; set; }

            public int get_score_1()
            {
                int score = 0;
   
                if ((my_hand == e_hand.rock && enemy_hand == e_hand.scissors) ||
                    (my_hand == e_hand.scissors && enemy_hand == e_hand.paper) ||
                    (my_hand == e_hand.paper && enemy_hand == e_hand.rock))
                {
                    score += 6;
                }
                else if (my_hand == enemy_hand)
                {
                    score += 3;
                }

                switch (my_hand)
                {
                    case e_hand.rock:
                        score += 1;
                        break;

                    case e_hand.paper:
                        score += 2;
                        break;

                    case e_hand.scissors:
                        score += 3;
                        break;
                }

                return score;
            }

            public int get_score_2()
            {
                int score = 0;

                switch (intended_result)
                {
                    case e_result.win:
                        score += 6;
                        break;

                    case e_result.tie:
                        score += 3;
                        break;
                }

                switch (enemy_hand, intended_result)
                {
                    case (e_hand.rock, e_result.tie):
                    case (e_hand.paper, e_result.lose):
                    case (e_hand.scissors, e_result.win):
                        score += 1;
                        break;

                    case (e_hand.rock, e_result.win):
                    case (e_hand.paper, e_result.tie):
                    case (e_hand.scissors, e_result.lose):
                        score += 2;
                        break;

                    case (e_hand.rock, e_result.lose):
                    case (e_hand.paper, e_result.win):
                    case (e_hand.scissors, e_result.tie):
                        score += 3;
                        break;
                }

                return score;
            }
        }

        internal static e_hand string_to_hand(string s)
        {
            switch (s)
            {
                case "A":
                case "X":
                    return e_hand.rock;

                case "B":
                case "Y":
                    return e_hand.paper;

                case "C":
                case "Z":
                    return e_hand.scissors;
            }

            throw new ArgumentException($"Invalid hand string '{s}'");
        }

        internal static e_result string_to_result(string s)
        {
            switch (s)
            {
                case "X":
                    return e_result.lose;

                case "Y":
                    return e_result.tie;

                case "Z":
                    return e_result.win;
            }

            throw new ArgumentException($"Invalid result string '{s}'");
        }

        internal static s_game[] parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            List<s_game> games = new List<s_game>();

            while (input_reader.has_more_lines())
            {
                string[] input_line = input_reader.read_line().Split(' ');

                games.Add(new s_game
                {
                    enemy_hand = string_to_hand(input_line[0]),
                    my_hand = string_to_hand(input_line[1]),
                    intended_result = string_to_result(input_line[1]),
                });
            }

            return games.ToArray();
        }

        public static void part_1(
            string input,
            bool pretty)
        {
            s_game[] games = parse_input(input, pretty);

            int score = games.Select(game => game.get_score_2()).Sum();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", score);
            Console.ResetColor();
        }

        public static void part_2(
            string input,
            bool pretty)
        {
            s_game[] games = parse_input(input, pretty);

            int score = games.Select(game => game.get_score_2()).Sum();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", score);
            Console.ResetColor();
        }
    }
}
