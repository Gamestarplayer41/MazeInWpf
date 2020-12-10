using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Policy;
using System.Windows;
using WpfMaze.Mazegame;
using WpfMaze.MazeGame.Space;

namespace WpfMaze.MazeGame.Algorithms
{
    public class WavePropagation : AAlgorithm
    {
        private int[,] Distance { get; }
        private List<(int, int)> CurrentNodes { get; set; } // x,y
        private HashSet<(int, int)> NewNodes { get; set; } // x,y

        private Direction[] Directions = new[] {Direction.Down, Direction.Left, Direction.Up, Direction.Right};

        private Stopwatch watch = new Stopwatch();

        public WavePropagation(MazeRewrite maze)
        {
            Maze = maze;
            Distance = new int[Maze.Height, Maze.Width];
            CurrentNodes = new List<(int, int)>();
            NewNodes = new HashSet<(int, int)>();
        }

        public override void SolveMaze()
        {
            watch.Start();
            this.newFun();
            watch.Stop();
            Console.WriteLine($"{watch.ElapsedMilliseconds}ms {Path.Directions.Count} Elements (Wave)");
        }

        private void newFun()
        {
            CurrentNodes.Add((Maze.Finish.X, Maze.Finish.Y));
            Distance[Maze.Finish.Y, Maze.Finish.X] = 1;
            (int deltaX, int deltaY, int x, int y, int directionsIndex, int currentNodesIndex) = (0, 0, 0, 0, 0, 0);
            while (true)
            {
                for (currentNodesIndex = 0; currentNodesIndex < CurrentNodes.Count; currentNodesIndex++)
                {
                    (x, y) = CurrentNodes[currentNodesIndex];
                    for (directionsIndex = 0; directionsIndex < Directions.Length; directionsIndex++)
                    {
                        (deltaX, deltaY) = Directions[directionsIndex].GetMovementDeltas();
                        if (!IsWall(Directions[directionsIndex], x, y) && Distance[y + deltaY, x + deltaX] == 0)
                        {
                            NewNodes.Add((x + deltaX, y + deltaY));
                            Distance[y + deltaY, x + deltaX] = Distance[y, x] + 1;
                            if (x + deltaX == Maze.Player.X && y + deltaY == Maze.Player.Y)
                                goto CalcPath;
                        }
                    }
                }
                CurrentNodes = NewNodes.ToList();
                NewNodes = new HashSet<(int, int)>();
            }

            CalcPath:
            findPath2();
        }

        private void findPath2()
        {
            bool found = false;
            (int x, int y, int deltaX, int deltaY, int currentDistance, int i) =
                (Maze.Player.X, Maze.Player.Y, 0, 0, 0, 0);
            while (!found)
            {
                currentDistance = Distance[y, x];
                for (i = 0; i < Directions.Length; i++)
                {
                    (deltaX, deltaY) = Directions[i].GetMovementDeltas();
                    if (currentDistance > Distance[y + deltaY, x + deltaX] && Distance[y + deltaY, x + deltaX] != 0)
                    {
                        Path.AddElement(Directions[i]);
                        x += deltaX;
                        y += deltaY;
                        found = Maze.Finish.X == x && Maze.Finish.Y == y;
                        goto nextIt;
                    }
                }
                Path.RemoveLastElement();
                Distance[y, x] = -1;
                nextIt:
                continue;
            }
        }

        private void printDistanceBoard()
        {
            for (int y = 0; y < Maze.Height; y++)
            {
                for (int x = 0; x < Maze.Width; x++)
                {
                    Console.Write($"|{Distance[y, x]}|".PadLeft(1, '|').PadRight(1, '|'));
                }

                Console.Write("\n");
            }
        }
    }
}