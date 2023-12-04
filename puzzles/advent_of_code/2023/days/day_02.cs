using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.input_reader;

namespace advent_of_code_2023.days
{
	internal class day_02
	{
		[DebuggerDisplay("red = {red} green = {green} blue = {blue}", Type = "c_hand")]
		internal class c_hand
		{
			public int red;
			public int green;
			public int blue;
		}

		[DebuggerDisplay("{id}", Type = "c_game")]
		internal class c_game
		{
			public int id;
			public c_hand[] hands;
		}

		internal static c_game[] parse_input(
			in string input,
			in bool pretty)
		{
			c_input_reader input_reader = new c_input_reader(input);

			List<c_game> games = new List<c_game>();

			// Read each line
			while (input_reader.has_more_lines())
			{
				string line = input_reader.read_line();

				// Split each line into the game id and the hands.
				string[] split_line = line.Split(": ");

				// Parse the game id.
				int game_id = int.Parse(split_line[0].Substring("Game ".Length));

				List<c_hand> hands = new List<c_hand>();

				// Parse each hand
				foreach (string hand_string in split_line[1].Split("; "))
				{
					c_hand hand = new c_hand();

					// Parse each color in the hand.
					foreach (string color_string in hand_string.Split(", "))
					{
						// Split a color into a name and amount
						string[] split_color_string = color_string.Split(' ');

						int color_amount = int.Parse(split_color_string[0]);
						string color_name_string = split_color_string[1];

						// Add the color to the hand.
						switch (color_name_string)
						{
							case "red": hand.red = color_amount; break;
							case "green": hand.green = color_amount; break;
							case "blue": hand.blue = color_amount; break;
							default: throw new ArgumentException($"Invalid color '{color_name_string}'");
						}
					}

					// Add the hand to the list of hands.
					hands.Add(hand);
				}

				// Add the game to the list of games.
				c_game game = new c_game();
				game.id = game_id;
				game.hands = hands.ToArray();

				games.Add(game);
			}

			return games.ToArray();
		}

		public static void part_1(
			string input,
			bool pretty)
		{
			c_game[] games = parse_input(input, pretty);

			const int k_red_count = 12;
			const int k_green_count = 13;
			const int k_blue_count = 14;

			c_game[] valid_games = games.Where(game => game.hands.All(hand =>
				hand.red <= k_red_count &&
				hand.green <= k_green_count &&
				hand.blue <= k_blue_count)).ToArray();

			int sum = valid_games.Sum(game => game.id);

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine();
			Console.WriteLine("Result = {0}", sum);
			Console.ResetColor();
		}

		public static void part_2(
			string input,
			bool pretty)
		{
			c_game[] games = parse_input(input, pretty);

			c_hand[] min_hands = games.Select(game => new c_hand
			{
				red = game.hands.Max(hand => hand.red),
				green = game.hands.Max(hand => hand.green),
				blue = game.hands.Max(hand => hand.blue)
			}).ToArray();

			int[] powers = min_hands.Select(hand => hand.red * hand.green * hand.blue).ToArray();

			int power = powers.Sum();

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine();
			Console.WriteLine("Result = {0}", power);
			Console.ResetColor();
		}
	}
}
