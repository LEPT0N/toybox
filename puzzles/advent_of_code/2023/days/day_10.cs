using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.display_helpers;
using advent_of_code_common.input_reader;

namespace advent_of_code_2023.days
{
	internal class day_10
	{
		[Flags] internal enum e_direction_flags
		{
			none = 0x0,
			up = 0x1,
			down = 0x2,
			left = 0x4,
			right = 0x8,
		}

		[DebuggerDisplay("{display_char}", Type = "c_tile")]
		internal class c_tile
		{
			readonly char display_char;
			readonly e_direction_flags neighbor_flags;

			public int distance_to_start;
			public bool is_biggest_loser;
			public List<c_tile> neighbors;

			public c_tile(
				char c,
				ref c_tile start_tile)
			{
				neighbor_flags = e_direction_flags.none;

				distance_to_start = int.MaxValue;
				is_biggest_loser = false;
				neighbors = new List<c_tile>();

				switch (c)
				{
					case '|':
						display_char = special_characters.k_box_icon_up_and_down;
						neighbor_flags |= e_direction_flags.up;
						neighbor_flags |= e_direction_flags.down;
						break;

					case '-':
						display_char = special_characters.k_box_icon_left_and_right;
						neighbor_flags |= e_direction_flags.left;
						neighbor_flags |= e_direction_flags.right;
						break;

					case 'L':
						display_char = special_characters.k_box_icon_up_and_right;
						neighbor_flags |= e_direction_flags.up;
						neighbor_flags |= e_direction_flags.right;
						break;

					case 'J':
						display_char = special_characters.k_box_icon_up_and_left;
						neighbor_flags |= e_direction_flags.up;
						neighbor_flags |= e_direction_flags.left;
						break;

					case '7':
						display_char = special_characters.k_box_icon_down_and_left;
						neighbor_flags |= e_direction_flags.down;
						neighbor_flags |= e_direction_flags.left;
						break;

					case 'F':
						display_char = special_characters.k_box_icon_down_and_right;
						neighbor_flags |= e_direction_flags.down;
						neighbor_flags |= e_direction_flags.right;
						break;

					case '.':
						display_char = special_characters.k_box_icon_none;
						break;

					case 'S':
						start_tile = this;
						display_char = 'S';
						neighbor_flags |= e_direction_flags.up;
						neighbor_flags |= e_direction_flags.down;
						neighbor_flags |= e_direction_flags.left;
						neighbor_flags |= e_direction_flags.right;
						distance_to_start = 0;
						break;
				}
			}

			public void set_neighbors(
				c_tile neighbor_down,
				c_tile neighbor_right)
			{
				if (this.neighbor_flags.HasFlag(e_direction_flags.down) &&
					neighbor_down.neighbor_flags.HasFlag(e_direction_flags.up))
				{
					this.neighbors.Add(neighbor_down);
					neighbor_down.neighbors.Add(this);
				}

				if (this.neighbor_flags.HasFlag(e_direction_flags.right) &&
					neighbor_right.neighbor_flags.HasFlag(e_direction_flags.left))
				{
					this.neighbors.Add(neighbor_right);
					neighbor_right.neighbors.Add(this);
				}
			}

			public void display()
			{
				if (is_biggest_loser)
				{
					Console.ForegroundColor = ConsoleColor.White;
					Console.Write('E');
				}
				else if (distance_to_start == 0)
				{
					Console.ForegroundColor = ConsoleColor.White;
					Console.Write(display_char);
				}
				else if (distance_to_start == int.MaxValue)
				{
					Console.ForegroundColor = ConsoleColor.DarkGray;
					Console.Write(display_char);
				}
				else
				{
					switch (distance_to_start % 8)
					{
						case 1: Console.ForegroundColor = ConsoleColor.Red; break;
						case 2: Console.ForegroundColor = ConsoleColor.Yellow; break;
						case 3: Console.ForegroundColor = ConsoleColor.DarkYellow; break;
						case 4: Console.ForegroundColor = ConsoleColor.Green; break;
						case 5: Console.ForegroundColor = ConsoleColor.DarkGreen; break;
						case 6: Console.ForegroundColor = ConsoleColor.Cyan; break;
						case 7: Console.ForegroundColor = ConsoleColor.Blue; break;
						case 0: Console.ForegroundColor = ConsoleColor.DarkBlue; break;
					}

					Console.Write(display_char);
				}
			}
		}

		[DebuggerDisplay("todo", Type = "c_tile_grid")]
		internal class c_tile_grid
		{
			c_tile[][] tiles;

			c_tile start_tile;

			public c_tile_grid(
				c_input_reader input_reader)
			{
				List<c_tile[]> tiles_list = new List<c_tile[]>();

				while (input_reader.has_more_lines())
				{
					tiles_list.Add(input_reader.read_line().Select(c => new c_tile(c, ref start_tile)).ToArray());
				}

				tiles = tiles_list.ToArray();

				for (int row = 0; row < tiles.Length - 1; row++)
				{
					for (int col = 0; col < tiles[row].Length - 1; col++)
					{
						c_tile current = tiles[row][col];
						c_tile neighbor_down = tiles[row + 1][col];
						c_tile neighbor_right = tiles[row][col + 1];

						current.set_neighbors(neighbor_down, neighbor_right);
					}
				}
			}

			public int find_loop()
			{
				c_tile biggest_loser = start_tile;

				PriorityQueue<c_tile, int> work_queue = new PriorityQueue<c_tile, int>();
				work_queue.Enqueue(start_tile, start_tile.distance_to_start);

				while (work_queue.Count > 0)
				{
					c_tile current = work_queue.Dequeue();
					int new_distance = current.distance_to_start + 1;

					foreach (c_tile neighbor in current.neighbors)
					{
						if (neighbor.distance_to_start > new_distance)
						{
							biggest_loser = neighbor;
							neighbor.distance_to_start = new_distance;
							work_queue.Enqueue(neighbor, new_distance);
						}
					}
				}

				biggest_loser.is_biggest_loser = true;

				return biggest_loser.distance_to_start;
			}

			public void display()
			{
				tiles.display(c => c.display());
			}
		}

		internal static c_tile_grid parse_input(
			in string input,
			in bool pretty)
		{
			c_input_reader input_reader = new c_input_reader(input);

			c_tile_grid tile_grid = new c_tile_grid(input_reader);

			return tile_grid;
		}

		public static void part_1(
			string input,
			bool pretty)
		{
			c_tile_grid tile_grid = parse_input(input, pretty);

			int longest_distance = tile_grid.find_loop();

			if (pretty)
			{
				tile_grid.display();
			}

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine();
			Console.WriteLine("Result = {0}", longest_distance);
			Console.ResetColor();
		}

		public static void part_2(
			string input,
			bool pretty)
		{
			// parse_input(input, pretty);

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine();
			Console.WriteLine("Result = {0}", 0);
			Console.ResetColor();
		}
	}
}
