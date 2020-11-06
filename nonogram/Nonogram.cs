using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Nonogram
{
    public class Nonogram : Form
    {
        private const int k_window_padding = 25;

        private IContainer m_components = null;

        private Board m_board;
        private Base_Button m_hint_button;
        private Base_Button m_undo_button;

        public Nonogram()
        {
            Size board_size = Board.Get_Size(15, 15);

            m_board = new Board(
                15, 15,
                new Point(k_window_padding,  k_window_padding));

            Size window_size = new Size(
                board_size.Width + k_window_padding * 2 + 150, // TODO: based on button sizes
                board_size.Height + k_window_padding * 2);

            m_hint_button = new Hint_Button(
                m_board,
                new Point(
                    window_size.Width - k_window_padding - 100, // TODO: based on button sizes
                    k_window_padding));

            m_undo_button = new Undo_Button(
                m_board,
                new Point(
                    window_size.Width - k_window_padding - 100, // TODO: based on button sizes
                    window_size.Height - k_window_padding - 100)); // TODO: based on button sizes

            this.m_components = new Container();
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = window_size;
            this.Text = "Nonogram";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (m_components != null))
            {
                m_components.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            m_board.Draw(e.Graphics);
            m_hint_button.Draw(e.Graphics);
            m_undo_button.Draw(e.Graphics);
        }

        protected override void OnClick(EventArgs e)
        {
            Point click_location = (e as MouseEventArgs).Location;

            // TODO route onclick in a less silly way
            if (m_board.Check_Click(click_location) ||
                m_hint_button.Check_Click(click_location) ||
                m_undo_button.Check_Click(click_location))
            {
                Refresh();
            }
        }
    }
}
