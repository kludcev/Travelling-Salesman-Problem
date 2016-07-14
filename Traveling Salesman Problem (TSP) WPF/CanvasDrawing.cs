using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Traveling_Salesman_Problem__TSP__WPF
{
    public static class CanvasDrawing
    {
        public static void ClearCanvas(string uiName, Canvas canvas)
        {
            List<UIElement> itemstoremove = canvas.Children.Cast<UIElement>().Where(ui => ui.Uid.StartsWith(uiName)).ToList();
            foreach (UIElement ui in itemstoremove)
            {
                canvas.Children.Remove(ui);
            }
        }

        public static int CountUiElements(string uiName, Canvas canvas)
        {
            List<UIElement> list = canvas.Children.Cast<UIElement>().Where(ui => ui.Uid.StartsWith(uiName)).ToList();
            return list.Count;

        }

        public static void DrawEllipse(Point p, Canvas canvas)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Uid = "Ellipse";
            ellipse.Height = 10;
            ellipse.Width = 10;
            RadialGradientBrush brush = new RadialGradientBrush();

            brush.GradientStops.Add(new GradientStop(Colors.Red, 0.250));
            brush.GradientStops.Add(new GradientStop(Colors.Red, 0.100));
            brush.GradientStops.Add(new GradientStop(Colors.Red, 8));
            ellipse.Fill = brush;
            Canvas.SetLeft(ellipse, p.X);
            Canvas.SetTop(ellipse, p.Y);

            canvas.Children.Add(ellipse);
        }
        public static void DrawNumber(Point p, string text, Canvas canvas)
        {
            Color color = Colors.Black;

            TextBlock textBlock = new TextBlock();

            textBlock.Uid = "TextBlock";

            textBlock.Text = text;

            textBlock.Foreground = new SolidColorBrush(color);

            Canvas.SetLeft(textBlock, p.X+6);

            Canvas.SetTop(textBlock, p.Y+5);

            canvas.Children.Add(textBlock);
        }

        public static void DrawLine(Point p1, Point p2, Canvas canvas)
        {
            Line line = new Line();
            line.Uid = "Line";
            line.Visibility = System.Windows.Visibility.Visible;
            line.StrokeThickness = 1;
            line.Stroke = System.Windows.Media.Brushes.Black;
            line.X1 = p1.X;
            line.Y1 = p1.Y;
            line.X2 = p2.X;
            line.Y2 = p2.Y;
            canvas.Children.Add(line);
        }
        public static void DrawLinkArrow(Point p1, Point p2, Canvas canvas)
        {
            GeometryGroup lineGroup = new GeometryGroup();
            double theta = Math.Atan2((p2.Y - p1.Y), (p2.X - p1.X)) * 180 / Math.PI;

            PathGeometry pathGeometry = new PathGeometry();
            PathFigure pathFigure = new PathFigure();
            Point p = new Point(p1.X + ((p2.X - p1.X) / 1.35), p1.Y + ((p2.Y - p1.Y) / 1.35));
            pathFigure.StartPoint = p;

            Point lpoint = new Point(p.X + 6, p.Y + 15);
            Point rpoint = new Point(p.X - 6, p.Y + 15);
            LineSegment seg1 = new LineSegment();
            seg1.Point = lpoint;
            pathFigure.Segments.Add(seg1);

            LineSegment seg2 = new LineSegment();
            seg2.Point = rpoint;
            pathFigure.Segments.Add(seg2);

            LineSegment seg3 = new LineSegment();
            seg3.Point = p;
            pathFigure.Segments.Add(seg3);

            pathGeometry.Figures.Add(pathFigure);
            RotateTransform transform = new RotateTransform();
            transform.Angle = theta + 90;
            transform.CenterX = p.X;
            transform.CenterY = p.Y;
            pathGeometry.Transform = transform;
            lineGroup.Children.Add(pathGeometry);

            LineGeometry connectorGeometry = new LineGeometry();
            connectorGeometry.StartPoint = p1;
            connectorGeometry.EndPoint = p2;
            lineGroup.Children.Add(connectorGeometry);
            System.Windows.Shapes.Path path = new System.Windows.Shapes.Path();
            path.Data = lineGroup;
            path.StrokeThickness = 2;
            path.Stroke = path.Fill = Brushes.DarkViolet;
            path.Uid = "Arrow";

            canvas.Children.Add(path);
            
        }
        public static void DrawCanvasLayout(Canvas canvas)
        {
            CanvasDrawing.ClearCanvas("Line", canvas);
            CanvasDrawing.ClearCanvas("TextBlock", canvas);
            CanvasDrawing.ClearCanvas("Ellipse", canvas);
            CanvasDrawing.ClearCanvas("Arrow", canvas);

            double h = Math.Truncate(canvas.ActualHeight / 100);
            int heigth = (int)h;
            double w = Math.Truncate(canvas.ActualWidth / 100);
            int width = (int)w;

            for (int i = 0; i <= heigth; i++)
            {
                Point p1 = new Point(0, i * 100);
                Point p2 = new Point(canvas.ActualWidth, i * 100);
                CanvasDrawing.DrawLine(p1, p2, canvas);
            }

            for (int j = 0; j <= width; j++)
            {
                var p1 = new Point(j * 100, canvas.ActualHeight);
                var p2 = new Point(j * 100, 0);
                CanvasDrawing.DrawLine(p1, p2, canvas);
            }
            CanvasDrawing.DrawNumber(new Point(100, 100), "(100;100)", canvas);

        }
    }

}