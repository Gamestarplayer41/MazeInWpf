using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfMaze.Mazegame.MazeGenerationAlgorithms;
using WpfMaze.MazeGame.Space;

namespace WpfMaze.Mazegame
{
    public class MazeRewrite
    {
        public delegate void MazeEvent(MazeRewrite maze, object args);

        public WriteableBitmap Bitmap { get; private set; }
        public byte[,] Board { get; }

        public Finish Finish { get; private set; }

        public Player Player { get; private set; }

        private List<AMazeAlgorithm> Algorithms = new List<AMazeAlgorithm>();

        public MazeRewrite(int width, int height, Type algorithm, bool randomize = true)
        {
            Board = new byte[height, width];
            this.SetAlgorithms();
            Application.Current.Dispatcher.Invoke(() =>
            {
                Bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Indexed8, BitmapPalettes.Halftone256);
            });
            this.calculateMaze(algorithm);
        }

        private async void calculateMaze(Type algorithm)
        {
             await Task.Run(() =>
            {
                AMazeAlgorithm algo = Algorithms.Find(x => x.GetType().Equals(algorithm));
                algo?.GenerateMaze();
                SetPlayerAndFinish();
                CalculateBitmap();
            });
        }

        private void SetAlgorithms()
        {
            Algorithms.Add(new BinaryTree(Board));
            Algorithms.Add(new PrimsAlgorithm(Board));
        }

        private void SetPlayerAndFinish()
        {
            Random random = new Random();
            int r, c;
            do
            {
                r = random.Next(1, Height - 2);
                c = random.Next(1, Width - 2);
            } while (Board[r, c] != 0);
            
            Player = new Player(c, r);
            int deltaX, deltaY, delta;
            // position exit point
            do
            {
                r = random.Next(1, Height - 2);
                c = random.Next(1, Width - 2);
                deltaX = Player.X - c;
                deltaY = Player.Y - r;
                delta = (int)(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
            } while (Board[r, c] != 0 || delta < 3);
            
            Finish = new Finish(c, r);
        }

        public bool IsSolved => Player == Finish;

        public int Width => Board.GetLength(1);

        public int Height => Board.GetLength(0);


        public event MazeEvent OnSolved;
        

        private bool IsInBounds(int row, int col)
        {
            if (row <= 0)
                return false;
            if (col <= 0)
                return false;
            if (col > Width - 2)
                return false;
            if (row > Height - 2)
                return false;
            return true;
        }

        private async void CalculateBitmap()
        {
            var backbuffer = new IntPtr();
            var stride = 0;
            Application.Current.Dispatcher.Invoke(() =>
            {
                Bitmap.TryLock(Duration.Forever);
                backbuffer = Bitmap.BackBuffer;
                stride = Bitmap.BackBufferStride;
            });
            await Task.Run(() =>
            {
                byte[] black = {0,0,0 };
                byte[] white = {255, 255, 255};
                for (var y = 0; y < Height; y++)
                    for (var x = 0; x < Width; x++)
                        if (Player.X == x && Player.Y == y)
                            DrawPixel(x, y, Player.Color, backbuffer, stride);
                        else if (Finish.X == x && Finish.Y == y)
                            DrawPixel(x, y, Finish.Color, backbuffer, stride);
                        else if (Board[y, x] == 1)
                            DrawPixel(x, y, black, backbuffer, stride);
                        else
                            DrawPixel(x, y, white, backbuffer, stride);
            });
            Application.Current.Dispatcher.Invoke(() =>
            {
                Bitmap.AddDirtyRect(new Int32Rect(0, 0, Width, Height));
                Bitmap.Unlock();
            });
        }

        private void RenderPlayerPosition(Player oldPlayerPosition)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Bitmap.TryLock(Duration.Forever);
                DrawPixel(oldPlayerPosition.X, oldPlayerPosition.Y, new byte[] { 237, 175, 166 }, Bitmap.BackBuffer,
                    Bitmap.BackBufferStride);
                DrawPixel(Player.X, Player.Y, Player.Color, Bitmap.BackBuffer, Bitmap.BackBufferStride);
                // Bitmap.AddDirtyRect(new Int32Rect(0, 0, Width, Height));
                Bitmap.AddDirtyRect(new Int32Rect(oldPlayerPosition.X,oldPlayerPosition.Y,1,1));
                Bitmap.AddDirtyRect(new Int32Rect(Player.X,Player.Y,1,1));
                Bitmap.Unlock();
            });
        }

        public bool MovePlayer(Direction direction)
        {
            if (!PlayerCanMove(direction))
                return false;
            var (deltaX, deltaY) = direction.GetMovementDeltas();
            var oldPlayerPosition = new Player(Player.X, Player.Y);
            Player.X += deltaX;
            Player.Y += deltaY;
            RenderPlayerPosition(oldPlayerPosition);
            if (IsSolved)
                OnSolved?.Invoke(this, null);
            return true;
        }

        public bool PlayerCanMove(Direction direction)
        {
            if (IsSolved)
                return false;
            var (deltaX, deltaY) = direction.GetMovementDeltas();
            return Board[Player.Y + deltaY, Player.X + deltaX] == 0;
        }

        private static void DrawPixel(int x, int y, byte[] color, IntPtr backBuffer, int stride)
        {
            var column = x;
            var row = y;
            unsafe
            {
                // Find the address of the pixel to draw.
                backBuffer += row * stride;
                backBuffer += column;
                // Compute the pixel's color.
                int colorData = (color[0] << 5) | (color[1] << 2) | color[2];

                // Assign the color data to the pixel.
                *(byte*)backBuffer = (byte)colorData;
            }
        }
    }
}