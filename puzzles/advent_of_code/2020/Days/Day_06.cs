using System;
using System.Linq;
using advent_of_code_common.input_reader;

namespace advent_of_code_2020.Days
{
    internal class Day_06
    {
        internal class c_group
        {
            public c_group(c_input_reader input_reader)
            {
                while (input_reader.has_more_lines())
                {
                    string line = input_reader.read_line();

                    if (string.IsNullOrEmpty(line))
                    {
                        break;
                    }

                    foreach(char character in line)
                    {
                        set_answer(character);
                    }

                    m_people++;
                }
            }

            private void set_answer(char character)
            {
                m_answers[character - 'a']++;
            }

            public int get_any_answer_count()
            {
                return m_answers.Sum(x => x != 0 ? 1 : 0);
            }

            public int get_all_answer_count()
            {
                return m_answers.Sum(x => x == m_people ? 1 : 0);
            }

            private readonly int[] m_answers = new int[26];
            private readonly int m_people;
        }

        public static void Part_1(string input, bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            int answer_count = 0;

            while (input_reader.has_more_lines())
            {
                c_group group = new c_group(input_reader);

                answer_count += group.get_any_answer_count();
            }

            Console.WriteLine("Total Answers = {0}", answer_count);
        }

        public static void Part_2(string input, bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            int answer_count = 0;

            while (input_reader.has_more_lines())
            {
                c_group group = new c_group(input_reader);

                answer_count += group.get_all_answer_count();
            }

            Console.WriteLine("Total Answers = {0}", answer_count);
        }
    }
}
