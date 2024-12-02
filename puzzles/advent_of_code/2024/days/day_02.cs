using System;
using System.Collections.Generic;
using System.Linq;
using advent_of_code_common.input_reader;

namespace advent_of_code_2024.days
{
	internal class day_02
	{
		internal static int[][] parse_input(
			in string input,
			in bool pretty)
		{
			c_input_reader input_reader = new c_input_reader(input);

			List<int[]> reports = new List<int[]>();

			while (input_reader.has_more_lines())
			{
				reports.Add(input_reader.read_line().Split(' ').Select(c => int.Parse(c)).ToArray());
			}

			return reports.ToArray();
		}

		internal static bool is_safe(int[] report)
		{
			int previous_level = report[0];
			bool is_increasing = report[1] > report[0];

			for (int level_index = 1; level_index < report.Length; level_index++)
			{
				int current_level = report[level_index];
				bool increasing = current_level > previous_level;

				if (current_level == previous_level ||
					is_increasing != increasing ||
					Math.Abs(current_level - previous_level) > 3)
				{
					return false;
				}
				else
				{
					previous_level = current_level;
				}
			}

			return true;
		}

		public static void part_1(
			string input,
			bool pretty)
		{
			int[][] reports = parse_input(input, pretty);

			int safe_report_count = 0;

			foreach (int[] report in reports)
			{
				if (is_safe(report))
				{
					safe_report_count++;
				}
			}

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine();
			Console.WriteLine("Result = {0}", safe_report_count);
			Console.ResetColor();
		}

		public static void part_2(
			string input,
			bool pretty)
		{
			int[][] reports = parse_input(input, pretty);

			int safe_report_count = 0;

			foreach (int[] report in reports)
			{
				bool safe = false;

				if (is_safe(report))
				{
					safe = true;
				}
				else
				{
					for (int level_index = 0; level_index < report.Length && !safe; level_index++)
					{
						int[] dampened_report = report.Where((value, index) => index != level_index).ToArray();

						if (is_safe(dampened_report))
						{
							safe = true;
						}
					}
				}

				if (safe)
				{
					safe_report_count++;
				}
			}

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine();
			Console.WriteLine("Result = {0}", safe_report_count);
			Console.ResetColor();
		}
	}
}
