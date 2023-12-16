using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.display_helpers;
using advent_of_code_common.extensions;
using advent_of_code_common.input_reader;

namespace advent_of_code_2023.days
{
	internal class day_16
	{
		internal enum e_tile_state
		{
			empty,
			mirror_bl_ur,
			mirror_tl_br,
			splitter_vertical,
			splitter_horizontal,
		}

		[Flags] internal enum e_tile_energy
		{
			none = 0x0,
			up = 0x1,
			down = 0x2,
			left = 0x4,
			right = 0x8,
		}

		[DebuggerDisplay("{state} = {energy}", Type = "c_tile")]
		internal class c_tile
		{
			public readonly e_tile_state state;
			public e_tile_energy energy { get; set; }

			public c_tile(char input)
			{
				energy = e_tile_energy.none;

				switch (input)
				{
					case '.': state = e_tile_state.empty; break;
					case '/': state = e_tile_state.mirror_bl_ur; break;
					case '\\': state = e_tile_state.mirror_tl_br; break;
					case '|': state = e_tile_state.splitter_vertical; break;
					case '-': state = e_tile_state.splitter_horizontal; break;
				}
			}

			public void display(bool show_energy = false)
			{
				char display_value;

				if (show_energy)
				{
					if (energy == e_tile_energy.none)
					{
						display_value = ' ';
					}
					else
					{
						display_value = '#';
					}
				}
				else
				{
					switch (state)
					{
						case e_tile_state.empty:
							{
								switch (energy)
								{
									case e_tile_energy.none: display_value = ' '; break;
									case e_tile_energy.up: display_value = '^'; break;
									case e_tile_energy.down: display_value = 'v'; break;
									case e_tile_energy.left: display_value = '<'; break;
									case e_tile_energy.right: display_value = '>'; break;
									default:
										{
											display_value = '!';
											break;
										}
								}
								break;
							}
						case e_tile_state.mirror_bl_ur: display_value = '/'; break;
						case e_tile_state.mirror_tl_br: display_value = '\\'; break;
						case e_tile_state.splitter_vertical: display_value = '|'; break;
						case e_tile_state.splitter_horizontal: display_value = '-'; break;
						default: throw new Exception($"Invalud tile state {state}");
					}
				}

				Console.Write(display_value);
			}
		}

		[DebuggerDisplay("[row {row} col {col}] = {direction}", Type = "c_particle")]
		internal class c_particle
		{
			public int row { get; private set; }
			public int col { get; private set; }
			public e_tile_energy direction { get; set; }

			public c_particle(int r, int c, e_tile_energy d)
			{
				row = r;
				col = c;
				direction = d;
			}

			public c_particle(c_particle p)
			{
				row = p.row;
				col = p.col;
				direction = p.direction;
			}

			public void tick()
			{
				switch (direction)
				{
					case e_tile_energy.up: row--; break;
					case e_tile_energy.down: row++; break;
					case e_tile_energy.left: col--; break;
					case e_tile_energy.right: col++; break;
				}
			}
		}

		[DebuggerDisplay("", Type = "c_contraption")]
		internal class c_contraption
		{
			c_tile[][] tile_grid;

			LinkedList<c_particle> particles;

			public c_contraption (c_input_reader input_reader)
			{
				List<c_tile[]> tile_grid_list = new List<c_tile[]>();

				while (input_reader.has_more_lines())
				{
					List<c_tile> tile_line = new List<c_tile>();

					foreach (char tile_input in input_reader.read_line())
					{
						tile_line.Add(new c_tile(tile_input));
					}

					tile_grid_list.Add(tile_line.ToArray());
				}

				tile_grid = tile_grid_list.ToArray();

				particles = new LinkedList<c_particle>();
			}

			private int compute_energy()
			{
				return tile_grid.Sum(tile_row => tile_row.Sum(tile => tile.energy != e_tile_energy.none ? 1 : 0));
			}

			public void add_particle(c_particle particle)
			{
				particles.AddLast(particle);
			}

			void remove_node(ref LinkedListNode<c_particle> node)
			{
				LinkedListNode<c_particle> deleted_node = node;

				node = node.Next;

				particles.Remove(deleted_node);
			}

			bool add_direction_energy(c_particle particle)
			{
				c_tile tile = tile_grid[particle.row][particle.col];

				if (tile.energy.HasFlag(particle.direction))
				{
					return true;
				}
				else
				{
					tile.energy |= particle.direction;

					return false;
				}
			}

			private bool tick()
			{
				for (LinkedListNode<c_particle> node = particles.First; node != null; node = node.Next)
				{
					c_particle particle = node.Value;

					particle.tick();

					if (particle.row < 0 || particle.row >= tile_grid.Length ||
						particle.col < 0 || particle.col >= tile_grid[0].Length)
					{
						// If a particle fell off of the grid, then remove it.
						remove_node(ref node);
					}
					else
					{
						c_tile tile = tile_grid[particle.row][particle.col];

						if (tile.state == e_tile_state.mirror_bl_ur || tile.state == e_tile_state.mirror_tl_br)
						{
							// If a particle meets a mirror, bounce it.

							switch (tile.state, particle.direction)
							{
								case (e_tile_state.mirror_bl_ur, e_tile_energy.up):
								case (e_tile_state.mirror_tl_br, e_tile_energy.down):
									particle.direction = e_tile_energy.right;
									break;

								case (e_tile_state.mirror_bl_ur, e_tile_energy.down):
								case (e_tile_state.mirror_tl_br, e_tile_energy.up):
									particle.direction = e_tile_energy.left;
									break;

								case (e_tile_state.mirror_bl_ur, e_tile_energy.right):
								case (e_tile_state.mirror_tl_br, e_tile_energy.left):
									particle.direction = e_tile_energy.up;
									break;

								case (e_tile_state.mirror_bl_ur, e_tile_energy.left):
								case (e_tile_state.mirror_tl_br, e_tile_energy.right):
									particle.direction = e_tile_energy.down;
									break;

								default:
									throw new Exception($"{tile.state} / {particle.direction} isn't valid");
							}

							if (add_direction_energy(particle))
							{
								// If the bounce lines the particle up with an existing path, no need to continue.
								// (This never happens)
								// remove_node(ref node);
								Debug.Assert(false);
							}
						}
						else if (tile.state == e_tile_state.splitter_vertical || tile.state == e_tile_state.splitter_horizontal)
						{
							// If the particle meets a splitter, potentially split it.

							c_particle new_particle = new c_particle(particle);

							switch (tile.state, particle.direction)
							{
								case (e_tile_state.splitter_vertical, e_tile_energy.left):
								case (e_tile_state.splitter_vertical, e_tile_energy.right):
									particle.direction = e_tile_energy.up;
									new_particle.direction = e_tile_energy.down;
									break;

								case (e_tile_state.splitter_horizontal, e_tile_energy.up):
								case (e_tile_state.splitter_horizontal, e_tile_energy.down):
									particle.direction = e_tile_energy.left;
									new_particle.direction = e_tile_energy.right;
									break;

								default:
									new_particle = null;
									break;
							}

							if (new_particle != null)
							{
								if (!add_direction_energy(new_particle))
								{
									particles.AddBefore(node, new_particle);
								}
								// else
								// {
								// If the split lines the new particle up with an existing path, no need to continue.
								// }
							}

							if (add_direction_energy(particle))
							{
								// If the split lines the particle up with an existing path, no need to continue.
								remove_node(ref node);
							}
						}
						else // tile.state == e_tile_state.empty
						{
							if (add_direction_energy(particle))
							{
								// If the bounce lines the particle up with an existing path, no need to continue.
								// (This never happens)
								// remove_node(ref node);
								Debug.Assert(false);
							}
						}
					}

					if (node == null)
					{
						break;
					}
				}

				return particles.First != null;
			}

			public int tick_until_completion(bool pretty)
			{
				while (tick())
				{
					if (pretty)
					{
						display();
					}
				}

				return compute_energy();
			}

			private void clear_all_energy()
			{
				tile_grid.for_each(tile_row => tile_row.for_each(tile => tile.energy = e_tile_energy.none));
			}

			public int find_max_energy(bool pretty)
			{
				int max_energy = 0;

				for (int row = 0; row < tile_grid.Length; row++)
				{
					{
						add_particle(new c_particle(row, -1, e_tile_energy.right));

						int energy = tick_until_completion(pretty);

						max_energy = Math.Max(energy, max_energy);
					}

					clear_all_energy();

					{
						add_particle(new c_particle(row, tile_grid[0].Length, e_tile_energy.left));

						int energy = tick_until_completion(pretty);

						max_energy = Math.Max(energy, max_energy);
					}

					clear_all_energy();
				}

				for (int col = 0; col < tile_grid[0].Length; col++)
				{
					{
						add_particle(new c_particle(-1, col, e_tile_energy.down));

						int energy = tick_until_completion(pretty);

						max_energy = Math.Max(energy, max_energy);
					}

					clear_all_energy();

					{
						add_particle(new c_particle(tile_grid.Length, col, e_tile_energy.up));

						int energy = tick_until_completion(pretty);

						max_energy = Math.Max(energy, max_energy);
					}

					clear_all_energy();
				}

				return max_energy;
			}

			public void display(bool show_energy = false)
			{
				tile_grid.display(tile => tile.display(show_energy));
			}
		}

		internal static c_contraption parse_input(
			in string input,
			in bool pretty)
		{
			c_input_reader input_reader = new c_input_reader(input);

			c_contraption contraption = new c_contraption(input_reader);

			if (pretty)
			{
				contraption.display();
			}

			return contraption;
		}

		public static void part_1(
			string input,
			bool pretty)
		{
			c_contraption contraption = parse_input(input, pretty);

			contraption.add_particle(new c_particle(0, -1, e_tile_energy.right));

			int energy = contraption.tick_until_completion(pretty);

			contraption.display();
			contraption.display(true);

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine();
			Console.WriteLine("Result = {0}", energy);
			Console.ResetColor();
		}

		public static void part_2(
			string input,
			bool pretty)
		{
			c_contraption contraption = parse_input(input, pretty);

			int max_energy = contraption.find_max_energy(pretty);

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine();
			Console.WriteLine("Result = {0}", max_energy);
			Console.ResetColor();
		}
	}
}
