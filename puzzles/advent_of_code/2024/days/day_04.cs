using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.display_helpers;
using advent_of_code_common.input_reader;

namespace advent_of_code_2024.days
{
    internal class day_04
    {
        [DebuggerDisplay("{value} ({usage_count})", Type = "s_board_entry")]
        internal struct s_board_entry
        {
            public readonly char value;

            public int usage_count;

            public s_board_entry(char v)
            {
                value = v;
                usage_count = 0;
            }
        }

        internal static void display_board(s_board_entry[][] board)
        {
            int max_usage = board.Max(row => row.Max(entry => entry.usage_count));

            board.display(entry =>
            {
                if (entry.usage_count == 0)
                {
                    Console.Write(' ');
                }
                else
                {
                    ConsoleColor color;

                    if (entry.usage_count <= max_usage / 4)
                    {
                        color = ConsoleColor.DarkCyan;
                    }
                    else if (entry.usage_count <= max_usage / 2)
                    {
                        color = ConsoleColor.DarkGreen;
                    }
                    else if (entry.usage_count <= max_usage * 3 / 4)
                    {
                        color = ConsoleColor.DarkYellow;
                    }
                    else
                    {
                        color = ConsoleColor.DarkRed;
                    }

                    Console.ForegroundColor = color;
                    Console.Write(entry.value);
                }
            });
        }

        internal static s_board_entry[][] parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            List<s_board_entry[]> lines = new List<s_board_entry[]>();

            while (input_reader.has_more_lines())
            {
                lines.Add(input_reader.read_line().Select(c => new s_board_entry(c)).ToArray());
            }

            return lines.ToArray();
        }

        internal static bool is_matching_word(s_board_entry[][] board, string word, int starting_row, int starting_col, int row_direction, int col_direction)
        {
            // See if the word is on the board at the specified spot and direction.

            int row = starting_row;
            int col = starting_col;

            for (int index = 0; index < word.Length; index++)
            {
                if (row < 0 || col < 0 ||
                    row >= board.Length || col >= board[row].Length ||
                    board[row][col].value != word[index])
                {
                    return false;
                }

                row += row_direction;
                col += col_direction;
            }

            row = starting_row;
            col = starting_col;

            for (int index = 0; index < word.Length; index++)
            {
                board[row][col].usage_count++;

                row += row_direction;
                col += col_direction;
            }

            return true;
        }

        public static void part_1(
            string input,
            bool pretty)
        {
            s_board_entry[][] board = parse_input(input, pretty);

            string word = "XMAS";

            int count = 0;

            // Search each starting position on the board.
            for (int row = 0; row < board.Length; row++)
            {
                for (int col = 0; col < board[row].Length; col++)
                {
                    // Only start a search on a board position matching the first letter in our word.
                    if (board[row][col].value == word[0])
                    {
                        // Search in every direction
                        for (int row_direction = -1; row_direction <= 1; row_direction++)
                        {
                            for (int col_direction = -1; col_direction <= 1; col_direction++)
                            {
                                if (row_direction != 0 || col_direction != 0)
                                {
                                    if (is_matching_word(board, word, row, col, row_direction, col_direction))
                                    {
                                        count++;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (pretty)
            {
                display_board(board);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", count);
            Console.ResetColor();
        }

        [DebuggerDisplay("{value} ({row_offset}, {col_offset})", Type = "s_template_entry")]
        internal struct s_template_entry
        {
            public readonly char value;
            public readonly int row_offset;
            public readonly int col_offset;

            public s_template_entry(char v, int r, int c)
            {
                value = v;
                row_offset = r;
                col_offset = c;
            }
        }

        internal static readonly s_template_entry[][] k_search_templates =
        {
            new s_template_entry[] { new s_template_entry('A', 0, 0), new s_template_entry('M', -1, -1), new s_template_entry('M', -1, 1), new s_template_entry('S', 1, 1), new s_template_entry('S', 1, -1) },
            new s_template_entry[] { new s_template_entry('A', 0, 0), new s_template_entry('S', -1, -1), new s_template_entry('M', -1, 1), new s_template_entry('M', 1, 1), new s_template_entry('S', 1, -1) },
            new s_template_entry[] { new s_template_entry('A', 0, 0), new s_template_entry('S', -1, -1), new s_template_entry('S', -1, 1), new s_template_entry('M', 1, 1), new s_template_entry('M', 1, -1) },
            new s_template_entry[] { new s_template_entry('A', 0, 0), new s_template_entry('M', -1, -1), new s_template_entry('S', -1, 1), new s_template_entry('S', 1, 1), new s_template_entry('M', 1, -1) },
        };

        internal static bool is_matching_template(s_board_entry[][] board, s_template_entry[] template, int row, int col)
        {
            // See if the template is on the board at the specified spot.

            for (int index = 0; index < template.Length; index++)
            {
                char item_char = template[index].value;
                int item_row = row + template[index].row_offset;
                int item_col = col + template[index].col_offset;

                if (item_row < 0 || item_col < 0 ||
                    item_row >= board.Length || item_col >= board[item_row].Length ||
                    board[item_row][item_col].value != item_char)
                {
                    return false;
                }
            }

            for (int index = 0; index < template.Length; index++)
            {
                board[row + template[index].row_offset][col + template[index].col_offset].usage_count++;
            }

            return true;
        }

        public static void part_2(
            string input,
            bool pretty)
        {
            s_board_entry[][] board = parse_input(input, pretty);

            int count = 0;

            // Search each starting position on the board.
            for (int row = 0; row < board.Length; row++)
            {
                for (int col = 0; col < board[row].Length; col++)
                {
                    // Search every template on this spot.
                    for (int template_index = 0; template_index < k_search_templates.Length; template_index++)
                    {
                        if (is_matching_template(board, k_search_templates[template_index], row, col))
                        {
                            count++;
                        }
                    }
                }
            }

            if (pretty)
            {
                display_board(board);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", count);
            Console.ResetColor();
        }
    }
}
