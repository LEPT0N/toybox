using System;
using System.Collections.Generic;
using System.Diagnostics;
using advent_of_code_common.display_helpers;
using advent_of_code_common.extensions;
using advent_of_code_common.input_reader;
using advent_of_code_common.int_math;

namespace advent_of_code_2023.days
{
	internal class day_17
	{
		[DebuggerDisplay("{tile} direction = {direction_facing} tier = {tier} distance_from_start = {distance_from_start}", Type = "c_search_node")]
		internal class c_search_node
		{
			public c_tile tile { get; private set; }
			private int cost_to_enter { get; set; }
			public int tier { get; private set; }
			private e_direction direction_facing { get; set; }

			public Dictionary<e_direction, c_search_node> neighbors { get; set; } = new Dictionary<e_direction, c_search_node>();

			public int distance_from_start { get; private set; } = int.MaxValue;
			public e_direction direction_from_start { get; private set; } = e_direction.none;
			public c_search_node search_node_from_start { get; private set; } = null;

			public c_search_node(c_tile owning_tile, int c, e_direction d, int t)
			{
				tile = owning_tile;
				cost_to_enter = c;
				direction_facing = d;
				tier = t;
			}

			public void set_as_start()
			{
				distance_from_start = 0;
			}

			private void consider_direction(c_search_node neighbor, e_direction direction, PriorityQueue<c_search_node, int> queue)
			{
				// Is travelling from neighbor faster than my current best?
				if (distance_from_start > neighbor.distance_from_start + cost_to_enter)
				{
					distance_from_start = neighbor.distance_from_start + cost_to_enter;
					direction_from_start = direction;
					search_node_from_start = neighbor;

					queue.Enqueue(this, distance_from_start);
				}
			}

			public void try_neighbors_reconsider(PriorityQueue<c_search_node, int> queue)
			{
				foreach (e_direction direction in neighbors.Keys)
				{
					neighbors[direction].consider_direction(this, c_int_math.rotate(direction, e_angle.angle_180), queue);
				}
			}
		}

		[DebuggerDisplay("row = {row} col = {col} cost = {cost_to_enter}", Type = "c_tile")]
		internal class c_tile
		{
			public const int k_tier_count = 10;

			// (direction, int) == (direction, tier)
			public Dictionary<(e_direction, int), c_search_node> search_nodes { get; set; } = new Dictionary<(e_direction, int), c_search_node>();

			int cost_to_enter { get; set; }
			int row { get; set; }
			int col { get; set; }

			public int distance_from_start { get; private set; } = int.MaxValue;
			e_direction direction_from_start { get; set; } = e_direction.none;
			c_search_node search_node_from_start { get; set; } = null;
			bool is_on_solution_path { get; set; } = false;

			public c_tile(int cost, int r, int c)
			{
				cost_to_enter = cost;
				row = r;
				col = c;

				for (e_direction direction = e_direction.up; direction < e_direction.count; direction++)
				{
					for (int tier = 0; tier < k_tier_count; tier++)
					{
						search_nodes[(direction, tier)] = new c_search_node(this, cost_to_enter, direction, tier);
					}
				}

			}

			public void set_as_start()
			{
				distance_from_start = 0;

				foreach (c_search_node search_node in search_nodes.Values)
				{
					// if (search_node.tier == 0)
					{
						search_node.set_as_start();
					}
				}
			}

			public void enqueue_all_search_nodes(PriorityQueue<c_search_node, int> queue)
			{
				foreach (c_search_node search_node in search_nodes.Values)
				{
					queue.Enqueue(search_node, search_node.distance_from_start);
				}
			}

			public void display()
			{
				switch (cost_to_enter)
				{
					case 0:
					case 1:
					case 2:
					case 3:
						Console.ForegroundColor = is_on_solution_path ? ConsoleColor.Cyan : ConsoleColor.DarkCyan;
						break;
					case 4:
					case 5:
						Console.ForegroundColor = is_on_solution_path ? ConsoleColor.Green : ConsoleColor.DarkGreen;
						break;
					case 6:
					case 7:
						Console.ForegroundColor = is_on_solution_path ? ConsoleColor.Yellow : ConsoleColor.DarkYellow;
						break;
					case 8:
					case 9:
						Console.ForegroundColor = is_on_solution_path ? ConsoleColor.Red : ConsoleColor.DarkRed;
						break;
				}

				if (is_on_solution_path)
				{
					switch (direction_from_start)
					{
						case e_direction.up:
							Console.Write("v");
							break;

						case e_direction.down:
							Console.Write("^");
							break;

						case e_direction.left:
							Console.Write(">");
							break;

						case e_direction.right:
							Console.Write("<");
							break;

						case e_direction.none:
							Console.ForegroundColor = ConsoleColor.White;
							Console.Write("O");
							break;
					}
				}
				else
				{
					Console.Write($"{cost_to_enter}");
				}

				Console.ResetColor();
			}

			public void draw_path_from_start(int min_straight_line, int max_straight_line)
			{
				int min_distance_from_start = int.MaxValue;
				c_search_node start_node = null;
				search_nodes.Values.for_each(search_node =>
				{
					if (search_node.tier >= min_straight_line - 1 &&
						search_node.tier < max_straight_line &&
						search_node.distance_from_start < min_distance_from_start)
					{
						min_distance_from_start = search_node.distance_from_start;
						start_node = search_node;
					}
				});

				for (c_search_node current = start_node; current != null; current = current.search_node_from_start)
				{
					current.tile.is_on_solution_path = true;

					current.tile.direction_from_start = current.direction_from_start;
					current.tile.distance_from_start = current.distance_from_start;
				}
			}
		}

		internal static c_tile[][] parse_input(
			in string input,
			in bool pretty)
		{
			c_input_reader input_reader = new c_input_reader(input);

			int row_count = 0;
			List<c_tile[]> tiles_list = new List<c_tile[]>();

			while (input_reader.has_more_lines())
			{
				int col_count = 0;
				List<c_tile> tile_row = new List<c_tile>();

				foreach (char tile_char in input_reader.read_line())
				{
					tile_row.Add(new c_tile(tile_char - '0', row_count, col_count));

					col_count++;
				}

				tiles_list.Add(tile_row.ToArray());

				row_count++;
			}

			return tiles_list.ToArray();
		}

		internal static void link_search_nodes(
			c_tile[][] tiles,
			int min_straight_line,
			int max_straight_line)
		{
			for (int row = 0; row < tiles.Length; row++)
			{
				for (int col = 0; col < tiles[row].Length; col++)
				{
					c_tile source_tile = tiles[row][col];

					if (row > 0)
					{
						c_tile target_tile = tiles[row - 1][col];

						for (int i = 0; i < max_straight_line - 1; i++)
						{
							source_tile.search_nodes[(e_direction.up, i)].neighbors[e_direction.up] =
								target_tile.search_nodes[(e_direction.up, i + 1)];
						}

						for (int tier = min_straight_line - 1; tier < max_straight_line; tier++)
						{
							source_tile.search_nodes[(e_direction.right, tier)].neighbors[e_direction.up] =
								target_tile.search_nodes[(e_direction.up, 0)];
							source_tile.search_nodes[(e_direction.left, tier)].neighbors[e_direction.up] =
								target_tile.search_nodes[(e_direction.up, 0)];
						}
					}

					if (row < tiles.Length - 1)
					{
						c_tile target_tile = tiles[row + 1][col];

						for (int i = 0; i < max_straight_line - 1; i++)
						{
							source_tile.search_nodes[(e_direction.down, i)].neighbors[e_direction.down] =
								target_tile.search_nodes[(e_direction.down, i + 1)];
						}

						for (int tier = min_straight_line - 1; tier < max_straight_line; tier++)
						{
							source_tile.search_nodes[(e_direction.left, tier)].neighbors[e_direction.down] =
								target_tile.search_nodes[(e_direction.down, 0)];

							source_tile.search_nodes[(e_direction.right, tier)].neighbors[e_direction.down] =
								target_tile.search_nodes[(e_direction.down, 0)];
						}
					}

					if (col > 0)
					{
						c_tile target_tile = tiles[row][col - 1];

						for (int i = 0; i < max_straight_line - 1; i++)
						{
							source_tile.search_nodes[(e_direction.left, 1)].neighbors[e_direction.left] =
								target_tile.search_nodes[(e_direction.left, i + 1)];
						}

						for (int tier = min_straight_line - 1; tier < max_straight_line; tier++)
						{
							source_tile.search_nodes[(e_direction.up, tier)].neighbors[e_direction.left] =
								target_tile.search_nodes[(e_direction.left, 0)];

							source_tile.search_nodes[(e_direction.down, tier)].neighbors[e_direction.left] =
								target_tile.search_nodes[(e_direction.left, 0)];
						}
					}

					if (col < tiles[row].Length - 1)
					{
						c_tile target_tile = tiles[row][col + 1];

						for (int i = 0; i < max_straight_line - 1; i++)
						{
							source_tile.search_nodes[(e_direction.right, i)].neighbors[e_direction.right] =
								target_tile.search_nodes[(e_direction.right, i + 1)];
						}

						for (int tier = min_straight_line - 1; tier < max_straight_line; tier++)
						{
							source_tile.search_nodes[(e_direction.down, tier)].neighbors[e_direction.right] =
								target_tile.search_nodes[(e_direction.right, 0)];

							source_tile.search_nodes[(e_direction.up, tier)].neighbors[e_direction.right] =
								target_tile.search_nodes[(e_direction.right, 0)];
						}
					}
				}
			}
		}

		public static void part_worker(
			string input,
			bool pretty,
			int min_straight_line,
			int max_straight_line)
		{
			c_tile[][] tiles = parse_input(input, pretty);

			link_search_nodes(tiles, min_straight_line, max_straight_line);

			c_tile start = tiles[0][0];
			c_tile end = tiles[tiles.Length - 1][tiles[0].Length - 1];

			start.set_as_start();

			PriorityQueue<c_search_node, int> queue = new PriorityQueue<c_search_node, int>();
			start.enqueue_all_search_nodes(queue);

			c_search_node element;
			int priority;
			while (queue.TryDequeue(out element, out priority))
			{
				element.try_neighbors_reconsider(queue);
			}

			end.draw_path_from_start(min_straight_line, max_straight_line);

			if (pretty)
			{
				tiles.display(tile => tile.display());
			}

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine();
			Console.WriteLine("Result = {0}", end.distance_from_start);
			Console.ResetColor();
		}

		public static void part_1(
			string input,
			bool pretty)
		{
			part_worker(input, pretty, 1, 3);
		}

		public static void part_2(
			string input,
			bool pretty)
		{
			part_worker(input, pretty, 4, 10);

			// too low: 1086

			// too high: 1147

			// nope: 1149
		}
	}
}
