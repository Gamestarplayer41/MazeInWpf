using System;
using System.Collections.Generic;
using System.Text;
using WpfMaze.Mazegame;

namespace WpfMaze.MazeGame.Algorithms
{
    abstract class AAlgorithm<T>
    {
        public Maze Maze;

        public AAlgorithm(T maze)
        {
        }
    }
}
