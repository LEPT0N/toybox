using digits;
using extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace extensions
{
	public static class extensions
	{
		public static bool test_bit(this uint bit_field, int index)
		{
			return (0U != (bit_field & (1U << index)));
		}

		public static void set_bit(ref this uint bit_field, int index)
		{
			bit_field |= (1U << index);
		}

		public static uint and(ref this uint bit_field, uint other_bit_field)
		{
			return bit_field & other_bit_field;
		}

		public static uint or(ref this uint bit_field, uint other_bit_field)
		{
			return bit_field | other_bit_field;
		}

		public static char to_char(ref this e_operator op)
		{
			switch (op)
			{
				case e_operator.addition: return '+';
				case e_operator.subtraction: return '-';
				case e_operator.multiplication: return '*';
				case e_operator.division: return '/';
				default: throw new Exception($"Unexpected operation {op}");
			}
		}
	}
}

namespace digits
{
	public enum e_operator
	{
		addition,
		subtraction,
		multiplication,
		division,
	}

	internal abstract class i_equation
	{
		public int value;
		public uint inputs_used;
		public abstract string debug_display { get; }
	}

	[DebuggerDisplay("{debug_display,nq}", Type = "c_simple_equation")]
	internal class c_simple_equation : i_equation
	{
		public override string debug_display
		{
			get
			{
				return $"{value}";
			}
		}
	}

	[DebuggerDisplay("{debug_display,nq}", Type = "c_complex_equation")]
	internal class c_complex_equation : i_equation
	{
		public i_equation left_equation;
		public i_equation right_equation;
		public e_operator operation;

		public c_complex_equation(
			i_equation left,
			i_equation right,
			e_operator op)
		{
			left_equation = left;
			right_equation = right;
			operation = op;

			switch (op)
			{
				case e_operator.addition:
					value = left.value + right.value;
					break;

				case e_operator.subtraction:
					value = left.value - right.value;
					break;

				case e_operator.multiplication:
					value = left.value * right.value;
					break;

				case e_operator.division:
					value = left.value / right.value;
					break;

				default: throw new Exception($"Unexpected operation {op}");
			}

			inputs_used = left_equation.inputs_used.or(right_equation.inputs_used);
		}

		public override string debug_display
		{
			get
			{
				char operation_char = operation.to_char();

				return $"({left_equation.debug_display} {operation_char} {right_equation.debug_display})";
			}
		}
	}

	/*	1, 2, 3, 4
	 *	
	 *	(1+2), (1+3), ...
	 *	
	 *	(1+2) * 3, 4 + (1+2), ...
	 * 
	 * 
	 * 
	 * 2	[1 op 1]...
	 * 
	 * 3	[1 op 2], [2 op 1]...
	 * 
	 * 4	[1 op 3], [2 op 2], [3 op 1],...
	 * 
	 * 5	[1 op 4], [2 op 3], [3 op 2], [4 op 1],...
	 * 
	 * 6	[1 op 5], [2 op 4], [3 op 3], [4 op 2], [5 op 1]...
	 * 
	 * 
	 */

	internal class digits
	{
		internal static bool is_commutative(e_operator op)
		{
			switch (op)
			{
				case e_operator.addition:
				case e_operator.multiplication:
					return true;

				case e_operator.subtraction:
				case e_operator.division:
					return false;

				default: throw new Exception($"Unexpected operation {op}");
			}
		}

		internal static bool is_valid(
			i_equation left,
			i_equation right,
			e_operator op)
		{
			if (left.inputs_used.and(right.inputs_used) != 0)
			{
				return false;
			}

			if (op == e_operator.subtraction)
			{
				if (left.value <= right.value)
				{
					return false;
				}
			}

			if (op == e_operator.division)
			{
				if (left.value % right.value != 0)
				{
					return false;
				}
			}

			return true;
		}

		static void Main(string[] args)
		{
			//int target = 483;
			//int[] inputs = { 5, 7, 11, 13, 20, 23 };

			//int target = 105875;
			//int[] inputs = { 2, 7, 13, 19, 21, 27, 31, 41 };

			int target = 443;
			int[] inputs = { 5, 9, 11, 13, 20, 23 };

			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			int total_equations_considered = 0;
			int best_distance = int.MaxValue;
			i_equation best_equation = null;

			i_equation[][] equations = new i_equation[inputs.Length][];

			// Build the tier[1] equations
			{
				List<i_equation> tier_1_equations = new List<i_equation>();

				for (int i = 0; i < inputs.Length; i++)
				{
					c_simple_equation simple_equation = new c_simple_equation { value = inputs[i] };
					simple_equation.inputs_used.set_bit(i);
					tier_1_equations.Add(simple_equation);
				}

				equations[1] = tier_1_equations.ToArray();
			}

			// Build the tier[2..n] equations
			for (int current_tier = 2; current_tier < inputs.Length; current_tier++)
			{
				List<i_equation> current_tier_equations = new List<i_equation>();

				// Loop over each possible operator
				foreach (e_operator current_operator in Enum.GetValues(typeof(e_operator)))
				{
					bool current_operator_is_commutative = is_commutative(current_operator);

					// only check half the tier combinations if the operator is commutative.
					int last_left_tier = current_tier - 1;
					if (current_operator_is_commutative)
					{
						last_left_tier = current_tier / 2;
					}

					// tier[n] is built with [tier[1] op tier[n-1]], [tier[2] op tier[n-2]]...
					for (int left_tier = 1; left_tier <= last_left_tier; left_tier++)
					{
						int right_tier = current_tier - left_tier;

						// Create all valid equations combining the two tiers

						for (int left_tier_index = 0; left_tier_index < equations[left_tier].Length; left_tier_index++)
						{
							i_equation left_equation = equations[left_tier][left_tier_index];

							// only check half the equation combinations if the operator is commutative.
							int right_tier_index_start = 0;
							if (current_operator_is_commutative)
							{
								right_tier_index_start = left_tier_index + 1;
							}

							for (int right_tier_index = right_tier_index_start; right_tier_index < equations[right_tier].Length; right_tier_index++)
							{
								i_equation right_equation = equations[right_tier][right_tier_index];

								// If the two equations are valid to combine with the current operator, then do so.
								if (is_valid(left_equation, right_equation, current_operator))
								{
									total_equations_considered++;

									c_complex_equation complex_equation = new c_complex_equation(
										left_equation,
										right_equation,
										current_operator);

									// If this is our new best equation, remember it.
									int distance = Math.Abs(complex_equation.value - target);
									if (distance < best_distance)
									{
										best_distance = distance;
										best_equation = complex_equation;

										// If this equation is a solution, then exit.
										if (best_distance == 0)
										{
											stopwatch.Stop();

											Console.WriteLine($"The best equation is {best_equation.debug_display} = {best_equation.value}");
											Console.WriteLine($"Considered {total_equations_considered} equations");
											Console.WriteLine("Time taken = {0}", stopwatch.Elapsed);

											return;
										}
									}

									current_tier_equations.Add(complex_equation);
								}
							}
						}
					}
				}

				equations[current_tier] = current_tier_equations.ToArray();
			}

			// We didn't find a solution, so return our closest match.

			stopwatch.Stop();

			Console.WriteLine($"The best equation is {best_equation.debug_display} = {best_equation.value}");
			Console.WriteLine($"Considered {total_equations_considered} equations");
			Console.WriteLine("Time taken = {0}", stopwatch.Elapsed);
		}
	}
}