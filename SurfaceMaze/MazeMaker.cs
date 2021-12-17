using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SurfaceMaze
{
    public class MazeMaker
    {

        public enum CellStatus
        {
            Maze,
            Wall,
            Reserved
        }

        public Bitmap Template { get; set; }
        public Bitmap Maze { get; set; }
        public int Resolution { get; set; }
        private CellStatus[,] Grid { get; set; }
        private List<Point> Pending = new List<Point>();
       

        public MazeMaker(Bitmap template, int resolution)
        {
            Template = template;
            Resolution = resolution;
        }

        public Bitmap Build()
        {
            Generate();
            DrawBitmap();
            return Maze;
        }

        private void Generate()
        {

            //intialize grid
            InitializeGrid();

            //pick a starting point and add to pending
            Pending.Add(GetStartingPoint());

            //while pending count > 0
            Random rnd = new Random();
            while (Pending.Count > 0)
            {

                //get random pending
                int rndIndex = rnd.Next(0, Pending.Count - 1);
                Point p = Pending[rndIndex];

                //is cell a valid cell to add to the maze?
                if (IsCellValidToAddToMaze(p))
                {

                    //add to maze
                    Grid[p.X, p.Y] = CellStatus.Maze;

                    //add neighbors to pending
                    foreach(Point n in GetNeighbors(p))
                    {
                        //check that it is valid
                        if (IsCellValidToAddToMaze(n))
                        {
                            Pending.Add(n);
                        }
                    }

                }

                // remove from pending
                Pending.RemoveAt(rndIndex);
            }

        }

        private void InitializeGrid()
        {
            Grid = new CellStatus[(Template.Width - Resolution) / Resolution, (Template.Height - Resolution) / Resolution];

            //iterate over bitmap pixels by resolution steps
            for (int x = 0; x < Template.Width - Resolution; x += Resolution)
            {
                for (int y = 0; y < Template.Height - Resolution; y += Resolution)
                {
                    //populate grid array for masked color
                    Color c = Template.GetPixel(x, y);
                    if (c.ToArgb() == Color.Black.ToArgb())
                    {
                        Grid[x / Resolution, y / Resolution] = CellStatus.Wall;
                    }
                    else
                    {
                        Grid[x / Resolution, y / Resolution] = CellStatus.Reserved;
                    }
                }
            }

        }

        private bool IsCellValidToAddToMaze(Point p)
        {

            //must be a wall
            if (Grid[p.X, p.Y] != CellStatus.Wall) { return false; }

            //neighbor count needs to be 1 (or 0 if it is the very first starting cell)
            int mazeCount = 0;
            foreach(Point n in GetNeighbors(p))
            {
                if(Grid[n.X,n.Y] == CellStatus.Maze) { mazeCount += 1; }
            }
            if(mazeCount > 1) { return false;}

            //valid!
            return true;

        }

        private List<Point> GetNeighbors(Point p)
        {

            List<Point> neighbors = new List<Point>();

            if(p.X > 0)
            {
                neighbors.Add(new Point(p.X - 1, p.Y));
            }
            if (p.X < Grid.GetLength(0)-1)
            {
                neighbors.Add(new Point(p.X + 1, p.Y));
            }
            if (p.Y > 0)
            {
                neighbors.Add(new Point(p.X, p.Y-1));
            }
            if (p.Y < Grid.GetLength(1) - 1)
            {
                neighbors.Add(new Point(p.X, p.Y + 1));
            }

            return neighbors;

        }

        private Point GetStartingPoint()
        {

            //grab first wall point encountered
            for (int x = 0; x < Grid.GetLength(0); x++)
            {
                for (int y= 0; y < Grid.GetLength(1); y++)
                {
                    if(Grid[x,y] == CellStatus.Wall) { return new Point(x, y); }
                }
            }

            return new Point(0, 0);

        }

        private void DrawBitmap()
        {

            //duplicate template
            Maze = new Bitmap(Template);

            //iterate over grid
            for (int x = 0; x < Grid.GetLength(0); x ++)
            {
                for (int y = 0; y < Grid.GetLength(1); y ++)
                {

                    //iterate block of pixels in bitmap from origin determined from grid position
                    for (int bx = x * Resolution; bx < (x + 1) * Resolution; bx++)
                    {
                        for (int by = y * Resolution; by < (y + 1) * Resolution; by++)
                        {



                            //draw to bitmap
                            Color c = Template.GetPixel(x, y);
                            if (Grid[x, y] == CellStatus.Maze)
                            {
                                Maze.SetPixel(bx, by, Color.Black);
                            }
                            else if (Grid[x, y] == CellStatus.Wall)
                            {
                                Maze.SetPixel(bx, by, Color.Transparent);
                            }
                            else if (Grid[x, y] == CellStatus.Reserved)
                            {
                                Maze.SetPixel(bx, by, Color.Red);
                            }
                        }
                    }
                }
            }
        }

    }
}
