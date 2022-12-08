using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using advent_of_code_common.input_reader;

namespace advent_of_code_2022.days
{
    internal class day_08
    {
        [DebuggerDisplay("({x},{y})", Type = "s_coordinate")]
        internal struct s_coordinate
        {
            public int x;
            public int y;

            public s_coordinate add(s_coordinate other)
            {
                return new s_coordinate { x = this.x + other.x, y = this.y + other.y };
            }
        }

        internal static s_coordinate[] k_neighbor_offsets = new s_coordinate[]
        {
            new s_coordinate { x = -1, y = 0 },
            new s_coordinate { x = 1, y = 0 },
            new s_coordinate { x = 0, y = -1 },
            new s_coordinate { x = 0, y = 1 },
        };

        [DebuggerDisplay("h = {height} v = {visible} p = {position} s = {scenic_score}", Type = "c_tree")]
        internal class c_tree
        {
            public readonly int height;
            public bool visible;
            public s_coordinate position;
            public int scenic_score;

            public c_tree(int h, int x, int y)
            {
                height = h;
                visible = false;
                position = new s_coordinate { x = x, y = y };
                scenic_score = 0;
            }

            public void print_visibility()
            {
                Console.ForegroundColor = visible ? ConsoleColor.Green : ConsoleColor.DarkGray;
                Console.Write($"{height}");
                Console.ResetColor();
            }

            public void print_scenic_score()
            {
                Console.Write("{0:X}", Math.Min(0xF, scenic_score));
            }
        }

        internal static c_tree[][] parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            List<c_tree[]> tree_grid = new List<c_tree[]>();

            int y = 0;

            while (input_reader.has_more_lines())
            {
                List<c_tree> tree_row = new List<c_tree>();

                string input_line = input_reader.read_line();

                for (int x = 0; x < input_line.Length; x++)
                {
                    tree_row.Add(new c_tree(input_line[x] - '0', x, y));
                }

                tree_grid.Add(tree_row.ToArray());

                y++;
            }

            return tree_grid.ToArray();
        }

        internal static void print_visibility(c_tree[][] tree_grid, bool pretty)
        {
            if (!pretty)
            {
                return;
            }

            foreach (c_tree[] tree_row in tree_grid)
            {
                foreach (c_tree tree in tree_row)
                {
                    tree.print_visibility();
                }

                Console.WriteLine();
            }

            Console.WriteLine();
        }

        public static bool is_tree_visible(
            c_tree[][] tree_grid,
            s_coordinate grid_size,
            s_coordinate start,
            s_coordinate offset)
        {
            c_tree start_tree = tree_grid[start.y][start.x];
            s_coordinate current = start.add(offset);

            while (current.x >= 0 && current.x < grid_size.x
                && current.y >= 0 && current.y < grid_size.y)
            {
                if (start_tree.height <= tree_grid[current.y][current.x].height)
                {
                    return false;
                }

                current = current.add(offset);
            }

            return true;
        }

        public static void part_1(
            string input,
            bool pretty)
        {
            // Parse input

            c_tree[][] tree_grid = parse_input(input, pretty);
            s_coordinate grid_size = new s_coordinate { x = tree_grid[0].Length, y = tree_grid.Length };

            // Calculate

            s_coordinate current = new s_coordinate();
            for (current.x = 0; current.x < grid_size.x; current.x++)
            {
                for (current.y = 0; current.y < grid_size.y; current.y++)
                {
                    if (k_neighbor_offsets.Any(offset => is_tree_visible(tree_grid, grid_size, current, offset)))
                    {
                        tree_grid[current.y][current.x].visible = true;
                    }
                }
            }

            // Output

            print_visibility(tree_grid, pretty);

            int result = tree_grid.Select(tree_row => tree_row.Count(tree => tree.visible)).Sum();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", result);
            Console.ResetColor();
        }

        internal static void print_scenic_score(c_tree[][] tree_grid, bool pretty)
        {
            if (!pretty)
            {
                return;
            }

            foreach (c_tree[] tree_row in tree_grid)
            {
                foreach (c_tree tree in tree_row)
                {
                    tree.print_scenic_score();
                }

                Console.WriteLine();
            }

            Console.WriteLine();
        }

        public static int get_scenic_score_in_direction(
            c_tree[][] tree_grid,
            s_coordinate grid_size,
            c_tree start_tree,
            s_coordinate offset)
        {
            int score = 0;

            s_coordinate current = start_tree.position.add(offset);

            while (current.x >= 0 && current.x < grid_size.x
                && current.y >= 0 && current.y < grid_size.y)
            {
                score++;

                if (start_tree.height <= tree_grid[current.y][current.x].height)
                {
                    break;
                }


                current = current.add(offset);
            }

            return score;
        }

        public static void part_2(
            string input,
            bool pretty)
        {
            // Parse input

            c_tree[][] tree_grid = parse_input(input, pretty);
            s_coordinate grid_size = new s_coordinate { x = tree_grid[0].Length, y = tree_grid.Length };

            // Calculate

            s_coordinate current = new s_coordinate();
            for (current.x = 0; current.x < grid_size.x; current.x++)
            {
                for (current.y = 0; current.y < grid_size.y; current.y++)
                {
                    c_tree tree = tree_grid[current.y][current.x];

                    tree.scenic_score = k_neighbor_offsets
                        .Select(offset => get_scenic_score_in_direction(tree_grid, grid_size, tree, offset))
                        .Aggregate(1, (a, b) => a * b);
                }
            }

            // Output

            print_scenic_score(tree_grid, pretty);

            int result = tree_grid.Max(tree_row => tree_row.Max(tree => tree.scenic_score));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", result);
            Console.ResetColor();
        }
    }
}
