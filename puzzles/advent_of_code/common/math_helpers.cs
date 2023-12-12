using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace advent_of_code_common.math_helpers
{
	public class math_helpers
	{
		public static List<UInt64> find_prime_factors(in UInt64 value)
		{
			UInt64 current_value = value;

			UInt64 end = Convert.ToUInt64(Math.Ceiling(Math.Sqrt(current_value)));

			List<UInt64> factors = new List<UInt64>();

			for (UInt64 i = 2; i <= end; i++)
			{
				if (current_value % i == 0)
				{
					factors.Add(i);

					current_value = current_value / i;

					end = Convert.ToUInt64(Math.Ceiling(Math.Sqrt(current_value)));

					i--;
				}
			}

			if (current_value != 1)
			{
				factors.Add(current_value);
			}

			Debug.Assert(value == factors.Aggregate((x, y) => x * y));

			return factors;
		}

		public static UInt64 find_least_common_multiple(in UInt64[] values)
		{
			UInt64 step = values[0];
			UInt64 lcm = values[0];

			for (int i = 1; i < values.Length; i++)
			{
				while (lcm % values[i] != 0)
				{
					lcm += step;
				}

				step = lcm;
			}

			return lcm;
		}
	}
}
