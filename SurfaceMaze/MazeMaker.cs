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
        private bool GridIsInitialized = false;
        public int WallThickness { get; set; }
        public int MaxDistance { get; set; }


        public MazeMaker(Bitmap template, int resolution, int thickness)
        {
            Template = template;
            Resolution = resolution;
            WallThickness = thickness;
        }

        public Bitmap Build(bool buildRandom)
        {
            InitializeGrid();
            Point start = GetStartingPoint();
            return Build(start, buildRandom);
        }

        public Bitmap Build(Point start, bool buildRandom)
        {
            InitializeGrid();
            Generate(start, buildRandom);
            DrawBitmap();
            return Maze;
        }

        public Bitmap Build(Point start, bool buildRandom, Point end, int distanceThreshold)
        {
           do
            {
                GridIsInitialized = false;
                MaxDistance = 0;
                InitializeGrid();
                Generate(start, buildRandom);
                Console.WriteLine("Max Distance: {0}, End Distance: {1}",MaxDistance, Grid[end.X, end.Y].Distance);
            } while (Grid[end.X, end.Y].Distance < MaxDistance - distanceThreshold);
            
            DrawBitmap();
            return Maze;
        }

        private void Generate(Point start, bool buildRandom)
        {

            Grid[start.X, start.Y].Origin = start; //set origin of start to itself
            Pending.Add(start);

            //while pending count > 0
            //Random rnd = new Random();
            while (Pending.Count > 0)
            {

                //get random pending
                //int rndIndex = rnd.Next(0, Pending.Count - 1);
                //Point p = Pending[rndIndex];
                int lastIndex = Pending.Count - 1;
                Point p = Pending[lastIndex];

                //is cell a valid cell to add to the maze?
                if (IsCellValidToAddToMaze(p))
                {

                    //add to maze
                    Grid[p.X, p.Y].Visited = true;

                    // get neighbors
                    List<Point> neighbors = GetNeighbors(p);
                    neighbors.Shuffle();

                    //add neighbors to pending
                    foreach (Point n in neighbors)
                    {
                        //check that it is valid
                        if (IsCellValidToAddToMaze(n) && !Pending.Contains(n))
                        {
                            //preset origin to come from this position
                            Grid[n.X, n.Y].Origin = p;
                            Grid[n.X, n.Y].Distance = Grid[p.X, p.Y].Distance + 1;
                            MaxDistance = Math.Max(MaxDistance, Grid[n.X, n.Y].Distance);

                            //add to pending
                            Pending.Add(n);
                        }
                    }

                }

                //update visited
                Grid[p.X, p.Y].Visited = true;

                // remove from pending
                Pending.RemoveAt(lastIndex);

                //shuffle pending
                if (buildRandom)
                {
                    Pending.Shuffle();
                }
            }

        }

        private void InitializeGrid()
        {

            if (GridIsInitialized) { return; }

            Grid = new CellStatus[(Template.Width ) / Resolution, (Template.Height ) / Resolution];

            // grid debug to pic
            //Bitmap gridPic = new Bitmap(Template.Width, Template.Height);

            //iterate over bitmap pixels by resolution steps
            for (int x = 0; x < Grid.GetLength(0); x ++)
            {
                for (int y = 0; y < Grid.GetLength(1); y++)
                {
                    //populate grid array for masked color
                    //Color c = Template.GetPixel(x * Resolution, y * Resolution);
                    //if (c.ToArgb() != Color.Black.ToArgb())
                    if(CheckColorAround(Color.Black,Template,x*Resolution,y*Resolution,Resolution-1))
                    {
                        Grid[x, y] = new CellStatus() { Reserved = false, Visited = false };
                        //gridPic.SetPixel(x * Resolution, y * Resolution, Color.Black);
                    }
                    else
                    {
                        Grid[x, y] = new CellStatus() { Reserved = true, Visited = false };
                        //gridPic.SetPixel(x * Resolution, y * Resolution, Color.Red);
                    }
                }
            }

            GridIsInitialized = true;

            //gridPic.Save(@"C:\Users\micha\OneDrive\Desktop\gridpic.bmp");

        }

        private bool CheckColorAround(Color mask, Bitmap b, int x, int y, int distance)
        {

            // this function assumes that distance will not be beyond height and width of template, which it needs to be (meaning resolution must divide into height & width)

            //top left
            if (b.GetPixel(x, y).ToArgb() == mask.ToArgb())
            {
                return false;
            }

            //top right
            if (b.GetPixel(x+distance, y).ToArgb() == mask.ToArgb())
            {
                return false;
            }

            //bottom left
            if (b.GetPixel(x, y+ distance).ToArgb() == mask.ToArgb())
            {
                return false;
            }

            //bottom right
            if (b.GetPixel(x+ distance, y + distance).ToArgb() == mask.ToArgb())
            {
                return false;
            }

            return true;

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
                    if(Grid[x,y].Reserved == false) { 
                        return new Point(x, y); 
                    }
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

                //set black background
                gr.Clear(Color.Black);
                //using (Brush b = new SolidBrush(Color.Black))
                //{
                //    gr.FillRectangle(b, 0, 0, Maze.Width, Maze.Height);
                //}

                //iterate over grid
                for (int x = 0; x < Grid.GetLength(0); x++)
                {
                    for (int y = 0; y < Grid.GetLength(1); y++)
                    {

                        CellStatus c = Grid[x, y];

                        if (!c.Reserved)
                        {
                            //if(c.Distance >= MaxDistance - 5)
                            //{

                            //    using (Brush b = new SolidBrush(Color.Blue))
                            //    {
                            //        Rectangle rect = new Rectangle(
                            //                Math.Min(x, c.Origin.X) * Resolution + WallThickness,
                            //                Math.Min(y, c.Origin.Y) * Resolution + WallThickness,
                            //                Math.Abs(x - c.Origin.X) * Resolution + Resolution - 1 - WallThickness,
                            //                Math.Abs(y - c.Origin.Y) * Resolution + Resolution - 1 - WallThickness);
                            //        gr.FillRectangle(b, rect);
                            //    }

                            //} else
                            //{

                                using (Brush b = new SolidBrush(Color.White))
                                {
                                    Rectangle rect = new Rectangle(
                                            Math.Min(x, c.Origin.X) * Resolution + WallThickness,
                                            Math.Min(y, c.Origin.Y) * Resolution + WallThickness,
                                            Math.Abs(x - c.Origin.X) * Resolution + Resolution - 1 - WallThickness,
                                            Math.Abs(y - c.Origin.Y) * Resolution + Resolution - 1 - WallThickness);
                                    gr.FillRectangle(b, rect);
                                }

                            //}

                        } else
                        {

                            using (Brush b = new SolidBrush(Color.Red))
                            {
                                Rectangle rect = new Rectangle(
                                        x * Resolution,
                                        y * Resolution,
                                        Resolution,
                                        Resolution);
                                gr.FillRectangle(b, rect);
                            }
                        }

                    }
                }

            }

            Maze.MakeTransparent(Color.Red);
        }

    }
}
