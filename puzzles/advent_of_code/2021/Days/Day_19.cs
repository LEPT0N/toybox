using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace advent_of_code_2021.Days
{
    internal class Day_19
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

        [DebuggerDisplay("[{beacons}]", Type = "c_scanner")]
        internal class c_scanner
        {
            public readonly c_vector scanner;
            public readonly c_vector[] beacons;

            private c_scanner(
                c_vector input_scanner,
                c_vector[] input_beacons)
            {
                scanner = input_scanner;
                beacons = input_beacons;
            }

            public c_scanner(c_input_reader input_reader)
            {
                List<c_vector> beacon_list = new List<c_vector>();

                while (input_reader.has_more_lines())
                {
                    string input_line = input_reader.read_line();

                    if (string.IsNullOrEmpty(input_line))
                    {
                        break;
                    }

                    int[] input_numbers = input_line.Split(",").Select(x => int.Parse(x)).ToArray();

                    beacon_list.Add(new c_vector(input_numbers[0], input_numbers[1], input_numbers[2]));
                }

                scanner = new c_vector(0, 0, 0);
                beacons = beacon_list.ToArray();
            }

            // create a new scanner that is equivalent to this, but with operation applied to all the objects.
            private c_scanner apply(c_matrix operation)
            {
                c_vector result_scanner = operation.multiply(scanner);

                c_vector[] result_beacons = new c_vector[beacons.Length];

                for (int i = 0; i < beacons.Length; i++)
                {
                    result_beacons[i] = operation.multiply(beacons[i]);
                }

                return new c_scanner(result_scanner, result_beacons);
            }

            // Create the set of all 24 rotation operations.
            private static c_matrix[] get_equivalence_rotation_operations()
            {
                List<c_matrix> result = new List<c_matrix>();

                (e_axis, e_angle)[] orientation_rotations =
                {
                    (e_axis.x, e_angle.angle_0),
                    (e_axis.x, e_angle.angle_90),
                    (e_axis.x, e_angle.angle_180),
                    (e_axis.x, e_angle.angle_270),

                    (e_axis.y, e_angle.angle_90),
                    (e_axis.y, e_angle.angle_270),
                };

                foreach ((e_axis, e_angle) orientation_rotation in orientation_rotations)
                {
                    c_matrix orientation_rotation_matrix = c_matrix.rotate(orientation_rotation.Item1, orientation_rotation.Item2);

                    e_angle[] spin_rotations =
                    {
                        e_angle.angle_0,
                        e_angle.angle_90,
                        e_angle.angle_180,
                        e_angle.angle_270,
                    };

                    foreach (e_angle spin_rotation in spin_rotations)
                    {
                        c_matrix spin_rotation_matrix = c_matrix.rotate(e_axis.z, spin_rotation);

                        result.Add(orientation_rotation_matrix.multiply(spin_rotation_matrix));
                    }
                }

                return result.ToArray();
            }

            // Create the set of all translation operatons that put one of the beacons at the origin
            private c_matrix[] get_equivalence_translation_operations()
            {
                List<c_matrix> result = new List<c_matrix>();

                foreach (c_vector beacon in beacons)
                {
                    result.Add(c_matrix.translate(beacon.inverse()));
                }

                return result.ToArray();
            }

            // Create the set of all 24 scanners that are equivalent to this, but rotated.
            private c_scanner[] get_equivalent_rotations()
            {
                List<c_scanner> result = new List<c_scanner>();

                c_matrix[] equivalent_operations = get_equivalence_rotation_operations();

                foreach(c_matrix equivalent_operation in equivalent_operations)
                {
                    result.Add(apply(equivalent_operation));
                }

                return result.ToArray();
            }

            // Sum the total number of beacons equivalent between two scanners
            public static int get_duplicate_beacon_count(c_scanner first, c_scanner second)
            {
                int result = 0;

                foreach (c_vector first_beacon in first.beacons)
                {
                    if (second.beacons.Any(second_beacon => second_beacon.equal_to(first_beacon)))
                    {
                        result++;
                    }
                }

                return result;
            }

            // Find which equivalent scanner to other has the most beacons in common with this.
            public (int, c_scanner) get_best_equivalence(
                c_scanner other,
                int min_required_equivalent_beacons)
            {
                // Note: originally this took 1:35s on the puzzle input data. However, I can ignore the last few
                // translation operations since I know I only care about a match if there are at least
                // min_required_equivalent_beacons. This brought the timing down to 0:35s.

                c_scanner best_equivalent_other = null;
                int best_equivalent_beacons = min_required_equivalent_beacons - 1;

                // Loop considering each beacon in this is the origin
                c_matrix[] this_translation_operations = get_equivalence_translation_operations();
                for (int i = 0; i < this_translation_operations.Length - min_required_equivalent_beacons; i++)
                // foreach (c_matrix this_translation_operation in this_translation_operations)
                {
                    c_matrix this_translation_operation = this_translation_operations[i];

                    c_scanner this_translated = apply(this_translation_operation);

                    // Loop considering each beacon in other is the origin.
                    c_matrix[] other_translation_operations = other.get_equivalence_translation_operations();
                    for (int j = 0; j < other_translation_operations.Length - min_required_equivalent_beacons; j++)
                    // foreach (c_matrix other_translation_operation in other_translation_operations)
                    {
                        c_matrix other_translation_operation = other_translation_operations[j];

                        c_scanner other_translated = other.apply(other_translation_operation);

                        // Loop considering all possible rotations in other
                        c_scanner[] equivalent_others = other_translated.get_equivalent_rotations();
                        foreach (c_scanner equivalent_other in equivalent_others)
                        {
                            int equivalent_beacons = get_duplicate_beacon_count(this_translated, equivalent_other);

                            // We found some duplicate beacons. Save our best match
                            if (equivalent_beacons > best_equivalent_beacons)
                            {
                                // Un-Translate to get into this's coordinates.
                                best_equivalent_other = equivalent_other.apply(c_matrix.invert_translation(this_translation_operation));
                                best_equivalent_beacons = equivalent_beacons;
                            }
                        }
                    }
                }

                return (best_equivalent_beacons, best_equivalent_other);
            }
        }

        internal static List<c_scanner> parse_input(
            string input,
            bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            List<c_scanner> scanners = new List<c_scanner>();

            while (input_reader.has_more_lines())
            {
                input_reader.read_line(); // ignore the scanner header

                scanners.Add(new c_scanner(input_reader));
            }

            return scanners;
        }

        public static void Day_19_Worker(
            string input,
            bool pretty)
        {
            List<c_scanner> unaligned_scanners = parse_input(input, pretty);
            List<c_scanner> aligned_scanners = new List<c_scanner>();

            // Consider the first scanner aligned with itself.
            aligned_scanners.Add(unaligned_scanners[0]);
            unaligned_scanners.RemoveAt(0);

            // Align the remaining scanners
            const int k_min_aligned_beacons = 12;
            while (unaligned_scanners.Count > 0)
            {
                bool found_alignment = false;

                for (int i = 0; !found_alignment && i < unaligned_scanners.Count; i++)
                {
                    for (int j = 0; !found_alignment && j < aligned_scanners.Count; j++)
                    {
                        // Try aligning these two. If they aligned well enough, consider this scanner aligned.

                        (int alignment_count, c_scanner newly_aligned_scanner) = aligned_scanners[j].get_best_equivalence(unaligned_scanners[i], k_min_aligned_beacons);

                        if (alignment_count >= k_min_aligned_beacons)
                        {
                            found_alignment = true;

                            aligned_scanners.Add(newly_aligned_scanner);
                            unaligned_scanners.RemoveAt(i);

                            Console.WriteLine(
                                "Aligned Scanners = {0}. Unaligned Scanners = {1} ...",
                                aligned_scanners.Count,
                                unaligned_scanners.Count);
                        }
                    }
                }
            }

            // Collect the set of unique beacons

            HashSet<(int, int, int)> unique_beacons = new HashSet<(int, int, int)>();
            foreach (c_scanner scanner in aligned_scanners)
            {
                foreach (c_vector beacon in scanner.beacons)
                {
                    unique_beacons.Add((beacon.x, beacon.y, beacon.z));
                }
            }

            // Determine which two beacons are farthest apart

            c_vector max_a = new c_vector();
            c_vector max_b = new c_vector();
            int max_distance = 0;
            for (int i = 0; i < aligned_scanners.Count - 1; i++)
            {
                for (int j = i + 1; j < aligned_scanners.Count; j++)
                {
                    c_vector a = aligned_scanners[i].scanner;
                    c_vector b = aligned_scanners[j].scanner;

                    int new_distance = a.taxi_distance(b);

                    if (new_distance > max_distance)
                    {
                        max_distance = new_distance;
                        max_a = a;
                        max_b = b;
                    }
                }
            }

            // Output results

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Unique Beacons = {0}", unique_beacons.Count);
            Console.WriteLine();
            Console.WriteLine("Max Beacon Distance = {0}", max_distance);
            Console.WriteLine("[{0}, {1}, {2}]", max_a.x, max_a.y, max_a.z);
            Console.WriteLine("[{0}, {1}, {2}]", max_b.x, max_b.y, max_b.z);
            Console.ResetColor();
        }

        public static void Part_1(
            string input,
            bool pretty)
        {
            Day_19_Worker(input, pretty);
        }

        public static void Part_2(
            string input,
            bool pretty)
        {
            Day_19_Worker(input, pretty);
        }
    }
}
