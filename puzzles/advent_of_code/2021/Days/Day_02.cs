using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace advent_of_code_2021.Days
{
    internal class Day_02
    {
        public static void Part_1(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            int horizontal_position = 0;
            int depth = 0;

            foreach (string line in lines)
            {
                string[] split_line = line.Split(' ');
                string direction = split_line[0];
                int amount = int.Parse(split_line[1]);

                switch (split_line[0])
                {
                    case "forward": horizontal_position += amount; break;
                    case "down": depth += amount; break;
                    case "up": depth -= amount; break;
                }
            }

            Console.WriteLine("Total forward position = " + horizontal_position);
            Console.WriteLine("Total depth = " + depth);
            Console.WriteLine("Result = " + (depth * horizontal_position));
        }
        public static void Part_2(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            int horizontal_position = 0;
            int depth = 0;
            int aim = 0;

            foreach (string line in lines)
            {
                string[] split_line = line.Split(' ');
                string direction = split_line[0];
                int amount = int.Parse(split_line[1]);

                switch (split_line[0])
                {
                    case "forward":
                        horizontal_position += amount;
                        depth += aim * amount;
                        break;

                    case "down": aim += amount; break;
                    case "up": aim -= amount; break;
                }
            }

            Console.WriteLine("Total forward position = " + horizontal_position);
            Console.WriteLine("Total depth = " + depth);
            Console.WriteLine("Total aim = " + aim);
            Console.WriteLine("Result = " + (depth * horizontal_position));
        }
    }
}
