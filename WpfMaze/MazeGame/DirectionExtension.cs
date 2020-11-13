namespace WpfMaze.Mazegame
{
    public static class DirectionExtension
    {
        public static (int x, int y) GetMovementDeltas(this Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return (-1, 0);
                case Direction.Down:
                    return (1, 0);
                case Direction.Left:
                    return (0, -1);
                case Direction.Right:
                    return (0, 1);
                default:
                    return (0, 0);
            }
        }
    }
}