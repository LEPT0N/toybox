using System.Drawing;

// TODO separate logic from visual

namespace Nonogram
{
    class Board
    {
        // Constant Declarations

        private const int k_board_start_offset = 50;
        private const int k_cell_size = 100;

        private readonly Pen k_exterior_pen = new Pen(Color.Black, 8);
        private readonly Pen k_interior_pen = new Pen(Color.DarkGray, 4);

        private readonly Brush k_cell_on_brush = new SolidBrush(Color.Black);
        private readonly Pen k_cell_off_pen = new Pen(Color.Black, 16);

        private readonly int k_cell_off_offset = k_board_start_offset * 3 / 4;

        // Private type definitions

        private enum Cell_State
        {
            off,
            maybe,
            on,
        }

        private struct Cell
        {
            public Cell_State State;

            public Rectangle Screen_Rectangle;
        }

        // Private data members

        private readonly int m_rows;
        private readonly int m_columns;

        private readonly Rectangle m_exterior_rectangle;

        private Cell[,] m_cells;

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
                    m_cells[x, y].State = Cell_State.maybe;

                    m_cells[x, y].Screen_Rectangle = new Rectangle(
                        m_exterior_rectangle.Left + k_cell_size * x,
                        m_exterior_rectangle.Top + k_cell_size * y,
                        k_cell_size, k_cell_size);
                }
            }
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

        public bool OnClick(Point click_location)
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
    }
}
