using System;
using System.Collections.Generic;
using System.Text;
using WpfMaze.Mazegame;

namespace WpfMaze.MazeGame.Algorithms
{
    abstract class AAlgorithm
    {
        public Maze Maze;

        public bool stopThread = false;
    }
}
