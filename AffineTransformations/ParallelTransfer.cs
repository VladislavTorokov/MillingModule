using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MillingModule
{
    public class ParallelTransfer
    {
        Matrix basicMatrix;
        float X, Y;

        public ParallelTransfer(float x, float y)
        {
            X = x;
            Y = y;
            basicMatrix = new Matrix();
            basicMatrix.elements[3, 0] = X*0.5f;
            basicMatrix.elements[3, 1] = Y*0.5f;
            basicMatrix.elements[3, 2] = 0;
        }

        public void TransferArc(List<Primitives.Arc> arcs)
        {
            float[] coordinates = new float[4] { 0, 0, 0, 1 };


            if (arcs != null)
            {
                for (int i = 0; i < arcs.Count; i++)
                {
                    coordinates[0] = arcs[i].Xc;
                    coordinates[1] = arcs[i].Yc;
                    coordinates[2] = arcs[i].R;
                    coordinates = basicMatrix.Multiplication(coordinates);
                    arcs[i].Xc = coordinates[0];
                    arcs[i].Yc = coordinates[1];
                    arcs[i].R = coordinates[2];
                }
            }
        }

        public void TransferCircles(List<Primitives.Circle> circles)
        {
            float[] coordinates = new float[4] { 0, 0, 0, 1 };


            if (circles != null)
            {
                for (int i = 0; i < circles.Count; i++)
                {
                    coordinates[0] = circles[i].Xc;
                    coordinates[1] = circles[i].Yc;
                    coordinates[2] = circles[i].R;
                    coordinates = basicMatrix.Multiplication(coordinates);
                    circles[i].Xc = coordinates[0];
                    circles[i].Yc = coordinates[1];
                    circles[i].R = coordinates[2];
                }
            }
        }

        public void TransferPoints(List<Primitives.Point> points)
        {
            float[] coordinates = new float[4] { 0, 0, 0, 1 };

            if (points != null)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    coordinates[0] = points[i].X;
                    coordinates[1] = points[i].Y;
                    coordinates[2] = points[i].RadiusForArc;
                    coordinates = basicMatrix.Multiplication(coordinates);
                    points[i].X = coordinates[0];
                    points[i].Y = coordinates[1];
                    points[i].RadiusForArc = coordinates[2];
                }
            }
        }
    }
}