using System;
using System.Collections.Generic;
using System.Linq;
using MillingModule.Primitives;

namespace MillingModule
{
    class DxfReader
    {
        string[] dxf_str;

        public Outline GetCoordinates(string dxf_text,Outline outline)
        {
            if ((dxf_text == null) || (dxf_text == ""))
            {
                throw new ArgumentException("Файл пуст");
            }
            dxf_text = dxf_text.Replace('.', ',');
            dxf_str = dxf_text.Split('\n');

            string section;

            section = "ENTITIES\r";
            int[] sectionEntitiesBeginEnd = FindSection(section);
            TransferCoordinateFromEntities(sectionEntitiesBeginEnd, outline);

            section = "BLOCKS\r";
            int[] sectionBlocksBeginEnd = FindSection(section);
            TransferCoordinateFromBlocks(sectionBlocksBeginEnd, outline);

            outline.GetMinMax();
            return outline;
        }

        private int[] FindSection(string entry)
        {
            int[] sectionBeginEnd = new int[2];
            int i = 0;
            bool sectionEntitiesOpen = false;

            while (dxf_str[i] != "EOF\r")
            {
                if (dxf_str[i] == entry)
                {
                    sectionBeginEnd[0] = i;
                    sectionEntitiesOpen = true;
                }

                if (sectionEntitiesOpen)
                    if (dxf_str[i] == "ENDSEC\r")
                    {
                        sectionBeginEnd[1] = i;
                        return sectionBeginEnd;
                    }

                i++;
            }
            return null;
        }

        private void TransferCoordinateFromEntities(int[] sectionBeginEnd,Outline graph)
        {
            int i = sectionBeginEnd[0];
            int[] step = { 20, 22, 26, 28, 30, 32 };

            while (i < sectionBeginEnd[1])
            {
                switch (dxf_str[i])
                {
                    case "LINE\r":
                        AddLine(step, i, 0, graph);
                        break;

                    case "CIRCLE\r":
                        graph.Points.Add(new Point()
                        {
                            X = RoundParse(dxf_str[i + step[0]]),
                            Y = RoundParse(dxf_str[i + step[1]]),
                            RadiusForArc = RoundParse(dxf_str[i + step[2]]),
                            isArc = true
                        });
                        graph.Circles.Add(new Circle(graph.Points.Last()));
                        break;

                    case "ARC\r":
                        graph.Points.Add(new Point()
                        {
                            X = RoundParse(dxf_str[i + step[0]]),
                            Y = RoundParse(dxf_str[i + step[1]]),
                            RadiusForArc = RoundParse(dxf_str[i + step[2]]),
                            isArc = true
                        });
                        graph.Arcs.Add(new Arc(graph.Points.Last(),
                        RoundParse(dxf_str[i + step[4]]),
                        RoundParse(dxf_str[i + step[5]]) == 0 ? 360 : RoundParse(dxf_str[i + step[5]])));
                        break;
                }
                i++;
            }
        }

        private void TransferCoordinateFromBlocks(int[] sectionBeginEnd,Outline graph)
        {
            int i = sectionBeginEnd[0];
            int[] step = { 20, 22, 26, 28, 30, 32 };

            while (i < sectionBeginEnd[1])
            {
                if (dxf_str[i] == "LINE\r")
                {
                    AddLine(step, i, 0,graph);
                    AddLine(step, i, 1, graph);
                    AddLine(step, i, 2, graph);
                    AddLine(step, i, 3, graph);
                    graph.Rectangles.Add(new Rectangle());
                    for (int j = graph.Lines.Count - 4; j < graph.Lines.Count; j++)
                    {
                        graph.Rectangles.Last().SetLine(graph.Lines[j]);
                    }
                    i += step[5] * 4;
                }
                i++;
            }
        }

        private void AddLine(int[] step,int currentNumberString, int numberOfLine,Outline graph)
        {
            graph.Points.Add(new Point()
            {
                X = RoundParse(dxf_str[currentNumberString + step[0] + step[5] * numberOfLine]),
                Y = RoundParse(dxf_str[currentNumberString + step[1] + step[5] * numberOfLine])
            });
            graph.Points.Add(new Point()
            {
                X = RoundParse(dxf_str[currentNumberString + step[2] + step[5] * numberOfLine]),
                Y = RoundParse(dxf_str[currentNumberString + step[3] + step[5] * numberOfLine])
            });
            graph.Lines.Add(new Line(graph.Points[graph.Points.Count - 2], graph.Points[graph.Points.Count - 1]));
        }

        private float RoundParse(string str)
        {
            return (float)Math.Round(double.Parse(str), 2);
        }
    }
}
