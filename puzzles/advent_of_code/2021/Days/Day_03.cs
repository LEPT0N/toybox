using advent_of_code_common.extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace advent_of_code_2021.Days
{
    public static class extension_utilities
    {
        public static int count_bits(this List<int> bit_fields, int index)
        {
            int total = 0;

            foreach(int bit_field in bit_fields)
            {
                if (bit_field.test_bit(index))
                {
                    total++;
                }
            }

            return total;
        }
    }

    internal class Day_03
    {
        public static void Part_1(string input, bool pretty)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            const int k_bit_count = 12;

            int[] sums = new int[k_bit_count];

            foreach (string line in lines)
            {
                int bit_field = Convert.ToInt32(line, 2);

                for (int index = 0; index < k_bit_count; index++)
                {
                    if (bit_field.test_bit(index))
                    {
                        sums[index]++;
                    }
                }
            }

            int gamma = 0;
            int epsilon = 0;

            int halfway = lines.Length / 2;

            for (int index = 0; index < k_bit_count; index++)
            {
                if (sums[index] > halfway)
                {
                    gamma.set_bit(index);
                }
                else
                {
                    epsilon.set_bit(index);
                }
            }

            Console.WriteLine("Sums = [" + String.Join(",", sums.Reverse()) + "]");
            Console.WriteLine("gamma = " + gamma + " (" + Convert.ToString(gamma, 2) + ")");
            Console.WriteLine("epsilon = " + epsilon + " (" + Convert.ToString(epsilon, 2) + ")");
            Console.WriteLine("Result = " + (gamma * epsilon));
        }

        public static void Part_2(string input, bool pretty)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            List<int> o2_ratings = new List<int>();
            List<int> co2_ratings = new List<int>();

            foreach (string line in lines)
            {
                int bit_field = Convert.ToInt32(line, 2);

                o2_ratings.Add(bit_field);
                co2_ratings.Add(bit_field);
            }

            const int k_bit_count = 12;

            for (int index = k_bit_count - 1; o2_ratings.Count > 1 && index >= 0; index--)
            {
                bool more_bits_set;

                int matching_o2_ratings = o2_ratings.count_bits(index);
                more_bits_set = (matching_o2_ratings * 2 >= o2_ratings.Count);
                o2_ratings = o2_ratings.Where(x => (x.test_bit(index) == more_bits_set)).ToList();
            }

            for (int index = k_bit_count - 1; co2_ratings.Count > 1 && index >= 0; index--)
            {
                bool more_bits_set;

                int matching_co2_ratings = co2_ratings.count_bits(index);
                more_bits_set = (matching_co2_ratings * 2 >= co2_ratings.Count);
                co2_ratings = co2_ratings.Where(x => (x.test_bit(index) != more_bits_set)).ToList();
            }

            int o2_rating = o2_ratings.First();
            int co2_rating = co2_ratings.First();

            Console.WriteLine("o2_rating = " + o2_rating + " (" + Convert.ToString(o2_rating, 2) + ")");
            Console.WriteLine("co2_rating = " + co2_rating + " (" + Convert.ToString(co2_rating, 2) + ")");
            Console.WriteLine("Result = " + (o2_rating * co2_rating));
        }
    }
}
