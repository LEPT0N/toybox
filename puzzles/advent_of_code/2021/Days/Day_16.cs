using System;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

/*
 * input = 1 packet
 * 
 * packet:
 *  
 *  can contain trailing zeroes. ignore them.
 *  
 *  packet header:
 *      3 bits = version
 *      3 bits = type id
 *  
 *  type id == 4 -> literal value
 *      encode a single binary number
 *      padded with leading zeroes until bits are multiple of four
 *      broken up into groups of four bits
 *      each group prefixed by a 1 bit except the last, which is prefixed by a 0 bit. (so each group is 5 bits)
 *      
 *      ex:
 *          input hex: D2FE28
 *          to binary: 110 100 1 0111 1 1110 0 0101 000
 *                     VVV TTT A AAAA B BBBB C CCCC 000
 *                  VVV == 110 -> version = 6
 *                  TTT == 110 -> id = 4 (literal value)
 *                  AAAAA = 1 0111
 *                  BBBBB = 1 1110
 *                  CCCCC = 0 0101
 *                  
 *              0b 0111 1110 1000 = 0n 2021
 * 
 * type id != 4 -> operator
 *      1 bit = length type id
 *          length type id == 0 -> next 15 bits are a number: total length in bits of sub packets
 *          length type id == 1 -> next 11 bits are a number: total number of sub packets
 * 
 *      ex: 
 *          input hex: 38006F45291200
 *          to binary: 001 110 0 000000000011011 11010001010 0101001000100100 0000000
 *                     VVV TTT I LLLLLLLLLLLLLLL AAAAAAAAAAA BBBBBBBBBBBBBBBB 0000000
 *              version = 1
 *              type = 6
 *              length type = 0
 *              length = 27
 *              A = 110 100 0 1010
 *                  VVV TTT A AAAA
 *                  version = 6
 *                  type = 4
 *                  value = 0b 1010 = 0n 10
 *              B = 010 100 1 0001 0 0100 0000000
 *                  VVV TTT A AAAA A AAAA
 *                  version = 2
 *                  type = 4
 *                  value = 0b 0001 0100 = 0n 20
 *
 */

namespace advent_of_code_2021.Days
{
    internal class Day_16
    {
        internal class c_bit_reader
        {
            private BitArray bit_array;
            private int index;

            public int count { get { return bit_array.Count; } }

            public c_bit_reader(string input)
            {
                bit_array = new BitArray(input.Length * 4, false);
                index = 0;

                int input_index = 0;
                foreach (char input_char in input)
                {
                    byte input_byte = byte.Parse("0" + input_char, NumberStyles.AllowHexSpecifier);

                    if ((input_byte & 0b1000) != 0) { bit_array.Set(input_index, true); }
                    input_index++;

                    if ((input_byte & 0b0100) != 0) { bit_array.Set(input_index, true); }
                    input_index++;

                    if ((input_byte & 0b0010) != 0) { bit_array.Set(input_index, true); }
                    input_index++;

                    if ((input_byte & 0b0001) != 0) { bit_array.Set(input_index, true); }
                    input_index++;
                }
            }

            public int bits_remaining()
            {
                return count - index;
            }

            public void discard(int bit_count, ref UInt64 total_bits_read)
            {
                total_bits_read += (UInt64)bit_count;
                index += bit_count;
            }

            public bool read_bit(ref UInt64 total_bits_read)
            {
                total_bits_read++;

                bool result = bit_array.Get(index);

                index++;

                return result;
            }

            public UInt64 read_number(int bit_count, ref UInt64 total_bits_read)
            {
                total_bits_read += (UInt64)bit_count;

                UInt64 result = 0;

                while (bit_count > 0)
                {
                    result = (result << 1);

                    if (bit_array.Get(index))
                    {
                        result = (result | 1);
                    }

                    index++;
                    bit_count--;
                }

                return result;
            }
        }

        internal interface i_packet_value
        {
            public UInt64 sum_version_numbers();
            public UInt64 calculate();
            public bool is_simple_type();
            public string to_string();
            public string to_multiline_string(string indent);
        }
        
        [DebuggerDisplay("value = {payload}", Type = "c_packet_integer_value")]
        internal class c_packet_integer_value : i_packet_value
        {
            public readonly UInt64 payload;

            public c_packet_integer_value(c_bit_reader bits, ref UInt64 total_bits_read)
            {
                payload = 0;

                bool continue_reading = true;

                while (continue_reading)
                {
                    continue_reading = bits.read_bit(ref total_bits_read);

                    payload = (payload << 4);

                    payload = (payload | bits.read_number(4, ref total_bits_read));
                }
            }

            public UInt64 sum_version_numbers()
            {
                return 0;
            }

            public UInt64 calculate()
            {
                return payload;
            }

            public bool is_simple_type()
            {
                return true;
            }

            public string to_string()
            {
                return payload.ToString();
            }

            public string to_multiline_string(string indent)
            {
                return indent + payload.ToString() + "\r\n";
            }
        }

        [DebuggerDisplay("operator {operator_type} = {sub_packets}", Type = "c_packet_operator_value")]
        internal class c_packet_operator_value : i_packet_value
        {
            private enum e_packet_operator_type
            {
                sum,
                product,
                min,
                max,
                greater_than,
                less_than,
                equal_to,
            }

            private readonly List<c_packet> sub_packets;
            private readonly e_packet_operator_type operator_type;

            public c_packet_operator_value(c_bit_reader bits, UInt64 type_id, ref UInt64 total_bits_read)
            {
                sub_packets = new List<c_packet>();

                if (bits.read_bit(ref total_bits_read))
                {
                    UInt64 sub_packet_count = bits.read_number(11, ref total_bits_read);

                    for (UInt64 i = 0; i < sub_packet_count; i++)
                    {
                        sub_packets.Add(new c_packet(bits, ref total_bits_read));
                    }
                }
                else
                {
                    UInt64 sub_packet_size = bits.read_number(15, ref total_bits_read);

                    UInt64 sub_packets_bits_read = 0;
                    while (sub_packet_size - sub_packets_bits_read >= c_packet.k_min_bit_count)
                    {
                        sub_packets.Add(new c_packet(bits, ref sub_packets_bits_read));
                    }
                    total_bits_read += sub_packets_bits_read;

                    bits.discard((int)(sub_packet_size - sub_packets_bits_read), ref total_bits_read);
                }

                switch (type_id)
                {
                    case 0: operator_type = e_packet_operator_type.sum; break;
                    case 1: operator_type = e_packet_operator_type.product; break;
                    case 2: operator_type = e_packet_operator_type.min; break;
                    case 3: operator_type = e_packet_operator_type.max; break;
                    case 5: operator_type = e_packet_operator_type.greater_than; break;
                    case 6: operator_type = e_packet_operator_type.less_than; break;
                    case 7: operator_type = e_packet_operator_type.equal_to; break;
                }
            }

            public UInt64 sum_version_numbers()
            {
                return sub_packets.Aggregate(0UL, (sum, packet) => sum + packet.sum_version_numbers());
            }

            public UInt64 calculate()
            {
                switch (operator_type)
                {
                    case e_packet_operator_type.sum:
                        return sub_packets.Aggregate(0UL, (sum, packet) => sum + packet.calculate());

                    case e_packet_operator_type.product:
                        return sub_packets.Aggregate(1UL, (product, packet) => product * packet.calculate());

                    case e_packet_operator_type.min:
                        return sub_packets.Aggregate(UInt64.MaxValue, (min, packet) => Math.Min(min, packet.calculate()));

                    case e_packet_operator_type.max:
                        return sub_packets.Aggregate(UInt64.MinValue, (max, packet) => Math.Max(max, packet.calculate()));

                    case e_packet_operator_type.greater_than:
                        return (sub_packets.First().calculate() > sub_packets.Last().calculate()) ? 1UL : 0UL;

                    case e_packet_operator_type.less_than:
                        return (sub_packets.First().calculate() < sub_packets.Last().calculate()) ? 1UL : 0UL;

                    case e_packet_operator_type.equal_to:
                        return (sub_packets.First().calculate() == sub_packets.Last().calculate()) ? 1UL : 0UL;
                }

                return int.MaxValue;
            }

            public bool is_simple_type()
            {
                return sub_packets.Count < 4 && sub_packets.All(x => x.is_simple_type());
            }

            public string to_string()
            {
                string result = "";

                switch (operator_type)
                {
                    case e_packet_operator_type.sum:
                        result += String.Format("({0})", string.Join(" + ", sub_packets.Select(packet => packet.to_string())));
                        break;

                    case e_packet_operator_type.product:
                        result += String.Format("({0})", string.Join(" * ", sub_packets.Select(packet => packet.to_string())));
                        break;

                    case e_packet_operator_type.min:
                        result += String.Format("min({0})", string.Join(", ", sub_packets.Select(packet => packet.to_string())));
                        break;

                    case e_packet_operator_type.max:
                        result += String.Format("max({0})", string.Join(", ", sub_packets.Select(packet => packet.to_string())));
                        break;

                    case e_packet_operator_type.greater_than:
                        result += String.Format("({0} > {1} ? 1 : 0)", sub_packets.First().to_string(), sub_packets.Last().to_string());
                        break;

                    case e_packet_operator_type.less_than:
                        result += String.Format("({0} < {1} ? 1 : 0)", sub_packets.First().to_string(), sub_packets.Last().to_string());
                        break;

                    case e_packet_operator_type.equal_to:
                        result += String.Format("({0} == {1} ? 1 : 0)", sub_packets.First().to_string(), sub_packets.Last().to_string());
                        break;
                }

                return result;
            }

            public string to_multiline_string(string indent)
            {
                string result = "";

                if (sub_packets.All(x => x.is_simple_type()))
                {
                    result = indent + to_string() + "\r\n";
                }
                else
                {
                    string prefix = "";
                    switch (operator_type)
                    {
                        case e_packet_operator_type.min: prefix = "min"; break;
                        case e_packet_operator_type.max: prefix = "max"; break;
                    }
                    result = indent + prefix + "(\r\n";

                    string joiner = "";

                    switch (operator_type)
                    {
                        case e_packet_operator_type.sum: joiner = "+"; break;
                        case e_packet_operator_type.product: joiner = "*"; break;
                        case e_packet_operator_type.min: joiner = ","; break;
                        case e_packet_operator_type.max: joiner = ","; break;
                        case e_packet_operator_type.greater_than: joiner = ">"; break;
                        case e_packet_operator_type.less_than: joiner = "<"; break;
                        case e_packet_operator_type.equal_to: joiner = "=="; break;
                    }

                    result += string.Join(indent + '\t' + joiner + "\r\n", sub_packets.Select(packet => packet.to_multiline_string(indent + '\t')));

                    result += indent + ")\r\n";
                }

                return result;
            }
        }

        [DebuggerDisplay("V={version} T={type_id} [{value}]", Type = "c_packet")]
        internal class c_packet
        {
            private const int k_type_integer = 0b100;

            public const int k_min_bit_count = 8;

            public readonly UInt64 version;
            public readonly UInt64 type_id;
            public readonly i_packet_value value;

            public c_packet(c_bit_reader bits, ref UInt64 total_bits_read)
            {
                version = bits.read_number(3, ref total_bits_read);
                type_id = bits.read_number(3, ref total_bits_read);

                if (type_id == k_type_integer)
                {
                    value = new c_packet_integer_value(bits, ref total_bits_read);
                }
                else
                {
                    value = new c_packet_operator_value(bits, type_id, ref total_bits_read);
                }
            }

            public UInt64 sum_version_numbers()
            {
                return version + value.sum_version_numbers();
            }

            public UInt64 calculate()
            {
                return value.calculate();
            }

            public bool is_simple_type()
            {
                return value.is_simple_type();
            }

            public string to_string()
            {
                return value.to_string();
            }

            public string to_multiline_string(string indent)
            {
                return value.to_multiline_string(indent);
            }
        }

        internal static c_bit_reader parse_input(
            string input)
        {
            c_input_reader input_reader = new c_input_reader(input);

            string input_line = input_reader.read_line();

            c_bit_reader bits = new c_bit_reader(input_line);

            return bits;
        }

        public static void Part_1(string input, bool pretty)
        {
            c_bit_reader bits = parse_input(input);

            UInt64 total_bits_read = 0;
            c_packet packet = new c_packet(bits, ref total_bits_read);

            if (pretty)
            {
                Console.WriteLine(packet.to_string());
                Console.WriteLine();
                Console.WriteLine(packet.to_multiline_string(""));
            }

            UInt64 result = packet.sum_version_numbers();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", result);
            Console.ResetColor();
        }

        public static void Part_2(string input, bool pretty)
        {
            c_bit_reader bits = parse_input(input);

            UInt64 total_bits_read = 0;
            c_packet packet = new c_packet(bits, ref total_bits_read);

            if (pretty)
            {
                Console.WriteLine(packet.to_string());
                Console.WriteLine();
                Console.WriteLine(packet.to_multiline_string(""));
            }

            UInt64 result = packet.calculate();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", result);
            Console.ResetColor();
        }
    }
}
