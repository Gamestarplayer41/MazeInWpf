using System;
using System.Collections.Generic;

namespace WpfMaze.Mazegame.MazeGenerationAlgorithms
{
    public class PrimsAlgorithm : AMazeAlgorithm
    {
        public PrimsAlgorithm(byte[,] board)
        {
            Board = board;
        }

        public override void GenerateMaze()
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
    }
}