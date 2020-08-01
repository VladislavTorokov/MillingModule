using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MillingModule.Primitives
{
    public class Point
    {
        public float X { get; set; }
        public float Y { get; set; }
        public bool isArc { get; set; }
        public bool isLeft { get; set; }
        public float RadiusForArc { get; set; }
        public Point()
        {
            isArc = false;
            isLeft = false;
        }
    }
}
