using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace advent_of_code_2021.Days
{
    internal class Day_21
    {
        internal const int k_max_board_position = 10;
        internal const int k_rolls_per_turn = 3;
        internal const int k_winning_score = 1000;

        [DebuggerDisplay("previous = {previous_roll}, count = {roll_count}", Type = "c_dice")]
        internal class c_dice
        {
            private const UInt64 k_min_dice_roll = 1UL;
            private const UInt64 k_max_dice_roll = 100UL;

            private UInt64 previous_roll = k_min_dice_roll - 1UL;
            public UInt64 roll_count { get; private set; } = 0;

            public UInt64 roll()
            {
                roll_count++;
                previous_roll++;

                if (previous_roll > k_max_dice_roll)
                {
                    previous_roll = k_min_dice_roll;
                }

                return previous_roll;
            }
        }

        [DebuggerDisplay("position = {position}, score = {score}", Type = "c_player")]
        internal class c_player
        {
            public UInt64 position { get; private set; }
            public UInt64 score { get; private set; }

            public c_player(string input)
            {
                string[] parsed_input = input.Split(": ");

                position = UInt64.Parse(parsed_input[1]);
                score = 0;
            }

            public void take_turn(c_dice dice)
            {
                position += dice.roll();
                position += dice.roll();
                position += dice.roll();

                while (position > k_max_board_position)
                {
                    position -= k_max_board_position;
                }

                score += position;
            }
        }

        internal static c_player[] parse_input(
            string input,
            bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            List<c_player> players = new List<c_player>();

            while (input_reader.has_more_lines())
            {
                players.Add(new c_player(input_reader.read_line()));
            }

            return players.ToArray();
        }

        public static void Part_1(
            string input,
            bool pretty)
        {
            c_player[] players = parse_input(input, pretty);

            c_dice dice = new c_dice();
            int current_player = 0;

            while (players.All(player => player.score < k_winning_score))
            {
                players[current_player].take_turn(dice);

                current_player = (current_player + 1) % 2;
            }

            UInt64 losing_score = players.Min(player => player.score);
            UInt64 result = losing_score * dice.roll_count;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", result);
            Console.ResetColor();
        }

        internal const int k_quantum_winning_score = 21;

        [DebuggerDisplay("p {position} s {score}", Type = "s_quantum_player")]
        internal struct s_quantum_player
        {
            public UInt64 position;
            public UInt64 score;
        }

        internal static s_quantum_player roll_quantum_dice(
            s_quantum_player player,
            in int dice_rolled_this_turn,
            in UInt64 dice_result)
        {
            player.position += dice_result;

            if (dice_rolled_this_turn == k_rolls_per_turn)
            {
                while (player.position > k_max_board_position)
                {
                    player.position -= k_max_board_position;
                }

                player.score += player.position;
            }

            return player;
        }

        public static void Part_2(
            string input,
            bool pretty)
        {
            c_player[] players = parse_input(input, pretty);

            UInt64[] player_win_count = { 0UL, 0UL };

            // Keep track of the list of all players currently still playing, bucketed by player state.
            // Keep track of each player separately so that the combinations don't explode.
            // Winning players are removed from the list.
            Dictionary<s_quantum_player, UInt64>[] quantum_players = new Dictionary<s_quantum_player, UInt64>[2];

            s_quantum_player initial_player_1;
            initial_player_1.position = players[0].position;
            initial_player_1.score = 0;
            quantum_players[0] = new Dictionary<s_quantum_player, UInt64>();
            quantum_players[0][initial_player_1] = 1UL;

            s_quantum_player initial_player_2;
            initial_player_2.position = players[1].position;
            initial_player_2.score = 0;
            quantum_players[1] = new Dictionary<s_quantum_player, UInt64>();
            quantum_players[1][initial_player_2] = 1UL;

            int current_player_index = 0;
            int dice_rolled_this_turn = 0;

            // Loop as long as there are still instances of both players that haven't won yet.
            while (quantum_players[0].Count > 0 && quantum_players[1].Count > 0)
            {
                Dictionary<s_quantum_player, UInt64> current_players = quantum_players[current_player_index];
                Dictionary<s_quantum_player, UInt64> new_current_players = new Dictionary<s_quantum_player, UInt64>();

                dice_rolled_this_turn++;

                // Loop through each possible current player
                foreach (s_quantum_player current_player in current_players.Keys)
                {
                    UInt64 current_player_count = current_players[current_player];

                    // Generate a new possible player for each possible dice result.
                    for (UInt64 dice_result = 1UL; dice_result <= 3UL; dice_result++)
                    {
                        s_quantum_player new_player = roll_quantum_dice(
                            current_player,
                            dice_rolled_this_turn,
                            dice_result);

                        // If the player won, then count the winning parallel universes and don't add these players back to our list of players.
                        if (new_player.score >= k_quantum_winning_score)
                        {
                            int other_player_index = (current_player_index + 1) % 2;
                            Dictionary<s_quantum_player, UInt64> other_players = quantum_players[other_player_index];

                            // The total number of winning parallel universes is the product of the number of current_player universes
                            // that ended up in this state and the sum of all universes where the other player is still playing.

                            UInt64 other_player_count = other_players.Aggregate(0UL, (sum, key_value) => sum + key_value.Value);

                            UInt64 winning_universes = current_player_count * other_player_count;

                            if (pretty)
                            {
                                Console.WriteLine("Player {0} wins detected. Player in {1} possible states. Other player in {2} possible states. Total Wins detected = {3}",
                                    current_player_index,
                                    current_player_count,
                                    other_player_count,
                                    winning_universes);
                            }

                            player_win_count[current_player_index] += winning_universes;
                        }
                        // Else the player didn't win, so add them back to the list of playing players.
                        else
                        {
                            new_current_players.TryAdd(new_player, 0);

                            new_current_players[new_player] += current_player_count;
                        }
                    }
                }

                // Update the list of current playes with the new list generated.
                quantum_players[current_player_index] = new_current_players;

                // update which roll/turn we're on.
                if (dice_rolled_this_turn == k_rolls_per_turn)
                {
                    dice_rolled_this_turn = 0;

                    current_player_index = (current_player_index + 1) % 2;
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Player 1 won {0} times.", player_win_count[0]);
            Console.WriteLine("Player 2 won {0} times.", player_win_count[1]);
            Console.ResetColor();
        }
    }
}
