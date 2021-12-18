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
            Point start = GetStartingPoint();
            Grid[start.X, start.Y].Origin = start; //set origin of start to itself
            Pending.Add(start);

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
                    Grid[p.X, p.Y].Visited = true;

                    //add neighbors to pending
                    foreach(Point n in GetNeighbors(p))
                    {
                        //check that it is valid
                        if (IsCellValidToAddToMaze(n) && !Pending.Contains(n))
                        {
                            //preset origin to come from this position
                            Grid[n.X, n.Y].Origin = p;

                            //add to pending
                            Pending.Add(n);
                        }
                    }

                }

                //update visited
                Grid[p.X, p.Y].Visited = true;

                // remove from pending
                Pending.RemoveAt(rndIndex);

                //shuffle pending
                Pending.Shuffle();
            }

        }

        private void InitializeGrid()
        {
            Grid = new CellStatus[(Template.Width ) / Resolution, (Template.Height ) / Resolution];

            //iterate over bitmap pixels by resolution steps
            for (int x = 0; x < Grid.GetLength(0); x ++)
            {
                for (int y = 0; y < Grid.GetLength(1); y++)
                {
                    //populate grid array for masked color
                    Color c = Template.GetPixel(x * Resolution, y * Resolution);
                    if (c.ToArgb() == Color.Black.ToArgb())
                    {
                        Grid[x, y] = new CellStatus() { Reserved = false, Visited = false };
                    }
                    else
                    {
                        Grid[x, y] = new CellStatus() { Reserved = true, Visited = false };
                    }
                }
            }

        }

        private bool IsCellValidToAddToMaze(Point p)
        {

            //can't have already been visited or be reserved
            if (Grid[p.X, p.Y].Visited == true) { return false; }
            if (Grid[p.X, p.Y].Reserved == true) { return false; }

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
                    if(Grid[x,y].Reserved == false) { return new Point(x, y); }
                }
            }

            return new Point(0, 0);

        }

        private void DrawBitmap()
        {

            //duplicate template
            Maze = new Bitmap(Template.Width,Template.Height);

            //get ready to draw on bitmap
            using (Graphics gr = Graphics.FromImage(Maze))
            {

                //iterate over grid
                for (int x = 0; x < Grid.GetLength(0); x++)
                {
                    for (int y = 0; y < Grid.GetLength(1); y++)
                    {

                        CellStatus c = Grid[x, y];

                        if (!c.Reserved)
                        {
                            using (Pen p = new Pen(Color.Black, 1))
                            {
                                gr.DrawLine(p, new Point(x * Resolution, y * Resolution), new Point(c.Origin.X * Resolution, c.Origin.Y * Resolution));
                            }
                             
                        }

                    }
                }
            }
        }

    }
}
