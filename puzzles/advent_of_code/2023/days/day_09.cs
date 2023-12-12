using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.extensions;
using advent_of_code_common.input_reader;

namespace advent_of_code_2023.days
{
	internal class day_09
	{
		[DebuggerDisplay("{data.Count}", Type = "c_dataset")]
		internal class c_dataset
		{
			List<List<Int64>> data = new List<List<Int64>>();

			public c_dataset(string input)
			{
				data.Add(input.Split(' ')
					.Select(input => Convert.ToInt64(input)).ToList());
			}

			public void expand()
			{
				bool needs_expansion = true;

				List<Int64> latest_row = data.Last();

				while (needs_expansion)
				{
					needs_expansion = false;

					List<Int64> new_row = new List<Int64>();

					for (int i = 0; i < latest_row.Count - 1; i++)
					{
						Int64 new_value = latest_row[i + 1] - latest_row[i];

						needs_expansion = needs_expansion || new_value != 0;

						new_row.Add(new_value);
					}

					latest_row = new_row;
					data.Add(new_row);
				}
			}

			public Int64 extrapolate()
			{
				data.Last().Add(0);

				for (int row = data.Count - 2; row >= 0; row--)
				{
					data[row].Add(data[row].Last() + data[row + 1].Last());
				}

				return data[0].Last();
			}

			public Int64 extrapolate_backwards()
			{
				data.Last().Insert(0, 0);

				for (int row = data.Count - 2; row >= 0; row--)
				{
					data[row].Insert(0, data[row].First() - data[row + 1].First());
				}

				return data[0].First();
			}
		}

		internal static c_dataset[] parse_input(
			in string input,
			in bool pretty)
		{
			c_input_reader input_reader = new c_input_reader(input);

			List<c_dataset> data = new List<c_dataset>();

			while (input_reader.has_more_lines())
			{
				data.Add(new c_dataset(input_reader.read_line()));
			}

			return data.ToArray();
		}

		public static void part_1(
			string input,
			bool pretty)
		{
			c_dataset[] data = parse_input(input, pretty);

			data.for_each(dataset => dataset.expand());

			Int64 sum = data.Sum(dataset => dataset.extrapolate());

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine();
			Console.WriteLine("Result = {0}", sum);
			Console.ResetColor();
		}

		public static void part_2(
			string input,
			bool pretty)
		{
			c_dataset[] data = parse_input(input, pretty);

			data.for_each(dataset => dataset.expand());

			Int64 sum = data.Sum(dataset => dataset.extrapolate_backwards());

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine();
			Console.WriteLine("Result = {0}", sum);
			Console.ResetColor();
		}
	}
}
