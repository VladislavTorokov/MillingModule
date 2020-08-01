using System.Collections.Generic;
using System;

namespace MillingModule
{
    public class Scaling
    {
        Matrix basicMatrix;
        float scalingFactor;

        public Scaling(float xLength, float yLength, float widthPBox, float heightPBox)
        {
            scalingFactor = GetScalingFactor(xLength, yLength, widthPBox, heightPBox);
            basicMatrix = new Matrix();
            basicMatrix.elements[0, 0] = scalingFactor;
            basicMatrix.elements[1, 1] = scalingFactor;
            basicMatrix.elements[2, 2] = scalingFactor;
        }

        public void ScalingArc(List<Primitives.Arc> arcs)
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

        public void ScalingCircles(List<Primitives.Circle> circles)
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

        public void ScalingPoints(List<Primitives.Point> points)
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

        private float GetScalingFactor(float xLength, float yLength, float widthPBox, float heightPBox)
        {
            if ((widthPBox / xLength) > (heightPBox / yLength))
                return heightPBox / yLength * 0.8f;
            else
                return widthPBox / xLength * 0.8f;
        }
    }
}
