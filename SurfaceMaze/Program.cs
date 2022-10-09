using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurfaceMaze
{
    class Program
    {
        static void Main(string[] args)
        {

            //Black = MASK, White = MAZE

            Bitmap bmp = new Bitmap(@"C:\Users\micha\OneDrive\Desktop\template.png");
            MazeMaker m = new MazeMaker(bmp, 20,1);
            m.Build(new Point(0,59),false);
            m.Maze.Save(@"C:\Users\micha\OneDrive\Desktop\maze2.png",ImageFormat.Png);

        }
    }
}
