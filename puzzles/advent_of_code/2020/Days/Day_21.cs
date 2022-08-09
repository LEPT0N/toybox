using System;
using System.Collections.Generic;
using System.Linq;
using advent_of_code_common.input_reader;

namespace advent_of_code_2020.Days
{
    internal class Day_21
    {
        internal class c_food_item
        {
            public readonly string[] ingredients;
            public readonly string[] allergens;

            public c_food_item(string input)
            {
                string[] split_input = input.Split(" (");

                ingredients = split_input[0].Split(" ");

                string allergen_input = split_input[1].Substring(9);
                allergen_input = allergen_input.Substring(0, allergen_input.Length - 1);

                allergens = allergen_input.Split(", ");
            }
        }

        internal static c_food_item[] parse_input(
            in string input,
            in bool pretty)
        {
            c_input_reader input_reader = new c_input_reader(input);

            List<c_food_item> food_items = new List<c_food_item>();

            while (input_reader.has_more_lines())
            {
                food_items.Add(new c_food_item(input_reader.read_line()));
            }

            return food_items.ToArray();
        }

        // Build a list of english/gibberish pairings for each allergen.
        internal static List<(string, string)> translate_allergens(c_food_item[] food_items)
        {
            // Build the translator for which english allergens match with which gibberish ingredients
            Dictionary<string, List<string>> translator = new Dictionary<string, List<string>>();
            List<string> newly_solved_allergens = new List<string>();

            foreach (c_food_item food_item in food_items)
            {
                foreach (string allergen in food_item.allergens)
                {
                    if (!translator.ContainsKey(allergen))
                    {
                        translator[allergen] = food_item.ingredients.ToList();
                    }
                    else
                    {
                        translator[allergen] = translator[allergen].Intersect(food_item.ingredients.ToList()).ToList();
                    }

                    if (translator[allergen].Count == 1)
                    {
                        newly_solved_allergens.Add(allergen);
                    }
                }
            }

            // Remove any solved allergens from other allergen's translation possiblities.
            while (newly_solved_allergens.Count > 0)
            {
                List<string> solved_allergens = new List<string>();

                foreach (string solved_allergen in newly_solved_allergens)
                {
                    foreach (string translator_key in translator.Keys)
                    {
                        if (translator[translator_key].Count > 1)
                        {
                            translator[translator_key] = translator[translator_key].Except(translator[solved_allergen]).ToList();

                            if (translator[translator_key].Count == 1)
                            {
                                solved_allergens.Add(translator_key);
                            }
                        }
                    }
                }

                newly_solved_allergens = solved_allergens;
            }

            // Convert the translator to a list
            return translator.Keys.Select(key => (key, translator[key].First())).ToList();
        }

        public static void Part_1(
            string input,
            bool pretty)
        {
            // Parse input
            c_food_item[] food_items = parse_input(input, pretty);

            // Build the translator for which english allergens match with which gibberish ingredients
            List<(string, string)> translator = translate_allergens(food_items);

            // Build a list of the allergens in their gibberish translations.
            List<string> translated_allergens = translator.Select(x => x.Item2).ToList();

            // Build a list of all ingredients in all food items that aren't allergens
            List<string> non_allergen_ingredients = new List<string>();

            foreach (c_food_item food_item in food_items)
            {
                non_allergen_ingredients.AddRange(food_item.ingredients.Except(translated_allergens));
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", non_allergen_ingredients.Count);
            Console.ResetColor();
        }

        public static void Part_2(
            string input,
            bool pretty)
        {
            // Parse input
            c_food_item[] food_items = parse_input(input, pretty);

            // Build the translator for which english allergens match with which gibberish ingredients
            List<(string, string)> translator = translate_allergens(food_items);

            // Sort by the english name.
            translator = translator.OrderBy(x => x.Item1).ToList();

            // Print the gibberish name.
            string result = string.Join(",", translator.Select(x => x.Item2));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Result = {0}", result);
            Console.ResetColor();
        }
    }
}
