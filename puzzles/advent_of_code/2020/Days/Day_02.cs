using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace advent_of_code_2020.Days
{
    internal class Day_02
    {
        internal class c_password_rule
        {
            public c_password_rule(string input)
            {
                for (uint i = 0; i < 26; i++)
                {
                    character_count_minimums[i] = uint.MinValue;
                    character_count_maximums[i] = uint.MaxValue;
                }

                string[] parsed_input = input.Split(" ");

                string character_range_input = parsed_input[0];
                uint[] character_range = character_range_input.Split("-").Select(x => uint.Parse(x)).ToArray();

                char character = parsed_input[1][0];

                character_count_minimums[get_index(character)] = character_range[0];
                character_count_maximums[get_index(character)] = character_range[1];
            }

            public bool is_valid_password(string password)
            {
                uint[] character_counts = new uint[26];

                foreach(char character in password)
                {
                    character_counts[get_index(character)]++;
                }

                for (uint i = 0; i < 26; i++)
                {
                    if (character_counts[i] < character_count_minimums[i] ||
                        character_counts[i] > character_count_maximums[i])
                    {
                        return false;
                    }
                }

                return true;
            }

            private int get_index(char character)
            {
                return character - 'a';
            }

            private uint[] character_count_minimums = new uint[26];
            private uint[] character_count_maximums = new uint[26];
        }

        public static void Part_1(string input, bool pretty)
        {
            string[] entries = System.IO.File.ReadAllLines(input).ToArray();

            int valid_passwords = 0;

            foreach (string entry in entries)
            {
                string[] entry_parsed = entry.Split(": ");

                c_password_rule rule = new c_password_rule(entry_parsed[0]);
                string password = entry_parsed[1];

                if (rule.is_valid_password(password))
                {
                    valid_passwords++;
                }
            }

            Console.WriteLine("Total number of valid passwords is = {0}", valid_passwords);
        }

        internal class c_password_rule_2
        {
            public c_password_rule_2(string input)
            {
                string[] parsed_input = input.Split(" ");

                int[] character_indicies = parsed_input[0].Split("-").Select(x => int.Parse(x)).ToArray();
                first_index = character_indicies[0] - 1;
                second_index = character_indicies[1] - 1;

                character = parsed_input[1][0];
            }

            public bool is_valid_password(string password)
            {
                if (first_index >= password.Length ||
                    second_index >= password.Length)
                {
                    return false;
                }

                int count = 0;

                if (password[first_index] == character)
                {
                    count++;
                }

                if (password[second_index] == character)
                {
                    count++;
                }

                return count == 1;
            }

            int first_index;
            int second_index;
            char character;
        }

        public static void Part_2(string input, bool pretty)
        {
            string[] entries = System.IO.File.ReadAllLines(input).ToArray();

            int valid_passwords = 0;

            foreach (string entry in entries)
            {
                string[] entry_parsed = entry.Split(": ");

                c_password_rule_2 rule = new c_password_rule_2(entry_parsed[0]);
                string password = entry_parsed[1];

                if (rule.is_valid_password(password))
                {
                    valid_passwords++;
                }
            }

            Console.WriteLine("Total number of valid? passwords is = {0}", valid_passwords);
        }
    }
}
