using System;
using System.Collections.Generic;
using WpfMaze.MazeGame.Space;

namespace WpfMaze.Mazegame.MazeGenerationAlgorithms
{
    public class BinaryTree : AMazeAlgorithm
    {
        public BinaryTree(byte[,] board)
        {
            Board = board;
            PossibleDirections = new List<Direction>();
        }

        private List<Direction> PossibleDirections { get; }

        public override void GenerateMaze()
        {
            var random = new Random();
            SetAllWalls();
            var first = true;
            for (var x = 1; x < Width; x += 2)
            for (var y = 1; y < Height; y += 2)
            {
                if (first)
                {
                    Board[y, x] = 0;
                    first = false;
                    continue;
                }

                Board[y, x] = 0;
                GetPossibleDirections(x, y);
                Direction randomDirection = PossibleDirections[random.Next(PossibleDirections.Count)];
                var (deltaX, deltaY) = randomDirection.GetMovementDeltas();
                Board[y + deltaY, x + deltaX] = 0;
            }

            for (var x = 0; x < Width; x++) Board[Height - 1, x] = 1;
            for (var y = 0; y < Height; y++) Board[y, Width - 1] = 1;
        }

        private void GetPossibleDirections(int x, int y)
        {
            PossibleDirections.Clear();
            if (IsInBounds(x - 2, y) && Board[y, x - 2] == 0)
                PossibleDirections.Add(Direction.Left);
            if (IsInBounds(x, y + 2) && Board[y + 2, x] == 0)
                PossibleDirections.Add(Direction.Down);
            if (IsInBounds(x + 2, y) && Board[y, x + 2] == 0)
                PossibleDirections.Add(Direction.Right);
            if (IsInBounds(x, y - 2) && Board[y - 2, x] == 0)
                PossibleDirections.Add(Direction.Up);
        }

        private bool IsInBounds(int x, int y)
        {
            if (x < 0)
                return false;
            if (y < 0)
                return false;
            if (x > Width - 1)
                return false;
            if (y > Height - 1)
                return false;
            return true;
        }

        private void SetAllWalls()
        {
            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
                Board[y, x] = 1;
        }
    }
}