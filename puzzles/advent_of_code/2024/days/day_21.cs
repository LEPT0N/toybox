using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using advent_of_code_common.extensions;
using advent_of_code_common.input_reader;
using advent_of_code_common.int_math;

namespace advent_of_code_2024.days
{
    internal class day_21
    {
        [DebuggerDisplay("{score} = {sequence}", Type = "c_keypad")]
        internal class c_scored_sequence
        {
            public readonly string sequence;
            public readonly int score;

            public c_scored_sequence(string input_sequence, int input_score)
            {
                sequence = input_sequence;
                score = input_score;
            }
        }

        [DebuggerDisplay("", Type = "c_keypad")]
        internal class c_keypad
        {
            private Dictionary<char, c_vector> keys = new Dictionary<char, c_vector>();

            public void add_key(char button_name, c_vector button_position)
            {
                keys[button_name] = button_position;
            }

            // Valid sequences can't wander away from the keys.
            private bool is_valid_sequence(c_vector start_position, e_direction[] directions)
            {
                c_vector position = new c_vector(start_position);

                foreach (e_direction direction in directions)
                {
                    position = position.add(direction);

                    if (!keys.Values.Any(key_position => key_position.equal_to(position)))
                    {
                        return false;
                    }
                }

                return true;
            }

            // Interpret an array of directions as a sequence (ending in 'A' to enter a value).
            private static string get_sequence_string(e_direction[] sequence)
            {
                return string.Join("", sequence.Select(d => d.to_char())) + 'A';
            }

            // Sum up the total distance between 'A', then all the buttons. (the sequence should already end in 'A')
            public int get_sequence_score(string sequence)
            {
                List<c_vector> positions = new List<c_vector>();
                positions.Add(keys['A']);

                foreach (char element in sequence)
                {
                    positions.Add(keys[element]);
                }

                int result = 0;

                for (int i = 0; i < positions.Count - 1; i++)
                {
                    result += positions[i].taxi_distance(positions[i + 1]);
                }

                return result;
            }

            private static int get_sequence_score(c_keypad arrow_keypad, e_direction[] sequence)
            {
                return arrow_keypad.get_sequence_score(get_sequence_string(sequence));
            }

            // Find all of the shortest sequences that move from start to end on the given keypad.
            private string[] get_best_sequences(
                c_keypad arrow_keypad,
                char start_button,
                char end_button)
            {
                c_vector start_position = keys[start_button];
                c_vector end_position = keys[end_button];

                // Find one set of directions from start to end.
                e_direction[] base_sequence = end_position
                    .subtract(start_position)
                    .to_directions()
                    .ToArray();

                // Find all permutations of that initial sequence.
                e_direction[][] all_sequences = base_sequence.get_all_permutations();

                // Discard permutations that are invalid.
                e_direction[][] valid_sequences = all_sequences
                    .Where(sequence => is_valid_sequence(start_position, sequence))
                    .ToArray();

                // Score each permutation.
                c_scored_sequence[] scored_sequences = valid_sequences
                    .Select(sequence => new c_scored_sequence(
                        get_sequence_string(sequence),
                        get_sequence_score(arrow_keypad, sequence)))
                    .ToArray();

                // Only take the permutations with the lowest score.
                int best_sequence_score = scored_sequences.Min(sequence => sequence.score);

                string[] best_sequences = scored_sequences
                    .Where(sequence => sequence.score == best_sequence_score)
                    .Select(sequence => sequence.sequence)
                    .ToArray();

                return best_sequences;
            }

            // Find all of the best sequences that press the given buttons on the keypad.
            public string[] get_best_sequences(
                c_keypad arrow_keypad,
                string buttons)
            {
                List<string[]> subsequences = new List<string[]>();

                buttons = 'A' + buttons;

                for (int i = 0; i < buttons.Length - 1; i++)
                {
                    subsequences.Add(get_best_sequences(
                        arrow_keypad,
                        buttons[i],
                        buttons[i + 1]));
                }

                // Once we have all of the possibilities that get us from each button to the next,
                // combine them in all possible ways to find our final list of possibilities.
                string[] result = subsequences.get_all_combinations().ToArray();

                return result;
            }

            // Same as above but with caching of results.
            public Int64 get_best_sequences_2(
                c_keypad arrow_keypad,
                string buttons,
                int recursion,
                Dictionary<(string, int), Int64> cached_results)
            {
                if (recursion == 0)
                {
                    // If we're done with recursion then just return the number of buttons.
                    return buttons.Length;
                }

                if (cached_results.ContainsKey((buttons, recursion)))
                {
                    // Yay use a cached result!
                    return cached_results[(buttons, recursion)];
                }

                // Sequences always start at 'A' but they don't have an 'A' at the start of the input.
                string button_path = 'A' + buttons;

                Int64 result = 0;

                for (int i = 0; i < button_path.Length - 1; i++)
                {
                    // Find all of the ways to get from button i to button i + 1
                    string[] subsequences = get_best_sequences(
                        arrow_keypad,
                        button_path[i],
                        button_path[i + 1]);

                    // Recurse to find out how long this subsequence will become after we look through all levels of recursion.

                    Int64 min_subsequence_result = Int64.MaxValue;

                    foreach (string subsequence in subsequences)
                    {
                        Int64 subsequence_result = arrow_keypad.get_best_sequences_2(
                            arrow_keypad,
                            subsequence,
                            recursion - 1,
                            cached_results);

                        min_subsequence_result = Math.Min(min_subsequence_result, subsequence_result);
                    }

                    // Note down that this subsection ballooned out to the smallest recursive result that we could find.
                    result += min_subsequence_result;
                }

                // Cache our result in case it's needed again.
                cached_results[(buttons, recursion)] = result;

                return result;
            }
        }

        internal static c_keypad create_number_keypad()
        {
            c_keypad keypad = new c_keypad();

            keypad.add_key('7', new c_vector(0, 0));
            keypad.add_key('8', new c_vector(0, 1));
            keypad.add_key('9', new c_vector(0, 2));

            keypad.add_key('4', new c_vector(1, 0));
            keypad.add_key('5', new c_vector(1, 1));
            keypad.add_key('6', new c_vector(1, 2));

            keypad.add_key('1', new c_vector(2, 0));
            keypad.add_key('2', new c_vector(2, 1));
            keypad.add_key('3', new c_vector(2, 2));

            keypad.add_key('0', new c_vector(3, 1));
            keypad.add_key('A', new c_vector(3, 2));

            return keypad;
        }

        internal static c_keypad create_arrow_keypad()
        {
            c_keypad keypad = new c_keypad();

            keypad.add_key('^', new c_vector(0, 1));
            keypad.add_key('A', new c_vector(0, 2));

            keypad.add_key('<', new c_vector(1, 0));
            keypad.add_key('v', new c_vector(1, 1));
            keypad.add_key('>', new c_vector(1, 2));

            return keypad;
        }

        internal static string[] parse_input(
            in c_input_reader input_reader,
            in bool pretty)
        {
            List<string> codes = new List<string>();

            while (input_reader.has_more_lines())
            {
                codes.Add(input_reader.read_line());
            }

            return codes.ToArray();
        }

        // After finding all sequences, discard any that don't have the best score.
        internal static string[] get_best_sequences(
            c_keypad arrow_keypad,
            string[] sequences)
        {
            c_scored_sequence[] scored_sequences = sequences
                .Select(sequence => new c_scored_sequence(
                    sequence,
                    arrow_keypad.get_sequence_score(sequence)))
                .ToArray();

            int best_sequence_score = scored_sequences.Min(sequence => sequence.score);

            string[] best_sequences = scored_sequences
                .Where(sequence => sequence.score == best_sequence_score)
                .Select(sequence => sequence.sequence)
                .ToArray();

            return best_sequences;
        }

        public static void part_1(
            c_input_reader input_reader,
            bool pretty)
        {
            string[] codes = parse_input(input_reader, pretty);

            c_keypad number_keypad = create_number_keypad();
            c_keypad arrow_keypad = create_arrow_keypad();

            Int64 result = 0;

            // Loop through each code.
            foreach (string code in codes)
            {
                // Calculate robot_1
                string[] robot_1_sequences;
                {
                    string[] robot_1_hopeful_sequences = number_keypad.get_best_sequences(arrow_keypad, code);

                    robot_1_sequences = get_best_sequences(arrow_keypad, robot_1_hopeful_sequences);
                }

                // Calculate robot_2
                string[] robot_2_sequences;
                {
                    List<string> robot_2_hopeful_sequences = new List<string>();

                    foreach (string robot_1_sequence in robot_1_sequences)
                    {
                        robot_2_hopeful_sequences.AddRange(arrow_keypad.get_best_sequences(arrow_keypad, robot_1_sequence));
                    }

                    robot_2_sequences = get_best_sequences(arrow_keypad, robot_2_hopeful_sequences.ToArray());
                }

                // Calculate human
                string[] human_sequences;
                {
                    List<string> human_hopeful_sequences = new List<string>();

                    foreach (string robot_2_sequence in robot_2_sequences)
                    {
                        human_hopeful_sequences.AddRange(arrow_keypad.get_best_sequences(arrow_keypad, robot_2_sequence));
                    }

                    human_sequences = get_best_sequences(arrow_keypad, human_hopeful_sequences.ToArray());
                }

                // Output any human sequence we found. They're all equally good.

                string human_sequence = human_sequences[0];

                Int64 human_sequence_length = human_sequence.Length;
                Int64 code_value = Int64.Parse(code.Substring(0, code.Length - 1));

                if (pretty)
                {
                    Console.WriteLine($"{code}");
                    Console.WriteLine($"    = {human_sequence}");
                    Console.WriteLine();
                    Console.WriteLine($"        ({human_sequence_length} * {code_value})");
                    Console.WriteLine();
                }

                Int64 code_Result = human_sequence_length * code_value;

                result += code_Result;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }

        public static void part_2(
            c_input_reader input_reader,
            bool pretty)
        {
            string[] codes = parse_input(input_reader, pretty);

            c_keypad number_keypad = create_number_keypad();
            c_keypad arrow_keypad = create_arrow_keypad();

            Int64 result = 0;

            const int k_robot_chain = 26;

            Dictionary<(string, int), Int64> cached_results = new Dictionary<(string, int), Int64>();

            // Loop through each code.
            foreach (string code in codes)
            {
                // Recursively determine how big it will get.
                Int64 human_sequence_length = number_keypad.get_best_sequences_2(arrow_keypad, code, k_robot_chain, cached_results);

                Int64 code_value = Int64.Parse(code.Substring(0, code.Length - 1));

                if (pretty)
                {
                    Console.WriteLine($"{code}");
                    Console.WriteLine();
                    Console.WriteLine($"        ({human_sequence_length} * {code_value})");
                    Console.WriteLine();
                }

                Int64 code_Result = human_sequence_length * code_value;

                result += code_Result;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }
    }
}
