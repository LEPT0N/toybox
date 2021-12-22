using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace advent_of_code_2021.Days
{
    internal class Day_22
    {
        // A single scalar range on [min, max]
        [DebuggerDisplay("[{min}, {max}]", Type = "c_bounds")]
        internal class c_bounds
        {
            public Int64 min;
            public Int64 max;

            public Int64 range { get { return max - min + 1L; } }

            public c_bounds(Int64 min_input, Int64 max_input)
            {
                min = min_input;
                max = max_input;
            }

            // Returns a bounds that is within both this and other
            public c_bounds intersect(c_bounds other)
            {
                if (max < other.min || min > other.max)
                {
                    return null;
                }

                return new c_bounds(Math.Max(min, other.min), Math.Min(max, other.max));
            }

            // Are this and other the exact same
            public bool is_equivalent(c_bounds other)
            {
                return (this.min == other.min) && (this.max == other.max);
            }

            // Are this and other flush against each other?
            public bool can_combine(c_bounds other)
            {
                return (this.max + 1 == other.min) || (other.max + 1 == this.min);
            }

            // add other into this.
            public void combine(c_bounds other)
            {
                // assert(can_combine(other))

                min = Math.Min(min, other.min);
                max = Math.Max(max, other.max);
            }
        }

        // A 3D range, combining bounds on all three axes.
        [DebuggerDisplay("{bounds_x}, {bounds_y}, {bounds_z} = {cube_count}", Type = "c_cuboid")]
        internal class c_cuboid
        {
            public c_bounds bounds_x;
            public c_bounds bounds_y;
            public c_bounds bounds_z;

            public Int64 range_x { get { return bounds_x.range; } }
            public Int64 range_y { get { return bounds_y.range; } }
            public Int64 range_z { get { return bounds_z.range; } }
            public Int64 cube_count {  get { return range_x * range_y * range_z; } }

            public c_cuboid(
                c_bounds bounds_x_input,
                c_bounds bounds_y_input,
                c_bounds bounds_z_input)
            {
                bounds_x = bounds_x_input;
                bounds_y = bounds_y_input;
                bounds_z = bounds_z_input;
            }

            // Returns a cuboid that is within both this and other
            public c_cuboid intersect(c_cuboid other)
            {
                c_bounds new_bounds_x = bounds_x.intersect(other.bounds_x);
                c_bounds new_bounds_y = bounds_y.intersect(other.bounds_y);
                c_bounds new_bounds_z = bounds_z.intersect(other.bounds_z);

                if (new_bounds_x == null || new_bounds_y == null || new_bounds_z == null)
                {
                    return null;
                }

                return new c_cuboid(new_bounds_x, new_bounds_y, new_bounds_z);
            }

            // Returns a list of cuboids defining this, then removing other.
            public List<c_cuboid> minus(c_cuboid other)
            {
                // Construct a set of cuboids representing all space outside of 'other'

                List<c_cuboid> valid_spaces = new List<c_cuboid>();

                c_bounds[] valid_bounds_list_x =
                {
                    new c_bounds(Int64.MinValue, other.bounds_x.min - 1L),
                    new c_bounds(other.bounds_x.min, other.bounds_x.max),
                    new c_bounds(other.bounds_x.max + 1L, Int64.MaxValue),
                };

                c_bounds[] valid_bounds_list_y =
                {
                    new c_bounds(Int64.MinValue, other.bounds_y.min - 1L),
                    new c_bounds(other.bounds_y.min, other.bounds_y.max),
                    new c_bounds(other.bounds_y.max + 1L, Int64.MaxValue),
                };

                c_bounds[] valid_bounds_list_z =
                {
                    new c_bounds(Int64.MinValue, other.bounds_z.min - 1L),
                    new c_bounds(other.bounds_z.min, other.bounds_z.max),
                    new c_bounds(other.bounds_z.max + 1L, Int64.MaxValue),
                };

                for(int x = 0; x < 3; x++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        for (int z = 0; z < 3; z++)
                        {
                            if (x != 1 || y != 1 || z != 1)
                            {
                                valid_spaces.Add(new c_cuboid(
                                    valid_bounds_list_x[x],
                                    valid_bounds_list_y[y],
                                    valid_bounds_list_z[z]));
                            }
                        }
                    }
                }

                // Intersect each valid space with 'this' and any non-null results are returned

                List<c_cuboid> results = new List<c_cuboid>();

                foreach(c_cuboid valid_space in valid_spaces)
                {
                    c_cuboid potential_result = this.intersect(valid_space);

                    if (potential_result != null)
                    {
                        results.Add(potential_result);
                    }
                }

                return results;
            }

            // It this and other share the same bounds on two axes and on the third axis they are flush against each other, then add other into this.
            public bool try_merge(c_cuboid other)
            {
                if (bounds_x.is_equivalent(other.bounds_x) &&
                    bounds_y.is_equivalent(other.bounds_y) &&
                    bounds_z.can_combine(other.bounds_z))
                {
                    bounds_z.combine(other.bounds_z);
                    return true;
                }
                else if (bounds_x.is_equivalent(other.bounds_x) &&
                    bounds_z.is_equivalent(other.bounds_z) &&
                    bounds_y.can_combine(other.bounds_y))
                {
                    bounds_y.combine(other.bounds_y);
                    return true;
                }
                else if (bounds_y.is_equivalent(other.bounds_y) &&
                    bounds_z.is_equivalent(other.bounds_z) &&
                    bounds_x.can_combine(other.bounds_x))
                {
                    bounds_x.combine(other.bounds_x);
                    return true;
                }

                return false;
            }
        }

        // Naive way to track total cubes 'on'. Doesn't scale well.
        [DebuggerDisplay("{cubes_on_count}", Type = "c_reactor_core")]
        internal class c_reactor_core
        {
            private bool[][][] cubes;

            private c_cuboid bounds = new c_cuboid(
                new c_bounds(-50L, 50L),
                new c_bounds(-50L, 50L),
                new c_bounds(-50L, 50L));

            public Int64 cubes_on_count
            {
                get { return cubes.Sum(x => x.Sum(y => y.Sum(z => z ? 1L : 0L))); }
            }

            public c_reactor_core(c_input_reader input_reader)
            {
                // Expand cubes to the corrext size and fill with 'false's.
                cubes = new bool[bounds.range_x][][];
                for (int x = 0; x < cubes.Length; x++)
                {
                    cubes[x] = new bool[bounds.range_y][];

                    for (int y = 0; y < cubes[x].Length; y++)
                    {
                        cubes[x][y] = new bool[bounds.range_z];
                    }
                }

                Regex input_regex = new Regex(@"^(\w+) x=(-?\d+)\.\.(-?\d+),y=(-?\d+)\.\.(-?\d+),z=(-?\d+)\.\.(-?\d+)$");

                // Parse each line as a cuboid
                while (input_reader.has_more_lines())
                {
                    string input_line = input_reader.read_line();

                    Match match = input_regex.Match(input_line);

                    bool enabled_value = match.Groups[1].Value == "on";

                    c_cuboid input_cuboid = new c_cuboid(
                        new c_bounds(
                            Int64.Parse(match.Groups[2].Value),
                            Int64.Parse(match.Groups[3].Value)),
                        new c_bounds(
                            Int64.Parse(match.Groups[4].Value),
                            Int64.Parse(match.Groups[5].Value)),
                        new c_bounds(
                            Int64.Parse(match.Groups[6].Value),
                            Int64.Parse(match.Groups[7].Value)));

                    c_cuboid valid_cuboid = input_cuboid.intersect(bounds);

                    if (valid_cuboid != null)
                    {
                        // set/clear each cube one at a time in our cuboid.

                        for (Int64 x = valid_cuboid.bounds_x.min; x <= valid_cuboid.bounds_x.max; x++)
                        {
                            for (Int64 y = valid_cuboid.bounds_y.min; y <= valid_cuboid.bounds_y.max; y++)
                            {
                                for (Int64 z = valid_cuboid.bounds_z.min; z <= valid_cuboid.bounds_z.max; z++)
                                {
                                    cubes[x - bounds.bounds_x.min][y - bounds.bounds_y.min][z - bounds.bounds_z.min] = enabled_value;
                                }
                            }
                        }
                    }
                }
            }
        }

        internal static c_reactor_core parse_input_1(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            c_reactor_core reactor_core = new c_reactor_core(input_reader);

            return reactor_core;
        }

        public static void Part_1(
            string input,
            bool pretty)
        {
            c_reactor_core reactor_core = parse_input_1(input, pretty);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", reactor_core.cubes_on_count);
            Console.ResetColor();
        }

        // More scalable collection of cubes. Keep track of a list of cuboids where all cubes within are enabled.
        [DebuggerDisplay("{cuboids_count} cuboids totalling {cubes_on_count} cubes", Type = "c_reactor_core")]
        internal class c_large_reactor_core
        {
            private List<c_cuboid> enabled_cuboids;

            public c_large_reactor_core()
            {
                enabled_cuboids = new List<c_cuboid>();
            }

            public Int64 cubes_on_count { get { return enabled_cuboids.Sum(x => x.cube_count); } }
            public int cuboids_count { get { return enabled_cuboids.Count; } }

            // Add all cubes in a cuboid to our enabled_cuboids.
            public void enable_cuboid(c_cuboid input_cuboid)
            {
                List<c_cuboid> input_cuboids = new List<c_cuboid>();
                input_cuboids.Add(input_cuboid);

                // Check against each existing cuboid that is already enabled.
                foreach (c_cuboid enabled_cuboid in enabled_cuboids)
                {
                    // Cut the existing cuboid out of the input cuboid

                    List<c_cuboid> temp_cuboids = new List<c_cuboid>();

                    foreach(c_cuboid cuboid in input_cuboids)
                    {
                        add_range(temp_cuboids, cuboid.minus(enabled_cuboid));
                    }

                    input_cuboids = temp_cuboids;
                }

                // Add the resulting cut-up cuboid into our list of enabled_cuboids
                add_range(enabled_cuboids, input_cuboids);
            }

            // Cut up or remove any existing cuboids that overlap with input_cuboid
            public void disable_cuboid(c_cuboid input_cuboid)
            {
                List<c_cuboid> result_cuboids = new List<c_cuboid>();

                // Check against each existing cuboid
                foreach (c_cuboid enabled_cuboid in enabled_cuboids)
                {
                    // Cut out the input cuboid from the existing cuboid.
                    add_range(result_cuboids, enabled_cuboid.minus(input_cuboid));
                }

                // The resulting list is our new set of enabled cuboids.
                enabled_cuboids = result_cuboids;
            }

            // Add the list of additions to the list of cuboids
            private void add_range(List<c_cuboid> cuboids, List<c_cuboid> additions)
            {
                // Loop through each addition
                foreach(c_cuboid addition in additions)
                {
                    bool merged = false;

                    // for each addition, check against each of the cuboids
                    foreach(c_cuboid cuboid in cuboids)
                    {
                        // If they can merge, merge them together.
                        merged = cuboid.try_merge(addition);

                        if (merged)
                        {
                            break;
                        }
                    }

                    // If this addition didn't merge with anything, add it to the list instead.
                    if (!merged)
                    {
                        cuboids.Add(addition);
                    }
                }
            }
        }

        internal static c_large_reactor_core parse_input_2(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            c_large_reactor_core reactor_core = new c_large_reactor_core();

            Regex input_regex = new Regex(@"^(\w+) x=(-?\d+)\.\.(-?\d+),y=(-?\d+)\.\.(-?\d+),z=(-?\d+)\.\.(-?\d+)$");

            // Loop through each input line
            while (input_reader.has_more_lines())
            {
                string input_line = input_reader.read_line();

                // Convert the string to a cuboid
                Match match = input_regex.Match(input_line);

                bool enabled_value = match.Groups[1].Value == "on";

                c_cuboid input_cuboid = new c_cuboid(
                    new c_bounds(
                        Int64.Parse(match.Groups[2].Value),
                        Int64.Parse(match.Groups[3].Value)),
                    new c_bounds(
                        Int64.Parse(match.Groups[4].Value),
                        Int64.Parse(match.Groups[5].Value)),
                    new c_bounds(
                        Int64.Parse(match.Groups[6].Value),
                        Int64.Parse(match.Groups[7].Value)));

                if (pretty)
                {
                    Console.WriteLine("Input = {0}", input_line);
                }

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                // process the input against all enabled cuboids in the reactor
                if (enabled_value)
                {
                    reactor_core.enable_cuboid(input_cuboid);
                }
                else
                {
                    reactor_core.disable_cuboid(input_cuboid);
                }

                stopwatch.Stop();


                if (pretty)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine(
                        "Time taken = {0} Cubes on = {1} Cuboids = {2}",
                        stopwatch.Elapsed,
                        reactor_core.cubes_on_count,
                        reactor_core.cuboids_count);
                    Console.WriteLine();
                    Console.ResetColor();
                }
            }

            return reactor_core;
        }

        public static void Part_2(
            string input,
            bool pretty)
        {
            c_large_reactor_core reactor_core = parse_input_2(input, pretty);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", reactor_core.cubes_on_count);
            Console.ResetColor();
        }
    }
}
