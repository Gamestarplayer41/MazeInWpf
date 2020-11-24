using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Navigation;
using WpfMaze.Mazegame;

namespace WpfMaze.MazeGame.Algorithms
{
    public interface IAlgorithm
    {
        public void SolveMaze();

        public void injectMaze(MazeRewrite maze);

        public bool stopThread
        {
            get;
            set;
        }
    }
}
