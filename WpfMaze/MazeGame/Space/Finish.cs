using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfMaze.Mazegame
{
    public class Finish : Point
    {

        public int[] Color = {245, 66,120};
        public Finish(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }
}
