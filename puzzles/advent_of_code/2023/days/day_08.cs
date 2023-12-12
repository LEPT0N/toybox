using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.math_helpers;
using advent_of_code_common.input_reader;

namespace advent_of_code_2023.days
{
	internal class day_08
	{
		internal enum e_direction
		{
			left,
			right,
		}

		[DebuggerDisplay("index = {current_direction_index}", Type = "c_directions")]
		internal class c_directions
		{
			readonly e_direction[] directions;
			int current_direction_index;

			public c_directions(string input)
			{
				current_direction_index = 0;

				directions = input.Select(c => (c == 'R' ? e_direction.right : e_direction.left)).ToArray();
			}

			public e_direction read_direction()
			{
				e_direction direction = directions[current_direction_index];

				current_direction_index = (current_direction_index + 1) % directions.Length;

				return direction;
			}
		}

		[DebuggerDisplay("{name} = [{left_name}, {right_name}]", Type = "c_node")]
		internal class c_node
		{
			string name;

			string left_name;
			c_node left;

			string right_name;
			c_node right;

			public c_node(string n, string l, string r)
			{
				name = n;
				left_name = l;
				right_name = r;
			}

			public string get_name()
			{
				return name;
			}

			public void fill_children(Dictionary<string, c_node> nodes)
			{
				left = nodes[left_name];
				right = nodes[right_name];
			}

			public c_node take_step(e_direction direction)
			{
				return (direction == e_direction.left) ? left : right;
			}
		}

		internal static (c_directions, Dictionary<string, c_node>) parse_input(
			in string input,
			in bool pretty)
		{
			c_input_reader input_reader = new c_input_reader(input);

			c_directions directions = new c_directions(input_reader.read_line());

			// eat the empty line
			input_reader.read_line();

			Dictionary<string, c_node> nodes = new Dictionary<string, c_node>();

			while (input_reader.has_more_lines())
			{
				string[] input_line = input_reader
					.read_line()
					.Split(new char[] { ' ', '=', '(', ',', ')' })
					.Where(s => !string.IsNullOrEmpty(s))
					.ToArray();

				nodes[input_line[0]] = new c_node(input_line[0], input_line[1], input_line[2]);
			}

			foreach (c_node node in nodes.Values)
			{
				node.fill_children(nodes);
			}

			return (directions, nodes);
		}

		public static void part_1(
			string input,
			bool pretty)
		{
			(c_directions directions, Dictionary<string, c_node> nodes)  = parse_input(input, pretty);

			int step_count = 0;
			c_node current_node = nodes["AAA"];

			while (current_node.get_name() != "ZZZ")
			{
				step_count++;

				current_node = current_node.take_step(directions.read_direction());
			}

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine();
			Console.WriteLine("Result = {0}", step_count);
			Console.ResetColor();
		}

		public static void part_2(
			string input,
			bool pretty)
		{
			(c_directions directions, Dictionary<string, c_node> nodes) = parse_input(input, pretty);

			UInt64 step_count = 0;
			c_node[] current_nodes = nodes.Values.Where(node => node.get_name().Last() == 'A').ToArray();

			UInt64[] first_found_z = new UInt64[current_nodes.Length];

			// Loop until all nodes are on a Z.
			while (current_nodes.Any(node => node.get_name().Last() != 'Z'))
			{
				step_count++;

				e_direction direction = directions.read_direction();

				// Take one step for each node.
				for (int node_index = 0; node_index < current_nodes.Length; node_index++)
				{
					current_nodes[node_index] = current_nodes[node_index].take_step(direction);

					// Note if we found a loop for the current node.
					if (current_nodes[node_index].get_name().Last() == 'Z')
					{
						if (first_found_z[node_index] == 0)
						{
							first_found_z[node_index] = step_count;
						}
					}
				}

				// We found a loop for all nodes. Exit early.
				if (first_found_z.All(v => v != 0))
				{
					break;
				}
			}

			// We didn't find all the Z's, but we found all of their loops.
			// Thankfully the input has all paths loop back through their beginning.
			if (current_nodes.Any(node => node.get_name().Last() != 'Z'))
			{
				step_count = math_helpers.find_least_common_multiple(first_found_z);
			}

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine();
			Console.WriteLine("Result = {0}", step_count);
			Console.ResetColor();
		}
	}
}
