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

        private struct Board_Blueprint
        {
            public int Cell_Size;

            public Pen Exterior_Pen;
            public Pen Interior_Pen;

            public Brush Cell_On_Brush;
            public Pen Cell_Off_Pen;

            public int Cell_Off_Size { get => Cell_Size * 3 / 8; }

            public Font Clue_Font;
            public static readonly StringFormat Clue_Format;
            public Brush Clue_Brush;
            public int Clue_Spacing;
            public int Clue_Board_Offset;

            static Board_Blueprint()
            {
                Clue_Format = new StringFormat();
                Clue_Format.Alignment = StringAlignment.Center;
                Clue_Format.LineAlignment = StringAlignment.Center;
            }
        }

        private enum Board_Blueprint_Type
        {
            Small = 0,
            Medium = 1,
            Large = 2,
            Huge = 3,
        }

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

            private readonly Board_Blueprint Blueprint;

            private readonly Line_Segment[] Off_Lines;

            public Cell(Cell_State initial_state, Board_Blueprint blueprint, Rectangle screen_rectangle)
            {
                State = initial_state;
                Screen_Rectangle = screen_rectangle;

                Blueprint = blueprint;

                int off_left = Screen_Rectangle.X + blueprint.Cell_Size / 2 - blueprint.Cell_Off_Size;
                int off_right = Screen_Rectangle.X + blueprint.Cell_Size / 2 + blueprint.Cell_Off_Size;
                int off_top = Screen_Rectangle.Y + blueprint.Cell_Size / 2 - blueprint.Cell_Off_Size;
                int off_bottom = Screen_Rectangle.Y + blueprint.Cell_Size / 2 + blueprint.Cell_Off_Size;

                Off_Lines = new Line_Segment[]
                {
                    new Line_Segment
                    {
                        First = new Point(off_left, off_top),
                        Second = new Point(off_right, off_bottom),
                    },
                    new Line_Segment
                    {
                        First = new Point(off_right, off_top),
                        Second = new Point(off_left, off_bottom),
                    },
                };
            }

            public bool Contains(Point point)
            {
                return Screen_Rectangle.Contains(point);
            }

            public void Draw(Graphics g)
            {
                switch (State)
                {
                    // on == filled in square
                    case Cell_State.on:
                        {
                            g.FillRectangle(Blueprint.Cell_On_Brush, Screen_Rectangle);
                        }
                        break;

                    // off == X
                    case Cell_State.off:
                        {
                            g.DrawLine(Blueprint.Cell_Off_Pen, Off_Lines[0].First, Off_Lines[0].Second);
                            g.DrawLine(Blueprint.Cell_Off_Pen, Off_Lines[1].First, Off_Lines[1].Second);
                        }
                        break;
                }
            }
        }

        private class Clue
        {
            public readonly int[] Group_Sizes;

            private readonly Board_Blueprint Blueprint;

            private readonly Point[] Group_Positions;

            // Initialize from a known solution
            public Clue(Cell_State[] states, Board_Blueprint blueprint, Point start_point, bool draw_groups_vertically)
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

                // Record data for Draw

                Blueprint = blueprint;

                Group_Positions = new Point[total_group_count];

                Initialize_Visual(start_point, draw_groups_vertically);
            }

            // Initialize from clues
            public Clue(int[] group_sizes, Board_Blueprint blueprint, Point start_point, bool draw_groups_vertically)
            {
                Group_Sizes = new int[group_sizes.Length];

                for (int i = 0; i < group_sizes.Length; i++)
                {
                    Group_Sizes[i] = group_sizes[i];
                }

                // Record data for Draw

                Blueprint = blueprint;

                Group_Positions = new Point[group_sizes.Length];

                Initialize_Visual(start_point, draw_groups_vertically);
            }

            private void Initialize_Visual(Point start_point, bool draw_groups_vertically)
            {
                if (draw_groups_vertically)
                {
                    start_point.Y -= Blueprint.Clue_Spacing * Group_Sizes.Length;
                }
                else
                {
                    start_point.X -= Blueprint.Clue_Spacing * Group_Sizes.Length;
                }

                for (int i = 0; i < Group_Sizes.Length; i++)
                {
                    Group_Positions[i] = start_point;

                    if (draw_groups_vertically)
                    {
                        start_point.Y += Blueprint.Clue_Spacing;
                    }
                    else
                    {
                        start_point.X += Blueprint.Clue_Spacing;
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
                        Blueprint.Clue_Font,
                        Blueprint.Clue_Brush,
                        Group_Positions[i],
                        Board_Blueprint.Clue_Format);
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

        private static readonly Board_Blueprint[] k_board_blueprints =
        {
            new Board_Blueprint // to work perfectly, the sizes here need to be a multiple of 12
            {
                Cell_Size = 108,
                Exterior_Pen = new Pen(Color.Black, 8),
                Interior_Pen = new Pen(Color.DarkGray, 4),
                Cell_On_Brush = new SolidBrush(Color.Black),
                Cell_Off_Pen = new Pen(Color.Black, 16),
                Clue_Font = new Font(FontFamily.GenericMonospace, 24),
                Clue_Brush = new SolidBrush(Color.Black),
                Clue_Spacing = 30,
                Clue_Board_Offset = 50,
            },
            new Board_Blueprint // /2
            {
                Cell_Size = 54,
                Exterior_Pen = new Pen(Color.Black, 4),
                Interior_Pen = new Pen(Color.DarkGray, 2),
                Cell_On_Brush = new SolidBrush(Color.Black),
                Cell_Off_Pen = new Pen(Color.Black, 8),
                Clue_Font = new Font(FontFamily.GenericMonospace, 12),
                Clue_Brush = new SolidBrush(Color.Black),
                Clue_Spacing = 15,
                Clue_Board_Offset = 25,
            },
            new Board_Blueprint // /3
            {
                Cell_Size = 36,
                Exterior_Pen = new Pen(Color.Black, 3),
                Interior_Pen = new Pen(Color.DarkGray, 1),
                Cell_On_Brush = new SolidBrush(Color.Black),
                Cell_Off_Pen = new Pen(Color.Black, 6),
                Clue_Font = new Font(FontFamily.GenericMonospace, 8),
                Clue_Brush = new SolidBrush(Color.Black),
                Clue_Spacing = 10,
                Clue_Board_Offset = 18,
            },
            new Board_Blueprint // /4
            {
                Cell_Size = 27,
                Exterior_Pen = new Pen(Color.Black, 2),
                Interior_Pen = new Pen(Color.DarkGray, 1),
                Cell_On_Brush = new SolidBrush(Color.Black),
                Cell_Off_Pen = new Pen(Color.Black, 4),
                Clue_Font = new Font(FontFamily.GenericMonospace, 6),
                Clue_Brush = new SolidBrush(Color.Black),
                Clue_Spacing = 8,
                Clue_Board_Offset = 12,
            },
        };

        // Private data members

        private readonly int m_rows;
        private readonly int m_columns;

        private readonly Board_Blueprint m_blueprint;

        private readonly Rectangle m_exterior_rectangle;

        private readonly Line_Segment[] m_interior_lines;

        // TODO work out what's readonly and what's not. is the lifetime of Boad a single puzzle?
        private Cell[,] m_cells;
        private Group[] m_groups;
        private Clue[] m_row_clues;
        private Clue[] m_column_clues;

        // Public methods

        public Board(int rows, int columns, Point start_point)
        {
            m_rows = rows;
            m_columns = columns;

            m_blueprint = Get_Blueprint(rows, columns);

            // Initialize the Board's background

            m_exterior_rectangle = new Rectangle(
                new Point(
                    start_point.X + m_blueprint.Clue_Spacing * 4,
                    start_point.Y + m_blueprint.Clue_Spacing * 4),
                new Size(
                    columns * m_blueprint.Cell_Size,
                    rows * m_blueprint.Cell_Size));

            m_interior_lines = new Line_Segment[rows - 1 + columns - 1];
            int line_segment_index = 0;

            for (int column = 1; column < m_columns; column++, line_segment_index++)
            {
                int x_position = m_exterior_rectangle.Left + m_blueprint.Cell_Size * column;

                m_interior_lines[line_segment_index] = new Line_Segment
                {
                    First = new Point(x_position, m_exterior_rectangle.Top),
                    Second = new Point(x_position, m_exterior_rectangle.Bottom),
                };
            }

            for (int row = 1; row < m_columns; row++, line_segment_index++)
            {
                int y_position = m_exterior_rectangle.Top + m_blueprint.Cell_Size * row;

                m_interior_lines[line_segment_index] = new Line_Segment
                {
                    First = new Point(m_exterior_rectangle.Left, y_position),
                    Second = new Point(m_exterior_rectangle.Right, y_position),
                };
            }

            // Initialize the grid of cells

            m_cells = new Cell[columns, rows];

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    m_cells[column, row] = new Cell(
                        Cell_State.maybe,
                        m_blueprint,
                        new Rectangle(
                            m_exterior_rectangle.Left + m_blueprint.Cell_Size * column,
                            m_exterior_rectangle.Top + m_blueprint.Cell_Size * row,
                            m_blueprint.Cell_Size, m_blueprint.Cell_Size));
                }
            }

            // TODO not a hardcoded solution maybe? naaaaaaaah

            // Initialize_To_Hardcoded_Heart();
            Initialize_To_Clues_2020_08_19();

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

        private void Initialize_To_Clues_2020_08_19()
        {
            int[][] column_hints = new int[][]
            {
                new int[]{ 2 },
                new int[]{ 2, 2 },
                new int[]{ 3, 2 },
                new int[]{ 2, 4 },
                new int[]{ 1, 3 },
                new int[]{ 6 },
                new int[]{ 1 },
                new int[]{ 6, 2 },
                new int[]{ 5, 5 },
                new int[]{ 6, 2 },
                new int[]{ 3, 2, 4 },
                new int[]{ 5, 7 },
                new int[]{ 4, 1, 2 },
                new int[]{ 2, 4 },
                new int[]{ 2 },
            };

            int[][] row_hints = new int[][]
            {
                new int[]{ 2 },
                new int[]{ 4 },
                new int[]{ 5 },
                new int[]{ 2, 3 },
                new int[]{ 3, 6 },
                new int[]{ 2, 1, 6 },
                new int[]{ 2, 1, 1 },
                new int[]{ 2, 1, 1, 1, 3 },
                new int[]{ 1, 1, 3, 1 },
                new int[]{ 2, 1, 2, 2 },
                new int[]{ 1, 1, 2, 2, 2 },
                new int[]{ 2, 2, 1, 2 },
                new int[]{ 3, 3, 1 },
                new int[]{ 2, 2, 2 },
                new int[]{ 1, 1, 1 },
            };

            m_column_clues = new Clue[m_columns];

            for (int column = 0; column < m_columns; column++)
            {
                Point clue_position = m_cells[column, 0].Screen_Rectangle.Location;
                clue_position.X += m_blueprint.Cell_Size / 2;
                clue_position.Y += m_blueprint.Cell_Size / 2 - m_blueprint.Clue_Board_Offset;

                m_column_clues[column] = new Clue(column_hints[column], m_blueprint, clue_position, true);
            }

            m_row_clues = new Clue[m_rows];

            for (int row = 0; row < m_rows; row++)
            {
                Point clue_position = m_cells[0, row].Screen_Rectangle.Location;
                clue_position.X += m_blueprint.Cell_Size / 2 - m_blueprint.Clue_Board_Offset;
                clue_position.Y += m_blueprint.Cell_Size / 2;

                m_row_clues[row] = new Clue(row_hints[row], m_blueprint, clue_position, false);
            }
        }

        private void Initialize_To_Hardcoded_Heart()
        {
            // Initialize the solution

            Cell_State[,] solution = new Cell_State[m_columns, m_rows];

            for (int row = 0; row < m_rows; row++)
            {
                for (int column = 0; column < m_columns; column++)
                {
                    solution[column, row] = Cell_State.off;
                }
            }

            // Heart

            solution[0, 1] = Cell_State.on;
            solution[0, 2] = Cell_State.on;

            solution[1, 0] = Cell_State.on;
            solution[1, 1] = Cell_State.on;
            solution[1, 2] = Cell_State.on;
            solution[1, 3] = Cell_State.on;

            solution[2, 1] = Cell_State.on;
            solution[2, 2] = Cell_State.on;
            solution[2, 3] = Cell_State.on;
            solution[2, 4] = Cell_State.on;

            solution[3, 0] = Cell_State.on;
            solution[3, 1] = Cell_State.on;
            solution[3, 2] = Cell_State.on;
            solution[3, 3] = Cell_State.on;

            solution[4, 1] = Cell_State.on;
            solution[4, 2] = Cell_State.on;

            // Generate the clues

            m_column_clues = new Clue[m_columns];

            for (int column = 0; column < m_columns; column++)
            {
                Cell_State[] solution_states = new Cell_State[m_rows];

                for (int row = 0; row < m_rows; row++)
                {
                    solution_states[row] = solution[column, row];
                }

                Point clue_position = m_cells[column, 0].Screen_Rectangle.Location;
                clue_position.X += m_blueprint.Cell_Size / 2;
                clue_position.Y += m_blueprint.Cell_Size / 2 - m_blueprint.Clue_Board_Offset;

                m_column_clues[column] = new Clue(solution_states, m_blueprint, clue_position, true);
            }

            m_row_clues = new Clue[m_rows];

            for (int row = 0; row < m_rows; row++)
            {
                Cell_State[] solution_states = new Cell_State[m_columns];

                for (int column = 0; column < m_columns; column++)
                {
                    solution_states[column] = solution[column, row];
                }

                Point clue_position = m_cells[0, row].Screen_Rectangle.Location;
                clue_position.X += m_blueprint.Cell_Size / 2 - m_blueprint.Clue_Board_Offset;
                clue_position.Y += m_blueprint.Cell_Size / 2;

                m_row_clues[row] = new Clue(solution_states, m_blueprint, clue_position, false);
            }
        }

        private static Board_Blueprint Get_Blueprint(int rows, int columns)
        {
            int largest_side = Math.Max(rows, columns);

            if (largest_side <= 5)
            {
                return k_board_blueprints[(int)Board_Blueprint_Type.Small];
            }
            else if (largest_side <= 10)
            {
                return k_board_blueprints[(int)Board_Blueprint_Type.Medium];
            }
            else if (largest_side <= 15)
            {
                return k_board_blueprints[(int)Board_Blueprint_Type.Large];
            }
            else
            {
                return k_board_blueprints[(int)Board_Blueprint_Type.Huge];
            }
        }

        public static Size Get_Size(int rows, int columns)
        {
            Board_Blueprint blueprint = Get_Blueprint(rows, columns);

            return new Size(
                columns * blueprint.Cell_Size + blueprint.Clue_Spacing * 4,
                rows * blueprint.Cell_Size + blueprint.Clue_Spacing * 4);
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
                g.DrawLine(m_blueprint.Interior_Pen, line.First, line.Second);
            }

            // Draw the outer rectangle

            g.DrawRectangle(m_blueprint.Exterior_Pen, m_exterior_rectangle);

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
                    permutation[i] = (i < off_section_count ? 0 : 1); // TODO: make these 'on' and off' for code readability.
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
                // TODO detect that the board is wrong! User made a mistake trying to solve something.

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
