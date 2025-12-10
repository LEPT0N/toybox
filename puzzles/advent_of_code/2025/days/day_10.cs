using advent_of_code_common.input_reader;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace advent_of_code_2025.days
{
    internal class day_10
    {
        [DebuggerDisplay("{DebuggerDisplay,nq}", Type = "c_button")]
        internal class c_button
        {
            private int[] toggles;

            public string DebuggerDisplay
            {
                get { return "(" + string.Join(',', toggles) + ")"; }
            }

            public c_button(
                string input)
            {
                toggles = input
                    .Substring(1, input.Length - 2)
                    .Split(',')
                    .Select(i => int.Parse(i))
                    .ToArray();
            }

            public int alter(
                int state_index)
            {
                for (int toggle_index = 0; toggle_index < toggles.Length; toggle_index++)
                {
                    state_index ^= (1 << toggles[toggle_index]);
                }

                return state_index;
            }
        }

        [DebuggerDisplay("{DebuggerDisplay,nq}", Type = "c_machine")]
        internal class c_machine
        {
            private bool[] final_state_lights;
            private c_button[] buttons;

            public string DebuggerDisplay
            {
                get { return "["
                        + string.Concat(final_state_lights.Select(l => l ? "#" : "."))
                        + "] "
                        + string.Join(' ', buttons.Select(b => b.DebuggerDisplay)); }
            }

            public c_machine(
                string input_lights,
                string input_buttons)
            {
                final_state_lights = input_lights
                    .Select(i => (i == '#'))
                    .ToArray();

                buttons = input_buttons
                    .Split(' ')
                    .Select(i => new c_button(i))
                    .ToArray();
            }

            [DebuggerDisplay("{DebuggerDisplay,nq}", Type = "c_machine_state")]
            internal class c_machine_state
            {
                private c_machine_state[] neighbors;
                private string lights;
                private int distance_to_final_state;

                public string DebuggerDisplay
                {
                    get { return $"[{lights}] = " + (distance_to_final_state == int.MaxValue ? "-" : distance_to_final_state); }
                }

                public c_machine_state(
                    int button_count,
                    int light_count,
                    int state_index)
                {
                    neighbors = new c_machine_state[button_count];
                    lights = "";
                    distance_to_final_state = int.MaxValue;

                    for (int light_index = 0; light_index < light_count; light_index++)
                    {
                        if ((state_index & (1 << light_index)) != 0)
                        {
                            lights += '#';
                        }
                        else
                        {
                            lights += '.';
                        }
                    }
                }

                public void add_neighbor(
                    c_machine_state neighbor,
                    int button_index)
                {
                    neighbors[button_index] = neighbor;
                }

                public void set_final_state()
                {
                    distance_to_final_state = 0;
                }

                public int get_distance_to_final_state()
                {
                    return distance_to_final_state;
                }

                public void add_promising_neighbors_to_queue(
                    PriorityQueue<c_machine_state, int> queue)
                {
                    int new_neighbor_distance = distance_to_final_state + 1;

                    foreach(c_machine_state neighbor in neighbors)
                    {
                        if (neighbor.distance_to_final_state > new_neighbor_distance)
                        {
                            neighbor.distance_to_final_state = new_neighbor_distance;
                            queue.Enqueue(neighbor, new_neighbor_distance);
                        }
                    }
                }
            }

            public int fewest_buttons_to_target()
            {
                int machine_state_count = (1 << final_state_lights.Length);

                // Build an array of all possible machine states.

                List<c_machine_state> machine_state_list = new List<c_machine_state>();

                for (int state_index = 0; state_index < machine_state_count; state_index++)
                {
                    machine_state_list.Add(new c_machine_state(buttons.Length, final_state_lights.Length, state_index));
                }

                c_machine_state[] machine_states = machine_state_list.ToArray();

                // Connect each machine state to its neighbor states.

                for (int source_state_index = 0; source_state_index < machine_states.Length; source_state_index++)
                {
                    c_machine_state source_state = machine_states[source_state_index];

                    for (int button_index = 0; button_index < buttons.Length; button_index++)
                    {
                        c_button button = buttons[button_index];

                        int target_state_index = button.alter(source_state_index);

                        c_machine_state target_state = machine_states[target_state_index];

                        source_state.add_neighbor(target_state, button_index);
                    }
                }

                // Find the machine state index of the final state.

                int final_state_index = 0;
                
                for (int light_index = 0; light_index < final_state_lights.Length; light_index++)
                {
                    if (final_state_lights[light_index])
                    {
                        final_state_index += (1 << light_index);
                    }
                }

                // Use Dijkstras to find how far away each state is to our final state;
                
                PriorityQueue<c_machine_state, int> states_to_check = new PriorityQueue<c_machine_state, int>();
                machine_state_list[final_state_index].set_final_state();
                states_to_check.Enqueue(machine_state_list[final_state_index], 0);

                while (states_to_check.Count > 0)
                {
                    c_machine_state current_state = states_to_check.Dequeue();

                    current_state.add_promising_neighbors_to_queue(states_to_check);
                }

                // Return the result.

                return machine_state_list[0].get_distance_to_final_state();
            }
        }

        static Regex k_input_line_pattern = new Regex(@"^\[(.+)\]\s(.+)\s\{(.+)\}$");

        internal static c_machine[] parse_input(
            in c_input_reader input_reader,
            in bool pretty)
        {
            List<c_machine> machines = new List<c_machine>();

            while (input_reader.has_more_lines())
            {
                string input_line = input_reader.read_line();

                Match input_line_match = k_input_line_pattern.Match(input_line);

                string input_lights = input_line_match.Groups[1].Value;
                string input_buttons = input_line_match.Groups[2].Value;
                string input_joltages = input_line_match.Groups[3].Value;

                machines.Add(new c_machine(input_lights, input_buttons));
            }

            return machines.ToArray();
        }

        public static void part_1(
            c_input_reader input_reader,
            bool pretty)
        {
            c_machine[] machines = parse_input(input_reader, pretty);

            int result = machines.Select(m => m.fewest_buttons_to_target()).Sum();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }

        public static void part_2(
            c_input_reader input_reader,
            bool pretty)
        {
            // parse_input(input_reader, pretty);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {0}");
            Console.ResetColor();
        }
    }
}
