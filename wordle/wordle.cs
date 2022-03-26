using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace wordle
{
    internal enum e_feedback_color
    {
        green,
        yellow,
        gray,
    }

    [DebuggerDisplay("{letter} {number}", Type = "s_letter_number_pair")]
    internal struct s_letter_number_pair
    {
        public readonly char letter;
        public readonly int number;

        public s_letter_number_pair(char l, int n)
        {
            letter = l;
            number = n;
        }
    }

    [DebuggerDisplay("{letter} = {color}", Type = "s_feedback")]
    internal struct s_feedback
    {
        public readonly char letter;
        public readonly e_feedback_color color;

        public s_feedback(char l, e_feedback_color c)
        {
            letter = l;
            color = c;
        }
    }

    internal class c_guess
    {
        // Direct parsing of the input
        private readonly s_feedback[] feedback;

        // required_letters[n] == 'x' -> word[n] == 'x'
        private readonly s_letter_number_pair[] required_letters;

        // banned_letters[n] == 'x' -> word[n] != 'x'
        private readonly s_letter_number_pair[] banned_letters;

        // required_letter_counts[n] == 'x' -> word.Count('x') == n
        private readonly s_letter_number_pair[] required_letter_counts;

        // required_letter_counts[n] == 'x' -> word.Count('x') >= n
        private readonly s_letter_number_pair[] minimum_letter_counts;

        public c_guess(string input)
        {
            {
                string[] split_input_line = input.Split(" = ");

                string word = split_input_line[0];
                e_feedback_color[] feedback_colors = split_input_line[1].Split(" ").Select(x => string_to_feedback_color(x)).ToArray();

                if (word.Length != wordle.k_word_length || feedback_colors.Length != wordle.k_word_length)
                {
                    throw new Exception(String.Format(
                        "Guess requires a word with {0} characters, and {0} feedback items",
                        wordle.k_word_length));
                }

                feedback = word.Zip(feedback_colors, (letter, color) => new s_feedback(letter, color)).ToArray();
            }

            List<s_letter_number_pair> required_letters_list = new List<s_letter_number_pair>();
            List<s_letter_number_pair> banned_letters_list = new List<s_letter_number_pair>();
            List<s_letter_number_pair> required_letter_counts_list = new List<s_letter_number_pair>();
            List<s_letter_number_pair> minimum_letter_counts_list = new List<s_letter_number_pair>();

            for (int i = 0; i < wordle.k_word_length; i++)
            {
                switch (feedback[i].color)
                {
                    case e_feedback_color.green:
                        {
                            // green letter 'x' => word[n] == 'x'
                            required_letters_list.Add(new s_letter_number_pair(feedback[i].letter, i));
                        }
                        break;

                    case e_feedback_color.yellow:
                        {
                            int non_gray_letter_count = feedback.Where(f => f.letter == feedback[i].letter && f.color != e_feedback_color.gray).Count();

                            // yellow letter 'x' => word.Count('x') >= feedback.Count(green, 'x') + feedback.Count(yellow, 'x')
                            minimum_letter_counts_list.Add(new s_letter_number_pair(feedback[i].letter, non_gray_letter_count));

                            // yellow letter 'x' => word[n] != 'x'
                            banned_letters_list.Add(new s_letter_number_pair(feedback[i].letter, i));
                        }
                        break;

                    case e_feedback_color.gray:
                        {
                            int non_gray_letter_count = feedback.Where(f => f.letter == feedback[i].letter && f.color != e_feedback_color.gray).Count();

                            // gray letter 'x' => word.Count('x') == feedback.Count(green, 'x') + feedback.Count(yellow, 'x')
                            required_letter_counts_list.Add(new s_letter_number_pair(feedback[i].letter, non_gray_letter_count));

                            if (non_gray_letter_count > 0)
                            {
                                // gray letter 'x' => word[n] != 'x'
                                banned_letters_list.Add(new s_letter_number_pair(feedback[i].letter, i));
                            }
                        }
                        break;
                }
            }

            required_letters = required_letters_list.ToArray();
            banned_letters = banned_letters_list.ToArray();

            required_letter_counts = required_letter_counts_list.ToArray();
            minimum_letter_counts = minimum_letter_counts_list.ToArray();
        }

        public bool matches(string word)
        {
            // word[n] == 'x'
            foreach (s_letter_number_pair required_letter in required_letters)
            {
                if (word[required_letter.number] != required_letter.letter)
                {
                    return false;
                }
            }

            // word[n] != 'x'
            foreach (s_letter_number_pair banned_letter in banned_letters)
            {
                if (word[banned_letter.number] == banned_letter.letter)
                {
                    return false;
                }
            }

            // word.Count('x') == n
            foreach (s_letter_number_pair required_letter_count in required_letter_counts)
            {
                if (word.Count(letter => letter == required_letter_count.letter) != required_letter_count.number)
                {
                    return false;
                }
            }

            // word.Count('x') >= n
            foreach (s_letter_number_pair minimum_letter_count in minimum_letter_counts)
            {
                if (word.Count(letter => letter == minimum_letter_count.letter) < minimum_letter_count.number)
                {
                    return false;
                }
            }

            return true;
        }

        private static e_feedback_color string_to_feedback_color(string input) => input switch
        {
            "green" => e_feedback_color.green,
            "g" => e_feedback_color.green,

            "yellow" => e_feedback_color.yellow,
            "y" => e_feedback_color.yellow,

            "gray" => e_feedback_color.gray,
            "b" => e_feedback_color.gray,

            _ => throw new Exception(String.Format("Unknown feedback color '{0}'", input))
        };

        public void write_line()
        {
            Console.Write("[");

            for (int i = 0; i < wordle.k_word_length; i++)
            {
                Console.ResetColor();
                Console.Write(" ");
                Console.ForegroundColor = ConsoleColor.White;

                switch (feedback[i].color)
                {
                    case e_feedback_color.green:
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        break;

                    case e_feedback_color.yellow:
                        Console.BackgroundColor = ConsoleColor.DarkYellow;
                        break;

                    case e_feedback_color.gray:
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                        break;
                }

                Console.Write(feedback[i].letter);
            }

            Console.ResetColor();
            Console.Write(" ]");
            Console.WriteLine();
        }
    }

    internal class c_dictionary
    {
        private List<string> m_words;
        private int[] m_letter_counts;

        public int word_count { get { return m_words.Count; } }

        public c_dictionary(string input_file)
        {
            // https://github.com/dwyl/english-words/

            m_words = new List<string>();

            foreach (string word in File.ReadAllLines(input_file))
            {
                if (word.Length == wordle.k_word_length)
                {
                    string lowercase_word = word.ToLower();

                    if (lowercase_word.All(letter => letter >= 'a' && letter <= 'z'))
                    {
                        m_words.Add(lowercase_word);
                    }
                }
            }

            calculate_letter_counts();
        }

        private c_dictionary(List<string> words)
        {
            m_words = words;

            calculate_letter_counts();
        }

        private void calculate_letter_counts()
        {
            m_letter_counts = new int[26];

            foreach (string word in m_words)
            {
                foreach (char letter in word)
                {
                    m_letter_counts[letter - 'a']++;
                }
            }
        }

        private int score_word(string word)
        {
            int score = 0;

            bool[] letters_used = new bool[26];

            foreach (char letter in word)
            {
                letters_used[letter - 'a'] = true;
            }

            for (int i = 0; i < letters_used.Length; i++)
            {
                if (letters_used[i])
                {
                    score += m_letter_counts[i];
                }
            }

            return score;
        }

        public void write_clues(string title)
        {
            Console.WriteLine(title);

            foreach (string word in m_words.OrderByDescending(x => score_word(x)).Take(5))
            {
                Console.WriteLine("    {0} [{1}]", word, score_word(word));
            }

            Console.WriteLine();
        }

        public c_dictionary apply(c_guess guess)
        {
            List<string> filtered_words = m_words.Where(word => guess.matches(word)).ToList();

            return new c_dictionary(filtered_words);
        }
    }

    internal class wordle
    {
        public static int k_word_length = 5;

        static void Main(string[] args)
        {
            Console.WriteLine("W O R D L E");
            Console.WriteLine();

            c_dictionary possibilities = new c_dictionary(args[0]);

            while(possibilities.word_count > 1)
            {
                Console.WriteLine("Dictionary Size = {0}", possibilities.word_count);
                possibilities.write_clues("Some Possibilities:");

                c_guess guess = null;

                while (guess == null)
                {
                    Console.Write("Input guess and result: ");

                    try
                    {
                        string guess_input = Console.ReadLine();
                        guess = new c_guess(guess_input);
                    }
                    catch (Exception)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("    Error - unable to parse input");
                        Console.ResetColor();
                    }
                }

                guess.write_line();

                possibilities = possibilities.apply(guess);
            }

            if (possibilities.word_count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine();
                Console.WriteLine("Error - no possibilities remaining");
                Console.ResetColor();
            }
            else
            {
                possibilities.write_clues("Solution:");
            }
        }
    }
}
