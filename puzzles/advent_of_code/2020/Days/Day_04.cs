using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using advent_of_code_common.input_reader;

namespace advent_of_code_2020.Days
{
    internal class Day_04
    {
        internal class c_passport
        {
            static string[] k_required_field_names = { "byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid" };
            static string[] k_optional_field_names = { "cid" };

            static Dictionary<string, int> k_required_fields;
            static Dictionary<string, int> k_optional_fields;

            static c_passport()
            {
                k_required_fields = new Dictionary<string, int>();
                for (int i = 0; i < k_required_field_names.Length; i++)
                {
                    k_required_fields.Add(k_required_field_names[i], i);
                }

                k_optional_fields = new Dictionary<string, int>();
                for (int i = 0; i < k_optional_field_names.Length; i++)
                {
                    k_optional_fields.Add(k_optional_field_names[i], i);
                }
            }

            internal void validate_number_field(string field_name, string field_value, int min, int max)
            {
                int value;
                if (!int.TryParse(field_value, out value))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid. Field {0} has non-integer value {1}", field_name, field_value);

                    Valid = false;
                }
                else if (value < min)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid. Field {0} has value {1} less than min {2}", field_name, field_value, min);

                    Valid = false;
                }
                else if (value > max)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid. Field {0} has value {1} greater than max {2}", field_name, field_value, max);

                    Valid = false;
                }
            }

            static Regex k_height_format = new Regex(@"^(\d+)(cm|in)$");

            internal void validate_height_field(string field_name, string field_value)
            {
                Match match = k_height_format.Match(field_value);

                if (!match.Success)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid. Field {0} has invalid height {1}", field_name, field_value);

                    Valid = false;
                    return;
                }

                GroupCollection groups = match.Groups;

                int height = int.Parse(match.Groups[1].Value);

                switch (match.Groups[2].Value)
                {
                    case "in":
                        if (height < 59 || height > 76)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Invalid. Field {0} has invalid parsed 'in' height {1}", field_name, field_value);

                            Valid = false;
                            return;
                        }
                        break;

                    case "cm":
                        if (height < 150 || height > 193)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Invalid. Field {0} has invalid parsed 'cm' height {1}", field_name, field_value);

                            Valid = false;
                            return;
                        }
                        break;

                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid. Field {0} has invalid parsed height {1}", field_name, field_value);

                        Valid = false;
                        return;
                }
            }

            static Regex k_hair_color_format = new Regex(@"^#[a-z0-9]{6}$");

            internal void validate_hair_color_field(string field_name, string field_value)
            {
                if (!k_hair_color_format.IsMatch(field_value))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid. Field {0} has invalid hair color {1}", field_name, field_value);

                    Valid = false;
                }
            }

            internal void validate_eye_color_field(string field_name, string field_value)
            {
                bool valid_field_value = false;

                switch (field_value)
                {
                    case "amb":
                    case "blu":
                    case "brn":
                    case "gry":
                    case "grn":
                    case "hzl":
                    case "oth":
                        valid_field_value = true;
                        break;
                }

                if (!valid_field_value)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid. Field {0} has invalid eye color {1}", field_name, field_value);

                    Valid = false;
                }
            }

            static Regex k_passport_id_format = new Regex(@"^\d{9}$");

            internal void validate_passport_id_field(string field_name, string field_value)
            {
                if (!k_passport_id_format.IsMatch(field_value))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid. Field {0} has invalid passport id {1}", field_name, field_value);

                    Valid = false;
                }
            }

            public c_passport(c_input_reader input_reader, bool use_enhanced_validation)
            {
                while (input_reader.has_more_lines())
                {
                    string input_line = input_reader.read_line();

                    if (input_line.Length == 0)
                    {
                        break;
                    }

                    string[] fields = input_line.Split(" ").ToArray();

                    foreach(string field in fields)
                    {
                        string[] parsed_field = field.Split(":").ToArray();
                        string field_name = parsed_field[0];
                        string field_value = parsed_field[1];

                        if (k_required_fields.ContainsKey(field_name))
                        {
                            m_required_field_values[k_required_fields[field_name]] = field_value;

                            if (use_enhanced_validation)
                            {
                                switch (field_name)
                                {
                                    case "byr":
                                        validate_number_field(field_name, field_value, 1920, 2002);
                                        break;

                                    case "iyr":
                                        validate_number_field(field_name, field_value, 2010, 2020);
                                        break;

                                    case "eyr":
                                        validate_number_field(field_name, field_value, 2020, 2030);
                                        break;

                                    case "hgt":
                                        validate_height_field(field_name, field_value);
                                        break;

                                    case "hcl":
                                        validate_hair_color_field(field_name, field_value);
                                        break;

                                    case "ecl":
                                        validate_eye_color_field(field_name, field_value);
                                        break;

                                    case "pid":
                                        validate_passport_id_field(field_name, field_value);
                                        break;
                                }
                            }
                        }
                        else if (k_optional_fields.ContainsKey(field_name))
                        {
                            m_optional_field_values[k_optional_fields[field_name]] = field_value;
                        }
                        else if(Valid)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Invalid. Unexpected field {0}", field_name);

                            Valid = false;
                        }
                    }
                }

                if (Valid)
                {
                    for (int i = 0; i < k_required_field_names.Length; i++)
                    {
                        if (String.IsNullOrEmpty(m_required_field_values[i]))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Invalid. Missing required field {0}", k_required_field_names[i]);

                            Valid = false;
                            break;
                        }
                    }
                }

                if (Valid)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Valid");
                }
            }

            public bool Valid { get; private set; } = true;

            string[] m_required_field_values = new string[k_required_field_names.Length];
            string[] m_optional_field_values = new string[k_optional_field_names.Length];
        }

        internal static void Day_4_helper(string input, bool use_enhanced_validation)
        {
            c_input_reader input_reader = new c_input_reader(input);

            List<c_passport> passports = new List<c_passport>();

            int valid_passport_count = 0;

            while (input_reader.has_more_lines())
            {
                c_passport passport = new c_passport(input_reader, use_enhanced_validation);

                if (passport.Valid)
                {
                    valid_passport_count++;
                }

                Console.WriteLine();

                passports.Add(passport);
            }

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Number of valid passports = {0}", valid_passport_count);

            Console.ResetColor();
        }

        public static void Part_1(string input, bool pretty)
        {
            Day_4_helper(input, false);
        }
        public static void Part_2(string input, bool pretty)
        {
            Day_4_helper(input, true);
        }
    }
}
