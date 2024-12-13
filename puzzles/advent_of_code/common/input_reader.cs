using System;
using System.Collections.Generic;
using System.IO;

namespace advent_of_code_common.input_reader
{
    public class c_input_reader
    {
        public c_input_reader(string input)
        {
            if (!Path.IsPathRooted(input))
            {
                // We were given a relative path, so see if we can find it.

                // Search the current directory
                string local_path = Path.Combine(Directory.GetCurrentDirectory(), input);

                if (Path.Exists(local_path))
                {
                    input = local_path;
                }
                else
                {
                    // Search the project directory (ex: running from bin/debug/net)
                    DirectoryInfo directory = new DirectoryInfo(Directory.GetCurrentDirectory());
                    directory = directory.Parent.Parent.Parent;
                    string project_path = Path.Combine(directory.FullName, input);

                    if (Path.Exists(project_path))
                    {
                        input = project_path;
                    }
                    else
                    {
                        throw new ArgumentException("Unable to find input '{input}'");
                    }
                }
            }

            m_line_number = 0;
            m_lines = System.IO.File.ReadAllLines(input);
        }

        public string read_line()
        {
            string result = m_lines[m_line_number];

            m_line_number++;

            return result;
        }

        public string try_read_line()
        {
            return has_more_lines() ? read_line() : null;
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
