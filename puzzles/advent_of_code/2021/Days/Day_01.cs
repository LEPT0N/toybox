using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace advent_of_code_2021.Days
{
    class Day_01
    {
        public static void Part_1(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            int depth_increases = 0;
            int previous_depth = Int32.MaxValue;

            foreach (string line in lines)
            {
                int depth = Int32.Parse(line);

                if (depth > previous_depth)
                {
                    depth_increases++;
                }

                // Console.WriteLine(line + "(" + (depth > previous_depth ? "increased" : "decreased") + ")");

                previous_depth = depth;
            }

            Console.WriteLine("The total number of depth increases is " + depth_increases);
        }

        public static void Part_2(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            List<int> depths = new List<int>();

            foreach(string line in lines)
            {
                depths.Add(Int32.Parse(line));
            }

            int depth_increases = 0;

            for (int i = 1; i < lines.Length - 2; i++)
            {
                int previous_depth_sum = depths[i - 1] + depths[i] + depths[i + 1];
                int depth_sum = depths[i] + depths[i + 1] + depths[i + 2];

                if (depth_sum > previous_depth_sum)
                {
                    depth_increases++;
                }

                //Console.WriteLine(depth_sum + "(" + (depth_sum > previous_depth_sum ? "increased" : "decreased") + ")" +
                //    " previous = [" + depths[i - 1] + "+" + depths[i] + "+" + depths[i + 1] + "]" +
                //    " current = [" + depths[i] + "+" + depths[i + 1] + "+" + depths[i + 2] + "]");

            }

            Console.WriteLine("The total number of depth increases is " + depth_increases);
        }
    }
}
