using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.extensions;
using advent_of_code_common.input_reader;

namespace advent_of_code_2024.days
{
    internal class day_09
    {
        [DebuggerDisplay("{id}", Type = "c_cell")]
        internal class c_cell
        {
            private const Int64 k_free_id = -1;

            private readonly Int64 id;

            public c_cell(
                Int64 i)
            {
                id = i;
            }

            public c_cell()
            {
                id = k_free_id;
            }

            public bool is_free { get { return id == k_free_id; } }

            public Int64 value {  get { return is_free ? 0 : id; } }

            public void display()
            {
                Console.Write(is_free ? "." : id);
            }
        }

        internal static c_cell[] parse_input_part1(
            in c_input_reader input_reader,
            in bool pretty)
        {
            Int64 next_file_id = 0;
            bool is_file = true;

            List<c_cell> result = new List<c_cell>();

            while (input_reader.has_more_lines())
            {
                foreach (char c in input_reader.read_line())
                {
                    int block_length = c - '0';

                    if (is_file)
                    {
                        result.AddRange(Enumerable.Repeat(next_file_id, block_length).Select(id => new c_cell(id)));

                        next_file_id++;
                    }
                    else
                    {
                        result.AddRange(Enumerable.Repeat(0, block_length).Select(id => new c_cell()));
                    }

                    is_file = !is_file;
                }
            }

            return result.ToArray();
        }


        public static int scan_forward_to_free_index(
            c_cell[] blocks,
            int start_index)
        {
            int index = start_index;
            while (index < blocks.Length && !blocks[index].is_free)
            {
                index++;
            }

            return index;
        }

        public static int scan_backwards_to_used_index(
            c_cell[] blocks,
            int start_index,
            int first_free_index)
        {
            int index = start_index;
            while (index > first_free_index && blocks[index].is_free)
            {
                index--;
            }

            return index;
        }

        public static void part_1(
            c_input_reader input_reader,
            bool pretty)
        {
            c_cell[] blocks = parse_input_part1(input_reader, pretty);

            if (pretty)
            {
                blocks.for_each(b => b.display());
                Console.WriteLine();
            }

            int first_free_index = scan_forward_to_free_index(blocks, 0);
            int last_file_index = scan_backwards_to_used_index(blocks, blocks.Length - 1, first_free_index);

            while (first_free_index < last_file_index)
            {
                blocks.swap_elements(first_free_index, last_file_index);

                first_free_index++;
                last_file_index--;

                first_free_index = scan_forward_to_free_index(blocks, first_free_index);
                last_file_index = scan_backwards_to_used_index(blocks, last_file_index, first_free_index);
            }

            Int64 result = blocks.Select((v, i) => v.value * i).Sum();

            if (pretty)
            {
                blocks.for_each(b => b.display());
                Console.WriteLine();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }


        [DebuggerDisplay("id = {id} size = {size}", Type = "c_block")]
        internal class c_block
        {
            private const Int64 k_free_id = -1;

            private readonly Int64 id;
            public Int64 size;

            public c_block(
                Int64 i,
                Int64 s)
            {
                id = i;
                size = s;
            }

            public c_block(
                Int64 s)
            {
                id = k_free_id;
                size = s;
            }

            public bool is_free { get { return id == k_free_id; } }

            public Int64 value { get { return is_free ? 0 : id * size; } }

            public Int64 get_value(ref Int64 index)
            {
                Int64 value = 0;

                if (!is_free)
                {
                    for (Int64 offset = 0; offset < size; offset++)
                    {
                        value += index * id;

                        index++;
                    }
                }
                else
                {
                    index += size;
                }

                return value;
            }

            public void display()
            {
                for (int i = 0; i < size; i++)
                {
                    Console.Write(is_free ? "." : id);
                }
            }
        }

        internal static LinkedList<c_block> parse_input_part2(
            c_input_reader input_reader,
            in bool pretty)
        {
            Int64 next_file_id = 0;
            bool is_file = true;

            LinkedList<c_block> result = new LinkedList<c_block>();

            while (input_reader.has_more_lines())
            {
                foreach (char c in input_reader.read_line())
                {
                    int block_length = c - '0';

                    if (is_file)
                    {
                        result.AddLast(new c_block(next_file_id, block_length));

                        next_file_id++;
                    }
                    else
                    {
                        result.AddLast(new c_block(block_length));
                    }

                    is_file = !is_file;
                }
            }

            return result;
        }

        public static void part_2(
            c_input_reader input_reader,
            bool pretty)
        {
            LinkedList<c_block> blocks = parse_input_part2(input_reader, pretty);

            if (pretty)
            {
                blocks.for_each(b => b.display());
                Console.WriteLine();
            }

            // Loop through all non-free nodes.
            for (LinkedListNode<c_block> last_file_node = blocks.Last;
                last_file_node != blocks.First;
                last_file_node = last_file_node.Previous)
            {
                if (!last_file_node.Value.is_free)
                {
                    // For each one, find the first free node that could fit it.
                    for (LinkedListNode<c_block> free_node = blocks.First;
                        free_node != last_file_node;
                        free_node = free_node.Next)
                    {
                        if (free_node.Value.is_free && free_node.Value.size >= last_file_node.Value.size)
                        {
                            Int64 free_node_size = free_node.Value.size;

                            c_block file_block = last_file_node.Value;

                            // mark the last file node as now being free.
                            last_file_node.Value = new c_block(file_block.size);

                            // Mark the free node as now being filled.
                            free_node.Value = file_block;

                            if (free_node_size > file_block.size)
                            {
                                // The free space was bigger than the file, so insert a new free node after it.

                                blocks.AddAfter(free_node, new c_block(free_node_size - file_block.size));
                            }
                            
                            if (last_file_node.Previous.Value.is_free)
                            {
                                // the last file node had free space in front of it, so we need to merge.

                                last_file_node.Value.size += last_file_node.Previous.Value.size;

                                blocks.Remove(last_file_node.Previous);
                            }

                            if (last_file_node != blocks.Last && last_file_node.Next.Value.is_free)
                            {
                                // the last file node had a free space after it, so we need to merge.

                                last_file_node.Value.size += last_file_node.Next.Value.size;

                                blocks.Remove(last_file_node.Next);
                            }

                            if (pretty)
                            {
                                blocks.for_each(b => b.display());
                                Console.WriteLine();
                            }

                            break;
                        }
                    }
                }
            }

            Int64 index = 0;
            Int64 result = blocks.Select(b => b.get_value(ref index)).Sum();

            if (pretty)
            {
                blocks.for_each(b => b.display());
                Console.WriteLine();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }
    }
}
