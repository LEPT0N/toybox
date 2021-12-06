using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace advent_of_code_2021.Days
{
    internal class Day_6
    {
        public static void Day_6_Worker(string input, int days)
        {
            c_input_reader input_reader = new c_input_reader(input);

            UInt64[] starting_ages = input_reader.read_line().Split(',').Select(x => UInt64.Parse(x)).ToArray();

            UInt64[] ages = new UInt64[9];

            foreach(int starting_age in starting_ages)
            {
                ages[starting_age]++;
            }

            for (int day = 1; day <= days; day++)
            {
                UInt64 spawned = ages[0];

                for (int i = 0; i < ages.Length - 1; i++)
                {
                    ages[i] = ages[i + 1];
                }

                ages[8] = spawned;
                ages[6] += spawned;
            }

            UInt64 total_alive = 0;

            for(int age = 0; age <= 8; age++)
            {
                Console.WriteLine("ages[{0}] = {1} ({2})", age, ages[age], ages[age].ToString("#,#"));

                total_alive += ages[age];
            }

            Console.WriteLine();
            Console.WriteLine("Total Alive = {0} ({1})", total_alive, total_alive.ToString("#,#"));
        }

        public static void Part_1(string input)
        {
            Day_6_Worker(input, 80);
        }

        public static void Part_2(string input)
        {
            Day_6_Worker(input, 256);

        }
    }
}
