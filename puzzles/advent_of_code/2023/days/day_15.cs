using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.input_reader;

namespace advent_of_code_2023.days
{
	internal class day_15
	{
		internal static int get_hash(string sequence)
		{
			int hash = 0;

			foreach (char item in sequence)
			{
				hash += item;
				hash *= 17;
				hash %= 256;
			}

			return hash;
		}

		[DebuggerDisplay("{label} {focal_length}", Type = "c_lens")]
		internal class c_lens
		{
			public readonly string label;
			public readonly int focal_length;
			public readonly int hash;

			public c_lens(string l, int fl = 0)
			{
				label = l;
				focal_length = fl;
				hash = get_hash(label);
			}
		}

		[DebuggerDisplay("{DebuggerDisplay,nq}", Type = "c_box")]
		internal class c_box
		{
			int box_number;
			LinkedList<c_lens> lenses;

			public string DebuggerDisplay
			{
				get { return $"Box {box_number}: " + string.Join(' ', lenses.Select(lens => $"[{lens.label} {lens.focal_length}]")); }
			}

			public c_box(int number)
			{
				box_number = number;
				lenses = new LinkedList<c_lens>();
			}

			public void add_lens(c_lens lens)
			{
				// If we can find a matching lens, replace it. Otherwise add it to the end.

				Debug.Assert(lens.hash == box_number);

				for (LinkedListNode<c_lens> node = lenses.First; node != null; node = node.Next)
				{
					if (lens.label == node.Value.label)
					{
						node.Value = lens;
						return;
					}
				}

				lenses.AddLast(lens);
			}

			public void remove_lens(c_lens lens)
			{
				// If we can find a matching lens, remove it.

				Debug.Assert(lens.hash == box_number);

				for (LinkedListNode<c_lens> node = lenses.First; node != null; node = node.Next)
				{
					if (lens.label == node.Value.label)
					{
						lenses.Remove(node);
						return;
					}
				}
			}

			public int get_focusing_power()
			{
				// sum the focusing power of each lens in the box.

				int focusing_power = 0;

				int slot = 1;
				LinkedListNode<c_lens> node = lenses.First;

				while (node != null)
				{
					focusing_power += (box_number + 1) * slot * node.Value.focal_length;

					slot++;
					node = node.Next;
				}

				return focusing_power;
			}
		}

		internal static List<string> parse_input(
			in string input,
			in bool pretty)
		{
			c_input_reader input_reader = new c_input_reader(input);

			return input_reader.read_line().Split(',').ToList();
		}

		public static void part_1(
			string input,
			bool pretty)
		{
			List<string> sequences = parse_input(input, pretty);

			int sum = sequences.Sum(sequence => get_hash(sequence));

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine();
			Console.WriteLine("Result = {0}", sum);
			Console.ResetColor();
		}

		public static void part_2(
			string input,
			bool pretty)
		{
			List<string> sequences = parse_input(input, pretty);

			c_box[] boxes = new c_box[256];

			foreach (string sequence in sequences)
			{
				c_lens lens;

				if (sequence.Last() == '-')
				{
					// remove lens.label from the appropriate box.

					string lens_label = sequence.Substring(0, sequence.Length - 1);

					lens = new c_lens(lens_label);

					if (boxes[lens.hash] != null)
					{
						boxes[lens.hash].remove_lens(lens);
					}
				}
				else
				{
					// add/replace lens.label from the appropriate box.

					string[] lens_input = sequence.Split('=');

					lens = new c_lens(lens_input[0], int.Parse(lens_input[1]));

					if (boxes[lens.hash] == null)
					{
						boxes[lens.hash] = new c_box(lens.hash);
					}

					boxes[lens.hash].add_lens(lens);
				}
			}

			int focusing_power = boxes.Sum(box => box == null ? 0 : box.get_focusing_power());

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine();
			Console.WriteLine("Result = {0}", focusing_power);
			Console.ResetColor();
		}
	}
}
