using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using WpfMaze.MazeGame;

namespace WpfMaze.Mazegame
{
    internal class Maze
    {
        public BitmapPart[] Bitmaps;
        public int Width => Board.GetLength(1);


        public int Height => Board.GetLength(0);

        public byte[,] Board { get; set; }

        public Player Player { get; set; }


        public Finish Finish { get; set; }

        public bool IsSolved => Player == Finish;

        public int Threads;

        internal delegate void MazeEvent(Maze maze);

        public event MazeEvent Rendered;

        public event MazeEvent Rendering;

        public Maze(int Width, int Height, bool randomize = false, int threads = 1)
        {
            Board = new byte[Height, Width];
            Threads = threads * threads;
            Application.Current.Dispatcher.Invoke(() =>
            {
                Bitmaps = new BitmapPart[Threads];
                for (var i = 0; i < Threads; i++)
                {
                    var dimensions = getBitmapPartSize();
                    var startingPoint = getStartingPoint(i + 1);
                    Bitmaps[i] = new BitmapPart(dimensions[1], dimensions[0], startingPoint.X, startingPoint.Y);
                }
            });
            if (randomize)
                this.randomize();
        }

        public Point getStartingPoint(int threadNumber)
        {
            var startingPoint = new Point();
            var dimensions = getBitmapPartSize();

            startingPoint.X = (int)(dimensions[1] * ((threadNumber - 1) % Math.Sqrt(Threads)));
            startingPoint.Y = (int)(dimensions[0] * Math.Ceiling(threadNumber / Math.Sqrt(Threads) - 1));

            return startingPoint;
        }

        /**
         * <returns>[0]=>y, [1]=>x</returns>
         */
        public int[] getBitmapPartSize()
        {
            var dimension = new int[2];

            dimension[0] = (int)(Height / Math.Sqrt(Threads));
            dimension[1] = (int)(Width / Math.Sqrt(Threads));


            return dimension;
        }

        public async void paintBitmaps()
        {
            Rendering?.Invoke(this);
            List<Task> tasks = new List<Task>();
            foreach (var bitmap in Bitmaps)
            {
                Stopwatch watch = new Stopwatch();
               (IntPtr,int,int,int) t =  Application.Current.Dispatcher.Invoke(() =>
                {
                    bitmap.Bitmap.Lock();

                    (IntPtr, int,int,int) tupel = (bitmap.Bitmap.BackBuffer, bitmap.Bitmap.BackBufferStride, (int) bitmap.Bitmap.Width,(int) bitmap.Bitmap.Height);
                    return tupel;
                });
                Console.WriteLine(watch.ElapsedMilliseconds);
                tasks.Add(Task.Run(() =>
               {
                   for (int x = 1; x != t.Item4 - 1; x++)
                   {
                       for (int y = 1; y != t.Item3 - 1; y++)
                       {
                           if (Board[x + bitmap.Start.X, y + bitmap.Start.Y] == 1)
                           {
                               BitmapPart.DrawPixel(x, y, new int[3] { 0, 0, 0 }, t.Item1, t.Item2);
                           }
                           else
                           {
                               BitmapPart.DrawPixel(x, y, new int[3] { 255, 255, 255 }, t.Item1, t.Item2);
                           }
                       }

                   }
               }));
            }
            await Task.WhenAll(tasks);
            foreach (var bitmap in Bitmaps)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    bitmap.Bitmap.AddDirtyRect(new Int32Rect(0, 0, (int)bitmap.Bitmap.Width, (int)bitmap.Bitmap.Height));
                    bitmap.Bitmap.Unlock();
                });

            }
            Rendered?.Invoke(this);
        }

        public void randomize()
        {
            // This algorithm is a randomized version of Prim's algorithm. (see https://en.wikipedia.org/wiki/Maze_generation_algorithm#Randomized_Prim%27s_algorithm)
            var random = new Random();
            int r = 0, c = 0;

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
                r = random.Next(1, Height - 2);
                c = random.Next(1, Height - 2);
            } while (Board[r, c] != 0 ||
                     Board[r, c + 1] + Board[r, c - 1] + Board[r + 1, c] + Board[r - 1, c] < 3);

            Player = new Player(c, r);

            // position exit point
            do
            {
                r = random.Next(1, Height - 2);
                c = random.Next(1, Height - 2);
            } while (Board[r, c] != 0 ||
                     Board[r, c + 1] + Board[r, c - 1] + Board[r + 1, c] + Board[r - 1, c] < 3);

            Finish = new Finish(c, r);
        }

        public bool MovePlayer(Direction direction)
        {
            if (this.PlayerCanMove(direction))
            {
                var (deltaX, deltaY) = direction.GetMovementDeltas();

                this.Player.X += deltaX;
                this.Player.Y += deltaY;

                //this.OnPlayerPositionChange?.Invoke(this, 1);

                //if (this.IsSolved)
                    //this.OnMazeSolved?.Invoke(new Maze(100, 100, true), 1);


                return true;
            }

            return false;
        }


        private bool IsInBounds(int row, int col)
        {
            if (row <= 0)
                return false;
            if (row > Height - 2)
                return false;
            if (col <= 0)
                return false;
            if (col > Width - 2)
                return false;
            return true;
        }

        public bool PlayerCanMove(Direction direction)
        {
            if (IsSolved)
                return false;

            var (deltaX, deltaY) = direction.GetMovementDeltas();
            return Board[Player.Y + deltaY, Player.X + deltaX] != 1;
        }
    }
}