using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace advent_of_code_2020.Days
{
    internal class Day_1
    {
        public static void Part_1(string input)
        {
            int[] entries= System.IO.File.ReadAllLines(input).Select(x => int.Parse(x)).ToArray();

            for (int i = 0; i < entries.Length - 1; i++)
            {
                for (int j = i + 1; j < entries.Length; j++)
                {
                    if (entries[i] + entries[j] == 2020)
                    {
                        Console.WriteLine("Matching entries: {0} {1}", entries[i], entries[j]);

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Answer: {0}", entries[i] * entries[j]);
                        Console.ResetColor();

                        return;
                    }
                }
            }
        }
        public static void Part_2(string input)
        {
            int[] entries = System.IO.File.ReadAllLines(input).Select(x => int.Parse(x)).ToArray();

            for (int i = 0; i < entries.Length - 2; i++)
            {
                for (int j = i + 1; j < entries.Length - 1; j++)
                {
                    for (int k = j + 1; k < entries.Length; k++)
                    {
                        if (entries[i] + entries[j] + entries[k] == 2020)
                        {
                            Console.WriteLine("Matching entries: {0} {1} {2}", entries[i], entries[j], entries[k]);

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Answer: {0}", entries[i] * entries[j] * entries[k]);
                            Console.ResetColor();

                            return;
                        }
                    }
                }
            }
        }
    }
}
