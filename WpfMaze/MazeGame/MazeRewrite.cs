using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.Printing;
using System.Security;
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
    public class MazeRewrite
    {
        private byte[,] Board;

        private Player Player;

        private Finish Finish;

        public bool isSolved
        {
            get => Player == Finish;
        }

        public int Width
        {
            get => Board.GetLength(0);
        }

        public int Height
        {
            get => Board.GetLength(1);
        }

        public WriteableBitmap Bitmap;

        delegate void MazeEvent(MazeRewrite maze, object args);

        private event MazeEvent onPlayerMove;

        private event MazeEvent onSolved;

        public MazeRewrite(int width, int height, bool randomize = true)
        {
            Board = new byte[height, width];
            Application.Current.Dispatcher.Invoke(() =>
            {
                Bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr32, null);
            });
            if (randomize)
                this.calculateMaze();
        }

        private void calculateMaze()
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
                c = random.Next(1, Width - 2);
            } while (Board[r, c] != 0 ||
                     Board[r, c + 1] + Board[r, c - 1] + Board[r + 1, c] + Board[r - 1, c] < 3);

            Player = new Player(c, r);
            // position exit point
            do
            {
                r = random.Next(1, Height - 2);
                c = random.Next(1, Width - 2);
            } while (Board[r, c] != 0 &&
                     Board[r, c + 1] + Board[r, c - 1] + Board[r + 1, c] + Board[r - 1, c] < 3);

            Finish = new Finish(c, r);
            this.calculateBitmap();
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

        private async void calculateBitmap()
        {
            IntPtr backbuffer = new IntPtr();
            int stride = 0;
            Application.Current.Dispatcher.Invoke(() =>
            {
                Bitmap.Lock();
                backbuffer = Bitmap.BackBuffer;
                stride = Bitmap.BackBufferStride;
            });
            await Task.Run(() =>
            {
                for (int y = 0; y < Width; y++)
                {
                    for (int x = 0; x < Height; x++)
                    {
                        if (Player.X == x && Player.Y == y)
                        {
                            DrawPixel(x, y, new int[3] {255, 215, 0}, backbuffer, stride);
                        }
                        else if (Finish.X == x && Finish.Y == y)
                        {
                            DrawPixel(x, y, new int[3] {255, 0, 0}, backbuffer, stride);
                        }
                        else if (Board[y, x] == 1)
                        {
                            DrawPixel(x, y, new int[3] {0, 0, 0}, backbuffer, stride);
                        }
                        else
                        {
                            DrawPixel(x, y, new int[3] {255, 255, 255}, backbuffer, stride);
                        }
                    }
                }
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
                this.Bitmap.Lock();
                DrawPixel(oldPlayerPosition.X, oldPlayerPosition.Y, new int[3] {255, 255, 255}, Bitmap.BackBuffer,
                    Bitmap.BackBufferStride);
                DrawPixel(Player.X,Player.Y,new int[3]{255,215,0},Bitmap.BackBuffer,Bitmap.BackBufferStride);
                Bitmap.AddDirtyRect(new Int32Rect(0,0,Width,Height));
                Bitmap.Unlock();
            });
        }

        public bool MovePlayer(Direction direction)
        {
            if (!PlayerCanMove(direction))
                return false;
            var (deltaX, deltaY) = direction.GetMovementDeltas();
            Player oldPlayerPosition = new Player(Player.X, Player.Y);
            Player.X += deltaX;
            Player.Y += deltaY;
            this.RenderPlayerPosition(oldPlayerPosition);
            if(isSolved)
                onSolved?.Invoke(this,null);
            return true;
        }

        private bool PlayerCanMove(Direction direction)
        {
            if (isSolved)
                return false;
            var (deltaX, deltaY) = direction.GetMovementDeltas();
            return Board[Player.Y + deltaY, Player.X + deltaX] == 0;
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
                *(int*) backBuffer = color_data;
            }

            // Specify the area of the bitmap that changed.
            //Application.Current.Dispatcher.Invoke(() =>
            //Bitmap.AddDirtyRect(new Int32Rect(column, row, 1, 1)));
        }
    }
}