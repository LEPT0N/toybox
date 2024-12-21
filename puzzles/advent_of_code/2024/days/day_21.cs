using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

            // TODO move this down
            private static int get_sequence_score(c_keypad arrow_keypad, e_direction[] sequence)
            {
                // return arrow_keypad.get_sequence_score(string.Join("", directions.Select(d => d.to_char())) + 'A');

                return arrow_keypad.get_sequence_score(get_sequence_string(sequence));

                // Sum up the total distance between 'A', then all the direciton buttons, then 'A' again.

                //List<c_vector> positions = new List<c_vector>();
                //positions.Add(arrow_keypad.keys['A']);

                //foreach (e_direction direction in directions)
                //{
                //    positions.Add(arrow_keypad.keys[direction.to_char()]);
                //}

                //positions.Add(arrow_keypad.keys['A']);

                //int result = 0;

                //for (int i = 0; i < positions.Count - 1; i++)
                //{
                //    result += positions[i].taxi_distance(positions[i + 1]);
                //}

                //return result;
            }

            private static string get_sequence_string(e_direction[] sequence)
            {
                return string.Join("", sequence.Select(d => d.to_char())) + 'A';
            }

            public int get_sequence_score(string sequence)
            {
                // Sum up the total distance between 'A', then all the buttons. (the sequence should already end in 'A')

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

            //private string get_sequence_UNUSED(c_keypad arrow_keypad, char start_button, char end_button)
            //{
            //    c_vector start_position = keys[start_button];
            //    c_vector end_position = keys[end_button];

            //    //string result = string.Join("", end_position
            //    //    .subtract(start_position)
            //    //    .to_directions().
            //    //    Select(direction => direction.to_char()).
            //    //    ToArray());

            //    e_direction[] base_direction_option = end_position
            //        .subtract(start_position)
            //        .to_directions()
            //        .ToArray();

            //    e_direction[][] direction_options = base_direction_option.get_all_permutations();

            //    e_direction[][] valid_direction_options = direction_options
            //        .Where(d => is_valid_sequence(start_position, d))
            //        .ToArray();

            //    int best_direction_option_score = valid_direction_options
            //        .Min(d => get_sequence_score(arrow_keypad, d));

            //    e_direction[] best_direction_option = valid_direction_options
            //        .Where(d => get_sequence_score(arrow_keypad, d) == best_direction_option_score)
            //        .First();

            //    string result = string.Join("", best_direction_option
            //        .Select(d => d.to_char()));

            //    return result + 'A';
            //}

            //private string[] get_best_sequences_OLD(c_keypad arrow_keypad, char start_button, char end_button)
            //{
            //    c_vector start_position = keys[start_button];
            //    c_vector end_position = keys[end_button];

            //    e_direction[] base_sequence = end_position
            //        .subtract(start_position)
            //        .to_directions()
            //        .ToArray();

            //    e_direction[][] all_sequences = base_sequence.get_all_permutations();

            //    e_direction[][] valid_sequences = all_sequences
            //        .Where(sequence => is_valid_sequence(start_position, sequence))
            //        .ToArray();

            //    int best_sequence_score = valid_sequences
            //        .Min(sequence => get_sequence_score(arrow_keypad, sequence));

            //    e_direction[][] best_sequences = valid_sequences
            //        .Where(sequence => get_sequence_score(arrow_keypad, sequence) == best_sequence_score)
            //        .ToArray();

            //    string[] results = best_sequences
            //        .Select(sequence => string.Join("", sequence
            //            .Select(d => d.to_char())) + 'A')
            //        .ToArray();

            //    return results;
            //}

            private string[] get_best_sequences(
                c_keypad arrow_keypad,
                char start_button,
                char end_button)
            {
                c_vector start_position = keys[start_button];
                c_vector end_position = keys[end_button];

                e_direction[] base_sequence = end_position
                    .subtract(start_position)
                    .to_directions()
                    .ToArray();

                e_direction[][] all_sequences = base_sequence.get_all_permutations();

                e_direction[][] valid_sequences = all_sequences
                    .Where(sequence => is_valid_sequence(start_position, sequence))
                    .ToArray();

                c_scored_sequence[] scored_sequences = valid_sequences
                    .Select(sequence => new c_scored_sequence(
                        get_sequence_string(sequence),
                        get_sequence_score(arrow_keypad, sequence)))
                    .ToArray();

                int best_sequence_score = scored_sequences.Min(sequence => sequence.score);

                string[] best_sequences = scored_sequences
                    .Where(sequence => sequence.score == best_sequence_score)
                    .Select(sequence => sequence.sequence)
                    .ToArray();

                return best_sequences;
            }

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

                return subsequences.get_all_combinations().ToArray();
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

            foreach (string code in codes)
            {
                string[] robot_1_sequences;
                {
                    string[] robot_1_hopeful_sequences = number_keypad.get_best_sequences(arrow_keypad, code);

                    robot_1_sequences = get_best_sequences(arrow_keypad, robot_1_hopeful_sequences);
                }

                string[] robot_2_sequences;
                {
                    List<string> robot_2_hopeful_sequences = new List<string>();

                    foreach (string robot_1_sequence in robot_1_sequences)
                    {
                        robot_2_hopeful_sequences.AddRange(arrow_keypad.get_best_sequences(arrow_keypad, robot_1_sequence));
                    }

                    robot_2_sequences = get_best_sequences(arrow_keypad, robot_2_hopeful_sequences.ToArray());
                }

                int[] robot_2_scores = robot_2_sequences.Select(s => arrow_keypad.get_sequence_score(s)).ToArray();

                string[] human_sequences;
                {
                    List<string> human_hopeful_sequences = new List<string>();

                    foreach (string robot_2_sequence in robot_2_sequences)
                    {
                        human_hopeful_sequences.AddRange(arrow_keypad.get_best_sequences(arrow_keypad, robot_2_sequence));
                    }

                    human_sequences = get_best_sequences(arrow_keypad, human_hopeful_sequences.ToArray());
                }

                // int[] human_scores = human_sequences.Select(s => arrow_keypad.get_sequence_score(s)).ToArray();

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

                Int64 derp = human_sequence_length * code_value;

                result += derp;
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

            const int k_robot_chain = 25;

            foreach (string code in codes)
            {
                string[] robot_sequences;
                {
                    string[] robot_hopeful_sequences = number_keypad.get_best_sequences(arrow_keypad, code);

                    robot_sequences = get_best_sequences(arrow_keypad, robot_hopeful_sequences);
                }

                for (int i = 0; i < k_robot_chain; i++)
                {
                    List<string> next_robot_hopeful_sequences = new List<string>();

                    foreach (string robot_sequence in robot_sequences)
                    {
                        next_robot_hopeful_sequences.AddRange(arrow_keypad.get_best_sequences(arrow_keypad, robot_sequence));
                    }

                    robot_sequences = get_best_sequences(arrow_keypad, next_robot_hopeful_sequences.ToArray());
                }

                string human_sequence = robot_sequences[0];

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

                Int64 derp = human_sequence_length * code_value;

                result += derp;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }
    }
}
