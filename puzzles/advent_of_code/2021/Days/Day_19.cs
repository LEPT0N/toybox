using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using advent_of_code_common.int_math;

namespace advent_of_code_2021.Days
{
    internal class Day_19
    {
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

                // Loop through each unaligned scanner
                for (int i = 0; !found_alignment && i < unaligned_scanners.Count; i++)
                {
                    // Check each unaligned scanner against all aligned scanners in parallel.

                    ConcurrentQueue<c_scanner> job_input = new ConcurrentQueue<c_scanner>();
                    ConcurrentQueue<c_scanner> job_output = new ConcurrentQueue<c_scanner>();

                    foreach (c_scanner scanner in aligned_scanners)
                    {
                        job_input.Enqueue(scanner);
                    }

                    // Each worker grabs an aligned scanner out of the queue to check alignment. If any output is found, all workers stop grabbing new input.
                    Action worker = () =>
                    {
                        c_scanner aligned_scanner;
                        while (job_input.TryDequeue(out aligned_scanner) && job_output.Count == 0)
                        {
                            (int alignment_count, c_scanner newly_aligned_scanner) = aligned_scanner.get_best_equivalence(unaligned_scanners[i], k_min_aligned_beacons);

                            if (alignment_count >= k_min_aligned_beacons)
                            {
                                job_output.Enqueue(newly_aligned_scanner);
                                return;
                            }
                        }
                    };

                    Parallel.Invoke(worker, worker, worker, worker, worker, worker, worker, worker);

                    // If any alignment was found, take the first one and transfer it from unaligned_scanners to aligned_scanners
                    c_scanner newly_aligned_scanner;
                    if (job_output.TryDequeue(out newly_aligned_scanner))
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
