using System.Threading;
using WpfMaze.Mazegame;

namespace WpfMaze.MazeGame.Algorithms
{
    public class AlgorithmThread
    {
        private Thread Thread;
        private IAlgorithm Algorithm;

        public AlgorithmThread(IAlgorithm algorithm)
        {
            Algorithm = algorithm;
            Thread = new Thread(Algorithm.SolveMaze);
        }

        public void startThread()
        {
            Thread.Start();
        }

        public void stopThread()
        {
            Algorithm.stopThread = true;
        }

        public void injectMaze(MazeRewrite maze)
        {
            Algorithm.injectMaze(maze);
        }
    }
}