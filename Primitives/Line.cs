using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MillingModule.Primitives
{
    public class Line
    {
        public Point From { get; set; }
        public Point To { get; set; }

        public Line(Point from, Point to)
        {
            From = from;
            To = to;
        }
    }
}
