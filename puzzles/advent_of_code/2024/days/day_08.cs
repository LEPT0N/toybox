using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.display_helpers;
using advent_of_code_common.input_reader;
using advent_of_code_common.int_math;

namespace advent_of_code_2024.days
{
	internal class day_08
	{
		[DebuggerDisplay("{bounds}", Type = "c_board")]
		internal class c_board
		{
			private Dictionary<char, List<c_vector>> antennas;

			HashSet<(int, int)> antinodes;

			private c_rectangle bounds;

			public c_board(char[][] input)
			{
				antennas = new Dictionary<char, List<c_vector>>();
				antinodes = new HashSet<(int, int)>();
				bounds = new c_rectangle(c_vector.k_vector_zero, new c_vector(input.Length - 1, input[0].Length - 1));

				for (int row = 0; row < input.Length; row++)
				{
					for (int col = 0; col < input[row].Length; col++)
					{
						char entry = input[row][col];

						if (entry != '.')
						{
							if (!antennas.ContainsKey(entry))
							{
								antennas[entry] = new List<c_vector>();
							}

							antennas[entry].Add(new c_vector(row, col));
						}
					}
				}
			}

			public int find_antinodes()
			{
				antinodes.Clear();

				foreach (List<c_vector> a in antennas.Values)
				{
					for (int i = 0; i < a.Count - 1; i++)
					{
						for (int j = i + 1; j < a.Count; j++)
						{
							c_vector first = a[i].scale(2).subtract(a[j]);

							if (bounds.contains(first))
							{
								antinodes.Add((first.x, first.y));
							}

							c_vector second = a[j].scale(2).subtract(a[i]);

							if (bounds.contains(second))
							{
								antinodes.Add((second.x, second.y));
							}
						}
					}
				}

				return antinodes.Count();
			}

			public int find_harmonic_antinodes()
			{
				antinodes.Clear();

				foreach (List<c_vector> a in antennas.Values)
				{
					for (int i = 0; i < a.Count - 1; i++)
					{
						for (int j = i + 1; j < a.Count; j++)
						{
							{
								c_vector step = a[j].subtract(a[i]);

								for (c_vector p = a[j]; bounds.contains(p); p = p.add(step))
								{
									antinodes.Add((p.x, p.y));
								}
							}

							{
								c_vector step = a[i].subtract(a[j]);

								for (c_vector p = a[i]; bounds.contains(p); p = p.add(step))
								{
									antinodes.Add((p.x, p.y));
								}
							}
						}
					}
				}

				return antinodes.Count();
			}

			public void display()
			{
				display_utilities.display_grid_top(bounds.height + 1);

				for (int row = 0; row <= bounds.width; row++)
				{
					display_utilities.display_grid_side();

					for (int col = 0; col <= bounds.height; col++)
					{
						char antenna = antennas.Keys.FirstOrDefault(key => antennas[key].Any(v => v.x == row && v.y == col));

						if (antenna != '\0')
						{
							Console.Write(antenna);
						}
						else if (antinodes.Contains((row, col)))
						{
							Console.Write('#');
						}
						else
						{
							Console.Write(' ');
						}
					}

					display_utilities.display_grid_side();
					Console.WriteLine();
				}

				display_utilities.display_grid_bottom(bounds.height + 1);
			}
		}

		internal static c_board parse_input(
			in string input,
			in bool pretty)
		{
			c_input_reader input_reader = new c_input_reader(input);

			List<char[]> input_grid = new List<char[]>();

			while (input_reader.has_more_lines())
			{
				input_grid.Add(input_reader.read_line().ToArray());
			}

			return new c_board(input_grid.ToArray());
		}

		public static void part_1(
			string input,
			bool pretty)
		{
			c_board board = parse_input(input, pretty);

			int result = board.find_antinodes();

			if (pretty)
			{
				board.display();
			}

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine();
			Console.WriteLine($"Result = {result}");
			Console.ResetColor();
		}

		public static void part_2(
			string input,
			bool pretty)
		{
			c_board board = parse_input(input, pretty);

			int result = board.find_harmonic_antinodes();

			if (pretty)
			{
				board.display();
			}

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine();
			Console.WriteLine($"Result = {result}");
			Console.ResetColor();
		}
	}
}
