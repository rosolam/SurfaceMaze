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

            Bitmap bmp = new Bitmap(@"C:\Users\micha\OneDrive\Desktop\template.png");
            MazeMaker m = new MazeMaker(bmp, 10);
            m.Build();
            m.Maze.Save(@"C:\Users\micha\OneDrive\Desktop\maze.png",ImageFormat.Png);

        }
    }
}
