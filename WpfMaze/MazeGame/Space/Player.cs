namespace WpfMaze.MazeGame.Space
{
    public class Player : Point
    {
        public readonly byte[] Color = {255, 0, 0};

        public Player(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}