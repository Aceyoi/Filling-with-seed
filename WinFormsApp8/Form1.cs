using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp8
{
    public partial class Form1 : Form
    {
        private Bitmap bmp;
        private Graphics g;
        private PointF[] starPoints;

        public Form1()
        {
            InitializeComponent();
            InitializeDrawing();
        }

        private void InitializeDrawing()
        {
            // ������� Bitmap � Graphics ��� ���������
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(bmp);
            g.Clear(Color.White); // ������� PictureBox

            // ���������� ����� ��� ������
            starPoints = CalculateStarPoints(5, new PointF(pictureBox1.Width / 2, pictureBox1.Height / 2), 100, 50);

            // ������ ������ ������
            g.DrawPolygon(Pens.Black, starPoints);

            // ���������� ����������� � PictureBox
            pictureBox1.Image = bmp;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            // ���������, ��������� �� ����� ����� ������ ������
            if (IsPointInPolygon(starPoints, e.Location))
            {
                // ��������� �������
                Fill4Connected(e.Location, Color.Yellow, Color.White);
                pictureBox1.Refresh(); // ��������� PictureBox
            }
        }

        private void Fill4Connected(Point startPoint, Color fillColor, Color targetColor)
        {
            // �������� ���� ��������� �����
            Color startColor = bmp.GetPixel(startPoint.X, startPoint.Y);

            // ���� ��������� ����� ��� ������ ��� �� ������������� �������� �����, �������
            if (startColor.ToArgb() != targetColor.ToArgb())
                return;

            // ���������� ���� ��� �������� �����, ������� ����� ������
            var stack = new System.Collections.Generic.Stack<Point>();
            stack.Push(startPoint);

            while (stack.Count > 0)
            {
                Point point = stack.Pop();

                // ���������, ��� ����� ��������� � �������� �����������
                if (point.X < 0 || point.X >= bmp.Width || point.Y < 0 || point.Y >= bmp.Height)
                    continue;

                // ���������, ��� ���� ����� ������������� �������� �����
                if (bmp.GetPixel(point.X, point.Y).ToArgb() != targetColor.ToArgb())
                    continue;

                // �������� �����
                bmp.SetPixel(point.X, point.Y, fillColor);

                // ��������� �������� ����� � ����
                stack.Push(new Point(point.X - 1, point.Y)); // �����
                stack.Push(new Point(point.X + 1, point.Y)); // ������
                stack.Push(new Point(point.X, point.Y - 1)); // �����
                stack.Push(new Point(point.X, point.Y + 1)); // ����
            }
        }

        private bool IsPointInPolygon(PointF[] polygon, Point point)
        {
            // �������� "Ray Casting" ��� ��������, ��������� �� ����� ������ ��������
            bool inside = false;
            int n = polygon.Length;
            for (int i = 0, j = n - 1; i < n; j = i++)
            {
                if (((polygon[i].Y > point.Y) != (polygon[j].Y > point.Y)) &&
                    (point.X < (polygon[j].X - polygon[i].X) * (point.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X))
                {
                    inside = !inside;
                }
            }
            return inside;
        }

        private PointF[] CalculateStarPoints(int numPoints, PointF center, float outerRadius, float innerRadius)
        {
            PointF[] points = new PointF[numPoints * 2];
            double angle = Math.PI / numPoints;

            for (int i = 0; i < numPoints * 2; i++)
            {
                double radius = i % 2 == 0 ? outerRadius : innerRadius;
                points[i] = new PointF(
                    center.X + (float)(radius * Math.Sin(i * angle)),
                    center.Y - (float)(radius * Math.Cos(i * angle))
                );
            }

            return points;
        }
    }
}