using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using advent_of_code_2024.days;
using advent_of_code_common.extensions;
using advent_of_code_common.input_reader;

namespace advent_of_code_2024
{
    internal class main
    {
        public static bool pretty { get; private set; }
        public static HashSet<string> options { get; private set; }

        static void Main(string[] args)
        {
            List<string> arguments = args.ToList();

            int day = Int32.Parse(arguments.remove_first());
            int part = Int32.Parse(arguments.remove_first());

            string input_file = arguments.remove_first();
            input_file = input_file.Replace("{day}", $"{day:00}");

            options = new HashSet<string>(args.Skip(3));

            pretty = options.Contains("pretty");

            string class_name = $"advent_of_code_2024.days.day_{day:00}";
            Type class_type = Type.GetType(class_name);

            string method_name = $"part_{part}";
            MethodInfo method = class_type?.GetMethod(method_name);

            if (method == null)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Unable to find {class_name}.{method_name}");
                Console.ResetColor();

                return;
            }

            c_input_reader input = new c_input_reader(input_file);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            method.Invoke(null, new object[] { input, pretty });

            stopwatch.Stop();

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"Time taken = {stopwatch.Elapsed}");
            Console.ResetColor();
        }
    }
}
