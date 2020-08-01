using System;
using System.IO;

namespace MillingModule
{
    class CoordOutput
    {
        string path = "Coordinates.txt";
        public void GetCoord(Trajectory trajectory, Outline outline, int StartingPointPosition)
        {
            if (File.Exists(path))
                File.Delete(path);
            File.AppendAllText(path, "G01");
            if (trajectory!=null)
            {
                switch (StartingPointPosition)
                {
                    case 0:
                        for (int i = 0; i < trajectory.referencePoints.Count; i++)
                        {
                            if (trajectory.referencePoints[i] != null)
                            {
                                if (i > 0)
                                    SetPoint(trajectory.referencePoints[i - 1], trajectory.referencePoints[i], outline.Xmin, outline.Ymin);
                                else
                                    SetPoint(null, trajectory.referencePoints[i], outline.Xmin, outline.Ymin);
                            }
                        }
                        break;
                    case 1:
                        for (int i = 0; i < trajectory.referencePoints.Count; i++)
                        {
                            if (trajectory.referencePoints[i] != null)
                            {
                                if (i > 0)
                                    SetPoint(trajectory.referencePoints[i - 1], trajectory.referencePoints[i], outline.Xmin, outline.Ymax);
                                else
                                    SetPoint(null, trajectory.referencePoints[i], outline.Xmin, outline.Ymax);
                            }
                        }
                        break;
                    case 2:
                        for (int i = 0; i < trajectory.referencePoints.Count; i++)
                        {
                            if (trajectory.referencePoints[i] != null)
                            {
                                if (i > 0)
                                    SetPoint(trajectory.referencePoints[i - 1], trajectory.referencePoints[i], outline.Xmax, outline.Ymax);
                                else
                                    SetPoint(null, trajectory.referencePoints[i], outline.Xmax, outline.Ymax);
                            }
                        }
                        break;
                    case 3:
                        for (int i = 0; i < trajectory.referencePoints.Count; i++)
                        {
                            if (trajectory.referencePoints[i] != null)
                            {
                                if (i > 0)
                                    SetPoint(trajectory.referencePoints[i - 1], trajectory.referencePoints[i], outline.Xmax, outline.Ymin);
                                else
                                    SetPoint(null, trajectory.referencePoints[i], outline.Xmax, outline.Ymin);
                            }
                        }
                        break;
                    case 4:
                        for (int i = 0; i < trajectory.referencePoints.Count; i++)
                        {
                            if (trajectory.referencePoints[i] != null)
                            {
                                if (i > 0)
                                    SetPoint(trajectory.referencePoints[i - 1], trajectory.referencePoints[i], (outline.Xmax - outline.Xmin) / 2, (outline.Ymax - outline.Ymin) / 2);
                                else
                                    SetPoint(null, trajectory.referencePoints[i], (outline.Xmax - outline.Xmin) / 2, (outline.Ymax - outline.Ymin) / 2);

                            }
                        }
                        break;
                }
            }
        }

        private void SetPoint(Primitives.Point previousPoint, Primitives.Point point, float x, float y)
        {
            string X = Math.Round(point.X - x, 2).ToString();
            string Y = Math.Round(point.Y - y, 2).ToString();

            if (point.isArc)
            {
                string R = Math.Round(point.RadiusForArc).ToString();
                if (previousPoint.Y > point.Y)
                    LeftOrRight(point.isLeft, X, Y, R);
                else
                    LeftOrRight(!point.isLeft, X, Y, R);
            }
            else
            {
                if (previousPoint != null)
                {
                    if (previousPoint.X == point.X)
                        File.AppendAllText(path, "\t\tY" + Y + "\r\n");
                    else if (previousPoint.Y == point.Y)
                        File.AppendAllText(path, "\tX" + X + "\r\n");
                    else
                        File.AppendAllText(path, "\tX" + X + "\tY" + Y + "\r\n");
                }
                else
                    File.AppendAllText(path, "\tX" + X + "\tY" + Y + "\r\n");
            }
        }

        private void LeftOrRight(bool isLeft, string X, string Y, string R)
        {
            if (isLeft)
                File.AppendAllText(path, "G03" + "\tX" + X + "\tY" + Y + "\tR" + R + "\r\nG01");
            else
                File.AppendAllText(path, "G02" + "\tX" + X + "\tY" + Y + "\tR" + R + "\r\nG01");
        }
    }
}
