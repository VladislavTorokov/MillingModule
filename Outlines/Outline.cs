using System;
using System.Collections.Generic;
using System.Linq;


namespace MillingModule
{
    public class Outline:ICloneable
    {
        public List<Primitives.Point> Points { get; set; }
        public List<Primitives.Arc> Arcs { get; set; }
        public List<Primitives.Line> Lines { get; set; }
        public List<Primitives.Circle> Circles { get; set; }
        public List<Primitives.Rectangle> Rectangles { get; set; }

        public float Xmax { get; set; }
        public float Xmin { get; set; }
        public float Ymax { get; set; }
        public float Ymin { get; set; }

        public Outline()
        {
            Points = new List<Primitives.Point>();
            Arcs = new List<Primitives.Arc>();
            Lines = new List<Primitives.Line>();
            Circles = new List<Primitives.Circle>();
            Rectangles = new List<Primitives.Rectangle>();
        }

        public void GetMinMax()
        {
            if (Points.Count!=0)
            {
                Xmax = Points.Max(point => point.X);
                Xmin = Points.Min(point => point.X);
                Ymax = Points.Max(point => point.Y);
                Ymin = Points.Min(point => point.Y);
            }
        }

        public object Clone()
        {
            return new Outline {Points = this.Points };
        }
    }
}
