using System;
using System.Collections.Generic;
using System.Text;
using WpfMaze.Mazegame;

namespace WpfMaze.MazeGame.Algorithms
{
    abstract class AAlgorithm
    {
        public MazeRewrite Maze;

        public bool stopThread = false;
    }
}
