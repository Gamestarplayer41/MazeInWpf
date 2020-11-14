using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WpfMaze.MazeGame;

namespace WpfMaze.Mazegame
{
    public class Maze
    {
        public WriteableBitmap Bitmap;
        public int Width => Board.GetLength(0);


        public int Height => Board.GetLength(1);

        public byte[,] Board { get; set; }

        public Player Player { get; set; }

        public Finish Finish { get; set; }

        public bool IsSolved => Player == Finish;

        public delegate void MazeEvent(Maze maze);

        public event MazeEvent Rendered;

        public event MazeEvent Rendering;

        public event MazeEvent OnMazeSolved;

        public delegate void playerPositionChange(Maze maze, Point player);

        public event playerPositionChange onPlayerPositionChange;

        public Maze(int width, int height, bool randomize = false)
        {
            Board = new byte[width, height];
            Application.Current.Dispatcher.Invoke(() =>
              {
                  this.Bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr32, null);
              });
            if (randomize)
                this.randomize();
        }

        private void changePlayerPosition(Point oldPlayerPosition)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                this.Bitmap.Lock();
                DrawPixel(oldPlayerPosition.X, oldPlayerPosition.Y, new int[3] { 255, 0, 255 }, this.Bitmap.BackBuffer, this.Bitmap.BackBufferStride);
                DrawPixel(Player.X, Player.Y, new int[3] { 255, 0, 0 }, this.Bitmap.BackBuffer, this.Bitmap.BackBufferStride);
                this.Bitmap.AddDirtyRect(new Int32Rect(0, 0,(int) Bitmap.Width,(int) Bitmap.Height));
                this.Bitmap.Unlock();
            });
        }

        public async void paintBitmaps(bool black = false)
        {
            Rendering?.Invoke(this);
            (IntPtr, int, int, int) t = Bitmap.Dispatcher.Invoke(() =>
               {
                   Bitmap.Lock();

                   (IntPtr, int, int, int) tupel = (Bitmap.BackBuffer, Bitmap.BackBufferStride, (int)Bitmap.Width, (int)Bitmap.Height);
                   return tupel;
               });
            await Task.Run(() =>
           {
               for (int x = 0; x < t.Item3 - 1; x++)
               {
                   for (int y = 0; y < t.Item4 - 1; y++)
                   {
                       if (x == Player.X && y == Player.Y)
                       {
                           DrawPixel(x, y, new int[3] { 255, 0, 0 }, t.Item1, t.Item2);
                       }
                       else if (x == Finish.X && y == Finish.Y)
                       {
                           DrawPixel(x, y, new int[3] { 0, 0, 255 }, t.Item1, t.Item2);
                       }
                       else if (Board[x, y] == 1)
                       {
                           DrawPixel(x, y, new int[3] { 0, 0, 0 }, t.Item1, t.Item2);
                       }
                       else
                       {
                           DrawPixel(x, y, (black) ? new int[3] { 0, 0, 0 } : new int[3] { 255, 255, 255 }, t.Item1, t.Item2);
                       }
                   }

               }
           });
            Bitmap.Dispatcher.Invoke(() =>
            {
                Bitmap.AddDirtyRect(new Int32Rect(0, 0, (int)Bitmap.Width, (int)Bitmap.Height));
                Bitmap.Unlock();
            });

            Rendered?.Invoke(this);
        }

        public void randomize()
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
                var (Row, Col) = walls[index];

                // If only one of the two cells that the wall divides is visited, then:
                if (random.Next(1) == 1)
                {
                    if (Board[Row, Col] != 0 && Board[Row + 1, Col] + Board[Row - 1, Col] == 1)
                    {
                        (int Row, int Col) unvisitedCell = (Row + 1, Col);

                        if (Board[Row - 1, Col] == 1)
                            unvisitedCell = (Row - 1, Col);

                        //  Make the wall a passage and mark the unvisited cell as part of the maze.
                        Board[Row, Col] = 0;

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
                    else if (Board[Row, Col] != 0 && Board[Row, Col + 1] + Board[Row, Col - 1] == 1)
                    {
                        (int Row, int Col) unvisitedCell = (Row, Col + 1);

                        if (Board[Row, Col - 1] == 1)
                            unvisitedCell = (Row, Col - 1);

                        //  Make the wall a passage and mark the unvisited cell as part of the maze.
                        Board[Row, Col] = 0;

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
                    if (Board[Row, Col] != 0 && Board[Row, Col + 1] + Board[Row, Col - 1] == 1)
                    {
                        (int Row, int Col) unvisitedCell = (Row, Col + 1);

                        if (Board[Row, Col - 1] == 1)
                            unvisitedCell = (Row, Col - 1);

                        //  Make the wall a passage and mark the unvisited cell as part of the maze.
                        Board[Row, Col] = 0;

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
                    else if (Board[Row, Col] != 0 && Board[Row + 1, Col] + Board[Row - 1, Col] == 1)
                    {
                        (int Row, int Col) unvisitedCell = (Row + 1, Col);

                        if (Board[Row - 1, Col] == 1)
                            unvisitedCell = (Row - 1, Col);

                        //  Make the wall a passage and mark the unvisited cell as part of the maze.
                        Board[Row, Col] = 0;

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
            do
            {
                r = random.Next(1, Width - 2);
                c = random.Next(1, Height - 2);
            } while ((Board[r, c] != 0 ||
                     Board[r, c + 1] + Board[r, c - 1] + Board[r + 1, c] + Board[r - 1, c] < 3));

            Finish = new Finish(r, c);
            Board[r, c] = 4;
        }

        public bool MovePlayer(Direction direction)
        {
            if (this.PlayerCanMove(direction))
            {
                var (deltaX, deltaY) = direction.GetMovementDeltas();
                Point oldPlayerPosition = new Point() { X = this.Player.X, Y = this.Player.Y };
                this.Player.X += deltaX;
                this.Player.Y += deltaY;
                this.changePlayerPosition(oldPlayerPosition);
                if (this.IsSolved)
                    this.OnMazeSolved?.Invoke(this);
                return true;
            }

            return false;
        }


        private bool IsInBounds(int row, int col)
        {
            if (row <= 0)
                return false;
            if (row > Width - 2)
                return false;
            if (col <= 0)
                return false;
            if (col > Height - 2)
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

        private static void DrawPixel(int x, int y, int[] Color, IntPtr backBuffer, int stride)
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
    }
}