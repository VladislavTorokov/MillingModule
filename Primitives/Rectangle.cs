using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MillingModule.Primitives
{
    public class Rectangle:Obstruction
    {
        List<Line> lines { get; set; }
        public float Xmax { get; set; }
        public float Xmin { get; set; }
        public float Ymax { get; set; }
        public float Ymin { get; set; }

        public Rectangle()
        {
            lines = new List<Line>(4);
        }

        public void SetLine(Line line)
        {
            lines.Add(line);
            if (lines.Count == 4)
            {
                Xmax = lines[lines.Count - 3].To.X;
                Xmin = lines[lines.Count - 4].From.X;
                Ymax = lines[lines.Count - 3].To.Y;
                Ymin = lines[lines.Count - 4].From.Y;
                Xc = (Xmax - Xmin) / 2 + Xmin;
                Yc = (Ymax - Ymin) / 2 + Ymin;
                R = Math.Abs(Xc - Xmin);
            }

        }

        public bool isEmpty()
        {
            if (lines.Count > 0)
            {
                if (lines[0] == null)
                    return true;
                else
                {
                    if ((lines.Last().From.X == 0) && (lines.Last().From.Y == 0) && (lines.Last().To.X == 0) && (lines.Last().To.Y == 0))
                        return true;
                    else
                        return false;
                }
            }
            else
                return true;
        }
    }
}
