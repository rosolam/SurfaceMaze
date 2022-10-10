using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurfaceMaze
{
    public class CellStatus
    {
        public bool Reserved { get; set; }
        public bool Visited { get; set; }
        public Point Origin { get; set; }
        public int Distance { get; set; }
    }
}
