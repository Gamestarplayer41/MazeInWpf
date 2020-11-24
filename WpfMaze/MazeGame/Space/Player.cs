using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfMaze.Mazegame
{
    public class Player : Point
    {
        public int[] Color = {11, 14, 56};
        public Player(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }
}
