using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.extensions;
using advent_of_code_common.input_reader;

namespace advent_of_code_2023.days
{
	internal class day_11
	{
		[DebuggerDisplay("galaxies = {galaxy_count}, width = {width}", Type = "c_sector")]
		internal class c_sector
		{
			public UInt64 galaxy_count { get; set; }
			public UInt64 width { get; set; }

			public c_sector(UInt64 g)
			{
				galaxy_count = g;
				width = 1;
			}
		}

		internal static (List<c_sector>, List<c_sector>) parse_input(
			in string input,
			in bool pretty)
		{
			c_input_reader input_reader = new c_input_reader(input);

			List<c_sector> columns = null;
			List<c_sector> rows = new List<c_sector>();

			while (input_reader.has_more_lines())
			{
				string line = input_reader.read_line();

				rows.Add(new c_sector(Convert.ToUInt64(line.Count(c => c == '#'))));

				if (columns == null)
				{
					columns = line.Select(c => new c_sector(c == '#' ? 1UL : 0UL)).ToList();
				}
				else
				{
					for (int i = 0; i < line.Length; i++)
					{
						if (line[i] == '#')
						{
							columns[i].galaxy_count++;
						}
					}
				}
			}

			return (columns, rows);
		}

		internal static void expand_empty_sectors(ref List<c_sector> data, UInt64 factor)
		{
			data.for_each(sector =>
			{
				if (sector.galaxy_count == 0)
				{
					sector.width *= factor;
				}
			});
		}

		internal static UInt64 sum_weighted_distances(List<c_sector> data)
		{
			UInt64 sum = 0;

			for (int i = 0; i < data.Count - 1; i++)
			{
				if (data[i].galaxy_count > 0)
				{
					for (int j = i + 1; j < data.Count; j++)
					{
						if (data[j].galaxy_count > 0)
						{
							for (int sector_index = i; sector_index < j; sector_index++)
							{
								sum += data[sector_index].width * data[i].galaxy_count * data[j].galaxy_count;
							}
						}
					}
				}
			}

			return sum;
		}

		private static void part_worker(
			string input,
			bool pretty,
			UInt64 factor)
		{
			(List<c_sector> columns, List<c_sector> rows) = parse_input(input, pretty);

			expand_empty_sectors(ref columns, factor);
			expand_empty_sectors(ref rows, factor);

			UInt64 sum = sum_weighted_distances(columns) + sum_weighted_distances(rows);

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine();
			Console.WriteLine("Result = {0}", sum);
			Console.ResetColor();
		}

		public static void part_1(
			string input,
			bool pretty)
		{
			part_worker(input, pretty, 2);
		}

		public static void part_2(
			string input,
			bool pretty)
		{
			part_worker(input, pretty, 1000000);
		}
	}
}
