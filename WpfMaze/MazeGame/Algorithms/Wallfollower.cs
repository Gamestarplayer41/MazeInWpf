using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using WpfMaze.Mazegame;

namespace WpfMaze.MazeGame.Algorithms
{
    class Wallfollower : AAlgorithm, IAlgorithm
    {
        public bool stopThread { get; set; } = false;
        private int dir = 1;
        private Direction Heading = Direction.Down;

        private int X, Y;

        private Path Path = new Path();


        public void SolveMaze()
        {
            this.followWallNew();
        }

        public void injectMaze(MazeRewrite maze)
        {
            this.Maze = maze;
            X = maze.Player.X;
            Y = maze.Player.Y;
        }

        private void followWallNew()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            bool found = false;
            while (!found)
            {
                Direction direction;
                for (int i = -1; i < 3; i++)
                {
                    direction = DirectionResolver(IntDirCalc(dir, i));
                    if (isWall(direction))
                        continue;
                    dir = IntDirCalc(dir, i);
                    Heading = direction;
                    Path.addElement(direction);
                    (int deltaX, int deltaY) = direction.GetMovementDeltas();
                    X += deltaX;
                    Y += deltaY;
                    if (X == Maze.Finish.X && Y == Maze.Finish.Y)
                        found = true;
                    break;
                }
            }
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds + "ms " + Path.Directions.Count + " elements");
        }

        private bool isWall(Direction direction)
        {
            (int deltaX, int deltaY) = direction.GetMovementDeltas();
            return Maze.Board[Y + deltaY, X + deltaX] == 1;
        }

        private void FollowWall()
        {
            while (!this.Maze.isSolved && !stopThread)
            {
                if (this.Maze.PlayerCanMove(DirectionResolver(IntDirCalc(dir, -1))))
                {
                    dir = IntDirCalc(dir, -1);
                    this.Maze.MovePlayer(DirectionResolver(dir));
                    this.Heading = DirectionResolver(dir);
                }
                else if (this.Maze.PlayerCanMove(DirectionResolver(dir)))
                {
                    this.Maze.MovePlayer(DirectionResolver(dir));
                }
                else if (this.Maze.PlayerCanMove(DirectionResolver(IntDirCalc(dir, 1))))
                {
                    dir = IntDirCalc(dir, 1);
                    this.Maze.MovePlayer(DirectionResolver(dir));
                    this.Heading = DirectionResolver(dir);
                }
                else if (this.Maze.PlayerCanMove(DirectionResolver(IntDirCalc(dir, 2))))
                {
                    dir = IntDirCalc(dir, 2);
                    this.Maze.MovePlayer(DirectionResolver(dir));
                    this.Heading = DirectionResolver(dir);
                }
            }
        }

        private int IntDirCalc(int dir, int add)
        {
            dir = dir + add;
            if (dir > 4)
                return dir - 4;
            if (dir == 0)
                return 4;
            return dir;
        }

        private Direction DirectionResolver(int dir)
        {
            switch (dir)
            {
                case 1:
                    return Direction.Down;
                case 2:
                    return Direction.Left;
                case 3:
                    return Direction.Up;
                case 4:
                    return Direction.Right;
                default:
                    return Direction.Down;
            }
        }
    }
}