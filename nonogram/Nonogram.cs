using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Nonogram
{
    public class Nonogram : Form
    {
        private IContainer m_components = null;

        private Board m_board;

        public Nonogram()
        {
            InitializeComponent();

            m_board = new Board(5, 5);
        }

        private void InitializeComponent()
        {
            this.m_components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 600); // TODO size based on the board
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
        }

        protected override void OnClick(EventArgs e)
        {
            if (m_board.OnClick((e as MouseEventArgs).Location))
            {
                Refresh();
            }
        }
    }
}
