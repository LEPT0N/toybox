using advent_of_code_common.big_int_math;
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

        // Shoelace formula is a way to find the area of a polygon.
        // assumes zn == 0
        public static Int64 shoelace_formula(c_big_vector[] points)
        {
            Int64 xy_sum = 0;
            Int64 yx_sum = 0;

            for (int i = 0; i < points.Length - 1; i++)
            {
                xy_sum += points[i].x * points[i + 1].y;
                yx_sum += points[i].y * points[i + 1].x;
            }

            xy_sum += points[points.Length - 1].x * points[0].y;
            yx_sum += points[points.Length - 1].y * points[0].x;

            return Math.Abs(xy_sum - yx_sum) / 2;
        }
    }
}
