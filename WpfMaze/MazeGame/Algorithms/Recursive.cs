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

        private List<Direction> PossibleDirections { get; }

        private Random Random { get; } = new Random();

        private bool[,] Visited;
        private int X, Y;

        public Recursive(MazeRewrite maze)
        {
            Maze = maze;
            Visited = new Boolean[Maze.Height, Maze.Width];
            PossibleDirections = new List<Direction>();
        }

        public override void SolveMaze()
        {
            var watch = new Stopwatch();
            watch.Start();
            var found = false;
            X = Maze.Player.X;
            Y = Maze.Player.Y;
            Visited[Y, X] = true;
            while (!found)
            {
                GetPossibleDirections(X, Y);
                if (PossibleDirections.Count == 0)
                {
                    if (Path.Directions.Count == 0)
                        return;
                    Direction oldDir = Path.RemoveLastElement();
                    var (deltaXOld, deltaYOld) = oldDir.GetMovementDeltas();
                    X -= deltaXOld;
                    Y -= deltaYOld;
                    continue;
                }
                Direction dir = PossibleDirections[Random.Next(PossibleDirections.Count)];
                var (deltaX, deltaY) = dir.GetMovementDeltas();
                Path.AddElement(dir);
                X += deltaX;
                Y += deltaY;
                Visited[Y, X] = true;
                if (Maze.Finish.X == X && Maze.Finish.Y == Y)
                    found = true;
            }
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds + "ms " + Path.Directions.Count + " Elements (Recursive)");
        }

        private void GetPossibleDirections(int x, int y)
        {
            PossibleDirections.Clear();
            if (Maze.Board[y + 1, x] == 0 && !Visited[y+1,x])
                PossibleDirections.Add(Direction.Down);
            if (Maze.Board[y, x + 1] == 0 && !Visited[y,x+1])
                PossibleDirections.Add(Direction.Right);
            if (Maze.Board[y - 1, x] == 0 && !Visited[y-1,x])
                PossibleDirections.Add(Direction.Up);
            if (Maze.Board[y, x - 1] == 0 && !Visited[y,x-1])
                PossibleDirections.Add(Direction.Left);
        }
    }
}