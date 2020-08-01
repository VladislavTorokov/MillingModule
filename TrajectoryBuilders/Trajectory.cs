using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MillingModule
{
    public class Trajectory : ICloneable
    {
        public List<Primitives.Point> referencePoints = new List<Primitives.Point>();
        public List<Primitives.Line> lines = new List<Primitives.Line>();
        public List<Primitives.Arc> arcs = new List<Primitives.Arc>();

        public void SetPointsForLines()
        {
            if (referencePoints.Count > 1)
            {
                lines.Add(new Primitives.Line(
                    referencePoints[referencePoints.Count - 2],
                    referencePoints[referencePoints.Count - 1]));
            }
        }
        public void SetPointsForArc(float x, float y, float r, float triangleLeg, bool isLeft)
        {
            float angleBegin = 0;
            float angleEnd = 0;
            if (isLeft)
            {
                angleBegin = (float)(Math.Acos(triangleLeg / r) * 180 / Math.PI);
                angleEnd = -(float)(Math.Acos(triangleLeg / r) * 180 / Math.PI);
            }
            else
            {
                angleBegin = -(float)(Math.Acos(triangleLeg / r) * 180 / Math.PI);
                angleEnd = (float)(Math.Acos(triangleLeg / r) * 180 / Math.PI);
            }
            arcs.Add(new Primitives.Arc(new Primitives.Point() { X = x, Y = y, RadiusForArc = r }, angleBegin, angleEnd));
        }

        public void Clear()
        {
            referencePoints.Clear();
            lines.Clear();
            arcs.Clear();
        }
        public object Clone()
        {
            return (Trajectory)this.MemberwiseClone();
        }
    }
}
