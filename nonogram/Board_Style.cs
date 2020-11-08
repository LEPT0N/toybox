using System.Diagnostics;
using System.Drawing;

namespace Nonogram
{
    public class Board_Style
    {
        public int Cell_Size;

        public Pen Exterior_Pen;
        public Pen Interior_Pen;

        public Brush Cell_On_Brush;
        public Pen Cell_Off_Pen;

        public int Cell_Off_Size { get => Cell_Size * 3 / 8; }

        public Font Clue_Font;
        public Brush Clue_Brush;
        public int Clue_Spacing;
        public int Clue_Board_Offset;

        private static readonly StringFormat k_clue_format;
        public StringFormat Clue_Format { get => k_clue_format; }

        static Board_Style()
        {
            k_clue_format = new StringFormat();
            k_clue_format.Alignment = StringAlignment.Center;
            k_clue_format.LineAlignment = StringAlignment.Center;
        }

        public enum Type
        {
            Small,
            Medium,
            Large,
            Huge,
        }

        public static Board_Style Get_Style(Type type)
        {
            // to work perfectly, the sizes for 'Small' need to be a multiple of 12

            switch (type)
            {
                case Type.Small:
                    return new Board_Style // /1
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
                    };

                case Type.Medium:
                    return new Board_Style // /2
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
                    };

                case Type.Large:
                    return new Board_Style // /3
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
                    };

                case Type.Huge:
                    return new Board_Style // /4
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
                    };

                default:
                    Debug.Fail($"Unknown Type '{type}'");
                    return null;
            }
        }
    }
}
