using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace advent_of_code_2021.Days
{
    internal class Day_08
    {
        public static void Part_1(string input)
        {
            c_input_reader input_reader = new c_input_reader(input);

            int result = 0;

            while (input_reader.has_more_lines())
            {
                string input_line = input_reader.read_line();
                string[] input_line_segments = input_line.Split(" | ");

                string[] output_values = input_line_segments[1].Split(" ");

                foreach (string output_value in output_values)
                {
                    int output_value_length = output_value.Length;

                    if (output_value_length == 2 ||
                        output_value_length == 3 ||
                        output_value_length == 4 ||
                        output_value_length == 7)
                    {
                        result++;
                    }
                }
            }

            Console.WriteLine("Result = {0}", result);
        }

        // A displayed number on the board and which lights are on for it
        internal class c_displayed_number
        {
            private bool[] m_lights = { false, false, false, false, false, false, false };

            public c_displayed_number(string input)
            {
                foreach (char c in input)
                {
                    m_lights[c - 'a'] = true;
                }
            }

            public c_displayed_number(char[] input)
            {
                foreach (char c in input)
                {
                    m_lights[c - 'a'] = true;
                }
            }

            public void set_light(char c)
            {
                m_lights[c - 'a'] = true;
            }

            public bool is_light_on(int i)
            {
                return m_lights[i];
            }

            public string Lights
            {
                get
                {
                    string result = "";

                    for (int i = 0; i < m_lights.Length; i++)
                    {
                        if (m_lights[i])
                        {
                            char light = 'a';
                            light += (char)i;
                            result += light;
                        }
                    }

                    return result;
                }
            }
        }

        // A light on the board, and which wires could connect to it
        internal class c_display_segment
        {
            private bool[] m_possibilities = { true, true, true, true, true, true, true };

            // Only call this when you know there's exactly one possibility.
            public char get_first_possibility()
            {
                for (int i = 0; i < m_possibilities.Length; i++)
                {
                    if (m_possibilities[i])
                    {
                        char first_possibility = 'a';
                        first_possibility += (char)i;
                        return first_possibility;
                    }
                }

                return '\0';
            }

            public void remove_all(c_displayed_number displayed_number)
            {
                for (int i = 0; i < m_possibilities.Length; i++)
                {
                    if (displayed_number.is_light_on(i))
                    {
                        m_possibilities[i] = false;
                    }
                }
            }

            public void remove_all(c_display_segment other)
            {
                for (int i = 0; i < m_possibilities.Length; i++)
                {
                    if (other.m_possibilities[i])
                    {
                        m_possibilities[i] = false;
                    }
                }
            }

            public void remove_all_except(c_displayed_number displayed_number)
            {
                for (int i = 0; i < m_possibilities.Length; i++)
                {
                    if (!displayed_number.is_light_on(i))
                    {
                        m_possibilities[i] = false;
                    }
                }
            }

            public void remove_all_except_matches_count(
                List<c_displayed_number> displayed_numbers,
                int count)
            {
                for (int i = 0; i < m_possibilities.Length; i++)
                {
                    if (count != displayed_numbers.Where(x => x.is_light_on(i)).Count())
                    {
                        m_possibilities[i] = false;
                    }
                }
            }

            // Useful for debugging
            public string Possibilities
            {
                get
                {
                    string result = "";

                    for (int i = 0; i < m_possibilities.Length; i++)
                    {
                        if (m_possibilities[i])
                        {
                            char possibility = 'a';
                            possibility += (char)i;
                            result += possibility;
                        }
                    }

                    return result;
                }
            }
        }

        // Keep track of what each of the c_display_numbers mean
        internal class c_display
        {
            // Index == which number this c_display_number is
            c_displayed_number[] m_displayed_numbers = new c_displayed_number[10];

            public c_display(string[] inputs)
            {
                // Convert the input strings to c_displayed_numbers and bucket by input length

                List<c_displayed_number>[] inputs_by_length = new List<c_displayed_number>[10];

                foreach (string input in inputs)
                {
                    if (inputs_by_length[input.Length] == null)
                    {
                        inputs_by_length[input.Length] = new List<c_displayed_number>();
                    }

                    c_displayed_number displayed_number = new c_displayed_number(input);
                    inputs_by_length[input.Length].Add(displayed_number);
                }

                // Build our c_display_segments and remove possibilities from them one at a time.
                c_display_segment top_left = new c_display_segment();
                c_display_segment top_right = new c_display_segment();
                c_display_segment bottom_left = new c_display_segment();
                c_display_segment bottom_right = new c_display_segment();
                c_display_segment top = new c_display_segment();
                c_display_segment middle = new c_display_segment();
                c_display_segment bottom = new c_display_segment();

                // 1
                {
                    // There is only one c_display_number with 2 lights on
                    c_displayed_number displayed_number = inputs_by_length[2][0];

                    top.remove_all(displayed_number);
                    middle.remove_all(displayed_number);
                    bottom.remove_all(displayed_number);
                    top_left.remove_all(displayed_number);
                    bottom_left.remove_all(displayed_number);

                    top_right.remove_all_except(displayed_number);
                    bottom_right.remove_all_except(displayed_number);
                }

                // 7
                {
                    // There is only one c_display_number with 3 lights on
                    c_displayed_number displayed_number = inputs_by_length[3][0];

                    middle.remove_all(displayed_number);
                    bottom.remove_all(displayed_number);
                    top_left.remove_all(displayed_number);
                    bottom_left.remove_all(displayed_number);

                    top.remove_all_except(displayed_number);
                    top_right.remove_all_except(displayed_number);
                    bottom_right.remove_all_except(displayed_number);
                }

                // 4
                {
                    // There is only one c_display_number with 4 lights on
                    c_displayed_number displayed_number = inputs_by_length[4][0];

                    top.remove_all(displayed_number);
                    bottom.remove_all(displayed_number);
                    bottom_left.remove_all(displayed_number);

                    middle.remove_all_except(displayed_number);
                    top_left.remove_all_except(displayed_number);
                    top_right.remove_all_except(displayed_number);
                    bottom_right.remove_all_except(displayed_number);
                }

                // 2 or 3 or 5
                {
                    List<c_displayed_number> displayed_numbers = inputs_by_length[5];

                    // bottom and bottom_left now only have (the same) two possibilities.
                    // bottom_left shows up once in [2, 3, 5], while bottom shows up in all of them.
                    bottom_left.remove_all_except_matches_count(displayed_numbers, 1);
                    bottom.remove_all(bottom_left);

                    // top_left and middle now only have (the same) two possibilities.
                    // top_left shows up once in [2, 3, 5], while middle shows up in all of them.
                    top_left.remove_all_except_matches_count(displayed_numbers, 1);
                    middle.remove_all(top_left);
                }

                // 6 or 9 or 0
                {
                    List<c_displayed_number> displayed_numbers = inputs_by_length[6];

                    // top_right and bottom_right now only have (the same) two possibilities.
                    // top_right shows up twice in [6, 9, 0], while bottom_right shows up in all of them.
                    top_right.remove_all_except_matches_count(displayed_numbers, 2);
                    bottom_right.remove_all(top_right);
                }

                // each of the c_display_segment's now only have one possibility remaining.
                char top_left_wire = top_left.get_first_possibility();
                char top_right_wire = top_right.get_first_possibility();
                char bottom_left_wire = bottom_left.get_first_possibility();
                char bottom_right_wire = bottom_right.get_first_possibility();
                char top_wire = top.get_first_possibility();
                char middle_wire = middle.get_first_possibility();
                char bottom_wire = bottom.get_first_possibility();

                // We now know exactly what wires are in each number, so save them for later.

                char[][] wires = new char[][]
                {
                    // 0
                    new char[]
                    {
                        top_left_wire,
                        top_right_wire,
                        bottom_left_wire,
                        bottom_right_wire,
                        top_wire,
                        // middle_wire,
                        bottom_wire,
                    },
                    
                    // 1
                    new char[]
                    {
                        // top_left_wire,
                        top_right_wire,
                        // bottom_left_wire,
                        bottom_right_wire,
                        // top_wire,
                        // middle_wire,
                        // bottom_wire,
                    },
                    
                    // 2
                    new char[]
                    {
                        // top_left_wire,
                        top_right_wire,
                        bottom_left_wire,
                        // bottom_right_wire,
                        top_wire,
                        middle_wire,
                        bottom_wire,
                    },
                    
                    // 3
                    new char[]
                    {
                        // top_left_wire,
                        top_right_wire,
                        // bottom_left_wire,
                        bottom_right_wire,
                        top_wire,
                        middle_wire,
                        bottom_wire,
                    },
                    
                    // 4
                    new char[]
                    {
                        top_left_wire,
                        top_right_wire,
                        // bottom_left_wire,
                        bottom_right_wire,
                        // top_wire,
                        middle_wire,
                        // bottom_wire,
                    },
                    
                    // 5
                    new char[]
                    {
                        top_left_wire,
                        // top_right_wire,
                        // bottom_left_wire,
                        bottom_right_wire,
                        top_wire,
                        middle_wire,
                        bottom_wire,
                    },
                    
                    // 6
                    new char[]
                    {
                        top_left_wire,
                        // top_right_wire,
                        bottom_left_wire,
                        bottom_right_wire,
                        top_wire,
                        middle_wire,
                        bottom_wire,
                    },
                    
                    // 7
                    new char[]
                    {
                        // top_left_wire,
                        top_right_wire,
                        // bottom_left_wire,
                        bottom_right_wire,
                        top_wire,
                        // middle_wire,
                        // bottom_wire,
                    },
                    
                    // 8
                    new char[]
                    {
                        top_left_wire,
                        top_right_wire,
                        bottom_left_wire,
                        bottom_right_wire,
                        top_wire,
                        middle_wire,
                        bottom_wire,
                    },
                    
                    // 9
                    new char[]
                    {
                        top_left_wire,
                        top_right_wire,
                        // bottom_left_wire,
                        bottom_right_wire,
                        top_wire,
                        middle_wire,
                        bottom_wire,
                    },
                };

                for (int i = 0; i < 10; i++)
                {
                    m_displayed_numbers[i] = new c_displayed_number(wires[i]);
                }
            }

            // figure out which stored c_displayed_number matches the input, and return the matching number.
            public int get_digit(c_displayed_number displayed_number)
            {
                for(int i = 0; i < 10; i++)
                {
                    if (m_displayed_numbers[i].Lights == displayed_number.Lights)
                    {
                        return i;
                    }
                }

                return 0;
            }
        }

        public static void Part_2(string input)
        {
            c_input_reader input_reader = new c_input_reader(input);

            int total_result = 0;

            while (input_reader.has_more_lines())
            {
                string input_line = input_reader.read_line();
                string[] input_line_segments = input_line.Split(" | ");

                string[] input_values = input_line_segments[0].Split(" ");
                string[] output_values = input_line_segments[1].Split(" ");

                c_display display = new c_display(input_values);

                int output_result = 0;

                foreach(string output_value in output_values)
                {
                    c_displayed_number displayed_number = new c_displayed_number(output_value);

                    int output_digit = display.get_digit(displayed_number);

                    output_result = output_result * 10 + output_digit;
                }

                Console.WriteLine("Output = {0}", output_result);

                total_result += output_result;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", total_result);
            Console.ResetColor();
        }
    }
}
