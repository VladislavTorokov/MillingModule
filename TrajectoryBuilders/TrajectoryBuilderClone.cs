using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime;
using System.Windows.Forms;

namespace MillingModule
{
    public class TrajectoryBuilderClone
    {
        Trajectory trajectory = new Trajectory();
        bool G01 = false;
        bool G02 = false;
        bool G03 = false;
        bool X = false;
        bool Y = false;

        public TrajectoryBuilderClone(string[] Gcode)
        {
            for (int i = 0; i < Gcode.Count(); i++)
            {
                if (Gcode[i].Contains("G01") || Gcode[i].Contains("G1"))
                {
                    G01 = true;
                    G02 = false;
                    G03 = false;
                }
                if (Gcode[i].Contains("G02") || Gcode[i].Contains("G2"))
                {
                    G01 = false;
                    G02 = true;
                    G03 = false;
                }
                if (Gcode[i].Contains("G03") || Gcode[i].Contains("G3"))
                {
                    G01 = false;
                    G02 = false;
                    G03 = true;
                }
                if (Gcode[i].Contains('X'))
                {
                    if (G01 || G02 || G03)
                    {
                        if (G01)
                        {
                            AddPoints(Gcode[i], 'X', 'Y', false);
                        }
                        else if (G02)
                        {
                            AddPoints(Gcode[i], 'X', 'Y', false);
                            FindRadius(Gcode[i], G02);
                        }
                        else if (G03)
                        {
                            AddPoints(Gcode[i], 'X', 'Y', false);
                            FindRadius(Gcode[i], false);
                        }
                    }
                    else
                    {
                        trajectory.Clear();
                        MessageBox.Show("Отсутствуют коды перемещения инструмента");
                        return;
                    }
                }

                else if (Gcode[i].Contains('Y'))
                {
                    if (G01 || G02 || G03)
                    {
                        if (G01)
                        {
                            AddPoints(Gcode[i], 'Y', 'X', false);
                        }
                        else if (G02)
                        {
                            AddPoints(Gcode[i], 'Y', 'X', false);
                            FindRadius(Gcode[i], G02);
                        }
                        else if (G03)
                        {
                            AddPoints(Gcode[i], 'Y', 'X', false);
                            FindRadius(Gcode[i], false);
                        }
                    }
                    else
                    {
                        trajectory.Clear();
                        MessageBox.Show("Отсутствуют коды перемещения инструмента");
                        return;
                    }
                }

            }
        }

        private void AddPoints(string Gcode, char coordinateFirst, char coordinateSecond, bool circularInterpolation)
        {
            trajectory.referencePoints.Add(new Primitives.Point());
            AddCoordinate(Gcode, coordinateFirst);
            if (Gcode.Contains(coordinateSecond))
            {
                AddCoordinate(Gcode, coordinateSecond);
                if (!circularInterpolation)
                    AddLine();
            }
            else
            {
                if (coordinateFirst == 'X')
                {
                    if (Y)
                    {
                        trajectory.referencePoints.Last().Y = trajectory.referencePoints[trajectory.referencePoints.Count - 2].Y;
                        if (!circularInterpolation)
                            AddLine();
                    }
                    else
                    {
                        MessageBox.Show("Отсутствует координата Y");
                        trajectory.Clear();
                        return;
                    }
                }
                else
                {
                    if (X)
                    {
                        trajectory.referencePoints.Last().X = trajectory.referencePoints[trajectory.referencePoints.Count - 2].X;
                        if (!circularInterpolation)
                            AddLine();
                    }
                    else
                    {
                        MessageBox.Show("Отсутствует координата X");
                        trajectory.Clear();
                        return;
                    }
                }

            }

        }

        private void AddCoordinate(string coordCode, char Coordinate)
        {
            int from = 0, to = 0;
            from = coordCode.IndexOf(Coordinate) + 1;
            for (int i = from; i < coordCode.Length; i++)
            {
                if (!char.IsDigit(coordCode[i]) && !(coordCode[i] == ',') && !(coordCode[i] == '-'))
                {
                    to = i;
                    break;
                }
                if (i == coordCode.Length - 1)
                {
                    to = i + 1;
                    break;
                }
            }
            coordCode = coordCode.Substring(from, to - from);
            if (Coordinate == 'X')
            {
                trajectory.referencePoints.Last().X = float.Parse(coordCode);
                X = true;
            }
            else if (Coordinate == 'Y')
            {
                trajectory.referencePoints.Last().Y = float.Parse(coordCode);
                Y = true;
            }
        }

        private void AddLine()
        {
            if (trajectory.referencePoints.Count > 1)
                trajectory.lines.Add(new Primitives.Line(trajectory.referencePoints[trajectory.referencePoints.Count - 2],
                                                         trajectory.referencePoints[trajectory.referencePoints.Count - 1]));
        }

        private void FindRadius(string coordCode, bool G02)
        {
            int from = 0, to = 0;
            from = coordCode.IndexOf('R') + 1;
            for (int i = from; i < coordCode.Length; i++)
            {
                if (!Char.IsDigit(coordCode[i]) && !(coordCode[i] == ','))
                {
                    to = i;
                    break;
                }
                if (i == coordCode.Length - 1)
                {
                    to = i + 1;
                    break;
                }
            }
            if (trajectory.lines.Last() != null)
                trajectory.lines.Remove(trajectory.lines.Last());
            float radius = float.Parse(coordCode.Substring(from, to - from));
            float x1 = trajectory.referencePoints[trajectory.referencePoints.Count - 2].X;
            float y1 = trajectory.referencePoints[trajectory.referencePoints.Count - 2].Y;
            float x2 = trajectory.referencePoints[trajectory.referencePoints.Count - 1].X;
            float y2 = trajectory.referencePoints[trajectory.referencePoints.Count - 1].Y;
            float chordLegth = (float)Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
            float height = (float)Math.Sqrt((radius * radius) - (chordLegth / 2 * chordLegth / 2));
            float Xc = x2 + (x1 - x2) / 2 + height * (y1 - y2) / chordLegth;
            float Yc = y2 + (y1 - y2) / 2 - (x1 - x2) / chordLegth;
            float[] angleBeginEnd = CalculateAngleForArc(x1 - Xc, x2 - Xc, radius, G02, y1 < y2, x1 < Xc && x2 < Xc);
            trajectory.arcs.Add(new Primitives.Arc(new Primitives.Point { X = Xc, Y = Yc, RadiusForArc = radius, isArc = true }, angleBeginEnd[0], angleBeginEnd[1]));
        }

        private float[] CalculateAngleForArc(float triangleLegX1, float triangleLegX2, float hypotenuse, bool G02, bool weGoingUp, bool isLeft)
        {
            float[] angleBeginEnd = new float[2];
            angleBeginEnd[0] = 0;
            angleBeginEnd[1] = 0;
            if (isLeft)
            {
                angleBeginEnd[0] = (float)(Math.Acos(triangleLegX1 / hypotenuse) * 180 / Math.PI);
                angleBeginEnd[1] = -(float)(Math.Acos(triangleLegX2 / hypotenuse) * 180 / Math.PI);
            }
            else
            {
                angleBeginEnd[0] = -(float)(Math.Acos(triangleLegX1 / hypotenuse) * 180 / Math.PI);
                angleBeginEnd[1] = (float)(Math.Acos(triangleLegX2 / hypotenuse) * 180 / Math.PI);
            }
            return angleBeginEnd;
        }

        public Trajectory GetTrajectory()
        {
            return trajectory;
        }
    }
}
