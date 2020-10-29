using nonogram;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Nonogram
{
    public class Nonogram : Form
    {
        private IContainer m_components = null;

        private Board m_board;
        private Base_Button m_hint_button;
        private Base_Button m_undo_button;

        public Nonogram()
        {
            InitializeComponent();

            m_board = new Board(5, 5);

            m_hint_button = new Hint_Button(
                m_board,
                new Point(600, 50)); // TODO again, global layout stuffs

            m_undo_button = new Undo_Button(
                m_board,
                new Point(600, 450)); // TODO again, global layout stuffs
        }

        private void InitializeComponent()
        {
            this.m_components = new Container();
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(750, 600); // TODO size based on the board
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
