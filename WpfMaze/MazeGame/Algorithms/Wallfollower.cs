using System;
using System.Diagnostics;
using System.Windows.Automation;
using WpfMaze.Mazegame;
using WpfMaze.MazeGame.Space;

namespace WpfMaze.MazeGame.Algorithms
{
    internal class Wallfollower : AAlgorithm
    {
        private int Dir { get; set; } = 1;
        private int X, Y;

        public Wallfollower(MazeRewrite maze)
        {
            base.InjectMaze(maze);
            X = maze.Player.X;
            Y = maze.Player.Y;
        }

        public override void SolveMaze()
        {
            FollowWallNew();
        }

        private void FollowWallNew()
        {
            var watch = new Stopwatch();
            watch.Start();
            var found = false;
            int steps = 0;
            int maximumSteps = (int) Math.Pow(Maze.Height * Maze.Width, 2);
            while (!found)
            {
                if(steps == maximumSteps)
                    return;
                Direction direction;
                for (var i = -1; i < 3; i++)
                {
                    direction = DirectionResolver(IntDirCalc(Dir, i));
                    if (IsWall(direction))
                        continue;
                    Dir = IntDirCalc(Dir, i);
                    Path.AddElement(direction);
                    var (deltaX, deltaY) = direction.GetMovementDeltas();
                    X += deltaX;
                    Y += deltaY;
                    if (X == Maze.Finish.X && Y == Maze.Finish.Y)
                        found = true;
                    break;
                }

                steps++;
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