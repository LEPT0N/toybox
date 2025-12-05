using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.extensions;
using advent_of_code_common.input_reader;

namespace advent_of_code_2025.days
{
    internal class day_05
    {
        [DebuggerDisplay("{min}-{max}", Type = "c_range")]
        internal class c_range
        {
            public UInt64 min;
            public UInt64 max;

            public c_range(string input)
            {
                IEnumerable<UInt64> inputs = input.Split('-').Select(s => UInt64.Parse(s));

                min = inputs.First();
                max = inputs.Last();
            }

            public c_range(UInt64 low, UInt64 high)
            {
                min = low;
                max = high;
            }

            public UInt64 get_size()
            {
                return max - min + 1;
            }

            public bool contains(c_id id)
            {
                return min <= id.value && id.value <= max;
            }

            public bool no_intersect(c_range other)
            {
                return other.max < min || max < other.min;
            }

            public bool try_combine(c_range other)
            {
                if (no_intersect(other))
                {
                    return false;
                }
                else
                {
                    min = Math.Min(min, other.min);
                    max = Math.Max(max, other.max);

                    return true;
                }
            }
        }

        [DebuggerDisplay("{value}", Type = "c_id")]
        internal class c_id
        {
            public readonly UInt64 value;

            public c_id(string input)
            {
                value = UInt64.Parse(input);
            }
        }

        internal static (c_range[], c_id[]) parse_input(
            in c_input_reader input_reader,
            in bool pretty)
        {
            List<c_range> ranges = new List<c_range>();
            List<c_id> ids = new List<c_id>();

            while (input_reader.has_more_lines())
            {
                string line = input_reader.read_line();

                if (line.Length == 0)
                {
                    break;
                }

                ranges.Add(new c_range(line));
            }

            while (input_reader.has_more_lines())
            {
                string line = input_reader.read_line();

                ids.Add(new c_id(line));
            }

            return (ranges.ToArray(), ids.ToArray());
        }

        public static void part_1(
            c_input_reader input_reader,
            bool pretty)
        {
            (c_range[] ranges, c_id[] ids) = parse_input(input_reader, pretty);

            int result = ids.Where(id => ranges.Any(range => range.contains(id))).Count();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }

        public static void part_2(
            c_input_reader input_reader,
            bool pretty)
        {
            (c_range[] range_input, _) = parse_input(input_reader, pretty);

            LinkedList<c_range> ranges = new LinkedList<c_range>(range_input);

            for (LinkedListNode<c_range> i = ranges.First; i?.Next != null; i = i.Next)
            {
                for (LinkedListNode<c_range> j = i.Next; j != null; j = j.Next)
                {
                    // If any j's combine with i, then delete j and start over looking at all the js again.
                    if (i.Value.try_combine(j.Value))
                    {
                        ranges.Remove(j);
                        j = i;
                    }
                }
            }

            UInt64 result = ranges.Select(r => r.get_size()).sum();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }
    }
}
