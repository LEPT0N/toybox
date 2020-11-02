﻿using System.Collections.Generic;
using System.Drawing;

// TODO separate logic from visual

// TODO just use Bungie naming conventions

namespace Nonogram
{
    public class Board
    {
        // Constant Declarations

        private const int k_board_start_offset = 250;
        private const int k_cell_size = 100;

        private static readonly Pen k_exterior_pen = new Pen(Color.Black, 8);
        private static readonly Pen k_interior_pen = new Pen(Color.DarkGray, 4);

        private static readonly Brush k_cell_on_brush = new SolidBrush(Color.Black);
        private static readonly Pen k_cell_off_pen = new Pen(Color.Black, 16);

        private static readonly int k_cell_off_offset = k_cell_size * 3 / 8;

        private static readonly Font k_font_brush = new Font(FontFamily.GenericMonospace, 24);
        private static readonly Brush k_clue_brush = new SolidBrush(Color.Black);
        private static readonly StringFormat k_clue_format = new StringFormat();
        private const int k_clue_spacing = 30;

        // Private type definitions

        private enum Cell_State
        {
            off,
            maybe,
            on,
        }

        private class Cell
        {
            public Cell_State State;

            public readonly Rectangle Screen_Rectangle;

            public Cell(Cell_State initial_state, Rectangle screen_rectangle)
            {
                State = initial_state;
                Screen_Rectangle = screen_rectangle;
            }
        }

        private class Clue
        {
            public readonly int[] Group_Sizes;

            public Clue(Cell_State[] states)
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
                    if (states[i] == Cell_State.on)
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

        // Private data members

        private readonly int m_rows;
        private readonly int m_columns;

        private readonly Rectangle m_exterior_rectangle;

        // TODO work out what's readonly and what's not. is the lifetime of Boad a single puzzle?
        private Cell[,] m_cells;
        private Group[] m_groups;
        private Cell_State[,] m_solution;
        private Clue[] m_row_clues;
        private Clue[] m_column_clues;

        // Public methods

        public Board(int rows, int columns)
        {
            m_rows = rows;
            m_columns = columns;

            m_exterior_rectangle = new Rectangle(
                new Point(k_board_start_offset, k_board_start_offset),
                new Size(columns * k_cell_size, rows * k_cell_size));

            // Initialize the grid of cells

            m_cells = new Cell[columns, rows];

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    m_cells[column, row] = new Cell(
                        Cell_State.maybe,
                        new Rectangle(
                            m_exterior_rectangle.Left + k_cell_size * column,
                            m_exterior_rectangle.Top + k_cell_size * row,
                            k_cell_size, k_cell_size));
                }
            }

            // Initialize the solution

            m_solution = new Cell_State[columns, rows];

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    m_solution[column, row] = Cell_State.off;
                }
            }

            // TODO not a hardcoded solution maybe?

            m_solution[0, 1] = Cell_State.on;
            m_solution[0, 2] = Cell_State.on;

            m_solution[1, 0] = Cell_State.on;
            m_solution[1, 1] = Cell_State.on;
            m_solution[1, 2] = Cell_State.on;
            m_solution[1, 3] = Cell_State.on;

            m_solution[2, 1] = Cell_State.on;
            m_solution[2, 2] = Cell_State.on;
            m_solution[2, 3] = Cell_State.on;
            m_solution[2, 4] = Cell_State.on;

            m_solution[3, 0] = Cell_State.on;
            m_solution[3, 1] = Cell_State.on;
            m_solution[3, 2] = Cell_State.on;
            m_solution[3, 3] = Cell_State.on;

            m_solution[4, 1] = Cell_State.on;
            m_solution[4, 2] = Cell_State.on;

            // Generate the clues

            k_clue_format.Alignment = StringAlignment.Center;
            k_clue_format.LineAlignment = StringAlignment.Center;

            m_column_clues = new Clue[columns];

            for (int column = 0; column < columns; column++)
            {
                Cell_State[] solution_states = new Cell_State[rows];

                for (int row = 0; row < rows; row++)
                {
                    solution_states[row] = m_solution[column, row];
                }

                m_column_clues[column] = new Clue(solution_states);
            }

            m_row_clues = new Clue[rows];

            for (int row = 0; row < rows; row++)
            {
                Cell_State[] solution_states = new Cell_State[columns];

                for (int column = 0; column < columns; column++)
                {
                    solution_states[column] = m_solution[column, row];
                }

                m_row_clues[row] = new Clue(solution_states);
            }

            // Collect the cells into groups

            m_groups = new Group[rows + columns];
            int group_count = 0;

            for (int column = 0; column < columns; column++, group_count++)
            {
                Group group = new Group(rows, m_column_clues[column]);

                for (int row = 0; row < rows; row++)
                {
                    group.Cells[row] = m_cells[column, row];
                }

                m_groups[group_count] = group;
            }

            for (int row = 0; row < rows; row++, group_count++)
            {
                Group group = new Group(columns, m_row_clues[row]);

                for (int column = 0; column < columns; column++)
                {
                    group.Cells[column] = m_cells[column, row];
                }

                m_groups[group_count] = group;
            }
        }

        public void Draw(Graphics g)
        {
            // Draw the cells

            for (int column = 0; column < m_columns; column++)
            {
                for (int row = 0; row < m_rows; row++)
                {
                    switch (m_cells[column, row].State)
                    {
                        // on == filled in square
                        case Cell_State.on:
                            {
                                g.FillRectangle(
                                    k_cell_on_brush,
                                    m_cells[column, row].Screen_Rectangle);
                            }
                            break;

                        // off == X
                        case Cell_State.off:
                            {
                                int center_x = m_exterior_rectangle.Left + k_cell_size * column + k_cell_size / 2;
                                int center_y = m_exterior_rectangle.Top + k_cell_size * row + k_cell_size / 2;

                                g.DrawLine(k_cell_off_pen,
                                    center_x - k_cell_off_offset, center_y - k_cell_off_offset,
                                    center_x + k_cell_off_offset, center_y + k_cell_off_offset);

                                g.DrawLine(k_cell_off_pen,
                                    center_x - k_cell_off_offset, center_y + k_cell_off_offset,
                                    center_x + k_cell_off_offset, center_y - k_cell_off_offset);
                            }
                            break;
                    }
                }
            }

            // Draw the inner lines

            for (int column = 1; column < m_columns; column++)
            {
                int x_position = m_exterior_rectangle.Left + k_cell_size * column;

                g.DrawLine(k_interior_pen,
                    x_position, m_exterior_rectangle.Top,
                    x_position, m_exterior_rectangle.Bottom);
            }

            for (int row = 1; row < m_columns; row++)
            {
                int y_position = m_exterior_rectangle.Top + k_cell_size * row;

                g.DrawLine(k_interior_pen,
                    m_exterior_rectangle.Left, y_position,
                    m_exterior_rectangle.Right, y_position);
            }

            // Draw the outer rectangle

            g.DrawRectangle(k_exterior_pen, m_exterior_rectangle);

            // Draw the clues

            for (int column = 0; column < m_columns; column++)
            {
                int[] group_sizes = m_column_clues[column].Group_Sizes;

                Point clue_position = m_cells[column, 0].Screen_Rectangle.Location;
                clue_position.X += k_cell_size / 2;
                clue_position.Y += k_cell_size / 2;

                clue_position.Y -= k_cell_size / 2 + k_clue_spacing * group_sizes.Length;

                for (int group_index = 0; group_index < group_sizes.Length; group_index++)
                {
                    string group_size_string = group_sizes[group_index].ToString();

                    g.DrawString(group_size_string, k_font_brush, k_clue_brush, clue_position, k_clue_format);

                    clue_position.Y += k_clue_spacing;
                }
            }

            for (int row = 0; row < m_rows; row++)
            {
                int[] group_sizes = m_row_clues[row].Group_Sizes;

                Point clue_position = m_cells[0, row].Screen_Rectangle.Location;
                clue_position.X += k_cell_size / 2;
                clue_position.Y += k_cell_size / 2;

                clue_position.X -= k_cell_size / 2 + k_clue_spacing * group_sizes.Length;

                for (int group_index = 0; group_index < group_sizes.Length; group_index++)
                {
                    string group_size_string = group_sizes[group_index].ToString();

                    g.DrawString(group_size_string, k_font_brush, k_clue_brush, clue_position, k_clue_format);

                    clue_position.X += k_clue_spacing;
                }
            }
        }

        public bool Check_Click(Point click_location)
        {
            for (int column = 0; column < m_columns; column++)
            {
                for (int row = 0; row < m_rows; row++)
                {
                    if (m_cells[column, row].Screen_Rectangle.Contains(click_location))
                    {
                        switch (m_cells[column, row].State)
                        {
                            case Cell_State.off:
                                m_cells[column, row].State = Cell_State.maybe;
                                break;

                            case Cell_State.maybe:
                                m_cells[column, row].State = Cell_State.on;
                                break;

                            case Cell_State.on:
                                m_cells[column, row].State = Cell_State.off;
                                break;
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        private Cell_State[] Convert_Permutation_To_Possibility(
            int[] group_sizes,
            int[] permutation)
        {
            int group_index = 0;
            List<Cell_State> result = new List<Cell_State>();

            for (int i = 0; i < permutation.Length; i++)
            {
                if (permutation[i] == 0)
                {
                    result.Add(Cell_State.off);
                }
                else
                {
                    if (group_index > 0)
                    {
                        result.Add(Cell_State.off);
                    }

                    for (int j = 0; j < group_sizes[group_index]; j++)
                    {
                        result.Add(Cell_State.on);
                    }

                    group_index++;
                }
            }

            return result.ToArray();
        }

        private bool Is_Valid_Possibility(Cell_State[] possibility, Cell[] cells)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                if (cells[i].State != Cell_State.maybe)
                {
                    if (cells[i].State != possibility[i])
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
                    permutation[i] = (i < off_section_count ? 0 : 1);
                }

                do
                {
                    permutations.Add(permutation.Copy());
                } while (permutation.Get_Next_Permutation());
            }

            // convert each permutation back to a possibile solution for the group

            List<Cell_State[]> possibilities = new List<Cell_State[]>();

            {
                foreach (int[] permutation in permutations)
                {
                    Cell_State[] possibility = Convert_Permutation_To_Possibility(
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

            Cell_State[] union_of_possibilities = new Cell_State[total_cell_count];
            {
                for (int cell_index = 0; cell_index < total_cell_count; cell_index++)
                {
                    union_of_possibilities[cell_index] = Cell_State.maybe;

                    bool cell_can_possibly_be_on = false;
                    bool cell_can_possibly_be_off = false;

                    foreach (Cell_State[] possibility in possibilities)
                    {
                        switch (possibility[cell_index])
                        {
                            case Cell_State.on:
                                cell_can_possibly_be_on = true;
                                break;

                            case Cell_State.off:
                                cell_can_possibly_be_off = true;
                                break;
                        }
                    }

                    if (cell_can_possibly_be_on && !cell_can_possibly_be_off)
                    {
                        union_of_possibilities[cell_index] = Cell_State.on;
                    }
                    else if (cell_can_possibly_be_off && !cell_can_possibly_be_on)
                    {
                        union_of_possibilities[cell_index] = Cell_State.off;
                    }
                    else
                    {
                        union_of_possibilities[cell_index] = Cell_State.maybe;
                    }
                }
            }

            // See if we can change anything in the group with our newfound knowledge

            bool changed_something = false;

            for (int cell_index = 0; cell_index < total_cell_count; cell_index++)
            {
                if (union_of_possibilities[cell_index] != Cell_State.maybe &&
                    union_of_possibilities[cell_index] != group.Cells[cell_index].State)
                {
                    group.Cells[cell_index].State = union_of_possibilities[cell_index];

                    changed_something = true;
                }
            }

            return changed_something;
        }

        public void Give_Hint()
        {
            // Check over each row and each column to see if any of them can add something

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
