using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using advent_of_code_2024.days;

namespace advent_of_code_2024
{
    internal class main
    {
        static void Main(string[] args)
        {
            int day = Int32.Parse(args[0]);
            int part = Int32.Parse(args[1]);

            string input = args[2];
            input = input.Replace("{day}", $"{day:00}");

            bool pretty = (args.Length >= 4 && args[3] == "pretty");

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
