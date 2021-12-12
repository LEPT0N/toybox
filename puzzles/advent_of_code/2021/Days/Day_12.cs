using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace advent_of_code_2021.Days
{
    internal class Day_12
    {
        internal enum e_cave_size
        {
            small,
            medium,
            big
        }

        [DebuggerDisplay("cave {name} visited {visited_count} times", Type = "c_cave")]
        internal class c_cave
        {
            public readonly string name;
            public e_cave_size size;
            public List<c_cave> neighbors { get; private set; }
            private int visited_count;

            public c_cave(string n)
            {
                name = n;
                size = (n != n.ToLower() ? e_cave_size.big: e_cave_size.small);
                neighbors = new List<c_cave>();
                visited_count = 0;
            }

            public static void make_neighbors(c_cave a, c_cave b)
            {
                a.neighbors.Add(b);
                b.neighbors.Add(a);
            }

            private bool can_visit()
            {
                switch(size)
                {
                    case e_cave_size.big: return true;
                    case e_cave_size.medium: return (visited_count < 2);
                    case e_cave_size.small: return (visited_count < 1);
                }

                return true;
            }

            private void get_paths_helper(Stack<c_cave> path_so_far, c_cave end, ref HashSet<string> paths)
            {
                visited_count++;

                path_so_far.Push(this);

                if (this == end)
                {
                    string final_path = string.Join(", ", path_so_far.ToArray().Reverse().Select(x => x.name));

                    paths.Add(final_path);
                }
                else
                {
                    foreach (c_cave neighbor in neighbors)
                    {
                        if (neighbor.can_visit())
                        {
                            neighbor.get_paths_helper(path_so_far, end, ref paths);
                        }
                    }
                }

                path_so_far.Pop();

                visited_count--;
            }

            public void get_paths(c_cave end, ref HashSet<string> paths)
            {
                get_paths_helper(
                    new Stack<c_cave>(),
                    end,
                    ref paths);
            }
        }

        public static Dictionary<string, c_cave> parse_input(string input)
        {
            c_input_reader input_reader = new c_input_reader(input);

            Dictionary<string, c_cave> caves = new Dictionary<string, c_cave>();

            while (input_reader.has_more_lines())
            {
                string input_line = input_reader.read_line();

                string[] new_cave_names = input_line.Split("-").ToArray();

                c_cave[] new_caves = new c_cave[2];

                for (int i = 0; i < new_caves.Length; i++)
                {
                    if (!caves.ContainsKey(new_cave_names[i]))
                    {
                        new_caves[i] = new c_cave(new_cave_names[i]);
                        caves.Add(new_cave_names[i], new_caves[i]);
                    }
                    else
                    {
                        new_caves[i] = caves[new_cave_names[i]];
                    }
                }

                c_cave.make_neighbors(new_caves[0], new_caves[1]);
            }

            return caves;
        }

        public static void Part_1(string input, bool pretty)
        {
            Dictionary<string, c_cave> caves = parse_input(input);

            // compute paths

            c_cave start_cave = caves["start"];
            c_cave end_cave = caves["end"];

            HashSet<string> paths = new HashSet<string>();
            
            start_cave.get_paths(end_cave, ref paths);

            // Display output

            Console.ForegroundColor = ConsoleColor.Gray;
            foreach (string path in paths)
            {
                Console.WriteLine(path);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", paths.Count);
            Console.ResetColor();
        }

        public static void Part_2(string input, bool pretty)
        {
            Dictionary<string, c_cave> caves = parse_input(input);

            // compute paths

            c_cave start_cave = caves["start"];
            c_cave end_cave = caves["end"];

            HashSet<string> paths = new HashSet<string>();

            foreach (c_cave cave in caves.Values)
            {
                if (cave.size == e_cave_size.small && cave != start_cave && cave != end_cave)
                {
                    cave.size = e_cave_size.medium;

                    start_cave.get_paths(end_cave, ref paths);

                    cave.size = e_cave_size.small;
                }
            }

            start_cave.get_paths(end_cave, ref paths);

            // Display output

            Console.ForegroundColor = ConsoleColor.Gray;
            foreach (string path in paths)
            {
                Console.WriteLine(path);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", paths.Count);
            Console.ResetColor();
        }
    }
}
