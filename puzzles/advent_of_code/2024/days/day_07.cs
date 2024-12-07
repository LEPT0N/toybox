using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using advent_of_code_common.input_reader;

namespace advent_of_code_2024.days
{
	internal class day_07
	{
		[DebuggerDisplay("{test_value}", Type = "c_equation")]
		internal class c_equation
		{
			public readonly Int64 test_value;
			private Int64[] operators;
			private bool allow_concatenation;

			public c_equation(string input, bool a)
			{
				string[] input_components = input.Split(": ");

				test_value = Int64.Parse(input_components[0]);

				operators = input_components[1].Split(' ').Select(s => Int64.Parse(s)).ToArray();

				allow_concatenation = a;
			}

			private bool has_solution(Int64 accumulated_value, int index)
			{
				if (index < operators.Length)
				{
					return has_solution(accumulated_value + operators[index], index + 1)
						|| has_solution(accumulated_value * operators[index], index + 1)
						|| (allow_concatenation && has_solution(
							Int64.Parse(accumulated_value.ToString() + operators[index].ToString()),
							index + 1));
				}
				else
				{
					return accumulated_value == test_value;
				}
			}

			public bool has_solution()
			{
				return has_solution(operators[0], 1);
			}

			private string try_get_solution(Int64 accumulated_value, string accumulated_result, int index)
			{
				if (index < operators.Length)
				{
					string solution;
					
					solution = try_get_solution(
						accumulated_value + operators[index],
						accumulated_result + " + " + operators[index],
						index + 1);

					if (solution != null)
					{
						return solution;
					}

					solution = try_get_solution(
						accumulated_value * operators[index],
						accumulated_result + " * " + operators[index],
						index + 1);

					if (solution != null)
					{
						return solution;
					}

					if (allow_concatenation)
					{
						solution = try_get_solution(
								Int64.Parse(accumulated_value.ToString() + operators[index].ToString()),
								accumulated_result + " || " + operators[index],
								index + 1);

						if (solution != null)
						{
							return solution;
						}
					}
				}
				else
				{
					if (accumulated_value == test_value)
					{
						return accumulated_result;
					}
				}

				return null;
			}

			public string try_get_solution()
			{
				return try_get_solution(operators[0], operators[0].ToString(), 1);
			}
		}

		internal static c_equation[] parse_input(
			in string input,
			in bool allow_concatenation,
			in bool pretty)
		{
			c_input_reader input_reader = new c_input_reader(input);

			List<c_equation> equations = new List<c_equation>();

			while (input_reader.has_more_lines())
			{
				equations.Add(new c_equation(input_reader.read_line(), allow_concatenation));
			}

			return equations.ToArray();
		}

		private static void part_worker(
			string input,
			bool allow_concatenation,
			bool pretty)
		{
			c_equation[] equations = parse_input(input, allow_concatenation, pretty);

			(c_equation, string)[] valid_equations = equations
				.Select(e => (e, e.try_get_solution()))
				.Where(e => e.Item2 != null)
				.ToArray();

			Int64 result = valid_equations
				.Select(e => e.Item1.test_value)
				.Sum();

			if (pretty)
			{
				foreach ((c_equation, string) equation in valid_equations)
				{
					Console.WriteLine(equation.Item1.test_value + " = " + equation.Item2);
				}
			}

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine();
			Console.WriteLine($"Result = {result}");
			Console.ResetColor();
		}

		public static void part_1(
			string input,
			bool pretty)
		{
			part_worker(input, false, pretty);
		}

		public static void part_2(
			string input,
			bool pretty)
		{
			part_worker(input, true, pretty);
		}
	}
}
