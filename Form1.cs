using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace MillingModule
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        Outline outline;
        Outline copyOutline;
        Trajectory trajectory;
        DxfReader dxfReader = new DxfReader();
        Scaling scaling;
        ParallelTransfer parallelTransfer;

        public Form1()
        {
            InitializeComponent();
        }

        private void ButtonOpenFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                // Считываение текста
                string fileName = openFile.FileName;
                string dxf_file = File.ReadAllText(openFile.FileName);
                string extension = Path.GetExtension(fileName);
                if (extension == ".dxf")
                {
                    float parallelTransferCoefX;
                    float parallelTransferCoefY;
                    float scalingCoefX;
                    float scalingCoefY;
                    outline = new Outline();
                    outline = dxfReader.GetCoordinates(dxf_file, outline);
                    Outline outlineTest = new Outline();
                    outlineTest = dxfReader.GetCoordinates(dxf_file, outlineTest);
                    copyOutline = outlineTest;
                    parallelTransferCoefX = -outline.Points.Min(pointX => pointX.X);
                    parallelTransferCoefY = -outline.Points.Min(pointY => pointY.Y);
                    parallelTransfer = new ParallelTransfer(parallelTransferCoefX,
                                                            parallelTransferCoefY);
                    scalingCoefX = outline.Xmax - outline.Xmin;
                    scalingCoefY = outline.Ymax - outline.Ymin;
                    scaling = new Scaling(scalingCoefX, scalingCoefY, pictureBox1.Width, pictureBox1.Height);
                    ParallelTransferObject(outlineTest, parallelTransfer);
                    ScalingObject(outlineTest, scaling);

                    ButtonDraw_Click(sender, e);

                    ButtonCreateRectangle.Enabled = false;
                    ButtonClear.Enabled = true;
                    ButtonCreateTrajectory.Enabled = true;
                    ButtonDraw.Enabled = true;
                }
                else
                    MessageBox.Show("Невозможно открыть файл");
            }
        }

        private void ButtonCreateRectangle_Click(object sender, EventArgs e)
        {
            float heightRectangle = float.Parse(tbHeightRectangle.Text);
            float widthRectangle = float.Parse(tbWidthRectangle.Text);

            copyOutline = CreateRectangleGraph(widthRectangle, heightRectangle);
            outline = CreateRectangleGraph(widthRectangle, heightRectangle);
            scaling = new Scaling(widthRectangle, heightRectangle, pictureBox1.Width, pictureBox1.Height);
            parallelTransfer = new ParallelTransfer(widthRectangle * 0.2f,
                                        heightRectangle * 0.2f);
            ParallelTransferObject(copyOutline, parallelTransfer);
            ScalingObject(copyOutline, scaling);

            ButtonDraw_Click(sender, e);

            ButtonCreateTrajectory.Enabled = false;
            ButtonClear.Enabled = true;
            ButtonCreateTrajectory.Enabled = true;
            ButtonDraw.Enabled = true;
        }

        private void ButtonDraw_Click(object sender, EventArgs e)
        {
            Graphics picture = pictureBox1.CreateGraphics();
            string[] Gcode = tbGCode.Text.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            File.WriteAllText("Coordinates.txt", tbGCode.Text);
            picture.Clear(Color.White);
            if (metroToggleOutline.Checked && copyOutline != null)
            {
                Draw(copyOutline.Arcs, picture, Color.Black);
                Draw(copyOutline.Lines, picture, Color.Black);
                Draw(copyOutline.Circles, picture, Color.Black);
            }
            if (metroToggleTrajectory.Checked && trajectory != null)
            {

                Draw(trajectory.lines, picture, Color.DarkOrange);
                Draw(trajectory.arcs, picture, Color.DarkOrange);
            }
        }

        private void ButtonCreateTrajectory_Click(object sender, EventArgs e)
        {
            Graphics picture = pictureBox1.CreateGraphics();
            CreateOutline(picture, dxfReader);
            ButtonDraw_Click(sender, e);

        }

        private void ButtonClear_Click(object sender, EventArgs e)
        {
            Graphics picture = pictureBox1.CreateGraphics();
            picture.Clear(Color.White);
        }

        private void CreateOutline(Graphics graphics, DxfReader dxfReader)
        {
            if (copyOutline != null)
            {
                int StartingPointPosition = 0;

                if (rbLeftBot.Checked)
                    StartingPointPosition = 0;
                if (rbLeftTop.Checked)
                    StartingPointPosition = 1;
                if (rbRightTop.Checked)
                    StartingPointPosition = 2;
                if (rbRightBot.Checked)
                    StartingPointPosition = 3;
                if (rbCentre.Checked)
                    StartingPointPosition = 4;

                float cuttingWidth = TrackBarWidthCut.Value;
                try
                {
                    float diameter = float.Parse(metrotbDiameter.Text);
                    if (diameter < 4)
                    {
                        diameter = 4;
                        MessageBox.Show("Диаметр фрезы не может быть меньше 4 мм\nтраектория постороена со значением D=4 мм");
                    }
                    else if (diameter > 160)
                    {
                        diameter = 160;
                        MessageBox.Show("Диаметр фрезы не может быть больше 160 мм\nтраектория постороена со значением D=160 мм");
                    }

                    TrajectoryManager trajectoryBuilder = new TrajectoryManager(outline.Rectangles, outline.Circles,cuttingWidth);
                    if (CBZigZag.Checked)
                        trajectory = trajectoryBuilder.GetZigZagTrajectory(outline, diameter, cuttingWidth);
                    if (CBSpiral.Checked)
                        trajectory = trajectoryBuilder.GetSpiralTrajectory(outline, diameter, cuttingWidth);
                    if (trajectory == null)
                    {
                        if (outline.Rectangles != null)
                        {
                            MessageBox.Show("Некоторые элементы расположены слишком близко друг к другу");
                            return;
                        }
                    }
                }
                catch (Exception e) { MessageBox.Show("Входная строка имела неверный формат"); }
                if (trajectory != null)
                {
                    CoordOutput coordOutput = new CoordOutput();
                    coordOutput.GetCoord(trajectory, outline, StartingPointPosition);

                    Trajectory trajectoryForDraw = (Trajectory)trajectory.Clone();
                    ParallelTransferObject(trajectoryForDraw, parallelTransfer);
                    ScalingObject(trajectoryForDraw, scaling);

                    string path = "Coordinates.txt";
                    if (File.Exists(path))
                        tbGCode.Text = String.Join(Environment.NewLine, File.ReadLines(path));
                }
            }
            else
                MessageBox.Show("загрузите dxf файл, либо постройте прямоугольник");
        }

        private Outline CreateRectangleGraph(float width, float height)
        {
            Outline graph = new Outline();
            graph.Points.Add(new Primitives.Point() { X = 0, Y = 0 });
            graph.Points.Add(new Primitives.Point() { X = 0, Y = height });
            graph.Points.Add(new Primitives.Point() { X = width, Y = height });
            graph.Points.Add(new Primitives.Point() { X = width, Y = 0 });
            for(int i=0;i<4;i++)
            {
                if(i==3)
                    graph.Lines.Add(new Primitives.Line(graph.Points[i], graph.Points[0]));
                else
                    graph.Lines.Add(new Primitives.Line(graph.Points[i], graph.Points[i + 1]));
            }
            graph.GetMinMax();
            return graph;
        }

        private void ParallelTransferObject(object objectForTransfer, ParallelTransfer parallelTransfer)
        {
            if (objectForTransfer is Outline)
            {
                Outline outline = (Outline)objectForTransfer;
                if (parallelTransfer != null)
                {
                    parallelTransfer.TransferPoints(outline.Points);
                    parallelTransfer.TransferArc(outline.Arcs);
                    parallelTransfer.TransferCircles(outline.Circles);
                }
            }
            if (objectForTransfer is Trajectory)
            {

                Trajectory trajectory = (Trajectory)objectForTransfer;
                if (parallelTransfer != null)
                {
                    parallelTransfer.TransferPoints(trajectory.referencePoints);
                    parallelTransfer.TransferArc(trajectory.arcs);
                }
            }
        }

        private void ScalingObject(object objectForScaling, Scaling scaling)
        {
            if (objectForScaling is Outline)
            {
                Outline outline = (Outline)objectForScaling;
                if (scaling != null)
                {
                    scaling.ScalingPoints(outline.Points);
                    scaling.ScalingCircles(outline.Circles);
                    scaling.ScalingArc(outline.Arcs);
                }
            }
            if (objectForScaling is Trajectory)
            {
                Trajectory trajectory = (Trajectory)objectForScaling;
                if (scaling != null)
                {
                    scaling.ScalingPoints(trajectory.referencePoints);
                    scaling.ScalingArc(trajectory.arcs);
                }
            }
        }

        //Прорисовка
        private void DrawAll(List<Primitives.Line> lines, List<Primitives.Circle> circles, List<Primitives.Arc> arcs, Graphics picture, Color color)
        {
            if (lines != null)
                Draw(lines, picture, color);
            if (circles != null)
                Draw(circles, picture, color);
            if (arcs != null)
                Draw(arcs, picture, color);
        }

        private void Draw(List<Primitives.Line> lines, Graphics picture, Color color)
        {
            if (lines != null)
            {
                float x0 = 0, y0 = pictureBox1.Height;
                foreach (Primitives.Line line in lines)
                {
                    picture.DrawLine(new Pen(color, 2), line.From.X,
                                                               -line.From.Y + y0,
                                                                line.To.X,
                                                               -line.To.Y + y0);
                }
            }
        }

        private void Draw(List<Primitives.Circle> circles, Graphics picture, Color color)
        {
            if (circles != null)
            {
                float x0 = 0, y0 = pictureBox1.Height;
                foreach (Primitives.Circle circle in circles)
                {
                    picture.DrawEllipse(new Pen(color, 2), circle.Xc - circle.R,
                                                                 -(circle.Yc - circle.R) + y0,
                                                                   circle.R * 2,
                                                                 -(circle.R * 2));
                }
            }
        }

        private void Draw(List<Primitives.Arc> arcs, Graphics picture, Color color)
        {
            if (arcs != null)
            {
                float x0 = 0, y0 = pictureBox1.Height;
                foreach (Primitives.Arc arc in arcs)
                {
                    picture.DrawArc(new Pen(color, 2), arc.Xc - arc.R,
                                                             -(arc.Yc + arc.R) + y0,
                                                               arc.R * 2,
                                                               arc.R * 2,
                                                              -arc.Angle_From,
                                                               arc.Angle_From > arc.Angle_To ? -(360 - arc.Angle_From + arc.Angle_To) : -(arc.Angle_To - arc.Angle_From));
                }
            }
        }

        private void metroTrackBar1_Scroll(object sender, ScrollEventArgs e)
        {
            LabelProcentWidthCut.Text = TrackBarWidthCut.Value.ToString() + "%";
        }

        private void CBZigZag_CheckedChanged(object sender, EventArgs e)
        {
            CBSpiral.Checked = false;
        }

        private void CBSpiral_CheckedChanged(object sender, EventArgs e)
        {
            CBZigZag.Checked = false;
        }
    }
}
