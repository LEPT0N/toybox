﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace advent_of_code_common.int_math
{
    [DebuggerDisplay("[{x}, {y}, {z}]", Type = "c_vector")]
    public class c_vector
    {
        public static c_vector k_vector_zero { get { return new c_vector(0, 0, 0); } }

        public int x;
        public int y;
        public int z;

        public int row { get { return x; } set { x = value; } }
        public int col { get { return y; } set { y = value; } }

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

        public string to_string()
        {
            return $"[{x}, {y}, {z}]";
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

        public c_vector subtract(c_vector other)
        {
            return new c_vector(
                x - other.x,
                y - other.y,
                z - other.z);
        }

        public c_vector scale(int value)
        {
            return new c_vector(
                x * value,
                y * value,
                z * value);
        }

        public c_vector mod(c_vector other)
        {
            return new c_vector(
                x % other.x,
                y % other.y,
                z % other.z);
        }

        public void normalize()
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

        public c_vector add(e_direction direction)
        {
            c_vector result = new c_vector(this);

            switch (direction)
            {
                case e_direction.none:
                    break;

                case e_direction.up:
                    result.row--;
                    break;

                case e_direction.down:
                    result.row++;
                    break;

                case e_direction.left:
                    result.col--;
                    break;

                case e_direction.right:
                    result.col++;
                    break;

                default:
                    throw new Exception($"Can't add direction '{direction}' to a vector");
            }

            return result;
        }

        public c_vector subtract(e_direction direction)
        {
            c_vector result = new c_vector(this);

            switch (direction)
            {
                case e_direction.none:
                    break;

                case e_direction.up:
                    result.row++;
                    break;

                case e_direction.down:
                    result.row--;
                    break;

                case e_direction.left:
                    result.col++;
                    break;

                case e_direction.right:
                    result.col--;
                    break;

                default:
                    throw new Exception($"Can't subtract direction '{direction}' to a vector");
            }

            return result;
        }

        public e_direction[] to_directions()
        {
            List<e_direction> directions = new List<e_direction>();

            e_direction up_down_direction = row > 0 ? e_direction.down : e_direction.up;

            for (int i = 0; i < int.Abs(row); i++)
            {
                directions.Add(up_down_direction);
            }

            e_direction left_right_direction = col > 0 ? e_direction.right : e_direction.left;

            for (int i = 0; i < int.Abs(col); i++)
            {
                directions.Add(left_right_direction);
            }

            return directions.ToArray();
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

    [DebuggerDisplay("{first} -> {second}", Type = "c_line")]
    public class c_line
    {
        public c_vector first { get; set; }
        public c_vector second { get; set; }

        public c_line()
        {
            first = new c_vector();
            second = new c_vector();
        }

        public c_line(c_vector f, c_vector s)
        {
            first = f;
            second = s;
        }

        public c_vector[] to_int_vectors()
        {
            List<c_vector> results = new List<c_vector>();

            int min_x = Math.Min(first.x, second.x);
            int max_x = Math.Max(first.x, second.x);

            for (int x = min_x; x <= max_x; x++)
            {
                int min_y = Math.Min(first.y, second.y);
                int max_y = Math.Max(first.y, second.y);

                for (int y = min_y; y <= max_y; y++)
                {
                    int min_z = Math.Min(first.z, second.z);
                    int max_z = Math.Max(first.z, second.z);

                    for (int z = min_z; z <= max_z; z++)
                    {
                        results.Add(new c_vector(x, y, z));
                    }
                }
            }

            return results.ToArray();
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

        public c_rectangle(c_vector v)
        {
            min = new c_vector(v.x, v.y, v.z);
            max = new c_vector(v.x, v.y, v.z);
        }

        public c_rectangle(c_vector first, c_vector second)
        {
            min = new c_vector(Math.Min(first.x, second.x), Math.Min(first.y, second.y), Math.Min(first.z, second.z));
            max = new c_vector(Math.Max(first.x, second.x), Math.Max(first.y, second.y), Math.Max(first.z, second.z));
        }

        public bool contains(c_vector v)
        {
            return min.x <= v.x
                && max.x >= v.x
                && min.y <= v.y
                && max.y >= v.y
                && min.z <= v.z
                && max.z >= v.z;
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

        public void expand_by(int amount)
        {
            min.x -= amount;
            max.x += amount;

            min.y -= amount;
            max.y += amount;

            min.z -= amount;
            max.z += amount;
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

    public enum e_direction
    {
        none,
        up,
        down,
        left,
        right,
        count,
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

    public class c_int_math
    {
        public static e_direction rotate(e_direction direction, e_angle angle)
        {
            switch ((direction, angle))
            {
                case (e_direction.up, e_angle.angle_0): return e_direction.up;
                case (e_direction.up, e_angle.angle_90): return e_direction.left;
                case (e_direction.up, e_angle.angle_180): return e_direction.down;
                case (e_direction.up, e_angle.angle_270): return e_direction.right;

                case (e_direction.down, e_angle.angle_0): return e_direction.down;
                case (e_direction.down, e_angle.angle_90): return e_direction.right;
                case (e_direction.down, e_angle.angle_180): return e_direction.up;
                case (e_direction.down, e_angle.angle_270): return e_direction.left;

                case (e_direction.left, e_angle.angle_0): return e_direction.left;
                case (e_direction.left, e_angle.angle_90): return e_direction.down;
                case (e_direction.left, e_angle.angle_180): return e_direction.right;
                case (e_direction.left, e_angle.angle_270): return e_direction.up;

                case (e_direction.right, e_angle.angle_0): return e_direction.right;
                case (e_direction.right, e_angle.angle_90): return e_direction.up;
                case (e_direction.right, e_angle.angle_180): return e_direction.left;
                case (e_direction.right, e_angle.angle_270): return e_direction.down;

                default: throw new Exception($"Invalid rotation {direction} {angle}");
            }
        }
    }

    public static class extensions
    {
        public static char to_char(this e_direction direction)
        {
            switch (direction)
            {
                case e_direction.up: return '^';
                case e_direction.down: return 'v';
                case e_direction.left: return '<';
                case e_direction.right: return '>';
                default: return ' ';
            }
        }
    }
}