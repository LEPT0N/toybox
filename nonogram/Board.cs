using System;
using System.Collections.Generic;
using System.Drawing;

// TODO separate logic from visual

// TODO just use Bungie naming conventions

// TODO split Board>Board_State, Board_Visual, (and Cell, and Clue) then have Board { Board_Visual Visual, Board_State State}

namespace Nonogram
{
    public class Board
    {
        // Private type definitions

        private class Clue
        {
            public readonly int[] Group_Sizes;

            private readonly Board_Style Style;

            private readonly Point[] Group_Positions;

            // Initialize from a known solution
            public Clue(Cell.State[] states, Board_Style style, Point start_point, bool draw_groups_vertically)
            {
                // Initialize our temp group list to 0s

                int[] temp_group_sizes = new int[states.Length / 2 + 1];
                
                for (int i = 0; i < temp_group_sizes.Length; i++)
                {
                    temp_group_sizes[i] = 0;
                }

                // Find each group size

                int total_group_count = 0;
                int current_group_length = 0;

                for (int i = 0; i < states.Length; i++)
                {
                    if (states[i] == Cell.State.on)
                    {
                        // If we find an 'on' cell, we're in a group.

                        current_group_length++;
                    }
                    else if (current_group_length > 0)
                    {
                        // if we find a non-on cell, record the previous group if there was one.

                        temp_group_sizes[total_group_count] = current_group_length;

                        total_group_count++;
                        current_group_length = 0;
                    }
                }

                // Record the last group if we ended in an 'on' cell.

                if (current_group_length > 0)
                {
                    temp_group_sizes[total_group_count] = current_group_length;

                    total_group_count++;
                    current_group_length = 0;
                }

                // Copy the temp group list to the final one, now that we know how big the array is.

                Group_Sizes = new int[total_group_count];

                for (int i = 0; i < total_group_count; i++)
                {
                    Group_Sizes[i] = temp_group_sizes[i];
                }

                // Record data for Draw

                Style = style;

                Group_Positions = new Point[total_group_count];

                Initialize_Visual(start_point, draw_groups_vertically);
            }

            // Initialize from clues
            public Clue(int[] group_sizes, Board_Style style, Point start_point, bool draw_groups_vertically)
            {
                Group_Sizes = new int[group_sizes.Length];

                for (int i = 0; i < group_sizes.Length; i++)
                {
                    Group_Sizes[i] = group_sizes[i];
                }

                // Record data for Draw

                Style = style;

                Group_Positions = new Point[group_sizes.Length];

                Initialize_Visual(start_point, draw_groups_vertically);
            }

            private void Initialize_Visual(Point start_point, bool draw_groups_vertically)
            {
                if (draw_groups_vertically)
                {
                    start_point.Y -= Style.Clue_Spacing * Group_Sizes.Length;
                }
                else
                {
                    start_point.X -= Style.Clue_Spacing * Group_Sizes.Length;
                }

                for (int i = 0; i < Group_Sizes.Length; i++)
                {
                    Group_Positions[i] = start_point;

                    if (draw_groups_vertically)
                    {
                        start_point.Y += Style.Clue_Spacing;
                    }
                    else
                    {
                        start_point.X += Style.Clue_Spacing;
                    }
                }
            }

            public void Draw(Graphics g)
            {
                for (int i = 0; i < Group_Sizes.Length; i++)
                {
                    string group_size_string = Group_Sizes[i].ToString();

                    g.DrawString(
                        group_size_string,
                        Style.Clue_Font,
                        Style.Clue_Brush,
                        Group_Positions[i],
                        Style.Clue_Format);
                }
            }
        }

        private class Group
        {
            public Cell[] Cells;

            public readonly Clue Group_Clue;

            public Group(int cell_count, Clue clue)
            {
                Cells = new Cell[cell_count];

                Group_Clue = clue;
            }
        }

        // Constant Declarations

        // Private data members

        private readonly int m_rows;
        private readonly int m_columns;

        private readonly Board_Style m_style;

        private readonly Rectangle m_exterior_rectangle;

        private readonly Line_Segment[] m_interior_lines;

        // TODO work out what's readonly and what's not. is the lifetime of Boad a single puzzle?
        private Cell[,] m_cells;
        private Group[] m_groups;
        private Clue[] m_row_clues;
        private Clue[] m_column_clues;

        // Public methods

        public Board(Puzzle puzzle, Point start_point)
        {
            m_rows = puzzle.Row_Hints.Length;
            m_columns = puzzle.Column_Hints.Length;

            m_style = Get_Style(puzzle);

            // Initialize the Board's background

            m_exterior_rectangle = new Rectangle(
                new Point(
                    start_point.X + m_style.Clue_Spacing * 4,
                    start_point.Y + m_style.Clue_Spacing * 4),
                new Size(
                    m_columns * m_style.Cell_Size,
                    m_rows * m_style.Cell_Size));

            m_interior_lines = new Line_Segment[m_rows - 1 + m_columns - 1];
            int line_segment_index = 0;

            for (int column = 1; column < m_columns; column++, line_segment_index++)
            {
                int x_position = m_exterior_rectangle.Left + m_style.Cell_Size * column;

                m_interior_lines[line_segment_index] = new Line_Segment
                {
                    First = new Point(x_position, m_exterior_rectangle.Top),
                    Second = new Point(x_position, m_exterior_rectangle.Bottom),
                };
            }

            for (int row = 1; row < m_rows; row++, line_segment_index++)
            {
                int y_position = m_exterior_rectangle.Top + m_style.Cell_Size * row;

                m_interior_lines[line_segment_index] = new Line_Segment
                {
                    First = new Point(m_exterior_rectangle.Left, y_position),
                    Second = new Point(m_exterior_rectangle.Right, y_position),
                };
            }

            // Initialize the grid of cells

            m_cells = new Cell[m_columns, m_rows];

            for (int row = 0; row < m_rows; row++)
            {
                for (int column = 0; column < m_columns; column++)
                {
                    m_cells[column, row] = new Cell(
                        Cell.State.maybe,
                        m_style,
                        new Rectangle(
                            m_exterior_rectangle.Left + m_style.Cell_Size * column,
                            m_exterior_rectangle.Top + m_style.Cell_Size * row,
                            m_style.Cell_Size, m_style.Cell_Size));
                }
            }

            // Initialize the clues to match the puzzle

            m_column_clues = new Clue[m_columns];

            for (int column = 0; column < m_columns; column++)
            {
                Point clue_position = m_cells[column, 0].Screen_Rectangle.Location;
                clue_position.X += m_style.Cell_Size / 2;
                clue_position.Y += m_style.Cell_Size / 2 - m_style.Clue_Board_Offset;

                m_column_clues[column] = new Clue(puzzle.Column_Hints[column], m_style, clue_position, true);
            }

            m_row_clues = new Clue[m_rows];

            for (int row = 0; row < m_rows; row++)
            {
                Point clue_position = m_cells[0, row].Screen_Rectangle.Location;
                clue_position.X += m_style.Cell_Size / 2 - m_style.Clue_Board_Offset;
                clue_position.Y += m_style.Cell_Size / 2;

                m_row_clues[row] = new Clue(puzzle.Row_Hints[row], m_style, clue_position, false);
            }

            // Collect the cells into groups

            m_groups = new Group[m_rows + m_columns];
            int group_count = 0;

            for (int column = 0; column < m_columns; column++, group_count++)
            {
                Group group = new Group(m_rows, m_column_clues[column]);

                for (int row = 0; row < m_rows; row++)
                {
                    group.Cells[row] = m_cells[column, row];
                }

                m_groups[group_count] = group;
            }

            for (int row = 0; row < m_rows; row++, group_count++)
            {
                Group group = new Group(m_columns, m_row_clues[row]);

                for (int column = 0; column < m_columns; column++)
                {
                    group.Cells[column] = m_cells[column, row];
                }

                m_groups[group_count] = group;
            }
        }

        private static Board_Style Get_Style(Puzzle puzzle)
        {
            int largest_side = Math.Max(puzzle.Row_Hints.Length, puzzle.Column_Hints.Length);

            if (largest_side <= 5)
            {
                return Board_Style.Get_Style(Board_Style.Type.Small);
            }
            else if (largest_side <= 10)
            {
                return Board_Style.Get_Style(Board_Style.Type.Medium);
            }
            else if (largest_side <= 15)
            {
                return Board_Style.Get_Style(Board_Style.Type.Large);
            }
            else
            {
                return Board_Style.Get_Style(Board_Style.Type.Huge);
            }
        }

        public Size Get_Size()
        {
            return new Size(
                m_columns * m_style.Cell_Size + m_style.Clue_Spacing * 4,
                m_rows * m_style.Cell_Size + m_style.Clue_Spacing * 4);
        }

        public void Draw(Graphics g)
        {
            // Draw the cells

            for (int column = 0; column < m_columns; column++)
            {
                for (int row = 0; row < m_rows; row++)
                {
                    m_cells[column, row].Draw(g);
                }
            }

            // Draw the inner lines

            foreach (Line_Segment line in m_interior_lines)
            {
                g.DrawLine(m_style.Interior_Pen, line.First, line.Second);
            }

            // Draw the outer rectangle

            g.DrawRectangle(m_style.Exterior_Pen, m_exterior_rectangle);

            // Draw the clues

            foreach (Clue clue in m_column_clues)
            {
                clue.Draw(g);
            }

            foreach (Clue clue in m_row_clues)
            {
                clue.Draw(g);
            }
        }

        public bool Check_Click(Point click_location)
        {
            for (int column = 0; column < m_columns; column++)
            {
                for (int row = 0; row < m_rows; row++)
                {
                    if (m_cells[column, row].Contains(click_location))
                    {
                        switch (m_cells[column, row].state)
                        {
                            case Cell.State.off:
                                m_cells[column, row].state = Cell.State.maybe;
                                break;

                            case Cell.State.maybe:
                                m_cells[column, row].state = Cell.State.on;
                                break;

                            case Cell.State.on:
                                m_cells[column, row].state = Cell.State.off;
                                break;
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        private Cell.State[] Convert_Permutation_To_Possibility(
            int[] group_sizes,
            int[] permutation)
        {
            int group_index = 0;
            List<Cell.State> result = new List<Cell.State>();

            for (int i = 0; i < permutation.Length; i++)
            {
                if (permutation[i] == 0)
                {
                    result.Add(Cell.State.off);
                }
                else
                {
                    if (group_index > 0)
                    {
                        result.Add(Cell.State.off);
                    }

                    for (int j = 0; j < group_sizes[group_index]; j++)
                    {
                        result.Add(Cell.State.on);
                    }

                    group_index++;
                }
            }

            return result.ToArray();
        }

        private bool Is_Valid_Possibility(Cell.State[] possibility, Cell[] cells)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                if (cells[i].state != Cell.State.maybe)
                {
                    if (cells[i].state != possibility[i])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool Process_Group(Group group)
        {
            // Count how many 'on' sections and 'off' sections we have in this group.

            int total_cell_count = group.Cells.Length;
            int on_section_count = group.Group_Clue.Group_Sizes.Length;
            int off_section_count = total_cell_count;
            int total_section_count;

            {
                for (int i = 0; i < on_section_count; i++)
                {
                    off_section_count -= group.Group_Clue.Group_Sizes[i];

                    if (i > 0)
                    {
                        off_section_count -= 1;
                    }
                }

                total_section_count = on_section_count + off_section_count;
            }

            // Build the list of possible permutations

            List<int[]> permutations = new List<int[]>();

            {
                int[] permutation = new int[total_section_count];
                for (int i = 0; i < permutation.Length; i++)
                {
                    permutation[i] = (i < off_section_count ? 0 : 1); // TODO: make these 'on' and off' for code readability.
                }

                do
                {
                    permutations.Add(permutation.Copy());
                } while (permutation.Get_Next_Permutation());
            }

            // convert each permutation back to a possibile solution for the group

            List<Cell.State[]> possibilities = new List<Cell.State[]>();

            {
                foreach (int[] permutation in permutations)
                {
                    Cell.State[] possibility = Convert_Permutation_To_Possibility(
                        group.Group_Clue.Group_Sizes,
                        permutation);

                    // But only count possibilities that are valid given the current group state.

                    if (Is_Valid_Possibility(possibility, group.Cells))
                    {
                        possibilities.Add(possibility);
                    }
                }
            }

            // Compute the union of all valid possibilities

            Cell.State[] union_of_possibilities = new Cell.State[total_cell_count];
            {
                for (int cell_index = 0; cell_index < total_cell_count; cell_index++)
                {
                    union_of_possibilities[cell_index] = Cell.State.maybe;

                    bool cell_can_possibly_be_on = false;
                    bool cell_can_possibly_be_off = false;

                    foreach (Cell.State[] possibility in possibilities)
                    {
                        switch (possibility[cell_index])
                        {
                            case Cell.State.on:
                                cell_can_possibly_be_on = true;
                                break;

                            case Cell.State.off:
                                cell_can_possibly_be_off = true;
                                break;
                        }
                    }

                    if (cell_can_possibly_be_on && !cell_can_possibly_be_off)
                    {
                        union_of_possibilities[cell_index] = Cell.State.on;
                    }
                    else if (cell_can_possibly_be_off && !cell_can_possibly_be_on)
                    {
                        union_of_possibilities[cell_index] = Cell.State.off;
                    }
                    else
                    {
                        union_of_possibilities[cell_index] = Cell.State.maybe;
                    }
                }
            }

            // See if we can change anything in the group with our newfound knowledge

            bool changed_something = false;

            for (int cell_index = 0; cell_index < total_cell_count; cell_index++)
            {
                // TODO detect that the board is wrong! User made a mistake trying to solve something.

                if (union_of_possibilities[cell_index] != Cell.State.maybe &&
                    union_of_possibilities[cell_index] != group.Cells[cell_index].state)
                {
                    group.Cells[cell_index].state = union_of_possibilities[cell_index];

                    changed_something = true;
                }
            }

            return changed_something;
        }

        public void Give_Hint()
        {
            // Check over each group of cells to see if any of them can deduce something

            for (int i = 0; i < m_groups.Length; i++)
            {
                if (Process_Group(m_groups[i]))
                {
                    return;
                }
            }
        }

        public void Undo()
        {
            // TODO
        }
    }
}
