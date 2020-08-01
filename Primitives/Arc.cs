using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MillingModule.Primitives
{
    public class Arc
    {
        public float Xc { get; set; }
        public float Yc { get; set; }
        public float R { get; set; }
        public float Angle_From { get; }
        public float Angle_To { get; }

        public Arc(Point pointCentre, float angleF, float angleT)
        {
            Xc = pointCentre.X;
            Yc = pointCentre.Y;
            R = pointCentre.RadiusForArc;
            Angle_From = angleF;
            Angle_To = angleT;
        }

        private float GetCoord(float angle)
        {
            return (float)Math.Round(R * Math.Cos(angle * Math.PI / 180));
        }
    }
}
