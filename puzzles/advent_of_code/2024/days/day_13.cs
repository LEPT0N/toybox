using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.big_int_math;
using advent_of_code_common.input_reader;

namespace advent_of_code_2024.days
{
    internal class day_13
    {
        [DebuggerDisplay("a = {a} b = {b} p = {p}", Type = "c_machine")]
        internal class c_machine
        {
            public readonly c_big_vector button_a;
            public readonly c_big_vector button_b;
            public readonly c_big_vector prize;

            public c_machine(
                c_big_vector a,
                c_big_vector b,
                c_big_vector p)
            {
                button_a = a;
                button_b = b;
                prize = p;
            }
        }

        internal static c_machine[] parse_input(
            in c_input_reader input_reader,
            Int64 prize_translation,
            in bool pretty)
        {
            List<c_machine> machines = new List<c_machine>();

            while (input_reader.has_more_lines())
            {
                Int64[] button_a_input = input_reader
                    .read_line()
                    .Substring("Button A: X+".Length)
                    .Split(", Y+")
                    .Select(s => Int64.Parse(s))
                    .ToArray();

                Int64[] button_b_input = input_reader
                    .read_line()
                    .Substring("Button B: X+".Length)
                    .Split(", Y+")
                    .Select(s => Int64.Parse(s))
                    .ToArray();

                Int64[] prize_input = input_reader
                    .read_line()
                    .Substring("Prize: X=".Length)
                    .Split(", Y=")
                    .Select(s => Int64.Parse(s) + prize_translation)
                    .ToArray();

                input_reader.try_read_line();

                machines.Add(new c_machine(
                    new c_big_vector(button_a_input[0], button_a_input[1]),
                    new c_big_vector(button_b_input[0], button_b_input[1]),
                    new c_big_vector(prize_input[0], prize_input[1])));
            }

            return machines.ToArray();
        }

        /* Given our equations:
         * 
         * a * xa + b * xb = xt
         * 
         * a * ya + b * yb = yt
         * 
         * --------------------
         * 
         * solve for a and b in each equation
         * 
         * a = (xt - b * xb) / xa       <==
         * 
         * b = (yt - a * ya) / yb
         * 
         * --------------------
         * 
         * substitute a into the b equation.
         * 
         * b = (yt - ((xt - b * xb) / xa) * ya) / yb
         * 
         * --------------------
         * 
         * solve for b
         * 
         * b * yb = yt - (xt - b * xb) * (ya / xa)
         * 
         * b * yb = yt - (xt * ya) / xa + (b * xb * ya) / xa
         * 
         * b (yb - (xb * ya) / xa) = yt - (xt * ya) / xa
         * 
         * b = (yt - (xt * ya) / xa) / (yb - (xb * ya) / xa)
         * 
         * b = (yt * xa - xt * ya) / (yb * xa - xb * ya))       <==
         * 
         */
        internal static void part_worker(
            c_input_reader input_reader,
            Int64 prize_translation,
            bool pretty)
        {
            c_machine[] machines = parse_input(input_reader, prize_translation, pretty);

            Int64 result = 0;

            foreach (c_machine machine in machines)
            {
                c_big_vector a = machine.button_a;
                c_big_vector b = machine.button_b;
                c_big_vector p = machine.prize;

                Int64 b_presses = (p.y * a.x - p.x * a.y) / (b.y * a.x - b.x * a.y);

                Int64 a_presses = (p.x - b_presses * b.x) / a.x;

                if (a_presses * a.x + b_presses * b.x == p.x &&
                    a_presses * a.y + b_presses * b.y == p.y)
                {
                    result += 3 * a_presses + b_presses;
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }

        public static void part_1(
            c_input_reader input_reader,
            bool pretty)
        {
            part_worker(input_reader, 0, pretty);
        }

        public static void part_2(
            c_input_reader input_reader,
            bool pretty)
        {
            part_worker(input_reader, 10000000000000, pretty);
        }
    }
}
