using System;
using System.Net;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Point = WpfMaze.Mazegame.Point;

namespace WpfMaze.MazeGame
{
    internal class BitmapPart
    {
        public WriteableBitmap Bitmap;
        public Point Start;

        public int written = 0;


        public BitmapPart(int width, int height, int x, int y)
        {
            Bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr32, null);
            Start = new Point();
            Start.X = x;
            Start.Y = y;
        }

        public int width;

        public int height;

        public static void DrawPixel(int x, int y, int[] Color, IntPtr backBuffer,int stride)
        {
            var column = y;
            var row = x;
            unsafe
            {
                // Get a pointer to the back buffer.

                // Find the address of the pixel to draw.
                backBuffer += row * stride;
                backBuffer += column * 4;

                // Compute the pixel's color.
                var color_data = Color[0] << 16; // R
                color_data |= Color[1] << 8; // G
                color_data |= Color[2] << 0; // B

                // Assign the color data to the pixel.
                *(int*)backBuffer = color_data;
            }
            // Specify the area of the bitmap that changed.
            //Application.Current.Dispatcher.Invoke(() =>
            //Bitmap.AddDirtyRect(new Int32Rect(column, row, 1, 1)));
        }

        public void ErasePixel(int x, int y)
        {
            byte[] ColorData = { 0, 0, 0, 0 }; // B G R
            var rect = new Int32Rect(y, x, 1, 1);

            Bitmap.WritePixels(rect, ColorData, 4, 0);
        }
    }
}