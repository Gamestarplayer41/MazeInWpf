namespace WpfMaze.MazeGame.Space
{
    public class Player : Point
    {
        public readonly int[] Color = {11, 14, 56};

        public Player(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}