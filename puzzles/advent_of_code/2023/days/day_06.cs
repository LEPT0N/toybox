using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.input_reader;

namespace advent_of_code_2023.days
{
	internal class day_06
	{
		[DebuggerDisplay("time = {time}, best_distance = {best_distance}", Type = "c_race")]
		internal class c_race
		{
			readonly UInt64 time;
			readonly UInt64 best_distance;

			public c_race(UInt64 t, UInt64 b)
			{
				time = t;
				best_distance = b;
			}

			public UInt64 get_distance(UInt64 held_time)
			{
				UInt64 speed = held_time;

				UInt64 moving_time = time - held_time;

				return speed * moving_time;
			}

			public UInt64 count_ways_to_beat_record()
			{
				// Solve for held_time as a function of best_distance and time:
				//
				// best_distance = speed * moving_time
				// best_distance = held_time * (time - held_time)
				// bd = ht * t - ht ^2;
				// ht^2 - t * ht + bd = 0
				//
				// x = -b - sqrt(b^2 - 4ac) / 2a
				//		a == 1
				//		b == -t
				//		c = bd
				// x = (t - sqrt(t*t - 4 * bd) / 2

				double min_held_time_to_tie_record = (time - Math.Sqrt(time * time - (4.0 * best_distance))) / 2.0;

				UInt64 min_held_time_to_beat_record = Convert.ToUInt64(Math.Floor(min_held_time_to_tie_record)) + 1;

				UInt64 max_held_time_to_beat_record = time - min_held_time_to_beat_record;

				return max_held_time_to_beat_record - min_held_time_to_beat_record + 1;
			}
		}

		internal static List<c_race> parse_input_1(
			in string input,
			in bool pretty)
		{
			c_input_reader input_reader = new c_input_reader(input);

			UInt64[] time_inputs = input_reader
				.read_line()
				.Substring("Time:      ".Length)
				.Split(' ')
				.Where(value => !string.IsNullOrEmpty(value))
				.Select(value => UInt64.Parse(value))
				.ToArray();

			UInt64[] distance_inputs = input_reader
				.read_line()
				.Substring("Distance:  ".Length)
				.Split(' ')
				.Where(value => !string.IsNullOrEmpty(value))
				.Select(value => UInt64.Parse(value))
				.ToArray();

			List<c_race> races = new List<c_race>();

			for (int i = 0; i < time_inputs.Length; i++)
			{
				races.Add(new c_race(time_inputs[i], distance_inputs[i]));
			}

			return races;
		}

		public static void part_1(
			string input,
			bool pretty)
		{
			List<c_race> races = parse_input_1(input, pretty);

			List<UInt64> ways_to_beat_records = races.Select(race => race.count_ways_to_beat_record()).ToList();

			UInt64 total_ways_to_beat_records = ways_to_beat_records.Aggregate((x, y) => x * y);

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine();
			Console.WriteLine("Result = {0}", total_ways_to_beat_records);
			Console.ResetColor();
		}

		internal static c_race parse_input_2(
			in string input,
			in bool pretty)
		{
			c_input_reader input_reader = new c_input_reader(input);

			UInt64 time_input = UInt64.Parse(String.Concat(input_reader
				.read_line()
				.Substring("Time:      ".Length)
				.Split(' ')));

			UInt64 distance_input = UInt64.Parse(String.Concat(input_reader
				.read_line()
				.Substring("Distance:  ".Length)
				.Split(' ')));

			return new c_race(time_input, distance_input);
		}

		public static void part_2(
			string input,
			bool pretty)
		{
			c_race race = parse_input_2(input, pretty);

			UInt64 total_ways_to_beat_record = race.count_ways_to_beat_record();

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine();
			Console.WriteLine("Result = {0}", total_ways_to_beat_record);
			Console.ResetColor();
		}
	}
}
