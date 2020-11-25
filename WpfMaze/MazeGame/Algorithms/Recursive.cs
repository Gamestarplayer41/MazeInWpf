using System;
using System.Collections.Generic;
using System.Diagnostics;
using WpfMaze.Mazegame;
using WpfMaze.MazeGame.Space;

namespace WpfMaze.MazeGame.Algorithms
{
    public class Recursive : IAlgorithm
    {
        //overhead prevention
        private readonly List<Direction> CleanedDirections = new List<Direction>();
        private readonly Path Path = new Path();
        private readonly List<Direction> PossibleDirections = new List<Direction>();
        private readonly Random Random = new Random();


        private MazeRewrite Maze;
        private byte[,] Visited;
        private int X, Y;
        public bool StopThread { get; set; }

        public void SolveMaze()
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

        public void InjectMaze(MazeRewrite maze)
        {
            Maze = maze;
            Visited = new byte[maze.Height, maze.Width];
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