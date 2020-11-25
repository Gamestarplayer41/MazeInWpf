using WpfMaze.Mazegame;

namespace WpfMaze.MazeGame.Algorithms
{
    public interface IAlgorithm
    {
        public bool StopThread { get; set; }

        public void SolveMaze();

        public void InjectMaze(MazeRewrite maze);
    }
}