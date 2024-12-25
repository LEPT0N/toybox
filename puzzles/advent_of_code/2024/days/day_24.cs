using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.input_reader;

namespace advent_of_code_2024.days
{
    internal class day_24
    {
        internal abstract class c_node(string n, string t)
        {
            public readonly string name = n;
            public readonly string type = t;

            public string nickname = n;

            public bool? value = null;

            public List<c_node> outputs = [];

            protected abstract void replace_input(string name, c_node other);

            protected void add_output(c_node output)
            {
                outputs.Add(output);
            }

            protected abstract void try_compute_value();

            public void try_compute()
            {
                if (!value.HasValue)
                {
                    try_compute_value();

                    if (value.HasValue)
                    {
                        foreach (c_node output in outputs)
                        {
                            output.try_compute();
                        }
                    }
                }
            }

            public void replace_with(c_node other)
            {
                foreach (c_node output in this.outputs)
                {
                    output.replace_input(this.name, other);
                }

                other.outputs = this.outputs;
                this.outputs = [];
            }
        }

        [DebuggerDisplay("{type} {name}: {value}", Type = "c_const_node")]
        internal class c_const_node(string n, bool v) : c_node(n, "const")
        {
            readonly private bool computed_value = v;

            protected override void try_compute_value()
            {
                value = computed_value;
            }

            protected override void replace_input(string name, c_node other)
            {
                throw new Exception("A const node should have never been a output");
            }
        }

        [DebuggerDisplay("{type} {name}: ???", Type = "c_unknown_node")]
        internal class c_unknown_node(string n) : c_node(n, "unknown")
        {
            protected override void try_compute_value()
            {
                return;
            }

            protected override void replace_input(string name, c_node other)
            {
                throw new Exception("A const node should have never been a output");
            }
        }

        internal abstract class c_two_input_node : c_node
        {
            public c_node input_a { get; private set; }
            public c_node input_b { get; private set; }

            public c_two_input_node(string n, string t, c_node a, c_node b) : base(n, t)
            {
                input_a = a;
                input_a.outputs.Add(this);

                input_b = b;
                input_b.outputs.Add(this);
            }

            protected abstract bool compute_value(
                bool input_a_value,
                bool input_b_value);

            protected override void try_compute_value()
            {
                if (input_a.value.HasValue && input_b.value.HasValue)
                {
                    value = compute_value(input_a.value.Value, input_b.value.Value);
                }
            }

            protected override void replace_input(string name, c_node other)
            {
                if (input_a.name.Equals(name))
                {
                    input_a = other;
                }

                if (input_b.name.Equals(name))
                {
                    input_b = other;
                }
            }
        }

        [DebuggerDisplay("{type} {name}/{nickname}: [{input_a.nickname} = {input_a.value}] AND [{input_b.nickname} = {input_b.value}] = {value}", Type = "c_and_node")]
        internal class c_and_node(string n, c_node a, c_node b) : c_two_input_node(n, "AND", a, b)
        {
            protected override bool compute_value(
                bool input_a_value,
                bool input_b_value)
            {
                return input_a_value && input_b_value;
            }
        }

        [DebuggerDisplay("{type} {name}/{nickname}: [{input_a.nickname} = {input_a.value}] OR [{input_b.nickname} = {input_b.value}] = {value}", Type = "c_or_node")]
        internal class c_or_node(string n, c_node a, c_node b) : c_two_input_node(n, "OR", a, b)
        {
            protected override bool compute_value(
                bool input_a_value,
                bool input_b_value)
            {
                return input_a_value || input_b_value;
            }
        }

        [DebuggerDisplay("{type} {name}/{nickname}: [{input_a.nickname} = {input_a.value}] XOR [{input_b.nickname} = {input_b.value}] = {value}", Type = "c_xor_node")]
        internal class c_xor_node(string n, c_node a, c_node b) : c_two_input_node(n, "XOR", a, b)
        {
            protected override bool compute_value(
                bool input_a_value,
                bool input_b_value)
            {
                return input_a_value ^ input_b_value;
            }
        }

        internal static c_node[] parse_input(
            in c_input_reader input_reader)
        {
            Dictionary<string, c_node> nodes = [];

            // Read the const nodes.

            while (!string.IsNullOrEmpty(input_reader.peek_line()))
            {
                string[] input_line = input_reader.read_line().Split(": ");

                string name = input_line[0];
                bool value = (input_line[1] != "0");

                nodes[name] = new c_const_node(name, value);
            }

            input_reader.read_line();

            // Read the two-input nodes.

            while (input_reader.has_more_lines())
            {
                string[] input_line = input_reader.read_line().Split(" -> ");

                string node_name = input_line[1];

                string[] inputs = input_line[0].Split(" ");

                string input_a_name = inputs[0];
                if (!nodes.TryGetValue(input_a_name, out c_node input_a))
                {
                    input_a = new c_unknown_node(input_a_name);
                    nodes[input_a_name] = input_a;
                }

                string input_b_name = inputs[2];
                if (!nodes.TryGetValue(input_b_name, out c_node input_b))
                {
                    input_b = new c_unknown_node(input_b_name);
                    nodes[input_b_name] = input_b;
                }

                c_node node = inputs[1] switch
                {
                    "AND" => new c_and_node(node_name, input_a, input_b),
                    "OR" => new c_or_node(node_name, input_a, input_b),
                    "XOR" => new c_xor_node(node_name, input_a, input_b),
                    _ => throw new Exception($"Unexpected operator '{inputs[1]}'"),
                };

                // If we had a previous node at this position, replace it.
                if (nodes.TryGetValue(node_name, out c_node pre_existing_node))
                {
                    if (pre_existing_node is c_unknown_node)
                    {
                        pre_existing_node.replace_with(node);
                    }
                    else
                    {
                        throw new Exception($"Duplicate node '{node_name}'");
                    }
                }

                nodes[node_name] = node;
            }

            return [.. nodes.Values];
        }

        public static void part_1(
            c_input_reader input_reader,
            bool pretty)
        {
            c_node[] nodes = parse_input(input_reader);

            if (pretty)
            {
                Console.WriteLine("Calculating...");
            }

            foreach (c_node node in nodes)
            {
                if (node is c_const_node)
                {
                    node.try_compute();
                }
                else if (node is c_unknown_node)
                {
                    throw new Exception($"An unknown node remained in the circuit: '{node.name}'");
                }
            }

            Int64 result = nodes
                .Where(n => n.name[0] == 'z')
                .OrderBy(n => int.Parse(n.name[1..]))
                .Select((n, i) => n.value.Value ? (1L << i) : 0L) // technically not correct but there are no gaps in z so it works.
                .Sum();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }

        public static void part_2(
            c_input_reader input_reader,
            bool pretty)
        {
            // Build the node graph.

            c_node[] nodes = parse_input(input_reader);

            // Find some suspicious nodes

            HashSet<c_node> suspicious_nodes = [];

            // Find suspicious 'z' nodes.

            c_two_input_node[] z_nodes = nodes
                .Where(n => n.name[0] == 'z')
                .OrderBy(n => int.Parse(n.name[1..]))
                .Select(n => (c_two_input_node)n)
                .ToArray();

            for (int i = 1; i < z_nodes.Length - 1; i++)
            {
                c_two_input_node node = z_nodes[i];

                if (node is not c_xor_node)
                {
                    if (pretty)
                    {
                        Console.WriteLine($"{node.name} is an output but not a 'XOR'.");
                    }
                    suspicious_nodes.Add(node);
                }
                else if (node.input_a is not c_xor_node && node.input_b is not c_xor_node)
                {
                    if (pretty)
                    {
                        Console.WriteLine($"{node.name} is an output but doesn't have a 'XOR' as input.");
                    }
                    suspicious_nodes.Add(node);
                }
            }

            // There were a lot of ways that I was able to tell there were problems, but I couldn't figure out a way to tell which node was the problem in a group.
            //
            // Instead, I was able to tell there was something fishy with z05, z11, z23, and z38, so if I just look at each group and try to fill in a full adder with the node names:
            // https://www.101computing.net/binary-additions-using-logic-gates/
            // my drawing made it obvious where the swap was needed each time.

            // 05 -> tst and z05
            c_node node_05_x = nodes.First(n => n.name == "x05");
            c_node node_05_y = nodes.First(n => n.name == "y05");
            c_node node_05_z = nodes.First(n => n.name == "z05");

            // 11 -> sps and z11
            c_node node_11_x = nodes.First(n => n.name == "x11");
            c_node node_11_y = nodes.First(n => n.name == "y11");
            c_node node_11_z = nodes.First(n => n.name == "z11");

            // 23 -> frt and z23
            c_node node_23_x = nodes.First(n => n.name == "x23");
            c_node node_23_y = nodes.First(n => n.name == "y23");
            c_node node_23_z = nodes.First(n => n.name == "z23");

            // 38 -> pmd and cgh
            c_node node_38_x = nodes.First(n => n.name == "x38");
            c_node node_38_y = nodes.First(n => n.name == "y38");
            c_node node_38_z = nodes.First(n => n.name == "z38");

            c_node node_pmd = nodes.First(n => n.name == "pmd");

            // My answer therefore is: cgh,frt,pmd,sps,tst,z05,z11,z23

            string result1 = string.Join(",", suspicious_nodes
                .Select(n => n.name)
                .Order());

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result1}");
            Console.ResetColor();
        }
    }
}
