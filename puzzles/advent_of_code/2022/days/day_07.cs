using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.input_reader;

namespace advent_of_code_2022.days
{
    internal class day_07
    {
        internal enum e_command_type
        {
            cd,
            ls,
        }

        internal enum e_command_result_type
        {
            directory,
            file,
        }

        [DebuggerDisplay("{type} {name} {size}", Type = "c_command_result")]
        internal class c_command_result
        {
            public e_command_result_type type;
            public UInt64 size;
            public string name;

            public c_command_result(string input_1, string input_2)
            {
                if (input_1 == "dir")
                {
                    type = e_command_result_type.directory;
                    size = 0;
                    name = input_2;
                }
                else
                {
                    type = e_command_result_type.file;
                    size = UInt64.Parse(input_1);
                    name = input_2;
                }
            }
        }

        [DebuggerDisplay("{type} {name}", Type = "c_file_system_object")]
        internal abstract class c_file_system_object
        {
            public c_directory parent;
            public string name = "";
            public UInt64 size = 0;

            public abstract void print(string prefix = "");
        }

        [DebuggerDisplay("{name} size = {size} child count = {children.Count}", Type = "c_directory")]
        internal class c_directory : c_file_system_object
        {
            public Dictionary<string, c_file_system_object> children;

            public c_directory(string input)
            {
                parent = null;
                name = input;
                size = 0;
                children = new Dictionary<string, c_file_system_object>();
            }

            public void ensure_subdirectory(string subdirectory_name)
            {
                if (!children.ContainsKey(subdirectory_name))
                {
                    c_directory subdirectory = new c_directory(subdirectory_name);
                    subdirectory.parent = this;

                    children.Add(subdirectory_name, subdirectory);
                }
            }

            public void set_file(string child_file_name, UInt64 child_file_size)
            {
                children[child_file_name] = new c_file(child_file_name, child_file_size);
            }

            public void calculate_directory_sizes()
            {
                size = 0;

                foreach (c_file_system_object child in children.Values)
                {
                    if (child is c_directory)
                    {
                        ((c_directory)child).calculate_directory_sizes();
                    }

                    size += child.size;
                }
            }

            public override void print(string prefix = "")
            {
                Console.WriteLine($"{prefix}- {name} (dir, size={size})");

                prefix += "  ";

                foreach (c_file_system_object child in children.Values)
                {
                    child.print(prefix);
                }
            }

            public c_directory[] list_all_directories()
            {
                List<c_directory> results = new List<c_directory>();
                results.Add(this);

                foreach (c_file_system_object child in children.Values)
                {
                    if (child is c_directory)
                    {
                        results.AddRange(((c_directory)child).list_all_directories());
                    }
                }

                return results.ToArray();
            }
        }

        [DebuggerDisplay("{name} {size}", Type = "c_file")]
        internal class c_file : c_file_system_object
        {
            public c_file(string input_name, UInt64 input_size)
            {
                parent = null;
                name = input_name;
                size = input_size;
            }

            public override void print(string prefix = "")
            {
                Console.WriteLine($"{prefix}- {name} (file, size={size})");
            }
        }

        [DebuggerDisplay("{type} {argument}", Type = "c_command")]
        internal class c_command
        {
            public e_command_type type;
            public string argument;
            public c_command_result[] results;

            public c_command(string[] input)
            {
                switch (input[1])
                {
                    case "cd": type = e_command_type.cd; break;
                    case "ls": type = e_command_type.ls; break;
                    default: throw new ArgumentException($"Invalid command type {input[1]}");
                }

                if (input.Length > 2)
                {
                    argument = input[2];
                }
            }
        }

        internal static c_command[] parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            List<c_command> commands = new List<c_command>();

            while (input_reader.has_more_lines())
            {
                string[] command_line = input_reader.read_line().Split(' ');

                c_command command = new c_command(command_line);

                List<c_command_result> file_system_objects = new List<c_command_result>();

                while (input_reader.has_more_lines() && input_reader.peek_line()[0] != '$')
                {
                    string[] argument_line = input_reader.read_line().Split(' ').ToArray();

                     file_system_objects.Add(new c_command_result(argument_line[0], argument_line[1]));
                }

                command.results = file_system_objects.ToArray();

                commands.Add(command);
            }

            return commands.ToArray();
        }

        internal static c_directory generate_root(
            c_command[] commands,
            bool pretty)
        {
            c_directory root = new c_directory("/");
            c_directory cwd = root;

            foreach (c_command command in commands)
            {
                switch (command.type)
                {
                    case e_command_type.cd:
                        {
                            if (command.argument == "/")
                            {
                                cwd = root;
                            }
                            else if (command.argument == "..")
                            {
                                cwd = cwd.parent;
                            }
                            else
                            {
                                cwd.ensure_subdirectory(command.argument);

                                cwd = (c_directory)cwd.children[command.argument];
                            }
                        }
                        break;

                    case e_command_type.ls:
                        {
                            foreach (c_command_result result in command.results)
                            {
                                switch (result.type)
                                {
                                    case e_command_result_type.directory:
                                        {
                                            cwd.ensure_subdirectory(result.name);
                                        }
                                        break;

                                    case e_command_result_type.file:
                                        {
                                            cwd.set_file(result.name, result.size);
                                        }
                                        break;
                                }
                            }
                        }
                        break;
                }
            }

            root.calculate_directory_sizes();

            if (pretty)
            {
                root.print();
            }

            return root;
        }

        public static void part_1(
            string input,
            bool pretty)
        {
            c_command[] commands = parse_input(input, pretty);
            c_directory root = generate_root(commands, pretty);

            UInt64 size_result = root
                .list_all_directories()
                .Select(dir => dir.size)
                .Where(size => size <= 100000)
                .Aggregate(0UL, (a, b) => a + b);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", size_result);
            Console.ResetColor();
        }

        public static void part_2(
            string input,
            bool pretty)
        {
            c_command[] commands = parse_input(input, pretty);
            c_directory root = generate_root(commands, pretty);

            UInt64 free_space = 70000000 - root.size;
            UInt64 min_delete_size = 30000000 - free_space;

            c_directory directory_to_delete = root
                .list_all_directories()
                .Where(dir => dir.size >= min_delete_size)
                .OrderBy(dir => dir.size)
                .First();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", directory_to_delete.size);
            Console.ResetColor();
        }
    }
}
