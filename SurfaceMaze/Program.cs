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

            Bitmap bmp = new Bitmap(@"C:\Users\micha\OneDrive\Desktop\template2.png");
            //MazeMaker m = new MazeMaker(bmp, 20,1);
            //m.Build(new Point(42,42), true,new Point(15,15),5);
            MazeMaker m = new MazeMaker(bmp, 24, 1);
            m.Build(new Point(10, 17), false, new Point(10, 13), 5);
            m.Maze.Save(@"C:\Users\micha\OneDrive\Desktop\maze3.png",ImageFormat.Png);

        }
    }
}
