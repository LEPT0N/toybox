using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using advent_of_code_common.extensions;
using advent_of_code_common.input_reader;
using advent_of_code_common.min_heap;

namespace advent_of_code_2022.days
{
    internal class day_16
    {
        [DebuggerDisplay("{focus}", Type = "c_valve_comparer")]
        public class c_valve_comparer : IComparer<c_valve>
        {
            private c_valve focus;

            public c_valve_comparer(c_valve f)
            {
                focus = f;
            }

            public int Compare(c_valve a, c_valve b)
            {
                return a.get_distance_to(focus).CompareTo(b.get_distance_to(focus));
            }
        }

        [DebuggerDisplay("{name} flow is {flow_rate}", Type = "c_valve")]
        internal class c_valve
        {
            private string name;
            private int flow_rate;
            private string[] neighbor_names;
            private Dictionary<string, int> distances;

            public c_valve(string n, int fr, string[] ns)
            {
                name = n;
                flow_rate = fr;
                neighbor_names = ns;
                distances = new Dictionary<string, int>();

                active = true;
            }

            public bool functional { get { return flow_rate > 0; } }

            public bool active { get; set; }

            public int get_distance_to(c_valve other)
            {
                return distances[other.name];
            }

            public void compute_distances(Dictionary<string, c_valve> valves)
            {
                // Initialize all valves distances to the current valve.
                valves.Values.for_each(valve => valve.distances[this.name] = int.MaxValue);
                this.neighbor_names.for_each(neighbor_name => valves[neighbor_name].distances[this.name] = 1);
                distances[this.name] = 0;

                // Create a min heap of valves to search, initializing with the current valve's neighbors.
                c_min_heap<c_valve> min_heap = new c_min_heap<c_valve>(new c_valve_comparer(this));
                this.neighbor_names.for_each(neighbor_name => min_heap.add(valves[neighbor_name]));

                // Loop as long as the heap isn't empty
                while (!min_heap.empty())
                {
                    // Pop a value off of the heap
                    c_valve current = min_heap.remove();
                    int current_distance_to_this = current.distances[this.name];

                    // Loop through that heap's neighbors
                    foreach (string neighbor_name in current.neighbor_names)
                    {
                        // If we find a neighbor who's distance to this can be updated, then do so and add it to the heap.
                        c_valve neighbor = valves[neighbor_name];
                        if (neighbor.distances[this.name] > current_distance_to_this + 1)
                        {
                            neighbor.distances[this.name] = current_distance_to_this + 1;
                            min_heap.add(neighbor);
                        }
                    }
                }
            }

            public void display_distances()
            {
                Console.WriteLine($"{name} (flow = {flow_rate})");

                foreach (KeyValuePair<string, int> other in distances.ToArray())
                {
                    Console.WriteLine($"\t{other.Key} distance = {other.Value}");
                }
            }

            public (int max_pressure, Dictionary<string, c_valve> valves_opened) step(
                Dictionary<string, c_valve> all_valves,
                Dictionary<string, c_valve> opened_valves,
                int minutes_remaining)
            {
                int pressure_released = 0;
                c_valve opened_valve = null;

                // Open the current valve.
                if (this.flow_rate > 0 && minutes_remaining > 0)
                {
                    minutes_remaining--;

                    pressure_released += opened_valves.Values.Sum(valve => valve.flow_rate);

                    opened_valves[this.name] = this;
                    opened_valve = this;
                }

                int max_child_pressure = 0;
                Dictionary<string, c_valve> child_valves_opened_for_max_child_pressure = new Dictionary<string, c_valve>();

                if (minutes_remaining > 0)
                {
                    // Loop through all other mapped valves
                    foreach (KeyValuePair<string, int> other in this.distances.ToArray())
                    {
                        string child_name = other.Key;
                        int child_distance = other.Value;
                        bool is_child_open = opened_valves.ContainsKey(child_name);
                        c_valve child = all_valves[child_name];

                        // Find one that is open and we can reach
                        if (!is_child_open && minutes_remaining >= child_distance && child.active)
                        {
                            // Add the pressure on the journey to the other
                            int child_pressure = child_distance * opened_valves.Values.Sum(valve => valve.flow_rate);

                            // Find the pressure released after arriving at the other.
                            var child_result = child.step(all_valves, opened_valves, minutes_remaining - child_distance);

                            // Add them together and update our max if appropriate.
                            child_pressure += child_result.max_pressure;

                            if (child_pressure > max_child_pressure)
                            {
                                max_child_pressure = child_pressure;
                                child_valves_opened_for_max_child_pressure = child_result.valves_opened;
                            }
                        }
                    }

                    // Compare child pressures to pressure added from doing nothing.
                    int do_nothing_pressure = minutes_remaining * opened_valves.Values.Sum(valve => valve.flow_rate);

                    if (do_nothing_pressure > max_child_pressure)
                    {
                        max_child_pressure = do_nothing_pressure;
                        child_valves_opened_for_max_child_pressure = new Dictionary<string, c_valve>();
                    }
                }

                // Close this valve
                opened_valves.Remove(this.name);

                // Return results

                if (opened_valve != null)
                {
                    child_valves_opened_for_max_child_pressure.Add(opened_valve.name, opened_valve);
                }

                return (pressure_released + max_child_pressure, child_valves_opened_for_max_child_pressure);
            }

            public static int step_with_elephant(
                Dictionary<string, c_valve> all_valves,
                c_valve target_1, int distance_to_1,
                c_valve target_2, int distance_to_2,
                Dictionary<string, c_valve> opened_valves,
                int minutes_remaining)
            {
                int pressure_released = 0;

                // Walk both until at least one arrives at their target.

                while (distance_to_1 > 0 && distance_to_2 > 0 && minutes_remaining > 0)
                {
                    distance_to_1--;
                    distance_to_2--;
                    minutes_remaining--;

                    pressure_released += opened_valves.Values.Sum(valve => valve.flow_rate);
                }

                // Open valves for whichever arrived on target.

                bool opened_1 = false;
                bool opened_2 = false;

                if (distance_to_1 == 0 && target_1.flow_rate > 0 && minutes_remaining > 0 && !opened_valves.ContainsKey(target_1.name))
                {
                    opened_1 = true;
                }

                if (distance_to_2 == 0 && target_2.flow_rate > 0 && minutes_remaining > 0 && !opened_valves.ContainsKey(target_2.name))
                {
                    opened_2 = true;
                }

                if (opened_1 || opened_2)
                {
                    pressure_released += opened_valves.Values.Sum(valve => valve.flow_rate);

                    if (opened_1)
                    {
                        opened_valves[target_1.name] = target_1;
                    }

                    if (opened_2)
                    {
                        opened_valves[target_2.name] = target_2;
                    }

                    // If just one opened, let the other walk a step.
                    if (distance_to_1 > 0)
                    {
                        distance_to_1--;
                    }

                    if (distance_to_2 > 0)
                    {
                        distance_to_2--;
                    }

                    minutes_remaining--;
                }

                // See if someone can start moving somewhere else.

                int max_child_pressure = 0;

                if (minutes_remaining > 0)
                {
                    // Find possible targets for both

                    List<Tuple<c_valve, int>> targets_1 = new List<Tuple<c_valve, int>>();

                    if (distance_to_1 > 0 || (target_1.flow_rate > 0 && !opened_valves.ContainsKey(target_1.name)))
                    {
                        // Still walking to current target
                        targets_1.Add(new Tuple<c_valve, int>(target_1, distance_to_1));
                    }
                    else
                    {
                        // Could possibly move to many targets
                        foreach (KeyValuePair<string, int> other in target_1.distances.ToArray())
                        {
                            string target_name = other.Key;
                            int target_distance = other.Value;
                            bool is_target_open = opened_valves.ContainsKey(target_name);

                            if (!is_target_open && minutes_remaining >= target_distance)
                            {
                                targets_1.Add(new Tuple<c_valve, int>(all_valves[other.Key], other.Value));
                            }
                        }
                    }

                    List<Tuple<c_valve, int>> targets_2 = new List<Tuple<c_valve, int>>();

                    if (distance_to_2 > 0 || (target_2.flow_rate > 0 && !opened_valves.ContainsKey(target_2.name)))
                    {
                        // Still walking to current target
                        targets_2.Add(new Tuple<c_valve, int>(target_2, distance_to_2));
                    }
                    else
                    {
                        // Could possibly move to many targets
                        foreach (KeyValuePair<string, int> other in target_2.distances.ToArray())
                        {
                            string target_name = other.Key;
                            int target_distance = other.Value;
                            bool is_target_open = opened_valves.ContainsKey(target_name);

                            if (!is_target_open && minutes_remaining >= target_distance)
                            {
                                targets_2.Add(new Tuple<c_valve, int>(all_valves[other.Key], other.Value));
                            }
                        }
                    }

                    // Loop through all target possibilities.
                    // This is the perf killer.

                    foreach (Tuple<c_valve, int> child_target_1 in targets_1)
                    {
                        foreach (Tuple<c_valve, int> child_target_2 in targets_2)
                        {
                            int child_pressure = step_with_elephant(
                                all_valves,
                                child_target_1.Item1, child_target_1.Item2,
                                child_target_2.Item1, child_target_2.Item2,
                                opened_valves,
                                minutes_remaining);

                            max_child_pressure = Math.Max(max_child_pressure, child_pressure);
                        }
                    }

                    // Compare child pressures to pressure added from doing nothing.
                    int do_nothing_pressure = minutes_remaining * opened_valves.Values.Sum(valve => valve.flow_rate);

                    max_child_pressure = Math.Max(max_child_pressure, do_nothing_pressure);
                }

                // Close any opened valves

                if (opened_1)
                {
                    opened_valves.Remove(target_1.name);
                }

                if (opened_2)
                {
                    opened_valves.Remove(target_2.name);
                }

                // Return result

                return pressure_released + max_child_pressure;
            }
        }

        internal static Regex input_regex = new Regex(@"^Valve (\w{2}) has flow rate=(\d+); tunnels? leads? to valves? (.+)$");

        internal static Dictionary<string, c_valve> parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            Dictionary<string, c_valve> valves = new Dictionary<string, c_valve>();

            while (input_reader.has_more_lines())
            {
                string input_line = input_reader.read_line();
                Match match = input_regex.Match(input_line);

                if (!match.Success)
                {
                    throw new Exception($"Bad input: {input}");
                }

                valves[match.Groups[1].Value] = new c_valve(
                    match.Groups[1].Value,
                    int.Parse(match.Groups[2].Value),
                    match.Groups[3].Value.Split(", "));
            }

            return valves;
        }

        public static void part_1(
            string input,
            bool pretty)
        {
            Dictionary<string, c_valve> valves = parse_input(input, pretty);

            c_valve[] functional_valves = valves.Values.Where(valve => valve.functional).ToArray();

            functional_valves.for_each(valve => valve.compute_distances(valves));

            if (pretty)
            {
                functional_valves.for_each(valve => valve.display_distances());
            }

            var result = valves["AA"].step(valves, new Dictionary<string, c_valve>(), 30);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", result.max_pressure);
            Console.ResetColor();
        }

        public static void part_2(
            string input,
            bool pretty)
        {
            Dictionary<string, c_valve> valves = parse_input(input, pretty);

            c_valve[] functional_valves = valves.Values.Where(valve => valve.functional).ToArray();

            functional_valves.for_each(valve => valve.compute_distances(valves));

            if (pretty)
            {
                functional_valves.for_each(valve => valve.display_distances());
            }

            int result = 0;

            if (pretty)
            {
                // Works but too slow for real input

                int combined_result = c_valve.step_with_elephant(
                    valves,
                    valves["AA"], 0,
                    valves["AA"], 0,
                    new Dictionary<string, c_valve>(),
                    26);

                result = combined_result;
            }
            else
            {
                // The human and the elephant each open disjoint sets of valves.
                // That means we can run each separately and combine the flows from both of their runs to get our total.
                // We just need to loop over all combinations of which actor is allowed to open which valves.

                int max_pressure = 0;

                int min = 0;
                min.set_bit(0);
                int max = 0;
                max.set_bit(functional_valves.Length);

                DateTime previous = DateTime.UtcNow;
                TimeSpan display_period = TimeSpan.FromSeconds(1);

                for(int human_allowed_valves = min; human_allowed_valves < max; human_allowed_valves++)
                {
                    // Set which valves the human is allowed to use.
                    for (int valve_index = 0; valve_index < functional_valves.Length; valve_index++)
                    {
                        functional_valves[valve_index].active = human_allowed_valves.test_bit(valve_index);
                    }

                    // Simulate the human
                    var human_result = valves["AA"].step(valves, new Dictionary<string, c_valve>(), 26);

                    // Set which valves the elephant is allowed to use.
                    functional_valves.for_each(valve => valve.active = true);
                    human_result.valves_opened.Values.for_each(valve => valve.active = false);

                    // Simulate the elephant
                    var elephant_result = valves["AA"].step(valves, new Dictionary<string, c_valve>(), 26);

                    // See if this is better
                    max_pressure = Math.Max(max_pressure, human_result.max_pressure + elephant_result.max_pressure);

                    DateTime now = DateTime.UtcNow;
                    if (now - previous >= display_period)
                    {
                        previous = now;

                        Console.WriteLine($"Checked {human_allowed_valves} of {max} cases. Current max pressure found is {max_pressure}");
                    }
                }

                result = max_pressure;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", result);
            Console.ResetColor();
        }
    }
}
