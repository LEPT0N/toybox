using advent_of_code_common.big_int_math;
using advent_of_code_common.input_reader;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace advent_of_code_2025.days
{
    internal class day_08
    {
        [DebuggerDisplay("[{position.x}, {position.y}, {position.z}]", Type = "c_box")]
        internal class c_box
        {
            public readonly c_big_vector position;

            public c_group group;

            public c_box(string input)
            {
                group = null;

                int[] inputs = input.Split(',').Select(i => int.Parse(i)).ToArray();
                position = new c_big_vector(inputs[0], inputs[1], inputs[2]);
            }
        }

        [DebuggerDisplay("{boxes.Count}", Type = "c_group")]
        internal class c_group
        {
            public List<c_box> boxes = new List<c_box>();
        }

        [DebuggerDisplay("{box_1} x {box_2} = {size}", Type = "c_link")]
        internal class c_link
        {
            public readonly c_box box_1;
            public readonly c_box box_2;
            public readonly double size;

            public c_link(c_box a, c_box b)
            {
                box_1 = a;
                box_2 = b;

                size = box_2.position.subtract(box_1.position).euclidean_magnitude();
            }
        }

        internal static c_box[] parse_input(
            in c_input_reader input_reader,
            in bool pretty)
        {
            List<c_box> boxes = new List<c_box>();

            while (input_reader.has_more_lines())
            {
                boxes.Add(new c_box(input_reader.read_line()));
            }

            return boxes.ToArray();
        }

        public static void create_links(
            c_box[] boxes,
            PriorityQueue<c_link, double> links)
        {
            // Just make all possible links between boxes.

            for (int i = 0; i < boxes.Length - 1; i++)
            {
                for (int j = i + 1; j < boxes.Length; j++)
                {
                    c_link link = new c_link(boxes[i], boxes[j]);
                    links.Enqueue(link, link.size);
                }
            }
        }

        public static void join_link(
            c_link link,
            HashSet<c_group> groups)
        {
            if (link.box_1.group == null && link.box_2.group == null)
            {
                // If we're linking two ungrouped boxes, create a new group and put them both in it.

                c_group group = new c_group();
                group.boxes.Add(link.box_1);
                group.boxes.Add(link.box_2);

                link.box_1.group = group;
                link.box_2.group = group;

                groups.Add(group);
            }
            else if (link.box_1.group == null && link.box_2.group != null)
            {
                // If we're linking one box in a group and one without a group, add the loner to the group.

                link.box_2.group.boxes.Add(link.box_1);
                link.box_1.group = link.box_2.group;
            }
            else if (link.box_1.group != null && link.box_2.group == null)
            {
                // If we're linking one box in a group and one without a group, add the loner to the group.

                link.box_1.group.boxes.Add(link.box_2);
                link.box_2.group = link.box_1.group;
            }
            else if (link.box_1.group != link.box_2.group)
            {
                // If both are in different groups, move them all into the same group and delete the second group.

                c_group combined_group = link.box_1.group;
                c_group dead_group = link.box_2.group;

                foreach (c_box box in dead_group.boxes)
                {
                    combined_group.boxes.Add(box);
                    box.group = combined_group;
                }

                groups.Remove(dead_group);
            }
        }

        public static void part_1(
            c_input_reader input_reader,
            bool pretty)
        {
            c_box[] boxes = parse_input(input_reader, pretty);

            PriorityQueue<c_link, double> links = new PriorityQueue<c_link, double>();

            HashSet<c_group> groups = new HashSet<c_group>();

            // Create all links

            create_links(boxes, links);

            // loop through the smallest N links to group them.

            for (int i = 0; i < 1000; i++)
            {
                c_link link = links.Dequeue();

                join_link(link, groups);
            }

            // multiply the size of the three largest groups.

             int result = groups
                .Select(group => group.boxes.Count)
                .OrderDescending()
                .Take(3)
                .Aggregate(1, (a, b) => a * b);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }

        public static void part_2(
            c_input_reader input_reader,
            bool pretty)
        {
            c_box[] boxes = parse_input(input_reader, pretty);

            PriorityQueue<c_link, double> links = new PriorityQueue<c_link, double>();

            HashSet<c_group> groups = new HashSet<c_group>();

            // Create all links

            create_links(boxes, links);

            // loop through links until all boxes are linked

            c_link link = null;

            while (boxes.Any(box => box.group == null))
            {
                link = links.Dequeue();

                join_link(link, groups);
            }

            // multiply the size of the three largest groups.

            Int64 result = link.box_1.position.x * link.box_2.position.x;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine($"Result = {result}");
            Console.ResetColor();
        }
    }
}
