using System.Collections.Generic;
using System.Linq;

namespace MillingModule
{
    class SpiralBuilder
    {
        Trajectory trajectory;
        List<Primitives.Line> lines;
        int stepCount;
        float stepCut;
        float partCut;
        float currentStep;
        float Ymax;

        public void GetSpiral(Outline outline, float cutterDiameter, float cuttingWidth)
        {
            trajectory = new Trajectory();
            lines = outline.Lines;
            stepCount = 0;
            stepCut = cuttingWidth * cutterDiameter / 100;
            partCut = stepCut - cutterDiameter / 2;
            currentStep = partCut + stepCut * stepCount;
            Ymax = outline.Ymax;

            trajectory.referencePoints.Add(new Primitives.Point());
            trajectory.referencePoints.Last().X = lines[0].From.X + partCut;
            trajectory.referencePoints.Last().Y = lines[0].From.Y + partCut;

            do
            {
                AddPassage(outline, currentStep);
                stepCount++;
                currentStep = partCut + stepCut * stepCount;
                Ymax = outline.Ymax - currentStep;
            } while (trajectory.referencePoints.Last().Y + cutterDiameter < Ymax);
        }

        private void AddPassage(Outline outline,float step)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                trajectory.referencePoints.Add(new Primitives.Point());

                if (i == lines.Count - 1)
                    trajectory.referencePoints.Last().X = lines[i].To.X + step + stepCut + partCut;
                else if (lines[i].To.X == outline.Xmax)
                    trajectory.referencePoints.Last().X = lines[i].To.X - step;
                else
                    trajectory.referencePoints.Last().X = lines[i].To.X + step;

                if (lines[i].To.Y == outline.Ymin)
                    trajectory.referencePoints.Last().Y = lines[i].To.Y + step;
                else
                    trajectory.referencePoints.Last().Y = lines[i].To.Y - step;
                trajectory.SetPointsForLines();

            }
        }

        public Trajectory GetTrajectory()
        {
            return trajectory;
        }
    }
}
