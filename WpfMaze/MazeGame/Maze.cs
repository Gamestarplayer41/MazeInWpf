using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfMaze.MazeGame.Space;
using Point = WpfMaze.MazeGame.Space.Point;

namespace WpfMaze.Mazegame
{
    public class Maze
    {
        public delegate void MazeEvent(Maze maze);

        public delegate void PlayerPositionChange(Maze maze, Point player);

        public WriteableBitmap Bitmap;

        public Maze(int width, int height, bool randomize = false)
        {
            Board = new byte[width, height];
            Application.Current.Dispatcher.Invoke(() =>
            {
                Bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr32, null);
            });
            if (randomize)
                Randomize();
        }

        public int Width => Board.GetLength(0);


        public int Height => Board.GetLength(1);

        public byte[,] Board { get; set; }

        public Player Player { get; set; }

        public Finish Finish { get; set; }

        public bool IsSolved => Player == Finish;

        public event MazeEvent Rendered;

        public event MazeEvent Rendering;

        public event MazeEvent OnMazeSolved;

        private void ChangePlayerPosition(Point oldPlayerPosition)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Bitmap.Lock();
                DrawPixel(oldPlayerPosition.X, oldPlayerPosition.Y, new[] {255, 0, 255}, Bitmap.BackBuffer,
                    Bitmap.BackBufferStride);
                DrawPixel(Player.X, Player.Y, new[] {255, 0, 0}, Bitmap.BackBuffer,
                    Bitmap.BackBufferStride);
                Bitmap.AddDirtyRect(new Int32Rect(0, 0, (int) Bitmap.Width, (int) Bitmap.Height));
                Bitmap.Unlock();
            });
        }

        public async void PaintBitmaps(bool black = false)
        {
            Rendering?.Invoke(this);
            (IntPtr intPtr, var item2, var item3, var item4) = Bitmap.Dispatcher.Invoke(() =>
            {
                Bitmap.Lock();

                (IntPtr, int, int, int) tuple = (Bitmap.BackBuffer, Bitmap.BackBufferStride, (int) Bitmap.Width,
                    (int) Bitmap.Height);
                return tuple;
            });
            await Task.Run(() =>
            {
                for (var x = 0; x < item3 - 1; x++)
                for (var y = 0; y < item4 - 1; y++)
                    if (x == Player.X && y == Player.Y)
                        DrawPixel(x, y, new[] {255, 0, 0}, intPtr, item2);
                    else if (x == Finish.X && y == Finish.Y)
                        DrawPixel(x, y, new[] {255, 215, 0}, intPtr, item2);
                    else if (Board[x, y] == 1)
                        DrawPixel(x, y, new[] {0, 0, 0}, intPtr, item2);
                    else
                        DrawPixel(x, y, black ? new[] {0, 0, 0} : new[] {255, 255, 255}, intPtr,
                            item2);
            });
            Bitmap.Dispatcher.Invoke(() =>
            {
                Bitmap.AddDirtyRect(new Int32Rect(0, 0, (int) Bitmap.Width, (int) Bitmap.Height));
                Bitmap.Unlock();
            });

            Rendered?.Invoke(this);
        }

        public void Randomize()
        {
            // This algorithm is a randomized version of Prim's algorithm. (see https://en.wikipedia.org/wiki/Maze_generation_algorithm#Randomized_Prim%27s_algorithm)
            var random = new Random();
            int r = 0, c = 0;

            // Start with a grid full of walls.
            for (r = 0; r < Width; r++)
            for (c = 0; c < Height; c++)
                Board[r, c] = 1;

            // Pick a cell, mark it as part of the maze. Add the walls of the cell to the wall list.
            var walls = new List<(int Row, int Col)>();

            r = random.Next(1, Width - 2);
            c = random.Next(1, Height - 2);

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
                r = random.Next(1, Width - 2);
                c = random.Next(1, Height - 2);
            } while (Board[r, c] != 0 ||
                     Board[r, c + 1] + Board[r, c - 1] + Board[r + 1, c] + Board[r - 1, c] < 3);

            Player = new Player(r, c);
            Board[r, c] = 3;

            // position exit point
            int deltaX, deltaY;
            do
            {
                r = random.Next(1, Width - 2);
                c = random.Next(1, Height - 2);
                deltaX = Math.Abs(Player.X - r);
                deltaY = Math.Abs(Player.Y - c);
            } while (Board[r, c] != 0 &&
                     Board[r, c + 1] + Board[r, c - 1] + Board[r + 1, c] + Board[r - 1, c] < 3 &&
                     (deltaX < 3 || deltaY < 3));


            Finish = new Finish(r, c);
            Board[r, c] = 4;
        }

        public bool MovePlayer(Direction direction)
        {
            if (PlayerCanMove(direction))
            {
                var (deltaX, deltaY) = direction.GetMovementDeltas();
                var oldPlayerPosition = new Point {X = Player.X, Y = Player.Y};
                Player.X += deltaX;
                Player.Y += deltaY;
                ChangePlayerPosition(oldPlayerPosition);
                if (IsSolved)
                    OnMazeSolved?.Invoke(this);
                return true;
            }

            return false;
        }


        private bool IsInBounds(int row, int col)
        {
            if (row <= 0)
                return false;
            if (row > Width - 1)
                return false;
            if (col <= 0)
                return false;
            if (col > Height - 1)
                return false;
            return true;
        }

        public bool PlayerCanMove(Direction direction)
        {
            if (IsSolved)
                return false;

            var (deltaX, deltaY) = direction.GetMovementDeltas();
            return Board[Player.X + deltaX, Player.Y + deltaY] != 1;
        }

        private static void DrawPixel(int x, int y, int[] color, IntPtr backBuffer, int stride)
        {
            var column = x;
            var row = y;
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
    }
}