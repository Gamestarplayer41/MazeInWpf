using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;


namespace WpfMaze.Mazegame
{
    public class Point
    {

        public int[] Color;
        public int X
        {
            get; set;
        }

        public int Y
        {
            get; set;
        }

        public static bool operator ==(Point a, Point b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Point a, Point b)
        {
            return a.X != b.X && a.Y != b.Y;
        }
    }
}
