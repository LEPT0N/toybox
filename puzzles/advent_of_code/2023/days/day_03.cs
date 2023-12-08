using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.input_reader;

namespace advent_of_code_2023.days
{
	internal class day_03
	{
		internal enum e_position_type
		{
			empty,
			symbol,
			gear,
			number,
		}

		[DebuggerDisplay("{value} = {adjacent_to_symbol}", Type = "c_number")]
		internal class c_number
		{
			public int value;
			public bool adjacent_to_symbol;
		}

		[DebuggerDisplay("{type} = [{number}]", Type = "c_position")]
		internal class c_position
		{
			public e_position_type type;
			public c_number number;
		}

		internal static (c_position[][], c_number[]) parse_input(
			in string input,
			in bool pretty)
		{
			c_input_reader input_reader = new c_input_reader(input);

			List<c_position[]> grid = new List<c_position[]>();
			List<c_number> numbers = new List<c_number>();

			while (input_reader.has_more_lines())
			{
				List<c_position> line = new List<c_position>();

				foreach (char input_char in input_reader.read_line())
				{
					c_position new_position = new c_position();

					if (input_char >= '0' && input_char <= '9')
					{
						int input_value = input_char - '0';

						if (line.Any() && line.Last().type == e_position_type.number)
						{
							new_position.number = line.Last().number;

							new_position.number.value *= 10;
							new_position.number.value += input_value;
						}
						else
						{
							c_number new_number = new c_number { value = input_value };

							new_position.number = new_number;
							numbers.Add(new_number);
						}

						new_position.type = e_position_type.number;
					}
					else if (input_char == '*')
					{
						new_position.type = e_position_type.gear;
					}
					else if (input_char == '.')
					{
						new_position.type = e_position_type.empty;
					}
					else
					{
						new_position.type = e_position_type.symbol;
					}

					line.Add(new_position);
				}

				grid.Add(line.ToArray());
			}

			return (grid.ToArray(), numbers.ToArray());
		}

		public static void part_1(
			string input,
			bool pretty)
		{
			(c_position[][] grid, c_number[] numbers ) = parse_input(input, pretty);

			// Mark which numbers are adjacent to a symbol.

			for (int y = 0; y < grid.Length; y++)
			{
				for (int x = 0; x < grid[y].Length; x++)
				{
					if (grid[y][x].type == e_position_type.symbol)
					{
						for (int y_offset = -1; y_offset <= 1; y_offset++)
						{
							int y_neighbor = y + y_offset;

							if (y_neighbor >= 0 && y_neighbor < grid.Length)
							{
								for (int x_offset = -1; x_offset <= 1; x_offset++)
								{
									int x_neighbor = x + x_offset;

									if (x >= 0 && x < grid[y_neighbor].Length)
									{
										if (grid[y_neighbor][x_neighbor].type == e_position_type.number)
										{
											grid[y_neighbor][x_neighbor].number.adjacent_to_symbol = true;
										}
									}
								}
							}
						}
					}
				}
			}

			int result = numbers.Where(number => number.adjacent_to_symbol).Sum(number => number.value);

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine();
			Console.WriteLine("Result = {0}", result);
			Console.ResetColor();
		}

		public static void part_2(
			string input,
			bool pretty)
		{
			(c_position[][] grid, c_number[] numbers) = parse_input(input, pretty);

			// Mark which numbers are adjacent to a symbol.

			int result = 0;

			for (int y = 0; y < grid.Length; y++)
			{
				for (int x = 0; x < grid[y].Length; x++)
				{
					if (grid[y][x].type == e_position_type.gear)
					{
						HashSet<c_number> adjacent_numbers = new HashSet<c_number>();

						for (int y_offset = -1; y_offset <= 1; y_offset++)
						{
							int y_neighbor = y + y_offset;

							if (y_neighbor >= 0 && y_neighbor < grid.Length)
							{
								for (int x_offset = -1; x_offset <= 1; x_offset++)
								{
									int x_neighbor = x + x_offset;

									if (x >= 0 && x < grid[y_neighbor].Length)
									{
										if (grid[y_neighbor][x_neighbor].type == e_position_type.number)
										{
											adjacent_numbers.Add(grid[y_neighbor][x_neighbor].number);

											grid[y_neighbor][x_neighbor].number.adjacent_to_symbol = true;
										}
									}
								}
							}
						}

						if (adjacent_numbers.Count == 2)
						{
							int gear_ratio = adjacent_numbers.Select(number => number.value).Aggregate((x, y) => x * y);

							result += gear_ratio;
						}
					}
				}
			}

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine();
			Console.WriteLine("Result = {0}", result);
			Console.ResetColor();
		}
	}
}
