using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using WpfMaze.Mazegame;
using WpfMaze.MazeGame.Space;

namespace WpfMaze.MazeGame.Algorithms
{
    public class WavePropagation : AAlgorithm
    {
        private int[,] Distance { get; }
        private List<(int, int)> CurrentNodes { get; set; } // x,y
        private List<(int, int)> NewNodes { get; } // x,y

        private Stopwatch Stopwatch { get; }= new Stopwatch();

        public WavePropagation(MazeRewrite maze)
        {
            Maze = maze;
            Distance = new int[Maze.Height, Maze.Width];
            CurrentNodes = new List<(int, int)>();
            NewNodes = new List<(int, int)>();
        }

        public override void SolveMaze()
        {
            Stopwatch.Start();
            this.FloodFill();
        }

        private void FloodFill()
        {
            CurrentNodes.Add((Maze.Finish.X, Maze.Finish.Y));
            Distance[Maze.Finish.Y, Maze.Finish.X] = 1;
            bool found = false;
            while (!found)
            {
                foreach (var currentNode in CurrentNodes)
                {
                    if (IsInBounds(currentNode.Item1, currentNode.Item2 + 1) &&
                        !IsWall(DirectionDown, currentNode.Item1, currentNode.Item2) &&
                        Distance[currentNode.Item2+1, currentNode.Item1] == 0)
                    {
                        NewNodes.Add((currentNode.Item1, currentNode.Item2 + 1));
                        Distance[currentNode.Item2 + 1, currentNode.Item1] =
                            Distance[currentNode.Item2, currentNode.Item1] + 1;
                        if (currentNode.Item1 == Maze.Player.X && currentNode.Item2 + 1 == Maze.Player.Y)
                            found = true;
                    }

                    if (IsInBounds(currentNode.Item1, currentNode.Item2 - 1) &&
                        !IsWall(DirectionUp, currentNode.Item1, currentNode.Item2)&&
                        Distance[currentNode.Item2-1, currentNode.Item1] == 0)
                    {
                        NewNodes.Add((currentNode.Item1, currentNode.Item2 - 1));
                        Distance[currentNode.Item2 - 1, currentNode.Item1] =
                            Distance[currentNode.Item2, currentNode.Item1] + 1;
                        if (currentNode.Item1 == Maze.Player.X && currentNode.Item2 - 1 == Maze.Player.Y)
                            found = true;
                    }

                    if (IsInBounds(currentNode.Item1 - 1, currentNode.Item2) &&
                        !IsWall(DirectionLeft, currentNode.Item1, currentNode.Item2)&&
                        Distance[currentNode.Item2, currentNode.Item1-1] == 0)
                    {
                        NewNodes.Add((currentNode.Item1 - 1, currentNode.Item2));
                        Distance[currentNode.Item2, currentNode.Item1 - 1] =
                            Distance[currentNode.Item2, currentNode.Item1] + 1;
                        if (currentNode.Item1 - 1 == Maze.Player.X && currentNode.Item2 == Maze.Player.Y)
                            found = true;
                    }

                    if (IsInBounds(currentNode.Item1 + 1, currentNode.Item2) &&
                        !IsWall(DirectionRight, currentNode.Item1, currentNode.Item2)&&
                        Distance[currentNode.Item2, currentNode.Item1+1] == 0)
                    {
                        NewNodes.Add((currentNode.Item1 + 1, currentNode.Item2));
                        Distance[currentNode.Item2, currentNode.Item1 + 1] =
                            Distance[currentNode.Item2, currentNode.Item1] + 1;
                        if (currentNode.Item1 + 1 == Maze.Player.X && currentNode.Item2 == Maze.Player.Y)
                            found = true;
                    }
                }
                CurrentNodes = NewNodes.Distinct().ToList();
                NewNodes.Clear();
            } 
            findPath();
        }

        private void findPath()
        {
            bool found = false;
            int x, y;
            x = Maze.Player.X;
            y = Maze.Player.Y;
            int currentDistance;
            while (!found)
            {
                currentDistance = Distance[y, x];
                //todo inBound check
                if (currentDistance> Distance[y + 1, x] && Distance[y+1,x]!=0)
                {
                    Path.AddElement(DirectionDown);
                    y++;
                    found = Maze.Finish.X == x && Maze.Finish.Y == y;
                    continue;
                }

                if (currentDistance> Distance[y - 1, x] && Distance[y-1,x] != 0)
                {
                    Path.AddElement(DirectionUp);
                    y--;
                    found = Maze.Finish.X == x && Maze.Finish.Y == y;
                    continue; 
                }

                if (currentDistance> Distance[y, x + 1] && Distance[y,x+1] !=0)
                {
                    Path.AddElement(DirectionRight);
                    x++;
                    found = Maze.Finish.X == x && Maze.Finish.Y == y;
                    continue;
                }

                if (currentDistance>Distance[y, x - 1] && Distance[y,x-1] !=0)
                {
                    Path.AddElement(DirectionLeft);
                    x--;
                    found = Maze.Finish.X == x && Maze.Finish.Y == y;
                    continue;
                }

                Path.RemoveLastElement();
                Distance[y, x] = -1;
            }
            printPath();
        }

        private void printPath()
        {
          
            Stopwatch.Stop();
            Console.WriteLine($"{Stopwatch.ElapsedMilliseconds}ms {Path.Directions.Count} Elements (Wave)");
        }

        private void printDistanceBoard()
        {
            for (int y = 0; y < Maze.Height; y++)
            {
                for (int x = 0; x < Maze.Width; x++)
                {
                    Console.Write($"|{Distance[y, x]}|".PadLeft(1,'|').PadRight(1,'|'));
                }

                Console.Write("\n");
            }
        }
    }
}