using System.Diagnostics;

namespace WpfMaze.MazeGame.Space
{
    public class Point
    {
        public int X { get; set; }

        public int Y { get; set; }

        public static bool operator ==(Point a, Point b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Point a, Point b)
        {
            return  a.X != b.X && a.Y != b.Y;
        }
    }
}