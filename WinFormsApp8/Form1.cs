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
            // Создаем Bitmap и Graphics для рисования
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(bmp);
            g.Clear(Color.White); // Очищаем PictureBox

            // Определяем точки для звезды
            starPoints = CalculateStarPoints(5, new PointF(pictureBox1.Width / 2, pictureBox1.Height / 2), 100, 50);

            // Рисуем контур звезды
            g.DrawPolygon(Pens.Black, starPoints);

            // Отображаем изображение в PictureBox
            pictureBox1.Image = bmp;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            // Проверяем, находится ли точка клика внутри звезды
            if (IsPointInPolygon(starPoints, e.Location))
            {
                // Запускаем заливку
                Fill4Connected(e.Location, Color.Yellow, Color.White);
                pictureBox1.Refresh(); // Обновляем PictureBox
            }
        }

        private void Fill4Connected(Point startPoint, Color fillColor, Color targetColor)
        {
            // Получаем цвет начальной точки
            Color startColor = bmp.GetPixel(startPoint.X, startPoint.Y);

            // Если начальная точка уже залита или не соответствует целевому цвету, выходим
            if (startColor.ToArgb() != targetColor.ToArgb())
                return;

            // Используем стек для хранения точек, которые нужно залить
            var stack = new System.Collections.Generic.Stack<Point>();
            stack.Push(startPoint);

            while (stack.Count > 0)
            {
                Point point = stack.Pop();

                // Проверяем, что точка находится в пределах изображения
                if (point.X < 0 || point.X >= bmp.Width || point.Y < 0 || point.Y >= bmp.Height)
                    continue;

                // Проверяем, что цвет точки соответствует целевому цвету
                if (bmp.GetPixel(point.X, point.Y).ToArgb() != targetColor.ToArgb())
                    continue;

                // Заливаем точку
                bmp.SetPixel(point.X, point.Y, fillColor);

                // Добавляем соседние точки в стек
                stack.Push(new Point(point.X - 1, point.Y)); // Влево
                stack.Push(new Point(point.X + 1, point.Y)); // Вправо
                stack.Push(new Point(point.X, point.Y - 1)); // Вверх
                stack.Push(new Point(point.X, point.Y + 1)); // Вниз
            }
        }

        private bool IsPointInPolygon(PointF[] polygon, Point point)
        {
            // Алгоритм "Ray Casting" для проверки, находится ли точка внутри полигона
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