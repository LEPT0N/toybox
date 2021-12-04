using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace advent_of_code_2021.Days
{
    internal class c_input_reader
    {
        public c_input_reader(string input)
        {
            m_line_number = 0;
            m_lines = System.IO.File.ReadAllLines(input);
        }

        public string read_line()
        {
            string result = m_lines[m_line_number];

            m_line_number++;

            return result;
        }

        public bool has_more_lines()
        {
            return m_line_number < m_lines.Length;
        }

        private string[] m_lines;
        private int m_line_number;
    }

    internal class c_bingo_board
    {
        private const int k_bingo_board_side = 5;

        internal class c_bingo_space
        {
            public c_bingo_space(int value)
            {
                Value = value;
                Marked = false;
            }

            public readonly int Value;
            public bool Marked;
        }

        public bool Winner { get; private set; } = false;
        public int Winning_Move{ get; private set; } = 0;
        public int Score { get; private set; } = 0;
        private c_bingo_space[,] m_spaces = new c_bingo_space[k_bingo_board_side, k_bingo_board_side];

        public void read_from_lines(c_input_reader input_reader)
        {
            input_reader.read_line();

            for (int row = 0; row < k_bingo_board_side; row++)
            {
                string[] values = input_reader.read_line().Split(' ').Where(x => x.Length > 0).ToArray();

                for (int column = 0; column < k_bingo_board_side; column++)
                {
                    m_spaces[row, column] = new c_bingo_space(int.Parse(values[column]));
                }
            }
        }

        public void print()
        {
            for (int row = 0; row < k_bingo_board_side; row++)
            {
                for (int column = 0; column < k_bingo_board_side; column++)
                {
                    Console.ForegroundColor = (m_spaces[row, column].Marked ?
                        ConsoleColor.Green :
                        ConsoleColor.Gray);

                    Console.Write(String.Format("{0,3}", m_spaces[row, column].Value));
                }

                Console.WriteLine();
            }

            Console.ResetColor();
        }

        public int check_move(int move)
        {
            bool found_space = false;

            for (int row = 0; row < k_bingo_board_side && !found_space; row++)
            {
                for (int column = 0; column < k_bingo_board_side && !found_space; column++)
                {
                    if (m_spaces[row,column].Value == move)
                    {
                        m_spaces[row, column].Marked = true;
                        found_space = true;
                    }
                }
            }

            if (check_if_winner())
            {
                Winning_Move = move;
                Winner = true;
                Score = calculate_score(move);
            }

            return Score;
        }

        private bool check_if_winner()
        {
            int[,,] k_sets =
            {
                { {0,0}, {0,1}, {0,2}, {0,3}, {0,4} },
                { {1,0}, {1,1}, {1,2}, {1,3}, {1,4} },
                { {2,0}, {2,1}, {2,2}, {2,3}, {2,4} },
                { {3,0}, {3,1}, {3,2}, {3,3}, {3,4} },
                { {4,0}, {4,1}, {4,2}, {4,3}, {4,4} },

                { {0,0}, {1,0}, {2,0}, {3,0}, {4,0} },
                { {0,1}, {1,1}, {2,1}, {3,1}, {4,1} },
                { {0,2}, {1,2}, {2,2}, {3,2}, {4,2} },
                { {0,3}, {1,3}, {2,3}, {3,3}, {4,3} },
                { {0,4}, {1,4}, {2,4}, {3,4}, {4,4} },

                { {0,0}, {1,1}, {2,2}, {3,3}, {4,4} },

                { {0,4}, {1,3}, {2,2}, {3,1}, {4,0} },
            };

            for(int i = 0; i < k_sets.GetLength(0); i++)
            {
                bool is_set_winner = true;

                for(int j = 0; j < k_sets.GetLength(1); j++)
                {
                    if (!m_spaces[k_sets[i,j,0], k_sets[i,j,1]].Marked)
                    {
                        is_set_winner = false;
                        break;
                    }
                }

                if (is_set_winner)
                {
                    return true;
                }
            }

            return false;
        }

        private int calculate_score(int move)
        {
            int score = 0;

            for (int row = 0; row < k_bingo_board_side; row++)
            {
                for (int column = 0; column < k_bingo_board_side; column++)
                {
                    if (!m_spaces[row, column].Marked)
                    {
                        score += m_spaces[row, column].Value;
                    }
                }
            }

            score *= move;

            return score;
        }
    }

    internal class Day_4
    {
        public static void Part_1(string input)
        {
            c_input_reader input_reader = new c_input_reader(input);

            int[] moves = input_reader.read_line()
                .Split(',')
                .Select(move => int.Parse(move))
                .ToArray();

            List<c_bingo_board> boards = new List<c_bingo_board>();
            while (input_reader.has_more_lines())
            {
                c_bingo_board board = new c_bingo_board();
                board.read_from_lines(input_reader);
                boards.Add(board);
            }

            int winning_score = 0;

            foreach(int move in moves)
            {
                foreach(c_bingo_board board in boards)
                {
                    int score = board.check_move(move);

                    if (score != 0)
                    {
                        Console.WriteLine("We have a winner!");
                        Console.WriteLine();
                        board.print();
                        Console.WriteLine();
                        Console.WriteLine("Winning Move = " + move);

                        winning_score = score;
                        break;
                    }
                }

                if (winning_score != 0)
                {
                    break;
                }
            }

            Console.WriteLine("Winning Score = " + winning_score);
        }

        public static void Part_2(string input)
        {
            c_input_reader input_reader = new c_input_reader(input);

            int[] moves = input_reader.read_line()
                .Split(',')
                .Select(move => int.Parse(move))
                .ToArray();

            List<c_bingo_board> boards = new List<c_bingo_board>();
            while (input_reader.has_more_lines())
            {
                c_bingo_board board = new c_bingo_board();
                board.read_from_lines(input_reader);
                boards.Add(board);
            }

            c_bingo_board first_winning_board = null;
            c_bingo_board last_winning_board = null;

            foreach (int move in moves)
            {
                foreach (c_bingo_board board in boards)
                {
                    board.check_move(move);

                    if (board.Winner)
                    {
                        if (first_winning_board == null)
                        {
                            first_winning_board = board;
                        }

                        last_winning_board = board;
                    }
                }

                boards = boards.Where(x => !x.Winner).ToList();
            }

            Console.WriteLine("First Winning Board:");
            Console.WriteLine();
            first_winning_board.print();
            Console.WriteLine();
            Console.WriteLine("Winning Move = " + first_winning_board.Winning_Move);
            Console.WriteLine("Winning Score = " + first_winning_board.Score);

            Console.WriteLine();

            Console.WriteLine("Last Winning Board:");
            Console.WriteLine();
            last_winning_board.print();
            Console.WriteLine();
            Console.WriteLine("Winning Move = " + last_winning_board.Winning_Move);
            Console.WriteLine("Winning Score = " + last_winning_board.Score);
        }
    }
}
