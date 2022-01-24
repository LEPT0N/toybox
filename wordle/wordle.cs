using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace wordle
{
    internal enum e_feedback
    {
        correct_spot,
        wrong_spot,
        no_spot,
    }

    internal class c_guess
    {
        public string word;
        public e_feedback[] feedback;

        public c_guess(string input)
        {
            string[] split_input_line = input.Split(" = ");

            word = split_input_line[0];
            feedback = split_input_line[1].Split(" ").Select(x => string_to_feedback(x)).ToArray();

            if (word.Length != wordle.k_word_length || feedback.Length != wordle.k_word_length)
            {
                throw new Exception(String.Format(
                    "Guess requires a word with {0} characters, and {0} feedback items",
                    wordle.k_word_length));
            }
        }

        private static e_feedback string_to_feedback(string input)
        {
            switch (input)
            {
                case "green": return e_feedback.correct_spot;
                case "yellow": return e_feedback.wrong_spot;
                case "gray": return e_feedback.no_spot;
            }

            throw new Exception(String.Format("Unknown feedback '{0}'", input));
        }

        public void write_line()
        {
            for (int i = 0; i < wordle.k_word_length; i++)
            {
                Console.ResetColor();
                Console.Write(" ");

                switch (feedback[i])
                {
                    case e_feedback.correct_spot:
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        break;

                    case e_feedback.wrong_spot:
                        Console.BackgroundColor = ConsoleColor.DarkYellow;
                        break;

                    case e_feedback.no_spot:
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                        break;
                }

                Console.Write(word[i]);
            }

            Console.ResetColor();
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
            m_words = new List<string>();

            foreach (string word in File.ReadAllLines(input_file))
            {
                if (word.Length == wordle.k_word_length)
                {
                    string lowercase_word = word.ToLower();

                    if (word.All(letter => letter >= 'a' && letter <= 'z'))
                    {
                        m_words.Add(word.ToLower());
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

        public void write_clues()
        {
            Console.WriteLine("Some Possibilities:");

            foreach (string word in m_words.OrderByDescending(x => score_word(x)).Take(3))
            {
                Console.WriteLine("    {0}", word);
            }
        }

        public c_dictionary apply(c_guess guess)
        {
            IEnumerable<string> words = m_words;

            for (int i = 0; i < guess.word.Length; i++)
            {
                IEnumerable<string> filtered_words;

                switch (guess.feedback[i])
                {
                    case e_feedback.correct_spot:
                        filtered_words = words.Where(word => word[i] == guess.word[i]).ToList();
                        break;

                    case e_feedback.wrong_spot:
                        filtered_words = words.Where(word =>
                            word[i] != guess.word[i] &&
                            word.Any(letter => letter == guess.word[i])).ToList();
                        break;

                    case e_feedback.no_spot:
                        filtered_words = words.Where(word => !word.Any(letter => letter == guess.word[i])).ToList();
                        /* TODO BUG:
                         *  solution = robot
                         *  guess = ratty
                         *  feedback = green, gray, yellow, gray, gray
                         *  
                         *  note the second 't' is gray. My filter would exclude all letters with 't' in them,
                         *  when in fact I need to only exclude words with a SECOND 't' in them.
                         *  
                         *  similarly:
                         *  solution = robot
                         *  guess = rotor
                         *  feedback = green, green, yellow, green, gray.
                         *  
                         *  The second 'r' is commenting on a second 'r' in the word, not the lack of any 'r'.
                         *  
                         *  Fun guesses for 'robot':
                         *  riped
                         *  rucks
                         *  ratty
                         *  rotor
                         */
                        
                        break;

                    default:
                        throw new Exception("unexpected");
                }

                words = filtered_words;
            }

            return new c_dictionary(words.ToList());
        }
    }

    internal class wordle
    {
        public static int k_word_length = 5;

        static void Main(string[] args)
        {
            Console.WriteLine("W O R D L E");
            Console.WriteLine();

            c_dictionary dictionary = new c_dictionary(args[0]);

            Console.WriteLine("Dictionary Size = {0}", dictionary.word_count);
            Console.WriteLine();

            c_dictionary possibilities = dictionary;

            while(true)
            {
                possibilities.write_clues();
                Console.WriteLine();

                Console.Write("Input guess and result: ");
                string guess_input = Console.ReadLine();

                c_guess guess = new c_guess(guess_input);

                possibilities = possibilities.apply(guess);

                Console.WriteLine();
                guess.write_line();
                Console.WriteLine("Dictionary Size = {0}", possibilities.word_count);
            }
        }
    }
}
