using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using advent_of_code_common.input_reader;

namespace advent_of_code_2024.days
{
    internal class day_24
    {
        internal abstract class c_node(string n)
        {
            public readonly string name = n;

            public string nickname = n;

            public bool? value = null;

            public List<c_node> children = [];

            protected abstract void replace_input(string name, c_node other);

            protected void add_child(c_node child)
            {
                children.Add(child);
            }

            protected abstract void try_compute_value();

            public void try_compute()
            {
                if (!value.HasValue)
                {
                    try_compute_value();

                    if (value.HasValue)
                    {
                        foreach (c_node child in children)
                        {
                            child.try_compute();
                        }
                    }
                }
            }

            public void replace_with(c_node other)
            {
                foreach (c_node child in this.children)
                {
                    child.replace_input(this.name, other);
                }

                other.children = this.children;
                this.children = [];
            }
        }

        [DebuggerDisplay("{name}: {value}", Type = "c_const_node")]
        internal class c_const_node(string n, bool v) : c_node(n)
        {
            private bool computed_value = v;

            protected override void try_compute_value()
            {
                value = computed_value;
            }

            protected override void replace_input(string name, c_node other)
            {
                throw new Exception("A const node should have never been a child");
            }
        }

        [DebuggerDisplay("{name}: ???", Type = "c_unknown_node")]
        internal class c_unknown_node(string n) : c_node(n)
        {
            protected override void try_compute_value()
            {
                return;
            }

            protected override void replace_input(string name, c_node other)
            {
                throw new Exception("A const node should have never been a child");
            }
        }

        internal abstract class c_two_input_node : c_node
        {
            public c_node input_a { get; private set; }
            public c_node input_b { get; private set; }

            public c_two_input_node(string n, c_node a, c_node b) : base(n)
            {
                input_a = a;
                input_a.children.Add(this);

                input_b = b;
                input_b.children.Add(this);
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

        [DebuggerDisplay("{nickname}: [{input_a.nickname} = {input_a.value}] AND [{input_b.nickname} = {input_b.value}] = {value}", Type = "c_and_node")]
        internal class c_and_node(string n, c_node a, c_node b) : c_two_input_node(n, a, b)
        {
            protected override bool compute_value(
                bool input_a_value,
                bool input_b_value)
            {
                return input_a_value && input_b_value;
            }
        }

        [DebuggerDisplay("{nickname}: [{input_a.nickname} = {input_a.value}] OR [{input_b.nickname} = {input_b.value}] = {value}", Type = "c_and_node")]
        internal class c_or_node(string n, c_node a, c_node b) : c_two_input_node(n, a, b)
        {
            protected override bool compute_value(
                bool input_a_value,
                bool input_b_value)
            {
                return input_a_value || input_b_value;
            }
        }

        [DebuggerDisplay("{nickname}: [{input_a.nickname} = {input_a.value}] XOR [{input_b.nickname} = {input_b.value}] = {value}", Type = "c_and_node")]
        internal class c_xor_node(string n, c_node a, c_node b) : c_two_input_node(n, a, b)
        {
            protected override bool compute_value(
                bool input_a_value,
                bool input_b_value)
            {
                return input_a_value ^ input_b_value;
            }
        }

        internal static c_node[] parse_input(
            in c_input_reader input_reader,
            in bool pretty)
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

                c_node node;
                switch (inputs[1])
                {
                    case "AND":
                        node = new c_and_node(node_name, input_a, input_b);
                        break;

                    case "OR":
                        node = new c_or_node(node_name, input_a, input_b);
                        break;

                    case "XOR":
                        node = new c_xor_node(node_name, input_a, input_b);
                        break;

                    default:
                        throw new Exception($"Unexpected operator '{inputs[1]}'");
                }

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
            c_node[] nodes = parse_input(input_reader, pretty);

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

            //c_node[] z_nodes = nodes.Where(n => n.name[0] == 'z').ToArray();

            //c_node[] ordered_z_nodes = z_nodes.OrderBy(n => int.Parse(n.name.Substring(1))).ToArray();

            //Int64[] bits = ordered_z_nodes.Select(n => n.value.Value ? (1L << n.id.Value) : 0L).ToArray();

            //Int64 result = bits.Sum();

            Int64 result = nodes
                .Where(n => n.name[0] == 'z')
                .OrderBy(n => int.Parse(n.name.Substring(1)))
                .Select((n, i) => n.value.Value ? (1L << i) : 0L) // technically not correct but there are no gaps in z so it works.
                .Sum();

            // Wrong: too low: 766293566
            //  =      2    d    a    c    b    6    3    e
            //  =   0010 1101 1010 1100 1011 0110 0011 1110

            /*

            0   0000
            1   0001
            2   0010
            3   0011
            4   0100
            5   0101
            6   0110
            7   0111
            8   1000
            9   1001
            a   1010
            b   1011
            c   1100
            d   1101
            e   1110
            f   1111

            */

            nodes[0].nickname = "DELETEME";

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }

        public static void part_2(
            c_input_reader input_reader,
            bool pretty)
        {
            c_node[] nodes = parse_input(input_reader, pretty);

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

            List<c_node> bad_nodes = [];

            c_node[] x_nodes = nodes
                .Where(n => n.name[0] == 'x')
                .OrderBy(n => int.Parse(n.name.Substring(1)))
                .ToArray();

            foreach (c_node node in x_nodes)
            {
                if (node.children.Count != 2)
                {
                    Console.WriteLine($"{node.name} is wrong.");
                    bad_nodes.Add(node);
                }
            }

            c_node[] y_nodes = nodes
                .Where(n => n.name[0] == 'y')
                .OrderBy(n => int.Parse(n.name.Substring(1)))
                .ToArray();

            foreach (c_node node in y_nodes)
            {
                if (node.children.Count != 2)
                {
                    Console.WriteLine($"{node.name} is wrong.");
                    bad_nodes.Add(node);
                }
            }

            c_xor_node[] xy_sum_nodes = x_nodes
                .Select(x => x.children[0] is c_xor_node ? x.children[0] : x.children[1])
                .Select(n => (c_xor_node)n)
                .ToArray();

            for (int i = 1; i < xy_sum_nodes.Length; i++)
            {
                c_xor_node node = xy_sum_nodes[i];

                node.nickname = $"xy{i:00}_sum";

                if ((node.input_a.name != $"x{i:00}" && node.input_a.name != $"y{i:00}") ||
                    (node.input_b.name != $"x{i:00}" && node.input_b.name != $"y{i:00}"))
                {
                    Console.WriteLine($"{xy_sum_nodes[i].name} is wrong.");
                    bad_nodes.Add(node);
                }
            }

            c_and_node[] xy_carry_nodes = x_nodes
                .Select(x => x.children[0] is c_and_node ? x.children[0] : x.children[1])
                .Select(n => (c_and_node)n)
                .ToArray();

            for (int i = 0; i < xy_carry_nodes.Length; i++)
            {
                c_and_node node = xy_carry_nodes[i];

                node.nickname = $"xy{i:00}_carry";

                if (i > 0)
                {
                    if ((node.input_a.name != $"x{i:00}" && node.input_a.name != $"y{i:00}") ||
                        (node.input_b.name != $"x{i:00}" && node.input_b.name != $"y{i:00}"))
                    {
                        Console.WriteLine($"{node.name} is wrong.");
                        bad_nodes.Add(node);
                    }
                }
            }

            c_or_node[] c_out_nodes = nodes
                .Where(n => n is c_or_node)
                .Select(n => (c_or_node)n)
                .ToArray();

            for (int i = 0; i < c_out_nodes.Length; i++)
            {
                c_or_node node = c_out_nodes[i];

                if (!(node.input_a is c_and_node) ||
                    !(node.input_b is c_and_node))
                {
                    Console.WriteLine($"{node.name} is an 'OR' node without two 'AND' inputs.");
                    bad_nodes.Add(node);
                }
                else if (node.input_a.nickname.EndsWith("_carry"))
                {
                    int number = int.Parse(node.input_a.nickname.Substring(2, 2));

                    node.nickname = $"c{number:00}_out";
                }
                else if (node.input_b.nickname.EndsWith("_carry"))
                {
                    int number = int.Parse(node.input_b.nickname.Substring(2, 2));

                    node.nickname = $"c{number:00}_out";
                }
                else
                {
                    Console.WriteLine($"{node.name} is an 'OR' node that doesn't connect up one of the xy##_carry inputs.");
                    bad_nodes.Add(node);
                }
            }

            c_and_node[] carry_combo_nodes = nodes
                .Where(n => n is c_and_node)
                .Select(n => (c_and_node)n)
                .Where(n => !xy_carry_nodes.Contains(n))
                .ToArray();

            for (int i = 0; i < carry_combo_nodes.Length; i++)
            {
                c_and_node node = carry_combo_nodes[i];

                /*if (!c_out_nodes.Contains(node.input_a) && !c_out_nodes.Contains(node.input_b))
                {
                    Console.WriteLine($"{node.name} is an 'AND' node that's not hooked up to an x## and y##, but also not hooked up to a c##_out.");
                    bad_nodes.Add(node);
                }
                else */
                if (!xy_sum_nodes.Contains(node.input_a) && !xy_sum_nodes.Contains(node.input_b))
                {
                    Console.WriteLine($"{node.name} is an 'AND' node that's not hooked up to an x## and y##, but also not hooked up to an xy##_sum.");
                    bad_nodes.Add(node);
                }
            }

            c_two_input_node[] z_nodes = nodes
                .Where(n => n.name[0] == 'z')
                .OrderBy(n => int.Parse(n.name.Substring(1)))
                .Select(n => (c_two_input_node)n)
                .ToArray();

            for (int i = 1; i < z_nodes.Length - 1; i++)
            {
                c_two_input_node node = z_nodes[i];

                if (!(node is c_xor_node))
                {
                    Console.WriteLine($"{node.name} is an output but not a 'XOR'.");
                    bad_nodes.Add(node);
                }
                else if (!xy_sum_nodes.Contains(node.input_a) && !xy_sum_nodes.Contains(node.input_b))
                {
                    Console.WriteLine($"{node.name} is an output but is not hooked up to an xy##_sum.");
                    bad_nodes.Add(node);
                }

                //if (!(z_nodes[i] is c_xor_node) ||
                //    (z_nodes[i].input_a.nickname != $"xy{i:00}_sum" && z_nodes[i].input_b.nickname != $"xy{i:00}_sum")
                //    )// || (z_nodes[i].input_a.nickname != $"c{i - 1:00}_out" && z_nodes[i].input_b.nickname != $"c{i - 1:00}_out"))

                ////(z_nodes[i].input_a.nickname != $"xy{i - 1:00}_carry" && z_nodes[i].input_a.nickname != $"xy{i:00}_sum") ||
                ////(z_nodes[i].input_b.nickname != $"xy{i - 1:00}_carry" && z_nodes[i].input_b.nickname != $"xy{i:00}_sum")
                //{
                //    Console.WriteLine($"{z_nodes[i].name} is wrong.");
                //}
            }


            /*
             *      c0 = x0 AND y0
             *      cn = (cn-1 AND (xn XOR yn)) OR (xn AND yn)
             * 
             *      z0 = x0 XOR y0
             *      z0-z44 = cn-1 XOR (xn XOR yn)
             *      z45 = the only OR
             *      
             *      z05, z11, and z23 are bad.
             *      
             *      z38 is suspicious?
             */

            /*
             * every xn and yn should combine in a XOR and an AND
             * 
             * 
             */

            string result = string.Join(",", bad_nodes
                .Select(n => n.name)
                .Order());

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();

            // NOT correct: fsb,jkm,jkv,rdj,z05,z11,z23,z38
        }
    }
}
