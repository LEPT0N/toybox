using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.display_helpers;
using advent_of_code_common.input_reader;
using advent_of_code_common.int_math;

namespace advent_of_code_2023.days
{
	internal class day_17
	{
		// might have to solve this with more tiles all with the same row/col.
		// tier 0 = had just previously turned. need 4 search nodes in this tile for which direction you're now facing.
			// Each teir 0 node connects to left/right tier 0 nodes and one tier 1 node straight ahead.
		// tier 1 = had just moved straight from previous. There are also four of these.
			// Each tier 1 node connects to left/right tier 0 nodes and one tier 2 node straight ahead.
		// tier 2 = had just moved straight from previous. There are also four of these.
			// Each tier 2 node only connects to left/right tier 0 nodes.

		// c_tile is the spot on the graph, and has 12 c_search_nodes in it.

		//internal class c_search_node
		//{
		//	int cost_to_enter;
		//	e_direction direction_facing;

		//	c_search_node next_node_left = null;
		//	c_search_node next_node_right = null;
		//	c_search_node next_node_straight = null;

		//	e_direction direction_to_end { get; set; } = e_direction.none;
		//	c_search_node searc_node_to_end { get; set; } = null;
		//}

		[DebuggerDisplay("cost = {cost_to_enter} distance = {distance_to_end}", Type = "c_tile")]
		internal class c_tile
		{
			//c_search_node[] search_nodes_facing_up = new c_search_node[3];
			//c_search_node[] search_nodes_facing_down = new c_search_node[3];
			//c_search_node[] search_nodes_facing_left = new c_search_node[3];
			//c_search_node[] search_nodes_facing_right = new c_search_node[3];

			int cost_to_enter { get; set; }

			//public c_tile neighbor_up { get; set; } = null;
			//public c_tile neighbor_down { get; set; } = null;
			//public c_tile neighbor_left { get; set; } = null;
			//public c_tile neighbor_right { get; set; } = null;

			public Dictionary<e_direction, c_tile> neighbors = new Dictionary<e_direction, c_tile>();

			public int distance_to_end { get; private set; } = int.MaxValue;
			e_direction direction_to_end { get; set; } = e_direction.none;
			c_tile neighbor_to_end { get; set; } = null;
			bool is_on_solution_path { get; set; } = false;

			public c_tile(int c)
			{
				cost_to_enter = c;
			}

			public void set_as_end()
			{
				distance_to_end = 0;
			}

			public void display(bool display_arrows = false)
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

				if (is_on_solution_path || display_arrows)
				{
					switch (direction_to_end)
					{
						case e_direction.up:
							Console.Write("^");
							break;

						case e_direction.down:
							Console.Write("v");
							break;

						case e_direction.left:
							Console.Write("<");
							break;

						case e_direction.right:
							Console.Write(">");
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

			private void consider_direction(c_tile neighbor, e_direction direction)
			{
				// Is travelling through neighbor faster than my current best?
				if (distance_to_end > neighbor.distance_to_end + neighbor.cost_to_enter)
				{
					distance_to_end = neighbor.distance_to_end + neighbor.cost_to_enter;
					direction_to_end = direction;
					neighbor_to_end = neighbor;

					compute_neighbors_distance_to_me();
				}
			}

			public void compute_neighbors_distance_to_me()
			{
				c_tile neighbor;

				if (neighbors.TryGetValue(e_direction.up, out neighbor)) { neighbor.consider_direction(this, e_direction.down); }
				if (neighbors.TryGetValue(e_direction.down, out neighbor)) { neighbor.consider_direction(this, e_direction.up); }
				if (neighbors.TryGetValue(e_direction.left, out neighbor)) { neighbor.consider_direction(this, e_direction.right); }
				if (neighbors.TryGetValue(e_direction.right, out neighbor)) { neighbor.consider_direction(this, e_direction.left); }
			}

			private void derp_consider_direction(c_tile neighbor, e_direction direction, PriorityQueue<c_tile, int> queue)
			{
				// Is travelling through neighbor faster than my current best?
				if (distance_to_end > neighbor.distance_to_end + neighbor.cost_to_enter)
				{
					distance_to_end = neighbor.distance_to_end + neighbor.cost_to_enter;
					direction_to_end = direction;
					neighbor_to_end = neighbor;

					// derp();
					queue.Enqueue(this, distance_to_end);
				}
			}

			public void derp(PriorityQueue<c_tile, int> queue)
			{
				int distance = 3;
				//for (int distance = 1; distance <= 3; distance++)
				{
					if (direction_to_end == e_direction.none || direction_to_end == e_direction.up || direction_to_end == e_direction.down)
					{
						c_tile neighbor = this;
						for (int neighbors_count = 0; neighbors_count < distance && neighbor != null; neighbors_count++)
						{
							c_tile previous_neighbor = neighbor;
							if (neighbor.neighbors.TryGetValue(e_direction.left, out neighbor))
							{
								neighbor.derp_consider_direction(previous_neighbor, e_direction.right, queue);
							}
						}

						neighbor = this;
						for (int neighbors_count = 0; neighbors_count < distance && neighbor != null; neighbors_count++)
						{
							c_tile previous_neighbor = neighbor;
							if (neighbor.neighbors.TryGetValue(e_direction.right, out neighbor))
							{
								neighbor.derp_consider_direction(previous_neighbor, e_direction.left, queue);
							}
						}
					}

					if (direction_to_end == e_direction.none || direction_to_end == e_direction.left || direction_to_end == e_direction.right)
					{
						c_tile neighbor = this;
						for (int neighbors_count = 0; neighbors_count < distance && neighbor != null; neighbors_count++)
						{
							c_tile previous_neighbor = neighbor;
							if (neighbor.neighbors.TryGetValue(e_direction.up, out neighbor))
							{
								neighbor.derp_consider_direction(previous_neighbor, e_direction.down, queue);
							}
						}

						neighbor = this;
						for (int neighbors_count = 0; neighbors_count < distance && neighbor != null; neighbors_count++)
						{
							c_tile previous_neighbor = neighbor;
							if (neighbor.neighbors.TryGetValue(e_direction.down, out neighbor))
							{
								neighbor.derp_consider_direction(previous_neighbor, e_direction.up, queue);
							}
						}
					}
				}
			}

			public void draw_path_to_end()
			{
				for (c_tile current = this; current != null; current = current.neighbor_to_end)
				{
					current.is_on_solution_path = true;
				}
			}
		}

		internal static c_tile[][] parse_input(
			in string input,
			in bool pretty)
		{
			c_input_reader input_reader = new c_input_reader(input);

			List<c_tile[]> tiles_list = new List<c_tile[]>();

			while (input_reader.has_more_lines())
			{
				List<c_tile> tile_row = new List<c_tile>();

				foreach (char tile_char in input_reader.read_line())
				{
					tile_row.Add(new c_tile(tile_char - '0'));
				}

				tiles_list.Add(tile_row.ToArray());
			}

			c_tile[][] tiles = tiles_list.ToArray();

			// Link up neighbors
			for (int row = 0; row < tiles.Length; row++)
			{
				for (int col = 0; col < tiles[row].Length; col++)
				{
					if (row > 0)
					{
						tiles[row][col].neighbors[e_direction.up] = tiles[row - 1][col];
					}

					if (row < tiles.Length - 1)
					{
						tiles[row][col].neighbors[e_direction.down] = tiles[row + 1][col];
					}

					if (col > 0)
					{
						tiles[row][col].neighbors[e_direction.left] = tiles[row][col - 1];
					}

					if (col < tiles[row].Length - 1)
					{
						tiles[row][col].neighbors[e_direction.right] = tiles[row][col + 1];
					}
				}
			}

			return tiles;
		}

		public static void part_1(
			string input,
			bool pretty)
		{
			c_tile[][] tiles = parse_input(input, pretty);

			c_tile end = tiles[0][0];
			c_tile start = tiles[tiles.Length - 1][tiles[0].Length - 1];

			end.set_as_end();
			//end.compute_neighbors_distance_to_me();
			//end.derp();

			PriorityQueue<c_tile, int> queue = new PriorityQueue<c_tile, int>();
			queue.Enqueue(end, end.distance_to_end);

			c_tile element;
			int priority;
			while (queue.TryDequeue(out element, out priority))
			{
				element.derp(queue);

				// tiles.display(tile => tile.display(true));
			}

			start.draw_path_to_end();

			tiles.display(tile => tile.display(true));

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine();
			Console.WriteLine("Result = {0}", start.distance_to_end);
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
