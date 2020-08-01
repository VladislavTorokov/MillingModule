using System;
using System.Collections.Generic;
using System.Linq;
using MillingModule.Primitives;

namespace MillingModule
{
    class ZigZagBuilder
    {
        Trajectory trajectory;
        int stepCount;
        float step;
        float partCut;
        float currentX;
        bool areGoingUp;

        public void GetZigZag(Outline outline, float cutterDiameter, float cuttingWidth,List<Obstruction> obstructions)
        {
            trajectory = new Trajectory();
            stepCount = 0;
            step = cuttingWidth * cutterDiameter / 100;
            partCut = step - cutterDiameter / 2;
            currentX = outline.Xmin + partCut + step * stepCount;
            areGoingUp = true;

            trajectory.referencePoints.Add(new Point());
            trajectory.referencePoints.Last().X = outline.Xmin + partCut;
            trajectory.referencePoints.Last().Y = outline.Ymin;

            while (currentX < outline.Xmax)
            {
                List<Obstruction> copyObstructions = new List<Obstruction>();
                copyObstructions.AddRange(obstructions);
                Obstruction obstruction;

                do
                {
                    obstruction = FindObstruction(currentX, step, cutterDiameter / 2, copyObstructions, areGoingUp);
                    if (!obstruction.IsEmpty())
                    {
                        if (obstruction is Circle)
                            AddArcPoints((Circle)obstruction, currentX, cutterDiameter / 2, areGoingUp);
                        else if (obstruction is Rectangle)
                            AddRectanglePoints((Rectangle)obstruction, currentX, cutterDiameter / 2, areGoingUp);
                        copyObstructions.Remove(obstruction);
                    }
                } while (!obstruction.IsEmpty());

                AddPoint(outline, partCut, currentX, areGoingUp);
                stepCount++;
                currentX = outline.Xmin + partCut + step * stepCount;
                AddPoint(outline, partCut, currentX, areGoingUp);

                if (areGoingUp)
                    areGoingUp = false;
                else
                    areGoingUp = true;
            }
        }

        private void AddPoint(Outline outline, float partCut, float currentX, bool areGoingUp)
        {
            trajectory.referencePoints.Add(new Point());
            if (areGoingUp)
            {
                trajectory.referencePoints.Last().X = currentX;
                trajectory.referencePoints.Last().Y = outline.Ymax + partCut;
                trajectory.SetPointsForLines();
            }
            else
            {
                trajectory.referencePoints.Last().X = currentX;
                trajectory.referencePoints.Last().Y = outline.Ymin - partCut;
                trajectory.SetPointsForLines();
            }

        }

        private void AddArcPoints(Circle circle, float currentX, float cutterRadius, bool areGoingUp)
        {
            for (int i = 0; i < 2; i++)
            {
                trajectory.referencePoints.Add(new Point());
                trajectory.referencePoints.Last().X = currentX;
                if (areGoingUp)
                {
                    trajectory.referencePoints.Last().Y = circle.Yc - CalculateArcPointY(circle, currentX, cutterRadius);
                    if (i == 0)
                        trajectory.SetPointsForLines();
                    else
                    {
                        trajectory.referencePoints.Last().isArc = true;
                        if (currentX < circle.Xc)
                            trajectory.referencePoints.Last().isLeft = true;
                        trajectory.referencePoints.Last().RadiusForArc = circle.R + cutterRadius;
                    }
                    areGoingUp = false;
                }
                else
                {
                    trajectory.referencePoints.Last().Y = circle.Yc + CalculateArcPointY(circle, currentX, cutterRadius);
                    if (i == 0)
                        trajectory.SetPointsForLines();
                    else
                    {
                        trajectory.referencePoints.Last().isArc = true;
                        if (currentX < circle.Xc)
                            trajectory.referencePoints.Last().isLeft = true;
                        trajectory.referencePoints.Last().RadiusForArc = circle.R + cutterRadius;
                    }
                    areGoingUp = true;
                }
            }
            trajectory.SetPointsForArc(circle.Xc,
                                       circle.Yc,
                                       circle.R + cutterRadius,
                                      (currentX <= circle.Xc) ? -(circle.Xc - currentX) :
                                                                 (currentX - circle.Xc),
                                       currentX <= circle.Xc);
        }

        private void AddRectanglePoints(Rectangle rectangle, float currentX, float cutterRadius, bool areGoingUp)
        {
            if (areGoingUp)
            {
                trajectory.referencePoints.Add(new Point());
                trajectory.referencePoints.Last().X = currentX;
                trajectory.referencePoints.Last().Y = rectangle.Ymin - cutterRadius;
                trajectory.SetPointsForLines();

                trajectory.referencePoints.Add(new Point());
                if (currentX < (rectangle.Xmax - rectangle.Xmin) / 2 + rectangle.Xmin)
                    trajectory.referencePoints.Last().X = rectangle.Xmin - cutterRadius;
                else
                    trajectory.referencePoints.Last().X = rectangle.Xmax + cutterRadius;
                trajectory.referencePoints.Last().Y = rectangle.Ymin - cutterRadius;
                trajectory.SetPointsForLines();

                trajectory.referencePoints.Add(new Point());
                if (currentX < (rectangle.Xmax - rectangle.Xmin) / 2 + rectangle.Xmin)
                    trajectory.referencePoints.Last().X = rectangle.Xmin - cutterRadius;
                else
                    trajectory.referencePoints.Last().X = rectangle.Xmax + cutterRadius;
                trajectory.referencePoints.Last().Y = rectangle.Ymax + cutterRadius;
                trajectory.SetPointsForLines();

                trajectory.referencePoints.Add(new Point());
                trajectory.referencePoints.Last().X = currentX;
                trajectory.referencePoints.Last().Y = rectangle.Ymax + cutterRadius;
                trajectory.SetPointsForLines();
            }
            else
            {
                trajectory.referencePoints.Add(new Point());
                trajectory.referencePoints.Last().X = currentX;
                trajectory.referencePoints.Last().Y = rectangle.Ymax + cutterRadius;
                trajectory.SetPointsForLines();

                trajectory.referencePoints.Add(new Point());
                if (currentX < (rectangle.Xmax - rectangle.Xmin) / 2 + rectangle.Xmin)
                    trajectory.referencePoints.Last().X = rectangle.Xmin - cutterRadius;
                else
                    trajectory.referencePoints.Last().X = rectangle.Xmax + cutterRadius;
                trajectory.referencePoints.Last().Y = rectangle.Ymax + cutterRadius;
                trajectory.SetPointsForLines();

                trajectory.referencePoints.Add(new Point());
                if (currentX < (rectangle.Xmax - rectangle.Xmin) / 2 + rectangle.Xmin)
                    trajectory.referencePoints.Last().X = rectangle.Xmin - cutterRadius;
                else
                    trajectory.referencePoints.Last().X = rectangle.Xmax + cutterRadius;
                trajectory.referencePoints.Last().Y = rectangle.Ymin - cutterRadius;
                trajectory.SetPointsForLines();

                trajectory.referencePoints.Add(new Point());
                trajectory.referencePoints.Last().X = currentX;
                trajectory.referencePoints.Last().Y = rectangle.Ymin - cutterRadius;
                trajectory.SetPointsForLines();
            }
        }

        private float CalculateArcPointY(Circle circle, float currentX, float cutterRadius)
        {
            return (float)Math.Sqrt((circle.R + cutterRadius) * (circle.R + cutterRadius) - (circle.Xc - currentX) *
                                                                                            (circle.Xc - currentX));
        }

        private Obstruction FindObstruction(float currentX, float step, float radius, List<Obstruction> copyObstructions, bool areGoingUp)
        {
            if (copyObstructions != null)
            {
                while (copyObstructions.Count > 0)
                {
                    Obstruction copyObstruction;
                    if (areGoingUp)
                        copyObstruction = copyObstructions.Find(obstruction => obstruction.Yc == copyObstructions.Min(minObstruction => minObstruction.Yc));
                    else
                        copyObstruction = copyObstructions.Find(obstruction => obstruction.Yc == copyObstructions.Max(maxObstruction => maxObstruction.Yc));

                    if ((currentX >= copyObstruction.Xc - copyObstruction.R - radius) && (currentX <= copyObstruction.Xc + copyObstruction.R + radius))
                    {
                        copyObstructions.Remove(copyObstruction);
                        return copyObstruction;
                    }
                    else
                        copyObstructions.Remove(copyObstruction);
                }
            }
            return new Obstruction();
        }



        public Trajectory GetTrajectory()
        {
            return trajectory;
        }
    }
}
