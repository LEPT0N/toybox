
namespace Nonogram
{
    public class Puzzle
    {
        public int[][] Column_Hints;

        public int[][] Row_Hints;
    }

    public static class Default_Puzzles
    {
        public static Puzzle Heart
        {
            get
            {
                Puzzle puzzle = new Puzzle();

                puzzle.Column_Hints = new int[][]
                {
                    new int[]{ 2 },
                    new int[]{ 4 },
                    new int[]{ 4 },
                    new int[]{ 4 },
                    new int[]{ 2 },
                };

                puzzle.Row_Hints = new int[][]
                {
                    new int[]{ 1, 1 },
                    new int[]{ 5 },
                    new int[]{ 5 },
                    new int[]{ 3 },
                    new int[]{ 1 },
                };

                return puzzle;
            }
        }

        public static Puzzle Nonogram_Com_2020_08_19
        {
            get
            {
                Puzzle puzzle = new Puzzle();

                puzzle.Column_Hints = new int[][]
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

                puzzle.Row_Hints = new int[][]
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

                return puzzle;
            }
        }

        public static Puzzle Expert_01
        {
            get
            {
                Puzzle puzzle = new Puzzle();

                puzzle.Column_Hints = new int[][]
                {
                    new int[]{ 1 },
                    new int[]{ 3 },
                    new int[]{ 2, 3 },
                    new int[]{ 3, 5 },
                    new int[]{ 3, 3, 3 },
                    new int[]{ 1, 3, 2, 6 },
                    new int[]{ 1, 2, 2, 8 },
                    new int[]{ 2, 3, 2, 5 },
                    new int[]{ 2, 8 },
                    new int[]{ 3, 6, 1 },
                    new int[]{ 4, 5, 3 },
                    new int[]{ 7, 5 },
                    new int[]{ 5, 3 },
                    new int[]{ 2, 4 },
                    new int[]{ 3, 5, 2 },
                    new int[]{ 3, 6 },
                    new int[]{ 3, 3 },
                    new int[]{ 4 },
                    new int[]{ 5 },
                    new int[]{ 2 },
                };

                puzzle.Row_Hints = new int[][]
                {
                    new int[]{ 5 },
                    new int[]{ 4 },
                    new int[]{ 3, 3 },
                    new int[]{ 7, 2 },
                    new int[]{ 8, 2 },
                    new int[]{ 2, 3, 5 },
                    new int[]{ 10 },
                    new int[]{ 9, 5 },
                    new int[]{ 11, 3 },
                    new int[]{ 3, 3, 3, 3 },
                    new int[]{ 2, 5, 3, 2 },
                    new int[]{ 2, 2, 5, 1 },
                    new int[]{ 1, 2, 2, 3 },
                    new int[]{ 2, 3, 3 },
                    new int[]{ 3, 2, 2 },
                    new int[]{ 2, 2, 2 },
                    new int[]{ 2, 1 },
                    new int[]{ 3 },
                    new int[]{ 3 },
                    new int[]{ 3 },
                };

                return puzzle;
            }
        }
        public static Puzzle Bungie_Feb_2021_Newsletter
        {
            get
            {
                Puzzle puzzle = new Puzzle();

                puzzle.Column_Hints = new int[][]
                {
                    new int[]{ 3 },
                    new int[]{ 4 },
                    new int[]{ 4 },
                    new int[]{ 5 },
                    new int[]{ 6 },
                    new int[]{ 3, 5 },
                    new int[]{ 3, 5 },
                    new int[]{ 6 },
                    new int[]{ 5 },
                    new int[]{ 4 },
                    new int[]{ 4 },
                    new int[]{ 3 },
                };

                puzzle.Row_Hints = new int[][]
                {
                    new int[]{ 3, 3 },
                    new int[]{ 4, 2, 4 },
                    new int[]{ 4, 2, 4 },
                    new int[]{ 3, 2, 3 },
                    new int[]{ 2, 2 },
                    new int[]{ 2, 2 },
                    new int[]{ 4 },
                    new int[]{ 4 },
                    new int[]{ 4 },
                    new int[]{ 4 },
                    new int[]{ 2 },
                };

                return puzzle;
            }
        }
    }
}
