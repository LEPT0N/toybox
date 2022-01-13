using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace advent_of_code_2020.Days
{
    internal class Day_23
    {
        [DebuggerDisplay("{value} -> {next?.value}", Type = "c_node")]
        internal class c_node
        {
            public c_node next;
            public readonly int value;

            public c_node(int input)
            {
                value = input;
                next = null;
            }
        }

        internal static c_node parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            c_node head = null;
            c_node current = null;

            foreach (char input_char in input_reader.read_line())
            {
                c_node new_node = new c_node(input_char - '0');

                if (head == null)
                {
                    head = new_node;
                    current = new_node;
                }
                else
                {
                    current.next = new_node;
                    current = new_node;
                }
            }

            current.next = head;

            return head;
        }

        internal static void move(ref c_node current)
        {
            // Pick up three cups and remove them from the circle.
            c_node first_picked_up = current.next;
            c_node last_picked_up = first_picked_up.next.next;
            current.next = last_picked_up.next;
            last_picked_up.next = null;

            // Find the destination cup
            c_node destination = null;
            int destination_value = current.value;

            while (destination == null)
            {
                // Keep decrementing the destination value until we find a match.
                destination_value--;
                if (destination_value < 1)
                {
                    destination_value = 9;
                }

                // Loop through the entire circle to try to find a match.
                for (c_node i = current.next; i != current; i = i.next)
                {
                    if (i.value == destination_value)
                    {
                        destination = i;
                        break;
                    }
                }
            }

            // Put down the picked up cups right after the destination cup.
            last_picked_up.next = destination.next;
            destination.next = first_picked_up;

            // advance the current cup.
            current = current.next;
        }

        internal static int score(c_node current)
        {
            // Find the '1' cup
            c_node one = current;
            
            while (one.value != 1)
            {
                one = one.next;
            }

            // turn the rest of the nodes into a single number.
            int result = 0;

            for (c_node i = one.next; i != one; i = i.next)
            {
                result *= 10;
                result += i.value;
            }

            return result;
        }

        public static void Part_1(
            string input,
            bool pretty)
        {
            c_node current = parse_input(input, pretty);

            for (int i = 0; i < 100; i++)
            {
                move(ref current);
            }

            int result = score(current);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", result);
            Console.ResetColor();
        }

        public static void Part_2(
            string input,
            bool pretty)
        {
            c_node head_input = parse_input(input, pretty);

            // Construct an array cups[] where cups[n] = m means after cup n is cup m.

            int[] cups = new int[1000001];
            {
                // For cups 10 through one million, each is in order.
                for (int i = 10; i < cups.Length; i++)
                {
                    cups[i] = i + 1;
                }

                // Hook up the first ten cups.
                c_node last = head_input;
                do
                {
                    cups[last.value] = last.next.value;

                    last = last.next;
                } while (last.next != head_input);

                // Hook up the two ends of each section
                cups[last.value] = 10;
                cups[1000000] = head_input.value;
            }

            // Run the ten million moves.

            int current = head_input.value;
            int[] picked_up = new int[3];

            for (int i = 0; i < 10000000; i++)
            {
                // Find the three cups picked up
                picked_up[0] = cups[current];
                picked_up[1] = cups[picked_up[0]];
                picked_up[2] = cups[picked_up[1]];

                int after_picked_up = cups[picked_up[2]];

                // Figure out the destination cup to insert the picked up cups after.
                int destination = current;
                while (destination == current ||
                    destination == picked_up[0] ||
                    destination == picked_up[1] ||
                    destination == picked_up[2])
                {
                    destination--;
                    if (destination < 1)
                    {
                        destination = 1000000;
                    }
                }

                // Insert the picked up cups after the destination.

                int after_destination = cups[destination];

                cups[current] = after_picked_up;
                cups[destination] = picked_up[0];
                cups[picked_up[2]] = after_destination;

                // Advance to the next cup.
                current = cups[current];
            }

            UInt64 after_cup_1 = (UInt64)cups[1];
            UInt64 after_after_cup_1 = (UInt64)cups[cups[1]];
            UInt64 result = after_cup_1 * after_after_cup_1;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", result);
            Console.ResetColor();
        }
    }
}
