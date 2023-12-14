using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.extensions;
using advent_of_code_common.input_reader;

namespace advent_of_code_2023.days
{
	internal class day_13
	{
		[DebuggerDisplay("todo", Type = "c_pattern")]
		internal class c_pattern
		{
			bool[,] data;

			int[] row_hashes;
			int[] col_hashes;

			public c_pattern(List<string> input)
			{
				List<List<bool>> data_list = new List<List<bool>>();

				foreach (string input_line in input)
				{
					List<bool> data_line = new List<bool>();

					foreach (char input_char in input_line)
					{
						data_line.Add(input_char == '#');
					}

					data_list.Add(data_line);
				}

				data = data_list.to_2d_array();

				hash_data();
			}

			void hash_data()
			{
				row_hashes = new int[data.GetLength(0)];

				for (int row = 0; row < data.GetLength(0); row++)
				{
					int row_hash = 0;

					for (int col = 0; col < data.GetLength(1); col++)
					{
						if (data[row, col])
						{
							row_hash += (1 << col);
						}
					}

					row_hashes[row] = row_hash;
				}

				col_hashes = new int[data.GetLength(1)];

				for (int col = 0; col < data.GetLength(1); col++)
				{
					int col_hash = 0;

					for (int row = 0; row < data.GetLength(0); row++)
					{
						if (data[row, col])
						{
							col_hash += (1 << row);
						}
					}

					col_hashes[col] = col_hash;
				}
			}

			static bool is_mirror_line(ref int[] hashes, int left)
			{
				for (int right = left + 1; 
					left >= 0 && right < hashes.Length;
					left--, right++)
				{
					if (hashes[left] != hashes[right])
					{
						return false;
					}
				}

				return true;
			}

			static bool find_mirror_index(ref int[] hashes, int ignored_result, out int result)
			{
				for (int left_start = 0; left_start < hashes.Length - 1; left_start++)
				{
					if (is_mirror_line(ref hashes, left_start))
					{
						if (left_start != ignored_result)
						{
							result = left_start;
							return true;
						}
					}
				}

				result = -1;
				return false;
			}

			public int find_mirror_score()
			{
				if (find_mirror_index(ref row_hashes, -1, out int row_result))
				{
					return (row_result + 1) * 100;
				}

				if (find_mirror_index(ref col_hashes, -1, out int col_result))
				{
					return (col_result + 1);
				}

				throw new ArgumentException("pattern has no mirror!");
			}

			public int find_smuged_mirror_score()
			{
				// First find our original mirror line
				find_mirror_index(ref row_hashes, -1, out int unsmudged_row_result);
				find_mirror_index(ref col_hashes, -1, out int unsmudged_col_result);

				int[] smudged_row_hashes = (int[])row_hashes.Clone();
				int[] smudged_col_hashes = (int[])col_hashes.Clone();

				// Smudge each element until we find another mirror line
				for (int row = 0; row < data.GetLength(0); row++)
				{
					for (int col = 0; col < data.GetLength(1); col++)
					{
						// Check if smudging [row, col] causes a mirror in the rows
						{
							smudged_row_hashes[row] = smudged_row_hashes[row] ^ (1 << col);

							if (find_mirror_index(ref smudged_row_hashes, unsmudged_row_result, out int smudged_row_result))
							{
									return (smudged_row_result + 1) * 100;
							}

							smudged_row_hashes[row] = row_hashes[row];
						}

						// Check if smudging [row, col] causes a mirror in the columns
						{
							smudged_col_hashes[col] = smudged_col_hashes[col] ^ (1 << row);

							if (find_mirror_index(ref smudged_col_hashes, unsmudged_col_result, out int smudged_col_result))
							{
								return (smudged_col_result + 1);
							}

							smudged_col_hashes[col] = col_hashes[col];
						}
					}
				}

				throw new ArgumentException("pattern has no smudge that makes another mirror!");
			}
		}

		internal static c_pattern[] parse_input(
			in string input,
			in bool pretty)
		{
			c_input_reader input_reader = new c_input_reader(input);

			List<c_pattern> patterns = new List<c_pattern>();

			while (input_reader.has_more_lines())
			{
				List<string> pattern_input = new List<string>();

				while (input_reader.has_more_lines() && !string.IsNullOrEmpty(input_reader.peek_line()))
				{
					pattern_input.Add(input_reader.read_line());
				}

				patterns.Add(new c_pattern(pattern_input));

				if (input_reader.has_more_lines())
				{
					// pass by each empty line between patterns;
					input_reader.read_line();
				}
			}

			return patterns.ToArray();
		}

		public static void part_1(
			string input,
			bool pretty)
		{
			c_pattern[] patterns = parse_input(input, pretty);

			int sum = patterns.Sum(pattern => pattern.find_mirror_score());

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine();
			Console.WriteLine("Result = {0}", sum);
			Console.ResetColor();
		}

		public static void part_2(
			string input,
			bool pretty)
		{
			c_pattern[] patterns = parse_input(input, pretty);

			int sum = patterns.Sum(pattern => pattern.find_smuged_mirror_score());

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine();
			Console.WriteLine("Result = {0}", sum);
			Console.ResetColor();
		}
	}
}
