using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfMaze.MazeGame.Space;

namespace WpfMaze.Mazegame
{
    public class MazeRewrite
    {
        public delegate void MazeEvent(MazeRewrite maze, object args);

        public WriteableBitmap Bitmap;
        public byte[,] Board;

        public Finish Finish;

        public Player Player;

        public MazeRewrite(int width, int height, bool randomize = true)
        {
            Board = new byte[height, width];
            Application.Current.Dispatcher.Invoke(() =>
            {
                Bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr32, null);
            });
            if (randomize)
                CalculateMaze();
        }

        public bool IsSolved => Player == Finish;

        public int Width => Board.GetLength(1);

        public int Height => Board.GetLength(0);


        public event MazeEvent OnSolved;

        private void CalculateMaze()
        {
            // This algorithm is a randomized version of Prim's algorithm. (see https://en.wikipedia.org/wiki/Maze_generation_algorithm#Randomized_Prim%27s_algorithm)
            var random = new Random();
            int r, c;

            // Start with a grid full of walls.
            for (r = 0; r < Height; r++)
            for (c = 0; c < Width; c++)
                Board[r, c] = 1;

            // Pick a cell, mark it as part of the maze. Add the walls of the cell to the wall list.
            var walls = new List<(int Row, int Col)>();

            r = random.Next(1, Height - 2);
            c = random.Next(1, Width - 2);

            Board[r, c] = 0;

            if (IsInBounds(r + 1, c))
                walls.Add((r + 1, c));

            if (IsInBounds(r - 1, c))
                walls.Add((r - 1, c));

            if (IsInBounds(r, c + 1))
                walls.Add((r, c + 1));

            if (IsInBounds(r, c - 1))
                walls.Add((r, c - 1));

            // While there are walls in the list:
            while (walls.Count > 0)
            {
                // Pick a random wall from the list. 
                var index = random.Next(walls.Count - 1);
                var (row, col) = walls[index];

                // If only one of the two cells that the wall divides is visited, then:
                if (random.Next(1) == 1)
                {
                    if (Board[row, col] != 0 && Board[row + 1, col] + Board[row - 1, col] == 1)
                    {
                        (int Row, int Col) unvisitedCell = (row + 1, col);

                        if (Board[row - 1, col] == 1)
                            unvisitedCell = (row - 1, col);

                        //  Make the wall a passage and mark the unvisited cell as part of the maze.
                        Board[row, col] = 0;

                        if (IsInBounds(unvisitedCell.Row, unvisitedCell.Col))
                            Board[unvisitedCell.Row, unvisitedCell.Col] = 0;


                        // Add the neighboring walls of the cell to the wall list.
                        if (IsInBounds(unvisitedCell.Row + 1, unvisitedCell.Col))
                            walls.Add((unvisitedCell.Row + 1, unvisitedCell.Col));
                        if (IsInBounds(unvisitedCell.Row - 1, unvisitedCell.Col))
                            walls.Add((unvisitedCell.Row - 1, unvisitedCell.Col));
                        if (IsInBounds(unvisitedCell.Row, unvisitedCell.Col + 1))
                            walls.Add((unvisitedCell.Row, unvisitedCell.Col + 1));
                        if (IsInBounds(unvisitedCell.Row, unvisitedCell.Col - 1))
                            walls.Add((unvisitedCell.Row, unvisitedCell.Col - 1));
                    }
                    else if (Board[row, col] != 0 && Board[row, col + 1] + Board[row, col - 1] == 1)
                    {
                        (int Row, int Col) unvisitedCell = (row, col + 1);

                        if (Board[row, col - 1] == 1)
                            unvisitedCell = (row, col - 1);

                        //  Make the wall a passage and mark the unvisited cell as part of the maze.
                        Board[row, col] = 0;

                        if (IsInBounds(unvisitedCell.Row, unvisitedCell.Col))
                            Board[unvisitedCell.Row, unvisitedCell.Col] = 0;

                        // Add the neighboring walls of the cell to the wall list.
                        if (IsInBounds(unvisitedCell.Row + 1, unvisitedCell.Col))
                            walls.Add((unvisitedCell.Row + 1, unvisitedCell.Col));
                        if (IsInBounds(unvisitedCell.Row - 1, unvisitedCell.Col))
                            walls.Add((unvisitedCell.Row - 1, unvisitedCell.Col));
                        if (IsInBounds(unvisitedCell.Row, unvisitedCell.Col + 1))
                            walls.Add((unvisitedCell.Row, unvisitedCell.Col + 1));
                        if (IsInBounds(unvisitedCell.Row, unvisitedCell.Col - 1))
                            walls.Add((unvisitedCell.Row, unvisitedCell.Col - 1));
                    }
                }
                else
                {
                    if (Board[row, col] != 0 && Board[row, col + 1] + Board[row, col - 1] == 1)
                    {
                        (int Row, int Col) unvisitedCell = (row, col + 1);

                        if (Board[row, col - 1] == 1)
                            unvisitedCell = (row, col - 1);

                        //  Make the wall a passage and mark the unvisited cell as part of the maze.
                        Board[row, col] = 0;

                        if (IsInBounds(unvisitedCell.Row, unvisitedCell.Col))
                            Board[unvisitedCell.Row, unvisitedCell.Col] = 0;


                        // Add the neighboring walls of the cell to the wall list.
                        if (IsInBounds(unvisitedCell.Row + 1, unvisitedCell.Col))
                            walls.Add((unvisitedCell.Row + 1, unvisitedCell.Col));
                        if (IsInBounds(unvisitedCell.Row - 1, unvisitedCell.Col))
                            walls.Add((unvisitedCell.Row - 1, unvisitedCell.Col));
                        if (IsInBounds(unvisitedCell.Row, unvisitedCell.Col + 1))
                            walls.Add((unvisitedCell.Row, unvisitedCell.Col + 1));
                        if (IsInBounds(unvisitedCell.Row, unvisitedCell.Col - 1))
                            walls.Add((unvisitedCell.Row, unvisitedCell.Col - 1));
                    }
                    else if (Board[row, col] != 0 && Board[row + 1, col] + Board[row - 1, col] == 1)
                    {
                        (int Row, int Col) unvisitedCell = (row + 1, col);

                        if (Board[row - 1, col] == 1)
                            unvisitedCell = (row - 1, col);

                        //  Make the wall a passage and mark the unvisited cell as part of the maze.
                        Board[row, col] = 0;

                        if (IsInBounds(unvisitedCell.Row, unvisitedCell.Col))
                            Board[unvisitedCell.Row, unvisitedCell.Col] = 0;


                        // Add the neighboring walls of the cell to the wall list.
                        if (IsInBounds(unvisitedCell.Row + 1, unvisitedCell.Col))
                            walls.Add((unvisitedCell.Row + 1, unvisitedCell.Col));
                        if (IsInBounds(unvisitedCell.Row - 1, unvisitedCell.Col))
                            walls.Add((unvisitedCell.Row - 1, unvisitedCell.Col));
                        if (IsInBounds(unvisitedCell.Row, unvisitedCell.Col + 1))
                            walls.Add((unvisitedCell.Row, unvisitedCell.Col + 1));
                        if (IsInBounds(unvisitedCell.Row, unvisitedCell.Col - 1))
                            walls.Add((unvisitedCell.Row, unvisitedCell.Col - 1));
                    }
                }

                walls.RemoveAt(index);
            }

            // position player spawn
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
                delta = (int) (Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
            } while (Board[r, c] != 0 || delta < 3);

            Finish = new Finish(c, r);
            CalculateBitmap();
        }

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
                for (var y = 0; y < Height; y++)
                for (var x = 0; x < Width; x++)
                    if (Player.X == x && Player.Y == y)
                        DrawPixel(x, y, Player.Color, backbuffer, stride);
                    else if (Finish.X == x && Finish.Y == y)
                        DrawPixel(x, y, Finish.Color, backbuffer, stride);
                    else if (Board[y, x] == 1)
                        DrawPixel(x, y, new[] {0, 0, 0}, backbuffer, stride);
                    else
                        DrawPixel(x, y, new[] {255, 255, 255}, backbuffer, stride);
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
                DrawPixel(oldPlayerPosition.X, oldPlayerPosition.Y, new[] {237, 175, 166}, Bitmap.BackBuffer,
                    Bitmap.BackBufferStride);
                DrawPixel(Player.X, Player.Y, Player.Color, Bitmap.BackBuffer, Bitmap.BackBufferStride);
                Bitmap.AddDirtyRect(new Int32Rect(0, 0, Width, Height));
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

        private static void DrawPixel(int x, int y, int[] color, IntPtr backBuffer, int stride)
        {
            var column = x;
            var row = y;
            unsafe
            {
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
        }
    }
}