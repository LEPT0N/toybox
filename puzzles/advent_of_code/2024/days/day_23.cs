using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.input_reader;

namespace advent_of_code_2024.days
{
    internal class day_23
    {
        [DebuggerDisplay("{name}", Type = "c_computer")]
        internal class c_computer(string n)
        {
            public readonly string name = n;

            public readonly HashSet<c_computer> neighbors = [];

            public void add_connection(c_computer other)
            {
                neighbors.Add(other);
            }

            public HashSet<c_computer_set> find_3_cliques()
            {
                HashSet<c_computer_set> sets = new(new c_computer_set_comparer());

                c_computer[] neighbor_array = [.. neighbors];

                for (int i = 0; i < neighbor_array.Length - 1; i++)
                {
                    for (int j = i + 1; j < neighbor_array.Length; j++)
                    {
                        if (neighbor_array[i].neighbors.Contains(neighbor_array[j]))
                        {
                            sets.Add(new c_computer_set([this, neighbor_array[i], neighbor_array[j]]));
                        }
                    }
                }

                return sets;
            }
        }

        [DebuggerDisplay("", Type = "c")]
        public class c_computer_comparer : IComparer<c_computer>
        {
            public int Compare(
                c_computer a,
                c_computer b)
            {
                return a.name.CompareTo(b.name);
            }
        }

        [DebuggerDisplay("{values.Length}", Type = "c_computer_set")]
        internal class c_computer_set
        {
            public c_computer[] values;

            public c_computer_set(
                c_computer[] input)
            {
                values = input;
                Array.Sort(values, new c_computer_comparer());
            }

            public bool contains_name_starting_with(char c)
            {
                return values.Any(computer => computer.name[0] == c);
            }

            public HashSet<c_computer_set> try_add(c_computer[] computers)
            {
                HashSet<c_computer_set> sets = new(new c_computer_set_comparer());

                foreach (c_computer computer in computers)
                {
                    if (values.All(entry => entry.neighbors.Contains(computer)))
                    {
                        List<c_computer> new_values = new(values)
                        {
                            computer,
                        };

                        sets.Add(new c_computer_set([.. new_values]));
                    }
                }

                return sets;
            }

            public string get_name()
            {
                return String.Join(",", values.Select(v => v.name));
            }
        }

        [DebuggerDisplay("", Type = "c_computer_set_3_comparer")]
        internal class c_computer_set_comparer : IEqualityComparer<c_computer_set>
        {
            public bool Equals(c_computer_set a, c_computer_set b)
            {
                if (a.values.Length != b.values.Length)
                {
                    return false;
                }

                for (int i = 0; i < a.values.Length; i++)
                {
                    if (a.values[i] != b.values[i])
                    {
                        return false;
                    }
                }

                return true;
            }

            public int GetHashCode(c_computer_set c)
            {
                HashCode hash_code = new();

                foreach (c_computer computer in c.values)
                {
                    hash_code.Add(computer);
                }

                return hash_code.ToHashCode();
            }
        }

        internal static Dictionary<string, c_computer> parse_input(
            in c_input_reader input_reader)
        {
            Dictionary<string, c_computer> computers = [];

            while (input_reader.has_more_lines())
            {
                string[] input_line = input_reader.read_line().Split("-");

                if (!computers.TryGetValue(input_line[0], out c_computer first))
                {
                    first = new c_computer(input_line[0]);

                    computers[input_line[0]] = first;
                }

                if (!computers.TryGetValue(input_line[1], out c_computer second))
                {
                    second = new c_computer(input_line[1]);

                    computers[input_line[1]] = second;
                }

                first.add_connection(second);
                second.add_connection(first);
            }

            return computers;
        }

        public static void part_1(
            c_input_reader input_reader,
            bool pretty)
        {
            Dictionary<string, c_computer> computers = parse_input(input_reader);

            HashSet<c_computer_set> cliques = new(new c_computer_set_comparer());

            foreach (c_computer computer in computers.Values)
            {
                cliques.UnionWith(computer.find_3_cliques());
            }

            int result = cliques.Count(clique => clique.contains_name_starting_with('t'));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }

        public static void part_2(
            c_input_reader input_reader,
            bool pretty)
        {
            Dictionary<string, c_computer> computers = parse_input(input_reader);

            HashSet<c_computer_set> cliques = new(new c_computer_set_comparer());

            // Use our solution from part 1 to find all sets of three interconnected computers.
            foreach (c_computer computer in computers.Values)
            {
                cliques.UnionWith(computer.find_3_cliques());
            }

            // While we have multiple sets (because our input is guaranteed to have one maximal clique)
            // loop through each old set and see if we can add any new computers to form new cliques.
            while (cliques.Count > 1)
            {
                HashSet<c_computer_set> new_cliques = new(new c_computer_set_comparer());

                foreach (c_computer_set set in cliques)
                {
                    new_cliques.UnionWith(set.try_add([.. computers.Values]));
                }

                cliques = new_cliques;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {cliques.First().get_name()}");
            Console.ResetColor();
        }
    }
}
