using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace advent_of_code_common.display_helpers
{
    public static class special_characters
    {
        // https://www.fileformat.info/info/unicode/block/box_drawing/images.htm

        public static char k_box_icon_left_and_right = '\u2500';
        public static char k_box_icon_up_and_down = '\u2502';

        public static char k_box_icon_down_and_right = '\u250C';
        public static char k_box_icon_down_and_left = '\u2510';
        public static char k_box_icon_up_and_right = '\u2514';
        public static char k_box_icon_up_and_left = '\u2518';

        public static char k_box_icon_all_but_up = '\u252C';
        public static char k_box_icon_all_but_down = '\u2534';
        public static char k_box_icon_all_but_left = '\u251C';
        public static char k_box_icon_all_but_right = '\u2524';

        public static char k_box_icon_up = '\u2575';
        public static char k_box_icon_down = '\u2577';
        public static char k_box_icon_left = '\u2574';
        public static char k_box_icon_right = '\u2576';

        public static char k_box_icon_all_four = '\u253C';
        public static char k_box_icon_none = ' ';
    }

    public static class display_utilities
    {
        public static void display_grid_top(int width)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(special_characters.k_box_icon_down_and_right);
            for (int column = 0; column < width; column++)
            {
                Console.Write(special_characters.k_box_icon_left_and_right);
            }
            Console.WriteLine(special_characters.k_box_icon_down_and_left);
            Console.ResetColor();
        }

        public static void display_grid_side()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(special_characters.k_box_icon_up_and_down);
            Console.ResetColor();
        }

        public static void display_grid_bottom(int width)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(special_characters.k_box_icon_up_and_right);
            for (int column = 0; column < width; column++)
            {
                Console.Write(special_characters.k_box_icon_left_and_right);
            }
            Console.WriteLine(special_characters.k_box_icon_up_and_left);
            Console.ResetColor();
        }
    }

    public static class extensions
    {
        public static void display<T>(this T[,] grid, Action<T> value_display_function = null)
        {
            display_utilities.display_grid_top(grid.GetLength(1));

            for (int row = 0; row < grid.GetLength(0); row++)
            {
                display_utilities.display_grid_side();

                for (int column = 0; column < grid.GetLength(1); column++)
                {
                    if (value_display_function != null)
                    {
                        value_display_function(grid[row, column]);
                    }
                    else
                    {
                        Console.Write(grid[row, column].ToString());
                    }
                }

                display_utilities.display_grid_side();
                Console.WriteLine();
            }

            display_utilities.display_grid_bottom(grid.GetLength(1));
        }

        public static void display<T>(this T[][] grid, Action<T> value_display_function = null)
        {
            display_utilities.display_grid_top(grid[0].Length);

            for (int row = 0; row < grid.Length; row++)
            {
                display_utilities.display_grid_side();

                for (int column = 0; column < grid[row].Length; column++)
                {
                    if (value_display_function != null)
                    {
                        value_display_function(grid[row][column]);
                    }
                    else
                    {
                        Console.Write(grid[row][column].ToString());
                    }
                }

                display_utilities.display_grid_side();
                Console.WriteLine();
            }

            display_utilities.display_grid_bottom(grid[0].Length);
        }

        public static Bitmap create_bitmap<T>(
            this T[][] grid,
            int scale,
            Func<T, Color> get_pixel_color)
        {
            Bitmap bitmap = new Bitmap(grid[0].Length * scale, grid.Length * scale);

            for (int y = 0; y < grid.Length; y++)
            {
                for (int x = 0; x < grid[y].Length; x++)
                {
                    Color cell_color = get_pixel_color(grid[y][x]);

                    int x_start = x * scale;
                    int y_start = y * scale;

                    for (int x_offset = 0; x_offset < scale; x_offset++)
                    {
                        for (int y_offset = 0; y_offset < scale; y_offset++)
                        {
                            bitmap.SetPixel(x_start + x_offset, y_start + y_offset, cell_color);
                        }
                    }
                }
            }

            return bitmap;
        }

        public static Bitmap create_bitmap<T>(
            this T[,] grid,
            int scale,
            Func<T, Color> get_pixel_color)
        {
            Bitmap bitmap = new Bitmap(grid.GetLength(1) * scale, grid.GetLength(0) * scale);

            for (int y = 0; y < grid.GetLength(0); y++)
            {
                for (int x = 0; x < grid.GetLength(1); x++)
                {
                    Color cell_color = get_pixel_color(grid[y,x]);

                    int x_start = x * scale;
                    int y_start = y * scale;

                    for (int x_offset = 0; x_offset < scale; x_offset++)
                    {
                        for (int y_offset = 0; y_offset < scale; y_offset++)
                        {
                            bitmap.SetPixel(x_start + x_offset, y_start + y_offset, cell_color);
                        }
                    }
                }
            }

            return bitmap;
        }

        internal static ImageCodecInfo get_encoder(
            ImageFormat image_format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == image_format.Guid)
                {
                    return codec;
                }
            }

            return null;
        }

        // Found by searching Bing for "Create animated gif in C# without 3rd party package" and AI showed me how to do this.

        // ... I need to learn what this is all about.

        public static MemoryStream create_gif(
            this List<Bitmap> frames)
        {
            // Save the first frame.

            MemoryStream stream = new MemoryStream();

            EncoderParameters encoder_parameters = new EncoderParameters(1);
            encoder_parameters.Param[0] = new EncoderParameter(
                Encoder.SaveFlag,
                (long)EncoderValue.MultiFrame);

            ImageCodecInfo gif_codec = get_encoder(ImageFormat.Gif);

            frames[0].Save(stream, gif_codec, encoder_parameters);

            // Save the remaining frames.

            encoder_parameters.Param[0] = new EncoderParameter(
                Encoder.SaveFlag,
                (long)EncoderValue.FrameDimensionTime);

            for (int i = 1; i < frames.Count; i++)
            {
                frames[0].SaveAdd(frames[i], encoder_parameters);
            }

            // End the animation.

            encoder_parameters.Param[0] = new EncoderParameter(
                Encoder.SaveFlag,
                (long)EncoderValue.Flush);

            frames[0].SaveAdd(encoder_parameters);

            return stream;
        }
    }
}
