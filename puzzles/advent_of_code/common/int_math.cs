using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace advent_of_code_common.int_math
{
    [DebuggerDisplay("[{x}, {y}, {z}]", Type = "c_vector")]
    public class c_vector
    {
        public int x;
        public int y;
        public int z;

        public c_vector(int input_x, int input_y, int input_z)
        {
            x = input_x;
            y = input_y;
            z = input_z;
        }

        public c_vector(int input_x, int input_y)
        {
            x = input_x;
            y = input_y;
            z = 0;
        }

        public c_vector(c_vector other)
        {
            x = other.x;
            y = other.y;
            z = other.z;
        }

        public c_vector() : this(0, 0, 0) { }

        public void print(string name)
        {
            Console.WriteLine(name);
            Console.WriteLine("[{0}, {1}, {2}]", x, y, z);
            Console.WriteLine();
        }

        public c_vector inverse()
        {
            return new c_vector(-x, -y, -z);
        }

        public bool equal_to(c_vector other)
        {
            return x == other.x && y == other.y && z == other.z;
        }

        public int taxi_distance(
            c_vector other)
        {
            return Math.Abs(x - other.x)
                + Math.Abs(y - other.y)
                + Math.Abs(z - other.z);
        }

        public int taxi_magnitude()
        {
            return taxi_distance(new c_vector());
        }

        public c_vector add(c_vector other)
        {
            return new c_vector(
                x + other.x,
                y + other.y,
                z + other.z);
        }

        public void taxi_normalize()
        {
            x = Math.Max(-1, Math.Min(1, x));
            y = Math.Max(-1, Math.Min(1, y));
            z = Math.Max(-1, Math.Min(1, z));
        }

        public bool adjacent(c_vector other)
        {
            return Math.Abs(x - other.x) <= 1
                && Math.Abs(y - other.y) <= 1
                && Math.Abs(z - other.z) <= 1;
        }
    }

    public class c_vector_comparer : IEqualityComparer<c_vector>
    {
        public bool Equals(c_vector a, c_vector b)
        {
            return a.x == b.x
                && a.y == b.y
                && a.z == b.z;
        }

        public int GetHashCode(c_vector v)
        {
            return HashCode.Combine(v.x, v.y, v.z);
        }
    }

    [DebuggerDisplay("{min} -> {max}", Type = "c_rectangle")]
    public class c_rectangle
    {
        public c_vector min { get; private set; }
        public c_vector max { get; private set; }
        public int width { get => max.x - min.x; }
        public int height { get => max.y - min.y; }
        public int depth { get => max.z - min.z; }

        public c_rectangle()
        {
            min = new c_vector();
            max = new c_vector();
        }

        public c_rectangle(c_vector first, c_vector second)
        {
            min = new c_vector(Math.Min(first.x, second.x), Math.Min(first.y, second.y), Math.Min(first.z, second.z));
            max = new c_vector(Math.Max(first.x, second.x), Math.Max(first.y, second.y), Math.Max(first.z, second.z));
        }

        public bool intersects(c_rectangle other)
        {
            return min.x <= other.max.x
                && max.x >= other.min.x
                && min.y <= other.max.y
                && max.y >= other.min.y
                && min.z <= other.max.z
                && max.z >= other.min.z;
        }

        public c_rectangle get_intersection(c_rectangle other)
        {
            if (intersects(other))
            {
                c_vector intersection_min = new c_vector(
                    Math.Max(min.x, other.min.x),
                    Math.Max(min.y, other.min.y),
                    Math.Max(min.z, other.min.z));

                c_vector intersection_max = new c_vector(
                    Math.Min(max.x, other.max.x),
                    Math.Min(max.y, other.max.y),
                    Math.Min(max.z, other.max.z));

                return new c_rectangle(intersection_min, intersection_max);
            }
            else
            {
                return null;
            }
        }

        public c_vector[] to_int_vectors()
        {
            List<c_vector> results = new List<c_vector>();

            for (int x = min.x; x <= max.x; x++)
            {
                for (int y = min.y; y <= max.y; y++)
                {
                    for (int z = min.z; z <= max.z; z++)
                    {
                        results.Add(new c_vector(x, y, z));
                    }
                }
            }

            return results.ToArray();
        }

        public void expand_to_fit(c_rectangle other)
        {
            min.x = Math.Min(min.x, other.min.x);
            max.x = Math.Max(max.x, other.max.x);

            min.y = Math.Min(min.y, other.min.y);
            max.y = Math.Max(max.y, other.max.y);

            min.z = Math.Min(min.z, other.min.z);
            max.z = Math.Max(max.z, other.max.z);
        }

        public void expand_to_fit(c_vector point)
        {
            min.x = Math.Min(min.x, point.x);
            max.x = Math.Max(max.x, point.x);

            min.y = Math.Min(min.y, point.y);
            max.y = Math.Max(max.y, point.y);

            min.z = Math.Min(min.z, point.z);
            max.z = Math.Max(max.z, point.z);
        }
    }

    public enum e_axis
    {
        x,
        y,
        z,
    }

    public enum e_angle
    {
        angle_0,
        angle_90,
        angle_180,
        angle_270,
    }

    public class c_matrix
    {
        public readonly int[,] values = new int[4, 4];

        public c_matrix() { }

        public c_matrix(c_matrix other)
        {
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    values[row, col] = other.values[row, col];
                }
            }
        }

        public void print(string name)
        {
            Console.WriteLine(name);
            Console.WriteLine("[{0}, {1}, {2}, {3}]", values[0, 0], values[0, 1], values[0, 2], values[0, 3]);
            Console.WriteLine("[{0}, {1}, {2}, {3}]", values[1, 0], values[1, 1], values[1, 2], values[1, 3]);
            Console.WriteLine("[{0}, {1}, {2}, {3}]", values[2, 0], values[2, 1], values[2, 2], values[2, 3]);
            Console.WriteLine("[{0}, {1}, {2}, {3}]", values[3, 0], values[2, 1], values[3, 2], values[3, 3]);
            Console.WriteLine();
        }

        public static c_matrix identity()
        {
            c_matrix result = new c_matrix();
            result.values[0, 0] = 1;
            result.values[1, 1] = 1;
            result.values[2, 2] = 1;
            result.values[3, 3] = 1;

            return result;
        }

        public static c_matrix translate(c_vector t)
        {
            c_matrix result = identity();
            result.values[0, 3] += t.x;
            result.values[1, 3] += t.y;
            result.values[2, 3] += t.z;

            return result;
        }

        public static c_matrix invert_translation(c_matrix m)
        {
            c_matrix result = new c_matrix(m);
            result.values[0, 3] = -m.values[0, 3];
            result.values[1, 3] = -m.values[1, 3];
            result.values[2, 3] = -m.values[2, 3];

            return result;
        }

        public static c_matrix scale(c_vector s)
        {
            c_matrix result = identity();
            result.values[0, 0] *= s.x;
            result.values[1, 1] *= s.y;
            result.values[2, 2] *= s.z;

            return result;
        }

        private static int cosine(e_angle angle)
        {
            switch (angle)
            {
                case e_angle.angle_0: return 1;
                case e_angle.angle_90: return 0;
                case e_angle.angle_180: return -1;
                case e_angle.angle_270: return 0;
            }

            throw new Exception("invalid angle");
        }

        private static int sine(e_angle angle)
        {
            switch (angle)
            {
                case e_angle.angle_0: return 0;
                case e_angle.angle_90: return 1;
                case e_angle.angle_180: return 0;
                case e_angle.angle_270: return -1;

                default: throw new Exception("invalid angle");
            }
        }

        public static c_matrix rotate(e_axis axis, e_angle angle)
        {
            c_matrix result = identity();

            switch (axis)
            {
                case e_axis.x:
                    result.values[1, 1] = cosine(angle);
                    result.values[1, 2] = -sine(angle);
                    result.values[2, 1] = sine(angle);
                    result.values[2, 2] = cosine(angle);
                    break;

                case e_axis.y:
                    result.values[0, 0] = cosine(angle);
                    result.values[0, 2] = sine(angle);
                    result.values[2, 0] = -sine(angle);
                    result.values[2, 2] = cosine(angle);
                    break;

                case e_axis.z:
                    result.values[0, 0] = cosine(angle);
                    result.values[0, 1] = -sine(angle);
                    result.values[1, 0] = sine(angle);
                    result.values[1, 1] = cosine(angle);
                    break;

                default: throw new Exception("invalid axis");
            }

            return result;
        }

        public c_matrix multiply(c_matrix other)
        {
            // result is performing other first, then this.

            c_matrix result = new c_matrix();
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    int value = 0;

                    for (int i = 0; i < 4; i++)
                    {
                        value += this.values[row, i] * other.values[i, col];
                    }

                    result.values[row, col] = value;
                }
            }

            return result;
        }

        public c_vector multiply(c_vector vector)
        {
            // applies the current operation to the inputted vector.

            c_vector result = new c_vector();

            result.x = vector.x * this.values[0, 0]
                + vector.y * this.values[0, 1]
                + vector.z * this.values[0, 2]
                + this.values[0, 3];

            result.y = vector.x * this.values[1, 0]
                + vector.y * this.values[1, 1]
                + vector.z * this.values[1, 2]
                + this.values[1, 3];

            result.z = vector.x * this.values[2, 0]
                + vector.y * this.values[2, 1]
                + vector.z * this.values[2, 2]
                + this.values[2, 3];

            return result;
        }
    }
}
