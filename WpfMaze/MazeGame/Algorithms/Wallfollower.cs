using System;
using System.Diagnostics;
using WpfMaze.Mazegame;
using WpfMaze.MazeGame.Space;

namespace WpfMaze.MazeGame.Algorithms
{
    internal class Wallfollower : AAlgorithm, IAlgorithm
    {
        private readonly Path Path = new Path();
        private int Dir = 1;

        private int X, Y;
        public new bool stopThread { get; set; } = false;


        public void SolveMaze()
        {
            FollowWallNew();
        }

        public void InjectMaze(MazeRewrite maze)
        {
            Maze = maze;
            X = maze.Player.X;
            Y = maze.Player.Y;
        }

        private void FollowWallNew()
        {
            var watch = new Stopwatch();
            watch.Start();
            var found = false;
            while (!found)
            {
                Direction direction;
                for (var i = -1; i < 3; i++)
                {
                    direction = DirectionResolver(IntDirCalc(Dir, i));
                    if (IsWall(direction))
                        continue;
                    Dir = IntDirCalc(Dir, i);
                    Path.addElement(direction);
                    var (deltaX, deltaY) = direction.GetMovementDeltas();
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

        private bool IsWall(Direction direction)
        {
            var (deltaX, deltaY) = direction.GetMovementDeltas();
            return Maze.Board[Y + deltaY, X + deltaX] == 1;
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