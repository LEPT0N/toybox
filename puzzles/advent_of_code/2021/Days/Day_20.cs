using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace advent_of_code_2021.Days
{
    internal class Day_20
    {
        internal class c_image_enhancement_algorithm
        {
            private BitArray bit_array;

            public c_image_enhancement_algorithm(string input)
            {
                bit_array = new BitArray(input.Length, false);

                for (int i = 0; i < input.Length; i++)
                {
                    if (input[i] == '#')
                    {
                        bit_array.Set(i, true);
                    }
                }
            }

            public bool is_set(int index)
            {
                return bit_array.Get(index);
            }
        }

        internal class c_image
        {
            bool[][] data;
            bool background;

            private c_image(int rows, int columns)
            {
                data = new bool[rows][];
                for (int row = 0; row < rows; row++)
                {
                    data[row] = new bool[columns];
                }

                background = false;
            }

            public c_image(c_input_reader input_reader)
            {
                List<bool[]> data_list = new List<bool[]>();

                while (input_reader.has_more_lines())
                {
                    string input_line = input_reader.read_line();

                    bool[] data_line = new bool[input_line.Length];

                    for (int i = 0; i < input_line.Length; i++)
                    {
                        if (input_line[i] == '#')
                        {
                            data_line[i] = true;
                        }
                    }

                    data_list.Add(data_line);
                }

                background = false;
                data = data_list.ToArray();
            }

            public void print()
            {
                foreach (bool[] row in data)
                {
                    foreach (bool value in row)
                    {
                        Console.Write("{0}", value ? '#' : '.');
                    }
                    Console.WriteLine();
                }
            }

            public int get_lit_pixel_count()
            {
                if (background)
                {
                    return int.MaxValue;
                }

                int result = 0;

                for (int row = 0; row < data.Length; row++)
                {
                    for (int column = 0; column < data[row].Length; column++)
                    {
                        if (data[row][column])
                        {
                            result++;
                        }
                    }
                }

                return result;
            }

            private bool is_pixel_lit(int row, int column)
            {
                if (row < 0 || row >= data.Length ||
                    column < 0 || column >= data[row].Length)
                {
                    return background;
                }

                return data[row][column];
            }

            private int enhance_pixel(int center_row, int center_column)
            {
                int result = 0;

                for (int row = center_row - 1; row <= center_row + 1; row++)
                {
                    for (int column = center_column - 1; column <= center_column + 1; column++)
                    {
                        result = result << 1;

                        if (is_pixel_lit(row, column))
                        {
                            result |= 1;
                        }
                    }
                }

                return result;
            }

            public c_image enhance(c_image_enhancement_algorithm algorithm)
            {
                c_image enhanced_image = new c_image(data.Length + 2, data[0].Length + 2);

                for (int input_row = -1; input_row <= data.Length; input_row++)
                {
                    for (int input_column = -1; input_column <= data[0].Length; input_column++)
                    {
                        int value = enhance_pixel(input_row, input_column);

                        if (algorithm.is_set(value))
                        {
                            enhanced_image.data[input_row + 1][input_column + 1] = true;
                        }
                    }
                }

                if (background)
                {
                    enhanced_image.background = algorithm.is_set(0x1ff);
                }
                else
                {
                    enhanced_image.background = algorithm.is_set(0);
                }

                return enhanced_image;
            }
        }

        internal static (c_image_enhancement_algorithm, c_image) parse_input(
            string input,
            bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            c_image_enhancement_algorithm algorithm = new c_image_enhancement_algorithm(input_reader.read_line());

            input_reader.read_line();

            c_image image = new c_image(input_reader);

            return (algorithm, image);
        }

        public static void Day_20_Worker(
            string input,
            bool pretty,
            int iterations)
        {
            (c_image_enhancement_algorithm algorithm, c_image image) = parse_input(input, pretty);

            if (pretty)
            {
                Console.WriteLine("input:");
                Console.WriteLine();
                image.print();
                Console.WriteLine();
            }

            for (int i = 0; i < iterations; i++)
            {
                image = image.enhance(algorithm);

                if (pretty)
                {
                    Console.WriteLine("iteration[{0}]:", i);
                    Console.WriteLine();
                    image.print();
                    Console.WriteLine();
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", image.get_lit_pixel_count());
            Console.ResetColor();
        }

        public static void Part_1(
            string input,
            bool pretty)
        {
            Day_20_Worker(input, pretty, 2);
        }

        public static void Part_2(
            string input,
            bool pretty)
        {
            Day_20_Worker(input, pretty, 50);
        }
    }
}
