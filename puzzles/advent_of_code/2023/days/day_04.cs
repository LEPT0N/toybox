using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.input_reader;

namespace advent_of_code_2023.days
{
	internal class day_04
	{
		[DebuggerDisplay("{id}: {winning_numbers} | {numbers_you_have}", Type = "c_card")]
		internal class c_card
		{
			public int id;
			public int copies = 1;
			public HashSet<int> winning_numbers;
			public HashSet<int> numbers_you_have;

			public int matching_number_count()
			{
				int matches = numbers_you_have.Where(number_you_have => winning_numbers.Contains(number_you_have)).Count();

				return matches;
			}

			public int compute_score()
			{
				int matches = matching_number_count();

				int score = 0;

				if (matches > 0)
				{
					score = 1 << (matches - 1);
				}

				return score;
			}
		}

		internal static c_card[] parse_input(
			in string input,
			in bool pretty)
		{
			c_input_reader input_reader = new c_input_reader(input);

			List<c_card> cards = new List<c_card>();

			while (input_reader.has_more_lines())
			{
				string line = input_reader.read_line();

				string[] split_line = line.Split(": ");

				c_card card = new c_card();

				card.id = int.Parse(split_line[0].Substring("Card ".Length));

				string[] split_numbers = split_line[1].Split(" | ");

				card.winning_numbers = new HashSet<int>(split_numbers[0]
					.Split(" ")
					.Where(x => !string.IsNullOrEmpty(x))
					.Select(value => int.Parse(value)));

				card.numbers_you_have = new HashSet<int>(split_numbers[1]
					.Split(" ")
					.Where(x => !string.IsNullOrEmpty(x))
					.Select(value => int.Parse(value)));

				cards.Add(card);
			}

			return cards.ToArray();
		}

		public static void part_1(
			string input,
			bool pretty)
		{
			c_card[] cards = parse_input(input, pretty);

			// Add the scores of all cards.

			int[] scores = cards.Select(card => card.compute_score()).ToArray();

			int score = scores.Sum();

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine();
			Console.WriteLine("Result = {0}", score);
			Console.ResetColor();
		}

		public static void part_2(
			string input,
			bool pretty)
		{
			c_card[] cards = parse_input(input, pretty);

			// For each card, add a copy for each of the next 'matching_number_count' cards.

			for (int index = 0; index < cards.Length; index++)
			{
				int matches = cards[index].matching_number_count();

				if (matches > 0)
				{
					for (int child_index = index + 1; child_index < cards.Length && child_index <= index + matches; child_index++)
					{
						// But we may have had more than one of the current card, so add one for each.

						cards[child_index].copies += cards[index].copies;
					}
				}
			}

			int[] copies = cards.Select(card => card.copies).ToArray();

			int total_copies = copies.Sum();

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine();
			Console.WriteLine("Result = {0}", total_copies);
			Console.ResetColor();
		}
	}
}
