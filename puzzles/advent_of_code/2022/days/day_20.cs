using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.extensions;
using advent_of_code_common.input_reader;

namespace advent_of_code_2022.days
{
    internal class day_20
    {
        [DebuggerDisplay("{prev?.name} <-> {name} <-> {next?.name}", Type = "c_node")]
        internal class c_node
        {
            public Int64 name { get; set; }
            public c_node next { get; private set; }
            public c_node prev { get; private set; }

            public c_node(Int64 n)
            {
                name = n;
            }

            public void link_to_next(c_node other)
            {
                this.next = other;
                other.prev = this;
            }

            public void link_to_prev(c_node other)
            {
                this.prev = other;
                other.next = this;
            }

            public void swap_by_name(Int64 list_length)
            {
                Int64 loop_count = (Math.Abs(name) % (list_length - 1));

                if (name > 0)
                {
                    for (Int64 i = 0; i < loop_count; i++)
                    {
                        c_node prev_node = this.prev;
                        c_node next_node = this.next;
                        c_node next_next_node = this.next.next;

                        prev_node.next = next_node;
                        next_node.prev = prev_node;

                        next_node.next = this;
                        this.prev = next_node;

                        this.next = next_next_node;
                        next_next_node.prev = this;
                    }
                }
                else if (name < 0)
                {
                    for (Int64 i = 0; i < loop_count; i++)
                    {
                        c_node prev_prev_node = this.prev.prev;
                        c_node prev_node = this.prev;
                        c_node next_node = this.next;

                        prev_prev_node.next = this;
                        this.prev = prev_prev_node;

                        this.next = prev_node;
                        prev_node.prev = this;

                        prev_node.next = next_node;
                        next_node.prev = prev_node;
                    }
                }
            }

            public void display()
            {
                c_node start = this;
                Console.Write($"{start.name}");

                for (c_node current = start.next; current != start; current = current.next)
                {
                    Console.Write($", {current.name}");
                }

                Console.WriteLine();
            }
        }

        [DebuggerDisplay("{name} = [{node}]", Type = "c_input")]
        internal class c_input
        {
            public Int64 name { get; private set; }
            public c_node node { get; private set; }

            public c_input(Int64 n)
            {
                name = n;
                node = new c_node(n);
            }

            public void update_name(Int64 n)
            {
                name = n;
                node.name = n;
            }
        }

        internal static c_input[] parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            List<c_input> inputs = new List<c_input>();

            while (input_reader.has_more_lines())
            {
                inputs.Add(new c_input(Int64.Parse(input_reader.read_line())));
            }

            c_input[] results = inputs.ToArray();

            for (int i = 0; i < results.Length; i++)
            {
                c_node current = results[i].node;

                int n_i = (i == results.Length - 1 ? 0 : i + 1);
                c_node next = results[n_i].node;

                int p_i = (i == 0 ? results.Length - 1 : i - 1);
                c_node prev = results[p_i].node;

                current.link_to_next(next);
                current.link_to_prev(prev);
            }

            return results;
        }

        public static void part_1(
            string input,
            bool pretty)
        {
            c_input[] list = parse_input(input, pretty);

            if (pretty)
            {
                list[0].node.display();
            }

            foreach (c_input item in list)
            {
                item.node.swap_by_name(list.Length);

                if (pretty)
                {
                    list[0].node.display();
                }
            }

            c_node node_0 = list.First(i => i.name == 0).node;

            c_node node_1000 = node_0;
            for (int i = 0; i < 1000; i++) { node_1000 = node_1000.next; }

            c_node node_2000 = node_1000;
            for (int i = 0; i < 1000; i++) { node_2000 = node_2000.next; }

            c_node node_3000 = node_2000;
            for (int i = 0; i < 1000; i++) { node_3000 = node_3000.next; }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", node_1000.name + node_2000.name + node_3000.name);
            Console.ResetColor();
        }

        internal static readonly int k_decryption_key = 811589153;

        public static void part_2(
            string input,
            bool pretty)
        {
            c_input[] list = parse_input(input, pretty);

            if (pretty)
            {
                list[0].node.display();
            }

            list.for_each(i => i.update_name(i.name * k_decryption_key));

            if (pretty)
            {
                list[0].node.display();
            }

            for (int i = 0; i < 10; i++)
            {
                foreach (c_input item in list)
                {
                    item.node.swap_by_name(list.Length);
                }

                if (pretty)
                {
                    list[0].node.display();
                }
            }

            c_node node_0 = list.First(i => i.name == 0).node;

            c_node node_1000 = node_0;
            for (int i = 0; i < 1000; i++) { node_1000 = node_1000.next; }

            c_node node_2000 = node_1000;
            for (int i = 0; i < 1000; i++) { node_2000 = node_2000.next; }

            c_node node_3000 = node_2000;
            for (int i = 0; i < 1000; i++) { node_3000 = node_3000.next; }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", node_1000.name + node_2000.name + node_3000.name);
            Console.ResetColor();
        }
    }
}
