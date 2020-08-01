using System;
using System.Collections.Generic;
using System.Linq;
using MillingModule.Primitives;

namespace MillingModule
{
    class TrajectoryManager
    {
        ZigZagBuilder zigzagBuilder;
        SpiralBuilder spiralBuilder;
        List<Obstruction> obstructions;

        public TrajectoryManager(List<Rectangle> _rectangles, List<Circle> _circles, float cutterDiameter)
        {
            if (!SetObstructions(_rectangles, _circles, cutterDiameter))
                obstructions=null;
        }

        public Trajectory GetZigZagTrajectory(Outline outline, float cutterDiameter, float cuttingWidth)
        {
            zigzagBuilder = new ZigZagBuilder();
            if (obstructions==null)
                return null;
            zigzagBuilder.GetZigZag(outline, cutterDiameter, cuttingWidth,obstructions);
            return zigzagBuilder.GetTrajectory();
        }

        public Trajectory GetSpiralTrajectory(Outline outline, float cutterDiameter, float cuttingWidth)
        {
            spiralBuilder = new SpiralBuilder();
            if (obstructions == null)
                return null;
            spiralBuilder.GetSpiral(outline, cutterDiameter, cuttingWidth);
            return spiralBuilder.GetTrajectory();
        }

        private bool SetObstructions(List<Rectangle> _rectangles, List<Circle> _circles, float cutterDiameter)
        {
            obstructions = new List<Obstruction>();
            if ((_rectangles != null) && CheckDistanceBetweenRectangles(_rectangles, cutterDiameter))
                foreach (Rectangle rectangle in _rectangles)
                    obstructions.Add(rectangle);
            else
                return false;
            if ((_circles != null) && CheckDistanceBetweenCircles(_circles, cutterDiameter))
                foreach (Circle circle in _circles)
                    obstructions.Add(circle);
            else
                return false;
            if (CheckDistanceBetweenCirclesAndRectangles(_circles, _rectangles, cutterDiameter))
                return true;
            else
                return false;
        }

        private bool CheckDistanceBetweenCircles(List<Circle> _circles, float cutterDiameter)
        {
            double length;
            //for (int i = 1; i < _circles.Count; i++)
            //{
            //    length = Math.Sqrt((_circles[i].Xc - _circles[i - 1].Xc) * (_circles[i].Xc - _circles[i - 1].Xc) +
            //                       (_circles[i].Yc - _circles[i - 1].Yc) * (_circles[i].Yc - _circles[i - 1].Yc));
            //    if (length - _circles[i].R - _circles[i - 1].R - cutterDiameter < 0)
            //        return false;
            //}
            return true;
        }

        private bool CheckDistanceBetweenRectangles(List<Rectangle> _rectangles, float cutterDiameter)
        {
            for (int i = 1; i < _rectangles.Count; i++)
            {
                if (_rectangles[i - 1].Ymax + cutterDiameter < _rectangles[i].Ymin ||
                    _rectangles[i - 1].Ymin + cutterDiameter > _rectangles[i].Ymax ||
                    _rectangles[i - 1].Xmax + cutterDiameter < _rectangles[i].Xmin ||
                    _rectangles[i - 1].Xmin + cutterDiameter > _rectangles[i].Xmax)
                { }
                else
                    return false;
            }
            return true;
        }

        private bool CheckDistanceBetweenCirclesAndRectangles(List<Circle> _circles, List<Rectangle> _rectangles, float cutterDiameter)
        {
            double Xmin;
            double Xmax;
            double Ymin;
            double Ymax;

            for (int i = 0; i < _circles.Count; i++)
                for (int j = 0; j < _rectangles.Count; j++)
                {
                    Xmin = _rectangles[j].Xmin - cutterDiameter;
                    Xmax = _rectangles[j].Xmax + cutterDiameter;
                    Ymin = _rectangles[j].Ymin - cutterDiameter;
                    Ymax = _rectangles[j].Ymax + cutterDiameter;
                    if (CommonSectionCircle(Xmin, Ymin, Xmin, Ymax, _circles[i].Xc, _circles[i].Yc, _circles[i].R) ||
                        CommonSectionCircle(Xmin, Ymax, Xmax, Ymax, _circles[i].Xc, _circles[i].Yc, _circles[i].R) ||
                        CommonSectionCircle(Xmax, Ymax, Xmax, Ymin, _circles[i].Xc, _circles[i].Yc, _circles[i].R) ||
                        CommonSectionCircle(Xmax, Ymin, Xmin, Ymin, _circles[i].Xc, _circles[i].Yc, _circles[i].R))
                        return false;
                }
            return true;
        }

        private bool CommonSectionCircle(double x1, double y1, double x2, double y2,
                                double xC, double yC, double R)
        {
            x1 -= xC;
            y1 -= yC;
            x2 -= xC;
            y2 -= yC;

            double dx = x2 - x1;
            double dy = y2 - y1;

            //составляем коэффициенты квадратного уравнения на пересечение прямой и окружности.
            //если на отрезке [0..1] есть отрицательные значения, значит отрезок пересекает окружность
            double a = dx * dx + dy * dy;
            double b = 2.0 * (x1 * dx + y1 * dy);
            double c = x1 * x1 + y1 * y1 - R * R;

            //а теперь проверяем, есть ли на отрезке [0..1] решения
            if (-b < 0)
                return (c < 0);
            if (-b < (2.0 * a))
                return ((4.0 * a * c - b * b) < 0);

            return (a + b + c < 0);
        }
    }
}
