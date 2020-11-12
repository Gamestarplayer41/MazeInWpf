using System;
using System.Collections.Generic;
using System.Text;
using WpfMaze.Mazegame;

namespace WpfMaze.MazeGame.Algorithms
{
     class Wallfollower : AAlgorithm<Maze>, IAlgorithm
    {
        public Maze Maze;
        private int dir = 1;
        private Direction Heading = Direction.Down;

        public Wallfollower(Maze maze) : base(maze)
        {
            this.Maze = maze;
        }

        public void SolveMaze()
        {
        }

        private void FollowWall()
        {
            Console.WriteLine("asd");
            while (!this.Maze.IsSolved)
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
