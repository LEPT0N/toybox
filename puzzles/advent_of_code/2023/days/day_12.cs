using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Resources;
using advent_of_code_common.extensions;
using advent_of_code_common.input_reader;

namespace advent_of_code_2023.days
{
	internal class day_12
	{
		internal enum e_element_state
		{
			on,
			off,
			unknown,
		}

		[DebuggerDisplay("{state_string}", Type = "c_row")]
		internal class c_row
		{
			string state_string;
			e_element_state[] state_restriction;
			int[] group_sizes;

			public c_row(string state_input, string group_sizes_input)
			{
				state_string = state_input;

				state_restriction = state_input.Select(c =>
				{
					switch (c)
					{
						case '#': return e_element_state.on;
						case '.': return e_element_state.off;
						case '?': return e_element_state.unknown;
						default: throw new ArgumentException($"Unexpected state '{c}'");
					}
				}).ToArray();

				group_sizes = group_sizes_input.Split(',').Select(c => int.Parse(c)).ToArray();
			}

			private int[] get_initial_permutation()
			{
				int num_floating_off_cells = state_restriction.Length;
				List<int> permutation = new List<int>();

				for (int group_index = 0; group_index < group_sizes.Length; group_index++)
				{
					num_floating_off_cells -= group_sizes[group_index];

					if (group_index > 0)
					{
						num_floating_off_cells--;
					}
				}

				for (int i = 0; i < num_floating_off_cells; i++)
				{
					permutation.Add(0);
				}

				for (int i = 0; i < group_sizes.Length; i++)
				{
					permutation.Add(1);
				}

				return permutation.ToArray();
			}

			private e_element_state[] convert_permutation_to_state(int[] permutation)
			{
				List<e_element_state> state = new List<e_element_state>();

				int next_group_index = 0;

				for (int i = 0; i < permutation.Length; i++)
				{
					if (permutation[i] == 0)
					{
						state.Add(e_element_state.off);
					}
					else
					{
						int group_size = group_sizes[next_group_index];

						if (next_group_index > 0)
						{
							state.Add(e_element_state.off);
						}

						for (int group_element_index = 0; group_element_index < group_size; group_element_index++)
						{
							state.Add(e_element_state.on);
						}

						next_group_index++;
					}
				}

				return state.ToArray();
			}

			private bool is_valid_state(e_element_state[] possible_state)
			{
				for(int i = 0; i < possible_state.Length; i++)
				{
					if ((possible_state[i] == e_element_state.on && state_restriction[i] == e_element_state.off) ||
						(possible_state[i] == e_element_state.off && state_restriction[i] == e_element_state.on))
					{
						return false;
					}
				}

				return true;
			}

			public int count_valid_permutations()
			{
				int[] permutation = get_initial_permutation();

				int[][] permutations = permutation.get_all_permutations();

				e_element_state[][] possible_states = permutations.Select(permutation => convert_permutation_to_state(permutation)).ToArray();

				e_element_state[][] valid_states = possible_states.Where(possible_state => is_valid_state(possible_state)).ToArray();

				return valid_states.Length;
			}
		}

		internal static c_row[] parse_input(
			in string input,
			in bool pretty,
			in int repetitions)
		{
			c_input_reader input_reader = new c_input_reader(input);

			List<c_row> rows = new List<c_row>();

			while (input_reader.has_more_lines())
			{
				string[] input_line = input_reader.read_line().Split(' ');

				string state_input = input_line[0];
				string group_sizes_input = input_line[1];

				for (int i = 1; i < repetitions; i++)
				{
					state_input += "?" + input_line[0];
					group_sizes_input += "," + input_line[1];
				}

				rows.Add(new c_row(state_input, group_sizes_input));
			}

			return rows.ToArray();
		}

		public static void part_1(
			string input,
			bool pretty)
		{
			c_row[] rows = parse_input(input, pretty, 1);

			int sum = rows.Sum(row => row.count_valid_permutations());

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine();
			Console.WriteLine("Result = {0}", sum);
			Console.ResetColor();
		}

		public static void part_2(
			string input,
			bool pretty)
		{
			c_row[] rows = parse_input(input, pretty, 5);

			int sum = rows.Sum(row => row.count_valid_permutations());

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine();
			Console.WriteLine("Result = {0}", sum);
			Console.ResetColor();
		}
	}
}
