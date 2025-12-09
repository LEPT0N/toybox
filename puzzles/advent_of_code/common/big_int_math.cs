using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace advent_of_code_common.big_int_math
{
    [DebuggerDisplay("[{x}, {y}, {z}]", Type = "c_big_vector")]
    public class c_big_vector
    {
        public static readonly c_big_vector k_vector_zero = new c_big_vector(0, 0, 0);

        public Int64 x;
        public Int64 y;
        public Int64 z;

        public c_big_vector(Int64 input_x, Int64 input_y, Int64 input_z)
        {
            x = input_x;
            y = input_y;
            z = input_z;
        }

        public c_big_vector(Int64 input_x, Int64 input_y)
        {
            x = input_x;
            y = input_y;
            z = 0;
        }

        public c_big_vector(c_big_vector other)
        {
            x = other.x;
            y = other.y;
            z = other.z;
        }

        public c_big_vector() : this(0, 0, 0) { }

        public void print(string name)
        {
            Console.WriteLine(name);
            Console.WriteLine(to_string());
            Console.WriteLine();
        }

        public string to_string()
        {
            return $"[{x}, {y}, {z}]";
        }

        public c_big_vector inverse()
        {
            return new c_big_vector(-x, -y, -z);
        }

        public bool equal_to(c_big_vector other)
        {
            return x == other.x && y == other.y && z == other.z;
        }

        public Int64 taxi_distance(
            c_big_vector other)
        {
            return Math.Abs(x - other.x)
                + Math.Abs(y - other.y)
                + Math.Abs(z - other.z);
        }

        public Int64 taxi_magnitude()
        {
            return taxi_distance(new c_big_vector());
        }

        public Int64 taxi_area(
            c_big_vector other)
        {
            return (Math.Abs(x - other.x) + 1)
                * (Math.Abs(y - other.y) + 1)
                * (Math.Abs(z - other.z) + 1);
        }

        public double euclidean_magnitude()
        {
            return Math.Sqrt(x * x + y * y + z * z);
        }

        public c_big_vector add(c_big_vector other)
        {
            return new c_big_vector(
                x + other.x,
                y + other.y,
                z + other.z);
        }

        public c_big_vector subtract(c_big_vector other)
        {
            return new c_big_vector(
                x - other.x,
                y - other.y,
                z - other.z);
        }

        public c_big_vector scale(Int64 value)
        {
            return new c_big_vector(
                x * value,
                y * value,
                z * value);
        }
    }

    public class c_big_vector_comparer : IEqualityComparer<c_big_vector>
    {
        public bool Equals(c_big_vector a, c_big_vector b)
        {
            return a.x == b.x
                && a.y == b.y
                && a.z == b.z;
        }

        public int GetHashCode(c_big_vector v)
        {
            return HashCode.Combine(v.x, v.y, v.z);
        }
    }
}
