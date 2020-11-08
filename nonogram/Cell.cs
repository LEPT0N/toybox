using System.Drawing;

namespace Nonogram
{
    public class Cell
    {
        public enum State
        {
            off,
            maybe,
            on,
        }

        public State state; // TODO: standardize casing for properties/fields/methods/classes

        public readonly Rectangle Screen_Rectangle;

        private readonly Board_Style Style;

        private readonly Line_Segment[] Off_Lines;

        public Cell(State initial_state, Board_Style style, Rectangle screen_rectangle)
        {
            state = initial_state;
            Screen_Rectangle = screen_rectangle;

            Style = style;

            int off_left = Screen_Rectangle.X + style.Cell_Size / 2 - style.Cell_Off_Size;
            int off_right = Screen_Rectangle.X + style.Cell_Size / 2 + style.Cell_Off_Size;
            int off_top = Screen_Rectangle.Y + style.Cell_Size / 2 - style.Cell_Off_Size;
            int off_bottom = Screen_Rectangle.Y + style.Cell_Size / 2 + style.Cell_Off_Size;

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
            switch (state)
            {
                // on == filled in square
                case State.on:
                    {
                        g.FillRectangle(Style.Cell_On_Brush, Screen_Rectangle);
                    }
                    break;

                // off == X
                case State.off:
                    {
                        g.DrawLine(Style.Cell_Off_Pen, Off_Lines[0].First, Off_Lines[0].Second);
                        g.DrawLine(Style.Cell_Off_Pen, Off_Lines[1].First, Off_Lines[1].Second);
                    }
                    break;
            }
        }
    }
}
