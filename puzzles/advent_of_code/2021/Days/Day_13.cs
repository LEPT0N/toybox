using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace advent_of_code_2021.Days
{
    internal class Day_13
    {
        internal class c_coordinate
        {
            public int X;
            public int Y;

            public override string ToString()
            {
                return string.Format("[{0}, {1}]", X, Y);
            }
        }

        internal static c_coordinate max_coordinate(c_coordinate a, c_coordinate b)
        {
            return new c_coordinate() { X = Math.Max(a.X, b.X), Y = Math.Max(a.Y, b.Y) };
        }

        internal enum e_orientation
        {
            horizontal,
            vertical,
        }

        [DebuggerDisplay("{orientation} at {position}", Type = "c_fold")]
        internal class c_fold
        {
            public int position;
            public e_orientation orientation;
        }

        internal static void parse_input(string input, out List<c_coordinate> points, out List<c_fold> folds, out c_coordinate max)
        {
            c_input_reader input_reader = new c_input_reader(input);

            points = new List<c_coordinate>();
            max = new c_coordinate();

            while (input_reader.has_more_lines())
            {
                string input_line = input_reader.read_line();

                if (string.IsNullOrEmpty(input_line))
                {
                    break;
                }

                int[] parsed_input_line = input_line.Split(",").Select(x => int.Parse(x)).ToArray();

                c_coordinate new_point = new c_coordinate() { X = parsed_input_line[0], Y = parsed_input_line[1] };
                points.Add(new_point);

                max = max_coordinate(max, new_point);
            }
            
            folds = new List<c_fold>();

            while (input_reader.has_more_lines())
            {
                string input_line = input_reader.read_line();

                string[] split_input_line = input_line.Split(" ").ToArray();

                string[] parsed_input_line = split_input_line[2].Split("=");

                int fold_position = int.Parse(parsed_input_line[1]);
                e_orientation fold_orientation = (parsed_input_line[0] == "x") ? e_orientation.vertical : e_orientation.horizontal;

                folds.Add(new c_fold { position = fold_position, orientation = fold_orientation });
            }
        }

        internal static void clamp_to_fold(c_coordinate point, c_fold fold)
        {
            if (fold.orientation == e_orientation.horizontal)
            {
                point.Y = Math.Min(point.Y, fold.position);
            }
            else
            {
                point.X = Math.Min(point.X, fold.position);
            }
        }

        internal static void fold_points(List<c_coordinate> points, c_fold fold)
        {
            for (int i = 0; i < points.Count; i++)
            {
                if (fold.orientation == e_orientation.horizontal)
                {
                    if (points[i].Y > fold.position)
                    {
                        points[i].Y = fold.position - (points[i].Y - fold.position);
                    }
                }
                else
                {
                    if (points[i].X > fold.position)
                    {
                        points[i].X = fold.position - (points[i].X - fold.position);
                    }
                }
            }
        }

        internal static void display_points(c_coordinate max, List<c_coordinate> points)
        {
            bool[,] output = new bool[max.X + 1, max.Y + 1];
            foreach (c_coordinate point in points)
            {
                output[point.X, point.Y] = true;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();

            for (int y = 0; y < output.GetLength(1); y++)
            {
                for (int x = 0; x < output.GetLength(0); x++)
                {
                    Console.Write(output[x, y] ? "O" : " ");
                }
                Console.WriteLine();
            }
            Console.ResetColor();
        }

        public static void Part_1(string input, bool pretty)
        {
            c_coordinate max;
            List<c_coordinate> points;
            List<c_fold> folds;
            parse_input(input, out points, out folds, out max);

            fold_points(points, folds[0]);
            clamp_to_fold(max, folds[0]);

            if (points.Count < 100)
            {
                display_points(max, points);
            }

            int result = points.Select(x => x.ToString()).Distinct().Count();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", result);
            Console.ResetColor();
        }

        public static void Part_2(string input, bool pretty)
        {
            c_coordinate max;
            List<c_coordinate> points;
            List<c_fold> folds;
            parse_input(input, out points, out folds, out max);

            foreach (c_fold fold in folds)
            {
                fold_points(points, fold);
                clamp_to_fold(max, fold);
            }

            display_points(max, points);
        }
    }
}
