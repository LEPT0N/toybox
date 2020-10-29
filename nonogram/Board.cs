using System.Drawing;

// TODO separate logic from visual

namespace Nonogram
{
    public class Board
    {
        // Constant Declarations

        private const int k_board_start_offset = 50;
        private const int k_cell_size = 100;

        private static readonly Pen k_exterior_pen = new Pen(Color.Black, 8);
        private static readonly Pen k_interior_pen = new Pen(Color.DarkGray, 4);

        private static readonly Brush k_cell_on_brush = new SolidBrush(Color.Black);
        private static readonly Pen k_cell_off_pen = new Pen(Color.Black, 16);

        private static readonly int k_cell_off_offset = k_board_start_offset * 3 / 4;

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

        // Private data members

        private readonly int m_rows;
        private readonly int m_columns;

        private readonly Rectangle m_exterior_rectangle;

        private Cell[,] m_cells;
        private Cell_State[,] m_solution;

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

            for (int y = 0; y < m_cells.GetLength(1); y++)
            {
                for (int x = 0; x < m_cells.GetLength(0); x++)
                {
                    m_cells[x, y] = new Cell(
                        Cell_State.maybe,
                        new Rectangle(
                            m_exterior_rectangle.Left + k_cell_size * x,
                            m_exterior_rectangle.Top + k_cell_size * y,
                            k_cell_size, k_cell_size));
                }
            }

            // Initialize the solution

            m_solution = new Cell_State[columns, rows];

            for (int y = 0; y < m_solution.GetLength(1); y++)
            {
                for (int x = 0; x < m_solution.GetLength(0); x++)
                {
                    m_solution[x, y] = Cell_State.off;
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
        }

        public void Draw(Graphics g)
        {
            // Draw the cells

            for (int x = 0; x < m_cells.GetLength(0); x++)
            {
                for (int y = 0; y < m_cells.GetLength(1); y++)
                {
                    switch (m_cells[x, y].State)
                    {
                        // on == filled in square
                        case Cell_State.on:
                            {
                                g.FillRectangle(
                                    k_cell_on_brush,
                                    m_cells[x, y].Screen_Rectangle);
                            }
                            break;

                        // off == X
                        case Cell_State.off:
                            {
                                int center_x = m_exterior_rectangle.Left + k_cell_size * x + k_cell_size / 2;
                                int center_y = m_exterior_rectangle.Top + k_cell_size * y + k_cell_size / 2;

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
        }

        public bool Check_Click(Point click_location)
        {
            for (int x = 0; x < m_cells.GetLength(0); x++)
            {
                for (int y = 0; y < m_cells.GetLength(1); y++)
                {
                    if (m_cells[x, y].Screen_Rectangle.Contains(click_location))
                    {
                        switch (m_cells[x, y].State)
                        {
                            case Cell_State.off:
                                m_cells[x, y].State = Cell_State.maybe;
                                break;

                            case Cell_State.maybe:
                                m_cells[x, y].State = Cell_State.on;
                                break;

                            case Cell_State.on:
                                m_cells[x, y].State = Cell_State.off;
                                break;
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        private bool Process_Group(Cell[] cells)
        {
            // TODO, do the thing: build list of all possibilities, then delete all the 'maybe's you can.

            if (cells[0].State != Cell_State.on)
            {
                // 'hello world' that Process_Group can do something.
                cells[0].State = Cell_State.on;

                return true;
            }

            return false;
        }

        public void Give_Hint()
        {
            // Check over each row and each column to see if any of them can add something

            for (int x = 0; x < m_cells.GetLength(0); x++)
            {
                Cell[] cells = new Cell[m_cells.GetLength(1)];

                for (int y = 0; y < m_cells.GetLength(1); y++)
                {
                    cells[y] = m_cells[x, y];
                }

                if (Process_Group(cells))
                {
                    return;
                }
            }

            for (int y = 0; y < m_cells.GetLength(1); y++)
            {
                Cell[] cells = new Cell[m_cells.GetLength(0)];

                for (int x = 0; x < m_cells.GetLength(0); x++)
                {
                    cells[x] = m_cells[x, y];
                }

                if (Process_Group(cells))
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
