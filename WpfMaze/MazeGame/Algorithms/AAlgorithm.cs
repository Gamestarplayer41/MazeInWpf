using WpfMaze.Mazegame;

namespace WpfMaze.MazeGame.Algorithms
{
    public abstract class AAlgorithm
    {
        protected MazeRewrite Maze { get; set; }


        public bool StopThread { get; set; }


        protected Path Path { get; } = new Path();


        public abstract void SolveMaze();

        public virtual void InjectMaze(MazeRewrite maze)
        {
            Maze = maze;
        }
    }
}