using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace advent_of_code_2020.Days
{
    internal class Day_22
    {
        internal static Queue<int> parse_player(c_input_reader input_reader)
        {
            // Discard the 'Player X' line.
            input_reader.read_line();

            Queue<int> player = new Queue<int>();

            // Loop until we either run out of input or read an empty line.
            string input_line;
            while (input_reader.has_more_lines())
            {
                input_line = input_reader.read_line();

                if (string.IsNullOrEmpty(input_line))
                {
                    break;
                }

                // Enqueue the input line as a number.
                player.Enqueue(int.Parse(input_line));
            }

            return player;
        }

        internal static (Queue<int>, Queue<int>) parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            Queue<int> player_1 = parse_player(input_reader);
            Queue<int> player_2 = parse_player(input_reader);

            return (player_1, player_2);
        }

        public static int calculate_score(Queue<int> player)
        {
            int score = 0;

            while (player.Count > 0)
            {
                score += player.Count * player.Dequeue();
            }

            return score;
        }

        public static void Part_1(
            string input,
            bool pretty)
        {
            (Queue<int> player_1, Queue<int> player_2) = parse_input(input, pretty);

            // Play until one player runs out of cards.
            while (player_1.Count > 0 && player_2.Count > 0)
            {
                // Each player draws a card.
                int player_1_card = player_1.Dequeue();
                int player_2_card = player_2.Dequeue();

                // The winner of the round gets the cards.
                if (player_1_card > player_2_card)
                {
                    player_1.Enqueue(player_1_card);
                    player_1.Enqueue(player_2_card);
                }
                else
                {
                    player_2.Enqueue(player_2_card);
                    player_2.Enqueue(player_1_card);
                }
            }

            Queue<int> winner = (player_1.Count > 0 ? player_1 : player_2);

            int score = calculate_score(winner);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", score);
            Console.ResetColor();
        }

        public static Queue<int> play_recursive_game(Queue<int> player_1, Queue<int> player_2)
        {
            // Optimization: If player 1 has the highest card AND that card is bigger than the total count of cards
            // in the game, then whenever player_1 draws that card the game will not recurse, and player_1 will lose the round.
            // This means that the game will either end via the recursion rule, or player 1 will eventually get all the cards
            // and win normally. I wish I had thought of this, but I found it on Reddit after solving it myself.
            // This optimization takes this from 16s to 0.5s.
            {
                int player_1_highest = player_1.Max();
                int player_2_highest = player_2.Max();
                if (player_1_highest > player_2_highest)
                {
                    if (player_1_highest > player_1.Count + player_2.Count)
                    {
                        return player_1;
                    }
                }
            }

            // Keep track of the previous states of the game.
            List<(int[], int[])> previous_game_states = new List<(int[], int[])>();

            while (player_1.Count > 0 && player_2.Count > 0)
            {
                (int[] player_1_state, int[] player_2_state) = (player_1.ToArray(), player_2.ToArray());

                // There has GOT to be a faster way to determine this but I can't think of how.
                if (previous_game_states.Any(previous_game_state =>
                    previous_game_state.Item1.SequenceEqual(player_1_state) &&
                    previous_game_state.Item2.SequenceEqual(player_2_state)))
                {
                    // If we've seen this state before in the current game, then we're in an infinite loop. Player 1 wins this game.
                    return player_1;
                }
                else
                {
                    // Otherwise, add the current state to the list of previous game states.
                    previous_game_states.Add((player_1_state, player_2_state));
                }

                // Each player draws a card.
                int player_1_card = player_1.Dequeue();
                int player_2_card = player_2.Dequeue();

                // If we can recurse, then do so.
                if (player_1.Count >= player_1_card && player_2.Count >= player_2_card)
                {
                    // Construct the new hands of cards for the subgame, and play it.
                    Queue<int> subgame_player_1 = new Queue<int>(player_1.ToArray().Take(player_1_card));
                    Queue<int> subgame_player_2 = new Queue<int>(player_2.ToArray().Take(player_2_card));

                    Queue<int> subgame_winner = play_recursive_game(subgame_player_1, subgame_player_2);

                    // The winner of the round is whoever won the subgame.
                    if (subgame_winner == subgame_player_1)
                    {
                        player_1.Enqueue(player_1_card);
                        player_1.Enqueue(player_2_card);
                    }
                    else
                    {
                        player_2.Enqueue(player_2_card);
                        player_2.Enqueue(player_1_card);
                    }
                }
                else
                {
                    // Otherwise, play a normal round.
                    if (player_1_card > player_2_card)
                    {
                        player_1.Enqueue(player_1_card);
                        player_1.Enqueue(player_2_card);
                    }
                    else
                    {
                        player_2.Enqueue(player_2_card);
                        player_2.Enqueue(player_1_card);
                    }
                }
            }

            return (player_1.Count > 0 ? player_1 : player_2);
        }

        public static void Part_2(
            string input,
            bool pretty)
        {
            (Queue<int> player_1, Queue<int> player_2) = parse_input(input, pretty);

            Queue<int> winner = play_recursive_game(player_1, player_2);

            int score = calculate_score(winner);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", score);
            Console.ResetColor();
        }
    }
}
