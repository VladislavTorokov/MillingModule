using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MillingModule.Primitives
{
    public class Circle:Obstruction
    {
        public Circle(Point point)
        {
            Xc = point.X;
            Yc = point.Y;
            R = point.RadiusForArc;
        }
    }
}
