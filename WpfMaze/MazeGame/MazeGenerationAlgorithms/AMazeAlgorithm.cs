using System.Dynamic;
using System.Resources.Extensions;
using WpfMaze.MazeGame.Space;

namespace WpfMaze.Mazegame.MazeGenerationAlgorithms
{
    public abstract class AMazeAlgorithm
    {
        protected int Height => Board.GetLength(0);
        protected int Width => Board.GetLength(1);
        protected byte[,] Board { get; set; }

        protected Player Player { get; set; }
        protected Finish Finish { get; set; }

        public abstract void GenerateMaze();
    }
}