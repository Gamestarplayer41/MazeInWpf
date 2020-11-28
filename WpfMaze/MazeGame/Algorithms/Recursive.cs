using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using WpfMaze.Mazegame;
using WpfMaze.MazeGame.Space;

namespace WpfMaze.MazeGame.Algorithms
{
    public class Recursive : AAlgorithm
    {
        private List<Direction> CleanedDirections { get; }

        private List<Direction> PossibleDirections { get; }

        private Random Random { get; } = new Random();

        private byte[,] Visited;
        private int X, Y;

        public Recursive(MazeRewrite maze)
        {
            Maze = maze;
            Visited = new byte[Maze.Height, Maze.Width];
            CleanedDirections = new List<Direction>();
            PossibleDirections = new List<Direction>();
        }

        public override void SolveMaze()
        {
            var watch = new Stopwatch();
            watch.Start();
            var found = false;
            X = Maze.Player.X;
            Y = Maze.Player.Y;
            Visited[Y, X] = 1;
            while (!found)
            {
                GetPossibleDirections(X, Y);
                GetDirectionsNotVisited();
                if (CleanedDirections.Count == 0)
                {
                    if (Path.Directions.Count == 0)
                        return;
                    Direction oldDir = Path.RemoveLastElement();
                    var (deltaXOld, deltaYOld) = oldDir.GetMovementDeltas();
                    X -= deltaXOld;
                    Y -= deltaYOld;
                    continue;
                }
                Direction dir = CleanedDirections[Random.Next(CleanedDirections.Count)];
                var (deltaX, deltaY) = dir.GetMovementDeltas();
                Path.AddElement(dir);
                X += deltaX;
                Y += deltaY;
                Visited[Y, X] = 1;
                if (Maze.Finish.X == X && Maze.Finish.Y == Y)
                    found = true;
            }
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds + "ms " + Path.Directions.Count + " elements");
        }

        private void GetDirectionsNotVisited()
        {
            CleanedDirections.Clear();
            foreach (Direction dir in PossibleDirections)
            {
                var (deltaX, deltaY) = dir.GetMovementDeltas();
                if (Visited[Y + deltaY, X + deltaX] == 0)
                    CleanedDirections.Add(dir);
            }
        }

        private void GetPossibleDirections(int x, int y)
        {
            PossibleDirections.Clear();
            if (Maze.Board[y + 1, x] == 0)
                PossibleDirections.Add(Direction.Down);
            if (Maze.Board[y, x + 1] == 0)
                PossibleDirections.Add(Direction.Right);
            if (Maze.Board[y - 1, x] == 0)
                PossibleDirections.Add(Direction.Up);
            if (Maze.Board[y, x - 1] == 0)
                PossibleDirections.Add(Direction.Left);
        }
    }
}