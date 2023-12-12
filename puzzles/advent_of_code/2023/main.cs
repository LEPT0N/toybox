using System;
using System.Diagnostics;
using advent_of_code_2023.days;

namespace advent_of_code_2023
{
	internal class main
	{
		static void Main(string[] args)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			int day = Int32.Parse(args[0]);
			int part = Int32.Parse(args[1]);
			string input = args[2];

			bool pretty = (args.Length >= 4 && args[3] == "pretty");

			switch (day, part)
			{
				case (1, 1): day_01.part_1(input, pretty); break;
				case (1, 2): day_01.part_2(input, pretty); break;
				case (2, 1): day_02.part_1(input, pretty); break;
				case (2, 2): day_02.part_2(input, pretty); break;
				case (3, 1): day_03.part_1(input, pretty); break;
				case (3, 2): day_03.part_2(input, pretty); break;
				case (4, 1): day_04.part_1(input, pretty); break;
				case (4, 2): day_04.part_2(input, pretty); break;
				case (5, 1): day_05.part_1(input, pretty); break;
				case (5, 2): day_05.part_2(input, pretty); break;
				case (6, 1): day_06.part_1(input, pretty); break;
				case (6, 2): day_06.part_2(input, pretty); break;
				case (7, 1): day_07.part_1(input, pretty); break;
				case (7, 2): day_07.part_2(input, pretty); break;
				case (8, 1): day_08.part_1(input, pretty); break;
				case (8, 2): day_08.part_2(input, pretty); break;
				case (9, 1): day_09.part_1(input, pretty); break;
				case (9, 2): day_09.part_2(input, pretty); break;
				case (10, 1): day_10.part_1(input, pretty); break;
				case (10, 2): day_10.part_2(input, pretty); break;
				case (11, 1): day_11.part_1(input, pretty); break;
				case (11, 2): day_11.part_2(input, pretty); break;

				default:
					Console.WriteLine("Unexpected day " + args[0]);
					break;
			}

			stopwatch.Stop();

			Console.WriteLine();
			Console.ForegroundColor = ConsoleColor.Blue;
			Console.WriteLine("Time taken = {0}", stopwatch.Elapsed);
			Console.ResetColor();
		}
	}
}