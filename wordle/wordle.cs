using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

        public c_guess(string guess, string answer)
		{
            if (guess.Length != wordle.k_word_length || answer.Length != wordle.k_word_length)
            {
                throw new Exception(String.Format(
                    "Guesses and answers must be {0} characters long",
                    wordle.k_word_length));
            }

            feedback = new s_feedback[wordle.k_word_length];
            
            for (int i = 0; i < wordle.k_word_length; i++)
			{
                if (guess[i] == answer[i])
				{
                    feedback[i] = new s_feedback(guess[i], e_feedback_color.green);
				}
			}
            
            for (int i = 0; i < wordle.k_word_length; i++)
			{
                e_feedback_color feedback_color;

                if (guess[i] == answer[i])
				{
                    continue;
				}
                else if (feedback.Count(feedback_item => feedback_item.letter == guess[i])
                    < answer.Count(answer_character => answer_character == guess[i]))
				{
                    feedback_color = e_feedback_color.yellow;
				}
                else
				{
                    feedback_color = e_feedback_color.gray;
				}

                feedback[i] = new s_feedback(guess[i], feedback_color);
			}

            parse_feedback(
                feedback,
                ref required_letters,
                ref banned_letters,
                ref required_letter_counts,
                ref minimum_letter_counts);
		}

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

            parse_feedback(
                feedback,
                ref required_letters,
                ref banned_letters,
                ref required_letter_counts,
                ref minimum_letter_counts);
        }

        private static void parse_feedback(
            s_feedback[] feedback,
            ref s_letter_number_pair[] required_letters,
            ref s_letter_number_pair[] banned_letters,
            ref s_letter_number_pair[] required_letter_counts,
            ref s_letter_number_pair[] minimum_letter_counts)
        {
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

        public UInt64 encode()
		{
            UInt64 result = 0;
            UInt64 mult = 1;

            foreach (s_feedback feedback_item in feedback)
			{
                switch (feedback_item.color)
				{
                    case e_feedback_color.green:
                        result += mult;
                        break;
                    case e_feedback_color.yellow:
                        result += mult * 2;
                        break;
                    case e_feedback_color.gray:
                        result += mult * 4;
                        break;
				}

                mult *= 8;
			}

            return result;
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
    
    [DebuggerDisplay("{worst_case}, {average_case}, {is_possible_answer}", Type = "s_hint_score")]
    internal struct s_hint_score
    {
        public readonly int worst_case;
        public readonly float average_case;
        public readonly bool is_possible_answer;

        public readonly static s_hint_score k_default = new s_hint_score(int.MaxValue, float.MaxValue, false);

        public s_hint_score(int worst, float average, bool possible_answer)
        {
            worst_case = worst;
            average_case = average;
            is_possible_answer = possible_answer;
        }
    }

    class c_hint_comparer : IComparer<s_hint_score>
    {
        public int Compare(s_hint_score a, s_hint_score b)
        {
            if (a.worst_case != b.worst_case)
            {
                return a.worst_case.CompareTo(b.worst_case);
            }
            else if (a.average_case != b.average_case)
            {
                return a.average_case.CompareTo(b.average_case);
            }
            else
            {
                return a.is_possible_answer.CompareTo(b.is_possible_answer);
            }
        }
    }

    internal class c_answer_list
	{
        private List<string> m_answers;

        public int count()
		{
            return m_answers.Count;
		}

        public c_answer_list(string input_file)
        {
            m_answers = new List<string>();

            foreach (string word in File.ReadAllLines(input_file))
            {
                if (word.Length == wordle.k_word_length)
                {
                    string lowercase_word = word.ToLower();

                    if (lowercase_word.All(letter => letter >= 'a' && letter <= 'z'))
                    {
                        m_answers.Add(lowercase_word);
                    }
                }
            }
        }

        public s_hint_score score_hint(string hint)
		{
            // For each potential answer, See how small the answer list would become for a given hint.
            // Score the hint based on the largest (worst-case) resulting answers list.

            int max_score = -1;
            float sum_score = 0;

            Dictionary<UInt64, int> guesses = new Dictionary<UInt64, int>();

            // Find the score for each potential answer being the real answer.
            foreach (string answer in m_answers)
			{
                c_guess guess = new c_guess(hint, answer);
             
                // Optimization: If two hint/answer pairs produce the same feedback, only do work once.
                UInt64 guess_encoded = guess.encode();
                if (!guesses.ContainsKey(guess_encoded))
                {
                    // See how big the resulting answers list would be.
                    int score = m_answers.Count(word => guess.matches(word));

                    // Debug-friendly score computation
                    // List<string> matches = m_answers.Where(word => guess.matches(word)).ToList();
                    // int score = matches.Count();

                    guesses.Add(guess_encoded, score);

                    // record the largest score.
                    if (score > max_score || (score > 0 && max_score == -1))
				    {
                        max_score = score;
				    }

                    sum_score += score;
                }
                else
                {
                    sum_score += guesses[guess_encoded];
                }
			}

            if (max_score == -1)
			{
                max_score = int.MaxValue;
			}

            bool hint_is_possible_answer = m_answers.Contains(hint);

            return new s_hint_score(max_score, sum_score / m_answers.Count, hint_is_possible_answer);
		}

        public void apply(c_guess guess)
        {
            m_answers = m_answers.Where(word => guess.matches(word)).ToList();
        }

        public void print_solution()
		{
            Console.WriteLine("    {0}", m_answers.First());
		}
	}

    internal class c_hint_list
	{
        private class c_hint
		{
            public string m_hint;
            public s_hint_score m_score;

            public c_hint(string hint, s_hint_score score)
			{
                m_hint = hint;
                m_score = score;
			}
		}

        private List<c_hint> m_hints;

        public c_hint_list(string input_file)
        {
            m_hints = new List<c_hint>();

            foreach (string word in File.ReadAllLines(input_file))
            {
                if (word.Length == wordle.k_word_length)
                {
                    string lowercase_word = word.ToLower();

                    if (lowercase_word.All(letter => letter >= 'a' && letter <= 'z'))
                    {
                        m_hints.Add(new c_hint(lowercase_word, s_hint_score.k_default));
                    }
                }
            }
        }

        public void set_hint_score(string hint_string, s_hint_score hint_score)
		{
            foreach (c_hint hint in m_hints)
			{
                if (hint.m_hint == hint_string)
				{
                    hint.m_score = hint_score;

                    return;
				}
			}
		}

        public void score_hints(c_answer_list answers)
		{
            // Debug one score computation.
            // foreach (c_hint hint in m_hints)
			// {
            //     if (hint.m_hint == "whump")
			// 	{
            //         hint.m_score = answers.score_hint(hint.m_hint);
            // 
            //         return;
			// 	}
			// }
 
            // Score hints in parallel to speed things up.

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            ConcurrentQueue<c_hint> job_input = new ConcurrentQueue<c_hint>(m_hints);

            Action score_worker = () =>
			{
                c_hint hint;
                while (job_input.TryDequeue(out hint))
				{
                    hint.m_score = answers.score_hint(hint.m_hint);
				}
            };

            Parallel.Invoke(score_worker, score_worker, score_worker, score_worker);

            stopwatch.Stop();
            Console.WriteLine("Time taken = {0}", stopwatch.Elapsed);
		}

        public void print_suggestions()
		{
            foreach (c_hint hint in m_hints.OrderBy(hint => hint.m_score, new c_hint_comparer()).Take(5))
            {
                Console.WriteLine("    {0} [{1}, {2}, {3}]",
                    hint.m_hint,
                    hint.m_score.worst_case,
                    hint.m_score.average_case,
                    hint.m_score.is_possible_answer);
            }
		}
	}

    internal interface i_bot
    {
        public bool solved();
        public void print_suggestions();
        public void print_solution();
        public void apply(c_guess guess);
    }

    internal class c_bot_1 : i_bot
    {
        private c_dictionary m_possible_answers;

        public c_bot_1(string answers_input_file)
		{
            m_possible_answers = new c_dictionary(answers_input_file);
        }

        public bool solved()
        {
            return m_possible_answers.word_count <= 1;
        }

        public void print_suggestions()
		{
            Console.WriteLine("Dictionary Size = {0}", m_possible_answers.word_count);
            m_possible_answers.write_clues("Some Possibilities:");

            Console.WriteLine();
		}

        public void print_solution()
		{
            m_possible_answers.write_clues("Solution:");
		}

        public void apply(c_guess guess)
        {
            m_possible_answers = m_possible_answers.apply(guess);
        }
    }

    internal class c_bot_2 : i_bot
	{
        private c_hint_list m_hints;
        private c_answer_list m_answers;

        public c_bot_2(string hints_input_file, string answers_input_file)
		{
            m_hints = new c_hint_list(hints_input_file);
            m_answers = new c_answer_list(answers_input_file);
            
            m_hints.set_hint_score("raise", new s_hint_score(168, 61.000862f, true));
            m_hints.set_hint_score("arise", new s_hint_score(168, 63.7257f, true));
            m_hints.set_hint_score("aesir", new s_hint_score(168, 69.882935f, false));
            m_hints.set_hint_score("reais", new s_hint_score(168, 71.6108f, false));
            m_hints.set_hint_score("serai", new s_hint_score(168, 72.92138f, false));

            // Use this to generate the first set of suggestions instead.
            // However seeing as how this takes about a minute and it's the same output each time,
            // I'm just putting them above.
            // m_hints.score_hints(m_answers);
		}

        public bool solved()
		{
            return m_answers.count() <= 1;
		}

        public void print_suggestions()
		{
            Console.WriteLine("Dictionary Size = {0}", m_answers.count());
            Console.WriteLine("Some Possibilities:");

            m_hints.print_suggestions();

            Console.WriteLine();
		}

        public void print_solution()
		{
            Console.WriteLine("Solution:");

            m_answers.print_solution();

            Console.WriteLine();
		}

        public void apply(c_guess guess)
        {
            m_answers.apply(guess);

            m_hints.score_hints(m_answers);
        }
	}

    internal class wordle
    {
        public static int k_word_length = 5;

        private static void solve(i_bot bot)
		{
            while(!bot.solved())
            {
                bot.print_suggestions();

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

                bot.apply(guess);
            }

            bot.print_solution();
		}

        static void Main(string[] args)
        {
            Console.WriteLine("W O R D L E");
            Console.WriteLine();

            i_bot bot;

            if (args[0] == "++")
			{
                bot = new c_bot_2(args[1], args[2]);
			}
            else
            {
                bot = new c_bot_1(args[0]);
            }

            solve(bot);
        }
    }
}
