using System;
using System.Collections.Generic;
using System.Diagnostics;
using WpfMaze.Mazegame;

namespace WpfMaze.MazeGame.Algorithms
{
    public class Recursive : IAlgorithm
    {
        public bool stopThread { get; set; }


        private MazeRewrite Maze;
        private Random random = new Random();
        private Path Path = new Path();
        private int X, Y;
        private byte[,] Visited;

        public void SolveMaze()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            bool found = false;
            X = Maze.Player.X;
            Y = Maze.Player.Y;
            Visited[Y, X] = 1;
            while (!found)
            {
                List<Direction> directions = getDirectionsNotVisited(getPossibleDirections(X, Y));
                if (directions.Count == 0)
                {
                    Direction oldDir = Path.removeLastElement();
                    var (deltaXOld, deltaYOld) = oldDir.GetMovementDeltas();
                    X -= deltaXOld;
                    Y -= deltaYOld;
                    continue;
                }
                Direction dir = directions[random.Next(directions.Count)];
                (int deltaX, int deltaY) = dir.GetMovementDeltas();
                Path.addElement(dir);
                X += deltaX;
                Y += deltaY;
                Visited[Y, X] = 1;
                if (Maze.Finish.X == X && Maze.Finish.Y == Y)
                    found = true;
            }
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds + "ms " + Path.Directions.Count + " elements");
        }

        private List<Direction> getDirectionsNotVisited(List<Direction> directions)
        {
            List<Direction> cleanedDirections = new List<Direction>();
            foreach (var dir in directions)
            {
                (int deltaX, int deltaY) = dir.GetMovementDeltas();
                if (Visited[Y + deltaY, X + deltaX] == 0)
                    cleanedDirections.Add(dir);
            }
            return cleanedDirections;
        }

        private List<Direction> getPossibleDirections(int x, int y)
        {
            List<Direction> directions = new List<Direction>();
            if (Maze.Board[y + 1, x] == 0)
                directions.Add(Direction.Down);
            if (Maze.Board[y, x + 1] == 0)
                directions.Add(Direction.Right);
            if (Maze.Board[y - 1, x] == 0)
                directions.Add(Direction.Up);
            if (Maze.Board[y, x - 1] == 0)
                directions.Add(Direction.Left);
            return directions;
        }

        public void injectMaze(MazeRewrite maze)
        {
            Maze = maze;
            Visited = new byte[maze.Height, maze.Width];
        }

        public void printSolutionBoard()
        {
            for (int y = 0; y < Visited.GetLength(1); y++)
            {
                for (int x = 0; x < Visited.GetLength(0); x++)
                {
                    Console.Write(Visited[y, x]);
                }

                Console.Write("\n");
            }

            Console.WriteLine(" ");
        }
    }
}