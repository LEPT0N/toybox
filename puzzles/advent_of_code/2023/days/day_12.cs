using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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

			public int count_valid_permutations_1()
			{
				int[] permutation = get_initial_permutation();

				int[][] permutations = permutation.get_all_permutations();

				e_element_state[][] possible_states = permutations.Select(permutation => convert_permutation_to_state(permutation)).ToArray();

				e_element_state[][] valid_states = possible_states.Where(possible_state => is_valid_state(possible_state)).ToArray();

				return valid_states.Length;
			}

			public UInt64 count_valid_permutations_recursive(
				in int start_state_index,
				in int current_group_index,
				in int[] largest_possible_groups,
				in int[] on_positions_remaining,
				ref Dictionary<(int, int), UInt64> recursive_results,
				ref e_element_state[] working_state)
			{
				// If we've already computed this result, then just return it again.
				if (recursive_results.TryGetValue((start_state_index, current_group_index), out UInt64 previous_result))
				{
					return previous_result;
				}

				if (current_group_index == group_sizes.Length)
				{
					// We've finished placing each of our groups into valid positions.

					if (start_state_index < state_restriction.Length &&
						on_positions_remaining[start_state_index] > 0)
					{
						// If there are still more required positions remaining, then leave.

						return 0;
					}
					else
					{
						// Otherwise, we found one valid permutation.

						string working_state_string = string.Concat(working_state.Select(element =>
						{
							switch (element)
							{
								case e_element_state.on: return '#';
								case e_element_state.off: return '.';
								case e_element_state.unknown: return '?';
								default: throw new ArgumentException($"Unexpected state '{element}'");
							}
						}));

						Debug.Assert(is_valid_state(working_state));

						return 1;
					}
				}

				if (start_state_index >= state_restriction.Length)
				{
					// If we're at the end of our row, exit.

					return 0;
				}

				UInt64 count = 0;

				int current_group_size = group_sizes[current_group_index];

				for (int state_index = start_state_index;
					state_index + current_group_size <= state_restriction.Length;
					state_index++)
				{
					// Loop through each spot to find one the fits our current group.

					if (largest_possible_groups[state_index] >= current_group_size)
					{
						// We found a spot that can fit our current group.

						if (state_index + current_group_size < state_restriction.Length &&
							state_restriction[state_index + current_group_size] == e_element_state.on)
						{
							// We found a spot that can fit, but if the next spot after is 'on',
							// then putting a group right before it isn't valid.

							if (state_restriction[state_index] == e_element_state.on)
							{
								// If we can't fit the current group into an 'on' position, then we can't just
								// skip it - there are no further valid permutations in this branch.
								break;
							}
							else
							{
								// the current spot was optional, so keep searching.
								continue;
							}
						}
						else if(state_index > 0 &&
							state_restriction[state_index - 1] == e_element_state.on)
						{
							// We found a spot that can fit, but if the previous stop is 'on',
							// then putting a group right after it isn't valid.
							continue;
						}
						else
						{
							Array.Fill(working_state, e_element_state.on, state_index, current_group_size);

							// It's valid for something to fit here, so recurse.
							count += count_valid_permutations_recursive(
								state_index + current_group_size + 1,
								current_group_index + 1,
								largest_possible_groups,
								on_positions_remaining,
								ref recursive_results,
								ref working_state);

							Array.Fill(working_state, e_element_state.off, state_index, current_group_size);
						}
					}
					
					if (state_restriction[state_index] == e_element_state.on)
					{
						// If we find an 'on' position, then we can't just
						// skip it - there are no further valid permutations in this branch.
						break;
					}
				}

				recursive_results[(start_state_index, current_group_index)] = count;

				return count;
			}

			public UInt64 count_valid_permutations_2(in bool pretty)
			{
				// The biggest group that can start at the current position.
				int[] largest_possible_groups = new int[state_restriction.Length];
				int largest_tracking = 0;
				for (int i = state_restriction.Length - 1; i >=0; i--)
				{
					if (state_restriction[i] == e_element_state.off)
					{
						largest_tracking = 0;
					}
					else
					{
						largest_tracking++;
					}

					largest_possible_groups[i] = largest_tracking;
				}

				// The number of required 'on' positions remaining in the string.
				int[] on_positions_remaining = new int[state_restriction.Length];
				int on_positions_found = 0;
				for (int i = state_restriction.Length - 1; i >= 0; i--)
				{
					if (state_restriction[i] == e_element_state.on)
					{
						on_positions_found++;
					}

					on_positions_remaining[i] = on_positions_found;
				}

				Dictionary<(int, int), UInt64> recursive_results = new Dictionary<(int, int), UInt64>();

				e_element_state[] working_state = new e_element_state[state_restriction.Length];
				Array.Fill(working_state, e_element_state.off);

				UInt64 result = count_valid_permutations_recursive(
					0, 0,
					largest_possible_groups,
					on_positions_remaining,
					ref recursive_results,
					ref working_state);

				if (pretty)
				{
					Console.WriteLine($"{state_string} => {result}");
				}

				return result;
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

				string state_input = String.Join('?', Enumerable.Repeat(input_line[0], repetitions));
				string group_sizes_input = String.Join(',', Enumerable.Repeat(input_line[1], repetitions));

				rows.Add(new c_row(state_input, group_sizes_input));
			}

			return rows.ToArray();
		}

		public static void part_1(
			string input,
			bool pretty)
		{
			c_row[] rows = parse_input(input, pretty, 1);

			int sum = rows.Sum(row => row.count_valid_permutations_1());

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

			UInt64 sum = rows.Select(row => row.count_valid_permutations_2(pretty)).sum();

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine();
			Console.WriteLine("Result = {0}", sum);
			Console.ResetColor();
		}
	}
}
