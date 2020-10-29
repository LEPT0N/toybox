using Nonogram;
using System.Drawing;

namespace nonogram
{
    public abstract class Base_Button
    {
        protected readonly Rectangle m_outline_rectangle;

        private readonly Pen m_outline_pen;

        private readonly Brush m_fill_brush;

        public Base_Button(
            Rectangle outline_rectangle,
            Pen outline_pen,
            Brush fill_brush)
        {
            m_outline_rectangle = outline_rectangle;
            m_outline_pen = outline_pen;
            m_fill_brush = fill_brush;
        }

        public abstract void Draw(Graphics g);

        protected void Draw_Outline(Graphics g)
        {
            g.FillRectangle(m_fill_brush, m_outline_rectangle);

            g.DrawRectangle(m_outline_pen, m_outline_rectangle);
        }

        protected abstract void On_Click();

        public bool Check_Click(Point click_location)
        {
            if (m_outline_rectangle.Contains(click_location))
            {
                On_Click();

                return true;
            }

            return false;
        }
    }

    public class Hint_Button : Base_Button
    {
        // TODO: standardize all brushes/fills in the UI to one set of definitions.

        private static readonly Pen k_pen = new Pen(Color.Black, 8);

        private static readonly Brush k_fill_brush = new SolidBrush(Color.LightGray);

        private static readonly Size k_size = new Size(100, 100);

        private readonly Board m_board;

        private readonly Rectangle m_circle_bounds;

        public Hint_Button(Board board, Point location)
            : base(new Rectangle(location, k_size), k_pen, k_fill_brush)
        {
            m_board = board;

            m_circle_bounds = new Rectangle(
                m_outline_rectangle.X + m_outline_rectangle.Width / 4,
                m_outline_rectangle.Y + m_outline_rectangle.Height / 4,
                m_outline_rectangle.Width / 2,
                m_outline_rectangle.Height / 2);
        }

        public override void Draw(Graphics g)
        {
            Draw_Outline(g);

            g.DrawEllipse(k_pen, m_circle_bounds);
        }

        protected override void On_Click()
        {
            m_board.Give_Hint();
        }
    }

    public class Undo_Button : Base_Button
    {
        private static readonly Pen k_pen = new Pen(Color.Black, 8);

        private static readonly Brush k_fill_brush = new SolidBrush(Color.LightGray);

        private static readonly Size k_size = new Size(100, 100);

        private readonly Board m_board;

        private readonly Point m_start_point;
        private readonly Point m_end_point;

        public Undo_Button(Board board, Point location)
            : base(new Rectangle(location, k_size), k_pen, k_fill_brush)
        {
            m_board = board;

            m_start_point = new Point(
                m_outline_rectangle.X + k_size.Width / 4,
                m_outline_rectangle.Y + k_size.Height / 2);

            m_end_point = new Point(
                m_outline_rectangle.X + k_size.Width - k_size.Width / 4,
                m_outline_rectangle.Y + k_size.Height / 2);
        }

        public override void Draw(Graphics g)
        {
            Draw_Outline(g);

            g.DrawLine(k_pen, m_start_point, m_end_point);
        }

        protected override void On_Click()
        {
            m_board.Undo();
        }
    }
}
