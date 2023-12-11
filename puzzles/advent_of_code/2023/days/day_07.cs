using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.input_reader;

namespace advent_of_code_2023.days
{
	internal class day_07
	{
		internal enum e_card
		{
			wild,
			two,
			three,
			four,
			five,
			six,
			seven,
			eight,
			nine,
			ten,
			jack,
			queen,
			king,
			ace,
		}

		internal enum e_type
		{
			high_card,
			one_pair,
			two_pair,
			three_of_a_kind,
			full_house,
			four_of_a_kind,
			five_of_a_kind,
		}

		[DebuggerDisplay("{DebuggerDisplay,nq}", Type = "c_hand")]
		internal class c_hand : IComparable
		{
			e_type type;
			e_card[] cards;
			int bid;

			private string DebuggerDisplay
			{
				get { return String.Format("{0} [ {1} ]", type, String.Join(", ", cards)); }
			}

			public int get_bid()
			{
				return bid;
			}

			public int CompareTo(object obj)
			{
				if (obj == null) throw new ArgumentException("CompareTo was passed null");

				c_hand other = obj as c_hand;
				if (other == null) throw new ArgumentException("CompareTo was passed a non-hand object");

				int type_compare = type.CompareTo(other.type);

				if (type_compare != 0)
				{
					return type_compare;
				}

				for (int i = 0; i < cards.Length; i++)
				{
					int card_compare = cards[i].CompareTo(other.cards[i]);

					if (card_compare != 0)
					{
						return card_compare;
					}
				}

				return 0;
			}

			public static c_hand create_hand(e_card[] c, int b)
			{
				c_hand hand = new c_hand();

				hand.cards = c;
				hand.bid = b;

				Dictionary<e_card, int> counts = new Dictionary<e_card, int>();

				// Count how many of each card we have.
				foreach (e_card card in hand.cards)
				{
					if (counts.ContainsKey(card))
					{
						counts[card] = counts[card] + 1;
					}
					else
					{
						counts.Add(card, 1);
					}
				}

				// Determine which hand type this is.
				if (counts.Values.Contains(5))
				{
					hand.type = e_type.five_of_a_kind;
				}
				else if (counts.Values.Contains(4))
				{
					hand.type = e_type.four_of_a_kind;
				}
				else if (counts.Values.Contains(3))
				{
					if (counts.Values.Contains(2))
					{
						hand.type = e_type.full_house;
					}
					else
					{
						hand.type = e_type.three_of_a_kind;
					}
				}
				else if (counts.Values.Contains(2))
				{
					if (counts.Values.Where(v => v == 2).Count() == 2)
					{
						hand.type = e_type.two_pair;
					}
					else
					{
						hand.type = e_type.one_pair;
					}
				}
				else
				{
					hand.type = e_type.high_card;
				}

				return hand;
			}

			public static c_hand create_hand_with_wild(e_card[] c, int b, e_card wild_card)
			{
				c_hand hand = new c_hand();

				hand.cards = c;
				hand.bid = b;

				Dictionary<e_card, int> counts = new Dictionary<e_card, int>();

				// Count how many of each card we have.
				foreach (e_card card in hand.cards)
				{
					if (counts.ContainsKey(card))
					{
						counts[card] = counts[card] + 1;
					}
					else
					{
						counts.Add(card, 1);
					}
				}

				// For sorting, wild cards are considered the lowest value.
				for (int i = 0; i < hand.cards.Length; i++)
				{
					if (hand.cards[i] == wild_card)
					{
						hand.cards[i] = e_card.wild;
					}
				}

				// Note how many wilds we have and remove them from our counts.
				int wild_count = 0;
				if (counts.ContainsKey(wild_card))
				{
					wild_count = counts[wild_card];
					counts.Remove(wild_card);
				}

				// Determine which hand type this is.
				if (wild_count == 5)
				{
					// JJJJJ
					hand.type = e_type.five_of_a_kind;
				}
				else if (counts.Values.Max() + wild_count == 5)
				{
					// AAAAA
					// AAAAJ
					// AAAJJ
					// AAJJJ
					// AJJJJ
					hand.type = e_type.five_of_a_kind;
				}
				else if (counts.Values.Max() + wild_count == 4)
				{
					// AAAA B
					// AAAJ B
					// AAJJ B
					// AJJJ B
					hand.type = e_type.four_of_a_kind;
				}
				else if (counts.Values.Max() + wild_count == 3)
				{
					// good			bad
					// AAA BB
					//
					//				AAA BJ		4
					// AAJ BB
					//
					//				AAJ BJ		4
					//				AAA JJ		5
					//				AJJ BB		4
					//
					//				JJJ BB		5
					//				AJJ BJ		4
					//				AAJ JJ		5
					//
					//				AJJ JJ		5
					//
					//				JJJ JJ		5

					if (counts.Values.Contains(3) && counts.Values.Contains(2))
					{
						// AAA BB
						hand.type = e_type.full_house;
					}
					else if (wild_count == 1 && counts.Values.Where(v => v == 2).Count() == 2)
					{
						// AAJ BB
						hand.type = e_type.full_house;
					}
					else
					{
						// The only other possibilities must be 3 of a kind (3oak) since 4oak and 5oak are already handled.

						// AAA B C
						// AAJ B C
						// AJJ B C
						hand.type = e_type.three_of_a_kind;
					}
				}
				else if (counts.Values.Max() + wild_count == 2)
				{
					// two wilds are always a 3oak or better.
					// one wild can never make 'two pairs', since you'd end up with 3oak.

					if (wild_count == 0 && counts.Values.Where(v => v == 2).Count() == 2)
					{
						// AA BB C
						hand.type = e_type.two_pair;
					}
					else
					{
						// AA B C D
						// AJ B C D
						hand.type = e_type.one_pair;
					}
				}
				else
				{
					// A B C D E
					hand.type = e_type.high_card;
				}

				return hand;
			}
		}

		internal static List<c_hand> parse_input(
			in string input,
			in bool jack_is_wild,
			in bool pretty)
		{
			c_input_reader input_reader = new c_input_reader(input);

			List<c_hand> hands = new List<c_hand>();

			while (input_reader.has_more_lines())
			{
				string[] input_line = input_reader.read_line().Split(' ');

				int bid = int.Parse(input_line[1]);

				string input_cards = input_line[0];

				e_card[] cards = new e_card[5];

				for (int i = 0; i < 5; i++)
				{
					switch (input_cards[i])
					{
						case '2': cards[i] = e_card.two; break;
						case '3': cards[i] = e_card.three; break;
						case '4': cards[i] = e_card.four; break;
						case '5': cards[i] = e_card.five; break;
						case '6': cards[i] = e_card.six; break;
						case '7': cards[i] = e_card.seven; break;
						case '8': cards[i] = e_card.eight; break;
						case '9': cards[i] = e_card.nine; break;
						case 'T': cards[i] = e_card.ten; break;
						case 'J': cards[i] = e_card.jack; break;
						case 'Q': cards[i] = e_card.queen; break;
						case 'K': cards[i] = e_card.king; break;
						case 'A': cards[i] = e_card.ace; break;
						default: throw new ArgumentException($"Unexpected card '{input_cards[i]}'");
					}
				}

				if (jack_is_wild)
				{
					hands.Add(c_hand.create_hand_with_wild(cards, bid, e_card.jack));
				}
				else
				{
					hands.Add(c_hand.create_hand(cards, bid));
				}
			}

			return hands;
		}

		internal static int get_total_winnings(List<c_hand> hands)
		{
			hands.Sort();

			int total_winnings = 0;

			for (int i = 0; i < hands.Count; i++)
			{
				total_winnings += (i + 1) * hands[i].get_bid();
			}

			return total_winnings;
		}

		public static void part_1(
			string input,
			bool pretty)
		{
			List<c_hand> hands = parse_input(input, false, pretty);

			int total_winnings = get_total_winnings(hands);

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine();
			Console.WriteLine("Result = {0}", total_winnings);
			Console.ResetColor();
		}

		public static void part_2(
			string input,
			bool pretty)
		{
			List<c_hand> hands = parse_input(input, true, pretty);

			int total_winnings = get_total_winnings(hands);

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine();
			Console.WriteLine("Result = {0}", total_winnings);
			Console.ResetColor();
		}
	}
}
