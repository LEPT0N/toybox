using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.extensions;
using advent_of_code_common.input_reader;

namespace advent_of_code_2023.days
{
	internal class day_14
	{
		[DebuggerDisplay("rocks", Type = "c_platform")]
		internal class c_platform
		{
			List<List<char>> rocks;

			public c_platform(List<string> input)
			{
				rocks = new List<List<char>>();

				foreach (string input_line in input)
				{
					List<char> rock_line = new List<char>();

					foreach (char input_char in input_line)
					{
						rock_line.Add(input_char);
					}

					rocks.Add(rock_line);
				}
			}

			// Probably good enough.
			public override int GetHashCode()
			{
				HashCode hash = new HashCode();

				rocks.for_each(rock_row => rock_row.for_each(rock => hash.Add(rock)));

				return hash.ToHashCode();
			}

			public void tilt_up()
			{
				for (int start_row = 1; start_row < rocks.Count; start_row++)
				{
					for (int col = 0; col < rocks[start_row].Count; col++)
					{
						if (rocks[start_row][col] == 'O')
						{
							int target_row = start_row;

							while (target_row > 0 && rocks[target_row - 1][col] == '.')
							{
								target_row--;
							}

							if (start_row != target_row)
							{
								rocks[start_row][col] = '.';

								rocks[target_row][col] = 'O';
							}
						}
					}
				}
			}

			public void tilt_down()
			{
				for (int start_row = rocks.Count - 2; start_row >= 0; start_row--)
				{
					for (int col = 0; col < rocks[start_row].Count; col++)
					{
						if (rocks[start_row][col] == 'O')
						{
							int target_row = start_row;

							while (target_row < rocks.Count - 1 && rocks[target_row + 1][col] == '.')
							{
								target_row++;
							}

							if (start_row != target_row)
							{
								rocks[start_row][col] = '.';

								rocks[target_row][col] = 'O';
							}
						}
					}
				}
			}

			public void tilt_left()
			{
				for (int start_col = 1; start_col < rocks[0].Count; start_col++)
				{
					for (int row = 0; row < rocks.Count; row++)
					{
						if (rocks[row][start_col] == 'O')
						{
							int target_col = start_col;

							while (target_col > 0 && rocks[row][target_col - 1] == '.')
							{
								target_col--;
							}

							if (start_col != target_col)
							{
								rocks[row][start_col] = '.';

								rocks[row][target_col] = 'O';
							}
						}
					}
				}
			}

			public void tilt_right()
			{
				for (int start_col = rocks[0].Count - 2; start_col >= 0; start_col--)
				{
					for (int row = 0; row < rocks.Count; row++)
					{
						if (rocks[row][start_col] == 'O')
						{
							int target_col = start_col;

							while (target_col < rocks[0].Count - 1 && rocks[row][target_col + 1] == '.')
							{
								target_col++;
							}

							if (start_col != target_col)
							{
								rocks[row][start_col] = '.';

								rocks[row][target_col] = 'O';
							}
						}
					}
				}
			}

			public int get_load()
			{
				int load = 0;

				for (int rock_row_index = 0; rock_row_index < rocks.Count; rock_row_index++)
				{
					load += (rocks.Count - rock_row_index) * rocks[rock_row_index].Count(rock => rock == 'O');
				}

				return load;
			}
		}

		internal static c_platform parse_input(
			in string input,
			in bool pretty)
		{
			c_input_reader input_reader = new c_input_reader(input);

			List<string> input_lines = new List<string>();

			while (input_reader.has_more_lines())
			{
				input_lines.Add(input_reader.read_line());
			}

			return new c_platform(input_lines);
		}

		public static void part_1(
			string input,
			bool pretty)
		{
			c_platform platform = parse_input(input, pretty);

			platform.tilt_up();

			int load = platform.get_load();

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine();
			Console.WriteLine("Result = {0}", load);
			Console.ResetColor();
		}

		public static void part_2(
			string input,
			bool pretty)
		{
			c_platform platform = parse_input(input, pretty);

			int i = 0;
			Dictionary<int, int> state_history = new Dictionary<int, int>();
			int latest_state = platform.GetHashCode();
			state_history.Add(latest_state, i);

			const int k_target_iterations = 1000000000;

			// Loop until either we're done or we detected a loop.
			for (i = 1; i < k_target_iterations; i++)
			{
				platform.tilt_up();
				platform.tilt_left();
				platform.tilt_down();
				platform.tilt_right();

				latest_state = platform.GetHashCode();

				if (state_history.ContainsKey(latest_state))
				{
					break;
				}

				state_history.Add(latest_state, i);
			}

			// fast forward to our last loop
			{
				int loop_start = state_history[latest_state];
				int loop_end = i;
				int loop_size = loop_end - loop_start;

				// loop_start + (loop_end - loop_start) * loop_count + remainder = target;

				int loop_count = (k_target_iterations - loop_start) / loop_size;
				int remainder = (k_target_iterations - loop_start) % loop_size;

				int total = loop_start + (loop_size * loop_count) + remainder;
				Debug.Assert(total == k_target_iterations);

				Console.WriteLine($"loop_start = {loop_start}");
				Console.WriteLine($"loop_end   = {loop_end}");
				Console.WriteLine($"loop_size  = {loop_size}");
				Console.WriteLine($"loop_count = {loop_count}");
				Console.WriteLine($"remainder  = {remainder}");

				i = loop_start + (loop_size * loop_count);
			}

			// Loop until the end
			for (; i < k_target_iterations; i++)
			{
				platform.tilt_up();
				platform.tilt_left();
				platform.tilt_down();
				platform.tilt_right();
			}

			int load = platform.get_load();

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine();
			Console.WriteLine("Result = {0}", load);
			Console.ResetColor();
		}
	}
}
