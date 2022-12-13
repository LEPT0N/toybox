using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.input_reader;

namespace advent_of_code_2022.days
{
    internal class day_13
    {
        internal enum e_ordering
        {
            less_than,
            equivalent,
            greater_than,
        }

        internal abstract class c_packet
        {
            public abstract string to_string { get; }
        }

        [DebuggerDisplay("{value}", Type = "c_value_packet")]
        internal class c_value_packet : c_packet
        {
            public readonly int value;

            public c_value_packet(int v)
            {
                value = v;
            }

            public static e_ordering get_ordering(c_value_packet left, c_value_packet right)
            {
                if (left.value < right.value)
                {
                    return e_ordering.less_than;
                }
                else if (left.value > right.value)
                {
                    return e_ordering.greater_than;
                }
                else
                {
                    return e_ordering.equivalent;
                }
            }

            public override string to_string
            {
                get
                {
                    return value.ToString();
                }
            }
        }

        [DebuggerDisplay("{to_string, nq}", Type = "c_list_packet")]
        internal class c_list_packet : c_packet
        {
            public readonly c_packet[] child_packets;

            public c_list_packet(c_packet[] c)
            {
                child_packets = c;
            }

            public c_list_packet(c_value_packet v)
            {
                child_packets = new c_packet[1] { v };
            }

            public static e_ordering get_ordering(c_list_packet left, c_list_packet right)
            {
                for (int i = 0; i < left.child_packets.Length && i < right.child_packets.Length; i++)
                {
                    e_ordering child_ordering = get_packet_ordering(left.child_packets[i], right.child_packets[i]);

                    if (child_ordering != e_ordering.equivalent)
                    {
                        return child_ordering;
                    }
                }

                if (left.child_packets.Length < right.child_packets.Length)
                {
                    return e_ordering.less_than;
                }
                else if (left.child_packets.Length > right.child_packets.Length)
                {
                    return e_ordering.greater_than;
                }
                else
                {
                    return e_ordering.equivalent;
                }
            }

            public override string to_string
            {
                get
                {
                    return string.Format("[{0}]",
                        string.Join(',', child_packets.Select(c => c.to_string)));
                }
            }
        }

        internal class c_packet_pair
        {
            private readonly int index;
            private readonly c_packet left;
            private readonly c_packet right;
            private readonly e_ordering ordering;

            public c_packet_pair(int i, c_packet l, c_packet r)
            {
                index = i;
                left = l;
                right = r;

                ordering = get_packet_ordering(left, right);
            }

            public int get_score()
            {
                return ((ordering == e_ordering.greater_than) ? 0 : index);
            }
        }

        internal static e_ordering get_packet_ordering(c_packet left, c_packet right)
        {
            if (left is c_value_packet && right is c_value_packet)
            {
                return c_value_packet.get_ordering((c_value_packet)left, (c_value_packet)right);
            }
            else
            {
                c_list_packet left_list_packet = left as c_list_packet ?? new c_list_packet(left as c_value_packet);
                c_list_packet right_list_packet = right as c_list_packet ?? new c_list_packet(right as c_value_packet);

                return c_list_packet.get_ordering(left_list_packet, right_list_packet);
            }
        }

        internal static c_packet parse_packet(string input)
        {
            if (input[0] != '[')
            {
                return new c_value_packet(int.Parse(input));
            }

            List<c_packet> child_packets = new List<c_packet>();

            int bracket_depth = 0;
            int child_input_start_index = 1;

            for (int input_index = 1; input_index < input.Length - 1; input_index++)
            {
                if (input[input_index] == '[')
                {
                    bracket_depth++;
                }
                else if (input[input_index] == ']')
                {
                    bracket_depth--;
                }
                else if (input[input_index] == ','
                    && bracket_depth == 0)
                {
                    child_packets.Add(parse_packet(input.Substring(
                        child_input_start_index,
                        input_index - child_input_start_index)));

                    child_input_start_index = input_index + 1;
                }
            }

            if (child_input_start_index < input.Length - 1)
            {
                child_packets.Add(parse_packet(input.Substring(
                    child_input_start_index,
                    input.Length - 1 - child_input_start_index)));
            }

            return new c_list_packet(child_packets.ToArray());
        }

        internal static c_packet_pair[] parse_input_1(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            List<c_packet_pair> packet_pairs = new List<c_packet_pair>();

            int index = 1;
            while (input_reader.has_more_lines())
            {
                packet_pairs.Add(new c_packet_pair(
                    index,
                    parse_packet(input_reader.read_line()),
                    parse_packet(input_reader.read_line())
                ));

                if (input_reader.has_more_lines())
                {
                    input_reader.read_line();
                }

                index++;
            }

            return packet_pairs.ToArray();
        }

        public static void part_1(
            string input,
            bool pretty)
        {
            // Build the list of packet pairs

            c_packet_pair[]  packet_pairs = parse_input_1(input, pretty);

            // Sum the total score for all packet pairs

            int[] scores = packet_pairs.Select(pair => pair.get_score()).ToArray();

            int score = scores.Sum();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", score);
            Console.ResetColor();
        }

        internal static c_packet[] parse_input_2(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            List<c_packet> packets = new List<c_packet>();

            while (input_reader.has_more_lines())
            {
                string input_line = input_reader.read_line();

                if (!string.IsNullOrEmpty(input_line))
                {
                    packets.Add(parse_packet(input_line));
                }
            }

            return packets.ToArray();
        }

        public class c_packet_comparer : IComparer<c_packet>
        {
            public int Compare(c_packet a, c_packet b)
            {
                e_ordering ordering = get_packet_ordering(a, b);

                switch (ordering)
                {
                    case e_ordering.less_than: return -1;
                    case e_ordering.equivalent: return 0;
                    case e_ordering.greater_than: return 1;

                    default:
                        throw new Exception($"Invalid ordering {ordering}");
                }
            }
        }

        public static void part_2(
            string input,
            bool pretty)
        {
            // Build the list of packets

            c_packet[] packets = parse_input_2(input, pretty);

            c_packet[] divider_packets = new string[] { "[[2]]", "[[6]]" }
                .Select(divider_input => parse_packet(divider_input)).ToArray();

            packets = packets.Concat(divider_packets).ToArray();

            // Sort them.

            Array.Sort(packets, new c_packet_comparer());

            // Multiply the 'index' of both divider packets

            int result = 1;
            for (int i = 0; i < packets.Length; i++)
            {
                if (divider_packets.Any(packet => packet == packets[i]))
                {
                    result *= (i + 1);
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", result);
            Console.ResetColor();
        }
    }
}
