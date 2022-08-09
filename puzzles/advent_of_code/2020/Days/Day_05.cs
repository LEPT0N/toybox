using System;
using System.Collections.Generic;
using advent_of_code_common.input_reader;

namespace advent_of_code_2020.Days
{
    internal class Day_05
    {
        internal class c_seat
        {
            public c_seat(string input)
            {
                int input_index = 0;

                {
                    int row_low = 0;
                    int row_high = 127;
                    int row_half = 64;

                    for (; input_index < 7; input_index++)
                    {
                        if (input[input_index] == 'F')
                        {
                            row_high -= row_half;
                        }
                        else
                        {
                            row_low += row_half;
                        }

                        row_half /= 2;
                    }

                    m_row = row_low;
                }

                {
                    int column_low = 0;
                    int column_high = 7;
                    int column_half = 4;

                    for (; input_index < 10; input_index++)
                    {
                        if (input[input_index] == 'L')
                        {
                            column_high -= column_half;
                        }
                        else
                        {
                            column_low += column_half;
                        }

                        column_half /= 2;
                    }

                    m_column = column_low;
                }
            }

            public int get_seat_id()
            {
                return m_row * 8 + m_column;
            }

            private int m_row;
            private int m_column;
        }

        public static void Part_1(string input, bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            int highest_seat_id = 0;

            while (input_reader.has_more_lines())
            {
                c_seat seat = new c_seat(input_reader.read_line());

                int seat_id = seat.get_seat_id();

                if (seat_id > highest_seat_id)
                {
                    highest_seat_id = seat_id;
                }
            }

            Console.WriteLine("The highest Seat ID = {0}", highest_seat_id);
        }
        public static void Part_2(string input, bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            List<int> seat_ids = new List<int>();

            while (input_reader.has_more_lines())
            {
                c_seat seat = new c_seat(input_reader.read_line());

                int seat_id = seat.get_seat_id();

                seat_ids.Add(seat_id);
            }

            seat_ids.Sort();

            for (int i = 0; i < seat_ids.Count; i++)
            {
                if (seat_ids[i] + 1 != seat_ids[i + 1])
                {
                    Console.WriteLine("My seat ID = {0}", seat_ids[i] + 1);

                    return;
                }
            }
        }
    }
}
