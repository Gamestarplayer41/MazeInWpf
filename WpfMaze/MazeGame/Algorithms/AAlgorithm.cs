using WpfMaze.Mazegame;
using WpfMaze.MazeGame.Space;

namespace WpfMaze.MazeGame.Algorithms
{
    public abstract class AAlgorithm
    {
        protected Direction DirectionDown = Direction.Down;
        protected Direction DirectionLeft = Direction.Left;
        protected Direction DirectionRight = Direction.Right;

        protected Direction DirectionUp = Direction.Up;
        protected MazeRewrite Maze { get; set; }


        public bool StopThread { get; set; }


        protected Path Path { get; } = new Path();


        public abstract void SolveMaze();

        public virtual void InjectMaze(MazeRewrite maze)
        {
            Maze = maze;
        }

        protected bool IsWall(Direction direction, int x, int y)
        {
            var (deltaX, deltaY) = direction.GetMovementDeltas();
            return Maze.Board[y + deltaY, x + deltaX] == 1;
        }

        protected bool IsInBounds(int x, int y)
        {
            if (x < 0)
                return false;
            if (y < 0)
                return false;
            if (y > Maze.Height - 1)
                return false;
            if (x > Maze.Width - 1)
                return false;
            return true;
        }
    }
}