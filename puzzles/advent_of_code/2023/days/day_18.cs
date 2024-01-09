using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using advent_of_code_common.big_int_math;
using advent_of_code_common.display_helpers;
using advent_of_code_common.extensions;
using advent_of_code_common.input_reader;
using advent_of_code_common.int_math;
using advent_of_code_common.math_helpers;

namespace advent_of_code_2023.days
{
	internal class day_18
	{
		public static readonly c_big_vector k_vector_up = new c_big_vector(-1, 0, 0);
		public static readonly c_big_vector k_vector_down = new c_big_vector(1, 0, 0);
		public static readonly c_big_vector k_vector_left = new c_big_vector(0, -1, 0);
		public static readonly c_big_vector k_vector_right = new c_big_vector(0, 1, 0);

		[DebuggerDisplay("{DebuggerDisplay,nq}", Type = "c_instruction")]
		internal class c_instruction
		{
			public readonly string direction_char;
			public readonly c_big_vector direction;
			public readonly Int64 length;
			public readonly Color color;

			private string DebuggerDisplay
			{
				get
				{
					return $"{direction_char} {length} (#{color.R.ToString("X")}{color.G.ToString("X")}{color.B.ToString("X")})";
				}
			}

			public c_instruction(string input)
			{
				string[] split_input = input.Split(' ');

				direction_char = split_input[0];

				direction = direction_char_to_vector(direction_char);

				length = int.Parse(split_input[1]);

				string color_input = split_input[2].Substring(2, split_input[2].Length - 3);

				int red = Convert.ToInt32(color_input.Substring(0, 2), 16);
				int green = Convert.ToInt32(color_input.Substring(2, 2), 16);
				int blue = Convert.ToInt32(color_input.Substring(4, 2), 16);

				color = Color.FromArgb(red, green, blue);
			}

			public c_instruction(int l, string d)
			{
				direction_char = d;
				length = l;
				color = Color.Black;

				direction = direction_char_to_vector(direction_char);
			}

			private static c_big_vector direction_char_to_vector(string direction_char)
			{
				c_big_vector direction_result;

				switch (direction_char)
				{
					case "U": direction_result = k_vector_up; break;
					case "D": direction_result = k_vector_down; break;
					case "L": direction_result = k_vector_left; break;
					case "R": direction_result = k_vector_right; break;
					default: throw new Exception($"Unexpected direction {direction_char}");
				}

				return direction_result;
			}
		}

		internal static c_instruction[] parse_input_1(
			in string input,
			in bool pretty)
		{
			c_input_reader input_reader = new c_input_reader(input);

			List<c_instruction> instructions = new List<c_instruction>();

			while (input_reader.has_more_lines())
			{
				instructions.Add(new c_instruction(input_reader.read_line()));
			}

			return instructions.ToArray();
		}

		internal enum e_field_entry_state
		{
			none,
			outside,
			hole,
		}

		[DebuggerDisplay("{origin}", Type = "c_field")]
		internal class c_field
		{
			private c_big_vector origin = new c_big_vector(2, 2);
			private e_field_entry_state[,] entries = new e_field_entry_state[5, 5];

			public c_field()
			{
				entries.fill(e_field_entry_state.none);
			}

			public void dig_hole_at(c_big_vector position)
			{
				c_big_vector index = position.add(origin);

				while (index.x <= 0 || index.x >= entries.GetLength(0) - 1
					|| index.y <= 0 || index.y >= entries.GetLength(1) - 1)
				{
					entries = entries.add_border(1);

					index.x++;
					index.y++;

					origin.x++;
					origin.y++;
				}

				entries[index.x, index.y] = e_field_entry_state.hole;
			}

			public void mark_outside()
			{
				Queue<c_vector> positions_to_consider = new Queue<c_vector>();

				positions_to_consider.Enqueue(new c_vector(0, 0));
				entries[0, 0] = e_field_entry_state.outside;

				while (positions_to_consider.Count > 0)
				{
					c_vector position = positions_to_consider.Dequeue();

					if (position.x > 0 && entries[position.x - 1, position.y] == e_field_entry_state.none)
					{
						entries[position.x - 1, position.y] = e_field_entry_state.outside;
						positions_to_consider.Enqueue(new c_vector(position.x - 1, position.y));
					}

					if (position.x < entries.GetLength(0) - 1 && entries[position.x + 1, position.y] == e_field_entry_state.none)
					{
						entries[position.x + 1, position.y] = e_field_entry_state.outside;
						positions_to_consider.Enqueue(new c_vector(position.x + 1, position.y));
					}

					if (position.y > 0 && entries[position.x, position.y - 1] == e_field_entry_state.none)
					{
						entries[position.x, position.y - 1] = e_field_entry_state.outside;
						positions_to_consider.Enqueue(new c_vector(position.x, position.y - 1));
					}

					if (position.y < entries.GetLength(1) - 1 && entries[position.x, position.y + 1] == e_field_entry_state.none)
					{
						entries[position.x, position.y + 1] = e_field_entry_state.outside;
						positions_to_consider.Enqueue(new c_vector(position.x, position.y + 1));
					}
				}
			}

			public int count_inside()
			{
				return entries.count(entry => entry == e_field_entry_state.none || entry == e_field_entry_state.hole);
			}

			public void display()
			{
				entries.display(entry =>
				{
					switch (entry)
					{
						case e_field_entry_state.none: Console.Write('.'); break;
						case e_field_entry_state.outside: Console.Write(' '); break;
						case e_field_entry_state.hole: Console.Write('#'); break;
					}
				});
			}
		}

		public static void part_1(
			string input,
			bool pretty)
		{
			c_instruction[] instructions = parse_input_1(input, pretty);

			c_field field = new c_field();

			c_big_vector current = c_big_vector.k_vector_zero;
			field.dig_hole_at(current);

			if (pretty)
			{
				field.display();
			}

			foreach(c_instruction instruction in instructions)
			{
				for (int i = 0; i < instruction.length; i++)
				{
					current = current.add(instruction.direction);

					field.dig_hole_at(current);

					if (pretty)
					{
						field.display();
					}
				}
			}

			if (pretty)
			{
				field.display();
			}

			field.mark_outside();

			field.display();

			int sum = field.count_inside();

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine();
			Console.WriteLine("Result = {0}", sum);
			Console.ResetColor();
		}

		internal static c_instruction[] parse_input_2(
			in string input,
			in bool pretty)
		{
			c_input_reader input_reader = new c_input_reader(input);

			List<c_instruction> instructions = new List<c_instruction>();

			while (input_reader.has_more_lines())
			{
				string input_line = input_reader.read_line();

				// int input_length = Convert.ToInt32(input_line.Substring(0, 5), 16);
				int uno = input_line.IndexOf('#');
				string dos = input_line.Substring(uno + 1, 5);

				int input_length = Convert.ToInt32(input_line.Substring(input_line.IndexOf('#') + 1, 5), 16);

				string instruction_char;
				switch (input_line[input_line.Length - 2])
				{
					case '0': instruction_char = "R"; break;
					case '1': instruction_char = "D"; break;
					case '2': instruction_char = "L"; break;
					case '3': instruction_char = "U"; break;
					default: throw new Exception($"Unexpected direction {input_line.Last()}");
				}

				instructions.Add(new c_instruction(input_length, instruction_char));
			}

			return instructions.ToArray();
		}

		public static void part_2(
			string input,
			bool pretty)
		{
			c_instruction[] instructions = parse_input_2(input, pretty);

			List<c_big_vector> points = new List<c_big_vector>();

			c_big_vector current = new c_big_vector();
			points.Add(current);

			foreach (c_instruction instruction in instructions)
			{
				current = current.add(instruction.direction.scale(instruction.length));
				points.Add(current);
			}

			// Pick's theorem: Find the area of a polygon with integer coordinates.
			//
			// A = area of polygon.
			// i = number of integer points interior to the polygon.
			// b = number of integer points on the perimeter.
			//
			// A = i + b/2 - 1.
			//
			// We want to solve for i + b.
			//	A = i + b/2 - 1
			//	A + b/2 = i + b - 1
			//	A + b/2 + 1 = i + b

			Int64 A = math_helpers.shoelace_formula(points.ToArray());

			Int64 b = 0;
			foreach (c_instruction instruction in instructions)
			{
				b += instruction.length;
			}

			Int64 result = A + (b / 2) + 1;

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine();
			Console.WriteLine("Result = {0}", result);
			Console.ResetColor();
		}
	}
}
