using System;
using System.Collections.Generic;

namespace advent_of_code_common.input_reader
{
    public class c_input_reader
    {
        public c_input_reader(string input)
        {
            m_line_number = 0;
            m_lines = System.IO.File.ReadAllLines(input);
        }

        public string read_line()
        {
            string result = m_lines[m_line_number];

            m_line_number++;

            return result;
        }

        public string[] read_all_lines()
        {
            return m_lines;
        }

        public string peek_line()
        {
            return m_lines[m_line_number];
        }

        public bool has_more_lines()
        {
            return m_line_number < m_lines.Length;
        }

        private string[] m_lines;
        private int m_line_number;
    }
}
