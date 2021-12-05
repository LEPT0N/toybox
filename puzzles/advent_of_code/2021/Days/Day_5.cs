using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace advent_of_code_2021.Days
{
    internal class Day_5
    {
        internal struct c_point
        {
            public c_point(int x, int y)
            {
                X = x;
                Y = y;
            }

            public c_point(string input)
            {
                int[] inputs = input.Split(',').Select(s => int.Parse(s)).ToArray();
                X = inputs[0];
                Y = inputs[1];
            }

            public int X { get; set; }
            public int Y { get; set; }

            public bool Equals(c_point point)
            {
                return X == point.X && Y == point.Y;
            }

            public c_point Plus(c_point point)
            {
                return new c_point(X + point.X, Y + point.Y);
            }
        }

        internal struct c_line_segment
        {
            public c_line_segment(string input)
            {
                string[] inputs = input.Split(" -> ");
                Start = new c_point(inputs[0]);
                End = new c_point(inputs[1]);
            }

            public c_point Start { get; set; }
            public c_point End { get; set; }

            public bool covers(c_point point)
            {
                Debug.Assert(!diagonal());

                return ((Start.X <= point.X && point.X <= End.X) || (End.X <= point.X && point.X <= Start.X))
                    && ((Start.Y <= point.Y && point.Y <= End.Y) || (End.Y <= point.Y && point.Y <= Start.Y));
            }

            public bool diagonal()
            {
                return Start.X != End.X && Start.Y != End.Y;
            }
        }

        public static void Part_1(string input)
        {
            c_input_reader input_reader = new c_input_reader(input);

            c_point min = new c_point(int.MaxValue, int.MaxValue);
            c_point max = new c_point(int.MinValue, int.MinValue);
            List<c_line_segment> line_segments = new List<c_line_segment>();

            while (input_reader.has_more_lines())
            {
                c_line_segment line_segment = new c_line_segment(input_reader.read_line());

                min.X = Math.Min(min.X, Math.Min(line_segment.Start.X, line_segment.End.X));
                min.Y = Math.Min(min.Y, Math.Min(line_segment.Start.Y, line_segment.End.Y));

                max.X = Math.Max(max.X, Math.Max(line_segment.Start.X, line_segment.End.X));
                max.Y = Math.Max(max.Y, Math.Max(line_segment.Start.Y, line_segment.End.Y));

                if (!line_segment.diagonal())
                {
                    line_segments.Add(line_segment);
                }
            }

            int points_with_overlap = 0;

            c_point point = new c_point();
            for (point.X = min.X; point.X < max.X; point.X++)
            {
                for (point.Y = min.Y; point.Y < max.Y; point.Y++)
                {
                    IEnumerable<c_line_segment> line_segments_covering =
                        line_segments.Where(line_segment => line_segment.covers(point));

                    int lines_covering = line_segments_covering.Count();

                    if (lines_covering > 1)
                    {
                        points_with_overlap++;
                    }
                }
            }

            Console.WriteLine("Points with overlap: " + points_with_overlap);
        }

        public static void Part_2(string input)
        {
            c_input_reader input_reader = new c_input_reader(input);

            c_point min = new c_point(int.MaxValue, int.MaxValue);
            c_point max = new c_point(int.MinValue, int.MinValue);
            List<c_line_segment> line_segments = new List<c_line_segment>();

            while (input_reader.has_more_lines())
            {
                c_line_segment line_segment = new c_line_segment(input_reader.read_line());

                min.X = Math.Min(min.X, Math.Min(line_segment.Start.X, line_segment.End.X));
                min.Y = Math.Min(min.Y, Math.Min(line_segment.Start.Y, line_segment.End.Y));

                max.X = Math.Max(max.X, Math.Max(line_segment.Start.X, line_segment.End.X));
                max.Y = Math.Max(max.Y, Math.Max(line_segment.Start.Y, line_segment.End.Y));

                line_segments.Add(line_segment);
            }

            int points_with_overlap = 0;

            int[,] overlap_map = new int[max.X + 1, max.Y + 1];

            foreach (c_line_segment line_segment in line_segments)
            {
                c_point increment = new c_point(0, 0);

                if (line_segment.Start.X < line_segment.End.X)
                {
                    increment.X = 1;
                }
                else if (line_segment.Start.X > line_segment.End.X)
                {
                    increment.X = -1;
                }

                if (line_segment.Start.Y < line_segment.End.Y)
                {
                    increment.Y = 1;
                }
                else if (line_segment.Start.Y > line_segment.End.Y)
                {
                    increment.Y = -1;
                }

                for (c_point point = line_segment.Start; !point.Equals(line_segment.End); point = point.Plus(increment))
                {
                    overlap_map[point.X, point.Y]++;
                }

                overlap_map[line_segment.End.X, line_segment.End.Y]++;
            }

            // Console.WriteLine();

            for (int y = min.Y; y <= max.Y; y++)
            {
                for (int x = min.X; x <= max.X; x++)
                {
                    if (overlap_map[x, y] > 1)
                    {
                        points_with_overlap++;
                    }

                    /* LOL this is huge

                    if (overlap_map[x, y] == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;

                        Console.Write(".");
                    }
                    else
                    {
                        if (overlap_map[x, y] == 1)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                        }

                        Console.Write(overlap_map[x, y].ToString());
                    }*/
                }

                // Console.WriteLine();
            }

            Console.ResetColor();

            Console.WriteLine();
            Console.WriteLine("Points with overlap: " + points_with_overlap);
            Console.WriteLine();
        }
    }
}
