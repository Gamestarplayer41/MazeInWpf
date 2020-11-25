using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Point = WpfMaze.MazeGame.Space.Point;

namespace WpfMaze.MazeGame.Painting
{
    internal class BitmapPart
    {
        public WriteableBitmap Bitmap;
        public Point Start;
        

        public BitmapPart(int width, int height, int x, int y)
        {
            Bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr32, null);
            Start = new Point();
            Start.X = x;
            Start.Y = y;
        }

        public static void DrawPixel(int x, int y, int[] color, IntPtr backBuffer, int stride)
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
                var colorData = color[0] << 16; // R
                colorData |= color[1] << 8; // G
                colorData |= color[2] << 0; // B

                // Assign the color data to the pixel.
                *(int*) backBuffer = colorData;
            }

            // Specify the area of the bitmap that changed.
            //Application.Current.Dispatcher.Invoke(() =>
            //Bitmap.AddDirtyRect(new Int32Rect(column, row, 1, 1)));
        }

        public void ErasePixel(int x, int y)
        {
            byte[] colorData = {0, 0, 0, 0}; // B G R
            var rect = new Int32Rect(y, x, 1, 1);

            Bitmap.WritePixels(rect, colorData, 4, 0);
        }
    }
}