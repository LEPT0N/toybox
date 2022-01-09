using advent_of_code_common.display_helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace advent_of_code_2021.Days
{
    internal class Day_15
    {
        internal enum e_map_direction
        {
            unknown,
            up,
            down,
            left,
            right,
            end,
        }

        [DebuggerDisplay("[{row}][{column}] = {m_risk_level}. Path = {m_direction_to_end} ({m_total_risk_to_end})", Type = "c_map_node")]
        internal class c_map_node
        {
            public bool is_on_best_path;
            public readonly int row;
            public readonly int column;
            public readonly int risk_level;

            public int total_risk_to_end { get; private set; }
            public e_map_direction direction_to_end { get; private set; }

            public c_map_node(int risk, int r, int c)
            {
                is_on_best_path = false;
                risk_level = risk;
                row = r;
                column = c;
                direction_to_end = e_map_direction.unknown;
            }

            public void mark_end()
            {
                total_risk_to_end = 0;
                direction_to_end = e_map_direction.end;
                is_on_best_path = true;
            }

            public bool consider_neighbor(e_map_direction direction, c_map_node other)
            {
                if (direction_to_end != e_map_direction.end)
                {
                    int total_risk_to_end_through_other = other.total_risk_to_end + other.risk_level;

                    if (direction_to_end == e_map_direction.unknown ||
                        total_risk_to_end_through_other < total_risk_to_end)
                    {
                        total_risk_to_end = total_risk_to_end_through_other;
                        direction_to_end = direction;

                        return true;
                    }
                }

                return false;
            }
        }

        internal static c_map_node[][] parse_input(
            string input)
        {
            c_input_reader input_reader = new c_input_reader(input);

            List<c_map_node[]> map = new List<c_map_node[]>();

            for (int row = 0; input_reader.has_more_lines(); row++)
            {
                string input_line = input_reader.read_line();

                List<c_map_node> map_line = new List<c_map_node>();

                for (int column = 0; column < input_line.Length; column++)
                {
                    map_line.Add(new c_map_node(input_line[column] - '0', row, column));
                }

                map.Add(map_line.ToArray());
            }

            return map.ToArray();
        }

        public static void draw_map_risk_levels(c_map_node[][] map)
        {
            foreach (c_map_node[] row in map)
            {
                foreach (c_map_node node in row)
                {
                    switch (node.risk_level)
                    {
                        case 1: Console.ForegroundColor = ConsoleColor.DarkBlue; break;
                        case 2: Console.ForegroundColor = ConsoleColor.Blue; break;
                        case 3: Console.ForegroundColor = ConsoleColor.Cyan; break;
                        case 4: Console.ForegroundColor = ConsoleColor.DarkGreen; break;
                        case 5: Console.ForegroundColor = ConsoleColor.Green; break;
                        case 6: Console.ForegroundColor = ConsoleColor.DarkYellow; break;
                        case 7: Console.ForegroundColor = ConsoleColor.Yellow; break;
                        case 8: Console.ForegroundColor = ConsoleColor.Red; break;
                        case 9: Console.ForegroundColor = ConsoleColor.DarkRed; break;
                    }
                    Console.Write(node.risk_level.ToString().PadLeft(4));
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.ResetColor();
        }

        public static void draw_map_path_as_total(c_map_node[][] map)
        {
            foreach (c_map_node[] row in map)
            {
                foreach (c_map_node node in row)
                {
                    if (node.is_on_best_path) { Console.ForegroundColor = ConsoleColor.White; }
                    else { Console.ForegroundColor = ConsoleColor.DarkGray; }

                    Console.Write(node.total_risk_to_end.ToString().PadLeft(4));
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.ResetColor();
        }

        public static void draw_map_path_as_individual(c_map_node[][] map)
        {
            foreach (c_map_node[] row in map)
            {
                foreach (c_map_node node in row)
                {
                    if (node.is_on_best_path) { Console.ForegroundColor = ConsoleColor.White; }
                    else { Console.ForegroundColor = ConsoleColor.DarkGray; }

                    Console.Write(node.risk_level.ToString().PadLeft(4));
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.ResetColor();
        }

        public static void draw_map_path_levels(c_map_node[][] map)
        {
            foreach (c_map_node[] row in map)
            {
                foreach (c_map_node node in row)
                {
                    if (node.is_on_best_path)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        if (node.total_risk_to_end < 10) { Console.ForegroundColor = ConsoleColor.DarkBlue; }
                        else if (node.total_risk_to_end < 20) { Console.ForegroundColor = ConsoleColor.Blue; }
                        else if (node.total_risk_to_end < 30) { Console.ForegroundColor = ConsoleColor.Cyan; }
                        else if (node.total_risk_to_end < 40) { Console.ForegroundColor = ConsoleColor.DarkGreen; }
                        else if (node.total_risk_to_end < 50) { Console.ForegroundColor = ConsoleColor.Green; }
                        else if (node.total_risk_to_end < 60) { Console.ForegroundColor = ConsoleColor.DarkYellow; }
                        else if (node.total_risk_to_end < 70) { Console.ForegroundColor = ConsoleColor.Yellow; }
                        else if (node.total_risk_to_end < 80) { Console.ForegroundColor = ConsoleColor.Red; }
                        else { Console.ForegroundColor = ConsoleColor.DarkRed; }
                    }

                    Console.Write(node.total_risk_to_end.ToString().PadLeft(4));
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.ResetColor();
        }

        internal static char k_map_icon_left_and_right = special_characters.k_box_icon_left_and_right;
        internal static char k_map_icon_up_and_down = special_characters.k_box_icon_up_and_down;

        internal static char k_map_icon_down_and_right = special_characters.k_box_icon_down_and_right;
        internal static char k_map_icon_down_and_left = special_characters.k_box_icon_down_and_left;
        internal static char k_map_icon_up_and_right = special_characters.k_box_icon_up_and_right;
        internal static char k_map_icon_up_and_left = special_characters.k_box_icon_up_and_left;

        internal static char k_map_icon_all_but_up = special_characters.k_box_icon_all_but_up;
        internal static char k_map_icon_all_but_down = special_characters.k_box_icon_all_but_down;
        internal static char k_map_icon_all_but_left = special_characters.k_box_icon_all_but_left;
        internal static char k_map_icon_all_but_right = special_characters.k_box_icon_all_but_right;

        internal static char k_map_icon_up = special_characters.k_box_icon_up;
        internal static char k_map_icon_down = special_characters.k_box_icon_down;
        internal static char k_map_icon_left = special_characters.k_box_icon_left;
        internal static char k_map_icon_right = special_characters.k_box_icon_right;

        internal static char k_map_icon_all_four = special_characters.k_box_icon_all_four;
        internal static char k_map_icon_empty = special_characters.k_box_icon_none;

        internal static char k_map_icon_endpoint = 'O';

        internal class c_map_path_node
        {
            public bool is_endpoint;
            public bool up;
            public bool down;
            public bool left;
            public bool right;

            public void add_incoming(e_map_direction direction)
            {
                switch (direction)
                {
                    case e_map_direction.up: down = true; break;
                    case e_map_direction.down: up = true; break;
                    case e_map_direction.left: right = true; break;
                    case e_map_direction.right: left = true; break;
                }
            }

            public void add_outgoing(e_map_direction direction)
            {
                switch (direction)
                {
                    case e_map_direction.up: up = true; break;
                    case e_map_direction.down: down = true; break;
                    case e_map_direction.left: left = true; break;
                    case e_map_direction.right: right = true; break;
                }
            }

            public bool is_disconnected()
            {
                return !up && !down && !left && !right;
            }

            public char get_char()
            {
                if (is_endpoint)
                {
                    return k_map_icon_endpoint;
                }

                switch (up, down, left, right)
                {
                    case (true, true, true, true): return k_map_icon_all_four;

                    case (false, true, true, true): return k_map_icon_all_but_up;
                    case (true, false, true, true): return k_map_icon_all_but_down;
                    case (true, true, false, true): return k_map_icon_all_but_left;
                    case (true, true, true, false): return k_map_icon_all_but_right;

                    case (true, true, false, false): return k_map_icon_up_and_down;
                    case (true, false, true, false): return k_map_icon_up_and_left;
                    case (false, true, true, false): return k_map_icon_down_and_left;
                    case (true, false, false, true): return k_map_icon_up_and_right;
                    case (false, true, false, true): return k_map_icon_down_and_right;
                    case (false, false, true, true): return k_map_icon_left_and_right;

                    case (true, false, false, false): return k_map_icon_up;
                    case (false, true, false, false): return k_map_icon_down;
                    case (false, false, true, false): return k_map_icon_left;
                    case (false, false, false, true): return k_map_icon_right;

                    case (false, false, false, false): return k_map_icon_empty;
                }
            }
        }

        public static void fill_map_paths(c_map_node[][] map, c_map_path_node[][] map_paths, c_map_node start, c_map_node end)
        {
            if (!map_paths[start.row][start.column].is_disconnected())
            {
                return;
            }

            c_map_node current = start;
            e_map_direction previous_direction = e_map_direction.unknown;

            while (current != end)
            {
                map_paths[current.row][current.column].add_incoming(previous_direction);
                map_paths[current.row][current.column].add_outgoing(current.direction_to_end);

                int next_row = current.row;
                int next_column = current.column;
                switch (current.direction_to_end)
                {
                    case e_map_direction.up: next_row--; break;
                    case e_map_direction.down: next_row++; break;
                    case e_map_direction.left: next_column--; break;
                    case e_map_direction.right: next_column++; break;
                }

                previous_direction = current.direction_to_end;
                current = map[next_row][next_column];

                if (!map_paths[current.row][current.column].is_disconnected())
                {
                    map_paths[start.row][start.column].is_endpoint = true;

                    map_paths[current.row][current.column].add_incoming(previous_direction);

                    break;
                }
            }

            map_paths[start.row][start.column].is_endpoint = true;
            map_paths[end.row][end.column].is_endpoint = true;
        }

        public static void draw_map_path(c_map_node[][] map, c_map_node start, c_map_node end)
        {
            int row_count = map.Length;
            int column_count = map[0].Length;
            c_map_path_node[][] map_paths = new c_map_path_node[row_count][];

            for (int row = 0; row < row_count; row++)
            {
                map_paths[row] = new c_map_path_node[column_count];

                for (int column = 0; column < column_count; column++)
                {
                    map_paths[row][column] = new c_map_path_node();
                }
            }

            fill_map_paths(map, map_paths, start, end);

            for (int row = 0; row < row_count; row++)
            {
                for (int column = 0; column < column_count; column++)
                {
                    fill_map_paths(map, map_paths, map[row][column], end);
                }
            }

            for (int row = 0; row < row_count; row++)
            {
                for (int column = 0; column < column_count; column++)
                {
                    c_map_node node = map[row][column];

                    if (map[row][column] == start || map[row][column] == end) { Console.ForegroundColor = ConsoleColor.Green; }
                    else if (map[row][column].is_on_best_path) { Console.ForegroundColor = ConsoleColor.White; }
                    else { Console.ForegroundColor = ConsoleColor.DarkGray; }

                    Console.Write(map_paths[row][column].get_char());
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.ResetColor();
        }

        public static void map_solver(c_map_node[][] map, bool pretty)
        {
            if (pretty) draw_map_risk_levels(map);

            c_map_node start = map[0][0];
            c_map_node end = map[map.Length - 1][map[0].Length - 1];
            end.mark_end();

            // TODO minheap to get smallest off the top instead of any
            // Eh, another time maybe.
            Queue<c_map_node> node_queue = new Queue<c_map_node>();
            node_queue.Enqueue(end);

            while (node_queue.Count > 0)
            {
                c_map_node current = node_queue.Dequeue();

                if (current.row > 0)
                {
                    c_map_node neighbor = map[current.row - 1][current.column];

                    if (neighbor.consider_neighbor(e_map_direction.down, current))
                    {
                        node_queue.Enqueue(neighbor);
                    }
                }

                if (current.row < map.Length - 1)
                {
                    c_map_node neighbor = map[current.row + 1][current.column];

                    if (neighbor.consider_neighbor(e_map_direction.up, current))
                    {
                        node_queue.Enqueue(neighbor);
                    }
                }

                if (current.column > 0)
                {
                    c_map_node neighbor = map[current.row][current.column - 1];

                    if (neighbor.consider_neighbor(e_map_direction.right, current))
                    {
                        node_queue.Enqueue(neighbor);
                    }
                }

                if (current.column < map[0].Length - 1)
                {
                    c_map_node neighbor = map[current.row][current.column + 1];

                    if (neighbor.consider_neighbor(e_map_direction.left, current))
                    {
                        node_queue.Enqueue(neighbor);
                    }
                }
            }

            c_map_node best_path_node = start;
            while (best_path_node != end)
            {
                best_path_node.is_on_best_path = true;

                switch (best_path_node.direction_to_end)
                {
                    case e_map_direction.up:
                        best_path_node = map[best_path_node.row - 1][best_path_node.column];
                        break;

                    case e_map_direction.down:
                        best_path_node = map[best_path_node.row + 1][best_path_node.column];
                        break;

                    case e_map_direction.left:
                        best_path_node = map[best_path_node.row][best_path_node.column - 1];
                        break;

                    case e_map_direction.right:
                        best_path_node = map[best_path_node.row][best_path_node.column + 1];
                        break;
                }
            }

            if (pretty) draw_map_path_as_total(map);
            if (pretty) draw_map_path_as_individual(map);
            if (pretty) draw_map_path_levels(map);
            if (pretty) draw_map_path(map, start, end);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", start.total_risk_to_end);
            Console.ResetColor();
        }

        public static void Part_1(string input, bool pretty)
        {
            c_map_node[][] map = parse_input(input);

            map_solver(map, pretty);
        }

        public static c_map_node[][] multiply_map_by(c_map_node[][] small_map, int map_multiplier)
        {
            int small_map_row_count = small_map.Length;
            int small_map_column_count = small_map[0].Length;

            int map_row_count = small_map_row_count * map_multiplier;
            int map_column_count = small_map_column_count * map_multiplier;

            c_map_node[][] map = new c_map_node[map_row_count][];
            for (int row = 0; row < map_row_count; row++)
            {
                map[row] = new c_map_node[map_column_count];
            }

            for (int small_map_row = 0; small_map_row < small_map.Length; small_map_row++)
            {
                for (int small_map_column = 0; small_map_column < small_map[small_map_row].Length; small_map_column++)
                {
                    c_map_node small_map_node = small_map[small_map_row][small_map_column];

                    for (int row_multiplier = 0; row_multiplier < map_multiplier; row_multiplier++)
                    {
                        for (int column_multiplier = 0; column_multiplier < map_multiplier; column_multiplier++)
                        {
                            int row = small_map_row + small_map_row_count * row_multiplier;
                            int column = small_map_column + small_map_column_count * column_multiplier;

                            // TODO less dumb way of calculating this
                            int risk_level = small_map_node.risk_level;
                            for (int i = 0; i < row_multiplier + column_multiplier; i++)
                            {
                                risk_level = Math.Max(1, (risk_level + 1) % 10);
                            }

                            map[row][column] = new c_map_node(risk_level, row, column);
                        }
                    }
                }
            }

            return map;
        }

        public static void Part_2(string input, bool pretty)
        {
            c_map_node[][] small_map = parse_input(input);

            c_map_node[][] map = multiply_map_by(small_map, 5);

            map_solver(map, pretty);
        }
    }
}
