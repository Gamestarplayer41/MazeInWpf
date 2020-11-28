using System.Threading;
using WpfMaze.Mazegame;

namespace WpfMaze.MazeGame.Algorithms
{
    public class AlgorithmThread
    {
        private readonly AAlgorithm Algorithm;
        private readonly Thread Thread;

        public AlgorithmThread(AAlgorithm algorithm)
        {
            Algorithm = algorithm;
            Thread = new Thread(Algorithm.SolveMaze);
        }

        public void StartThread()
        {
            Thread.Start();
        }

        public void StopThread()
        {
            Algorithm.StopThread = true;
        }

        public void InjectMaze(MazeRewrite maze)
        {
            Algorithm.InjectMaze(maze);
        }
    }
}