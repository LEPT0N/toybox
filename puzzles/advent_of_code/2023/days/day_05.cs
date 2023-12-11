using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.input_reader;

namespace advent_of_code_2023.days
{
	internal class day_05
	{
		[DebuggerDisplay("[{low}, {high}]", Type = "c_range")]
		internal class c_range
		{
			public readonly UInt64 low;
			public readonly UInt64 high;

			public c_range(UInt64 l, UInt64 h)
			{
				low = l;
				high = h;
			}

			public bool contains(UInt64 value)
			{
				return low <= value && value <= high;
			}
		}

		[DebuggerDisplay("{source} -> {destination}", Type = "c_range_map")]
		internal class c_range_map
		{
			c_range source;
			c_range destination;

			public c_range_map(string input)
			{
				UInt64[] input_numbers = input
					.Split(' ')
					.Select(number => UInt64.Parse(number))
					.ToArray();

				UInt64 destination_base = input_numbers[0];
				UInt64 source_base = input_numbers[1];
				UInt64 range_size = input_numbers[2];

				source = new c_range(source_base, source_base + range_size - 1);
				destination = new c_range(destination_base, destination_base + range_size - 1);
			}

			public bool map(ref UInt64 value)
			{
				bool mapped = false;

				if (source.contains(value))
				{
					UInt64 offset = value - source.low;

					value = destination.low + offset;

					mapped = true;
				}

				return mapped;
			}

			public (List<c_range>, List<c_range>) map(c_range values)
			{
				List<c_range> mapped = new List<c_range>();
				List<c_range> unmapped = new List<c_range>();

				if (values.low <= source.high && values.high >= source.low)
				{
					if (values.low < source.low)
					{
						unmapped.Add(new c_range(values.low, source.low - 1));
					}

					if (source.high < values.high)
					{
						unmapped.Add(new c_range(source.high + 1, values.high));
					}

					UInt64 intersect_low = Math.Max(source.low, values.low);
					UInt64 intersect_high = Math.Min(source.high, values.high);

					map(ref intersect_low);
					map(ref intersect_high);

					mapped.Add(new c_range(intersect_low, intersect_high));
				}
				else
				{
					unmapped.Add(values);
				}

				return (mapped, unmapped);
			}
		}

		internal static (UInt64[], c_range_map[][]) parse_input(
			in string input,
			in bool pretty)
		{
			c_input_reader input_reader = new c_input_reader(input);

			UInt64[] seeds = input_reader
				.read_line()
				.Substring("seeds: ".Length)
				.Split(' ')
				.Select(number => UInt64.Parse(number))
				.ToArray();

			List<c_range_map[]> mappings = new List<c_range_map[]>();

			while (input_reader.has_more_lines())
			{
				List<c_range_map> ranges = new List<c_range_map>();

				// empty line
				input_reader.read_line();

				// map title
				input_reader.read_line();

				while (input_reader.has_more_lines() && !string.IsNullOrEmpty(input_reader.peek_line()))
				{
					c_range_map range = new c_range_map(input_reader.read_line());

					ranges.Add(range);
				}

				mappings.Add(ranges.ToArray());
			}

			return (seeds, mappings.ToArray());
		}

		public static void part_1(
			string input,
			bool pretty)
		{
			(UInt64[] seeds, c_range_map[][] mappings) = parse_input(input, pretty);

			// Loop through each mapping
			foreach (c_range_map[] mapping in mappings)
			{
				// Loop through each seed for the current mapping
				for (int seed_index = 0; seed_index < seeds.Length; seed_index++)
				{
					// Try running the seed through each range
					bool mapped = false;
					for (int map_index = 0; map_index < mapping.Length && !mapped; map_index++)
					{
						mapped = mapping[map_index].map(ref seeds[seed_index]);
					}
				}
			}

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine();
			Console.WriteLine("Result = {0}", seeds.Min());
			Console.ResetColor();
		}

		public static void part_2(
			string input,
			bool pretty)
		{
			(UInt64[] seed_input, c_range_map[][] range_map_collection) = parse_input(input, pretty);

			// Convert the seed input to a collection of seed ranges.
			List<c_range> seed_ranges = new List<c_range>();
			for (int seed_input_index = 0; seed_input_index < seed_input.Length; seed_input_index += 2)
			{
				UInt64 seed_start = seed_input[seed_input_index];
				UInt64 seed_range_size = seed_input[seed_input_index + 1];

				c_range seed_range = new c_range(seed_start, seed_start + seed_range_size - 1);

				seed_ranges.Add(seed_range);
			}

			// Loop through each mapping
			foreach (c_range_map[] range_maps in range_map_collection)
			{
				// Consider each range to be unmapped by the current mapping
				List<c_range> unmapped_seeds_list = new List<c_range>();
				unmapped_seeds_list.AddRange(seed_ranges);

				List<c_range> mapped_seeds = new List<c_range>();

				// Loop through each range map in the mapping
				for (int range_index = 0; range_index < range_maps.Length; range_index++)
				{
					c_range_map range_map = range_maps[range_index];

					List<c_range> still_unmapped_seeds = new List<c_range>();

					// Look at each range of unmapped seeds
					foreach(c_range unmapped_seeds in unmapped_seeds_list)
					{
						(List<c_range> mapped, List<c_range> unmapped) = range_map.map(unmapped_seeds);

						mapped_seeds.AddRange(mapped);
						still_unmapped_seeds.AddRange(unmapped);
					}

					// cull down the unmapped seeds in case any were mapped.
					unmapped_seeds_list = still_unmapped_seeds;
				}

				// After applying the current list of range maps, put together all the mapped and unmapped values to be our inputs to the next range maps.
				seed_ranges = mapped_seeds.Concat(unmapped_seeds_list).ToList();
			}

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine();
			Console.WriteLine("Result = {0}", seed_ranges.Select(seed_range => seed_range.low).Min());
			Console.ResetColor();
		}
	}
}
