using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Tsp.Ui
{
    public interface IUiElementsBuilder
    {
        Ellipse GetEllipse();
        TextBlock GetTextBlock(string text);
        Line GetLine(Point point1, Point point2);
        Path GetPath(Point point1, Point point2);
    }

    public class UiElementsBuilder : IUiElementsBuilder
    {
        public const string EllipseUid = "Ellipse";
        public const string TextBlockUid = "TextBlock";
        public const string LineUid = "Line";

        public static readonly string[] UserElementUids = { EllipseUid, TextBlockUid, LineUid };

        public Ellipse GetEllipse()
        {
            return new Ellipse
            {
                Uid = EllipseUid,
                Height = 10,
                Width = 10,
                Fill = new RadialGradientBrush
                {
                    GradientStops = new GradientStopCollection
                    {
                        new GradientStop(Colors.Red, 0.250),
                        new GradientStop(Colors.Red, 0.100),
                        new GradientStop(Colors.Red, 8),
                    }
                }
            };
        }
        public TextBlock GetTextBlock(string text)
        {
            return new TextBlock
            {
                Uid = TextBlockUid,
                Text = text,
                Foreground = new SolidColorBrush(Colors.Black),
            };
        }

        public Line GetLine(Point point1, Point point2)
        {
            return new Line
            {
                Uid = LineUid,
                Visibility = Visibility.Visible,
                StrokeThickness = 1,
                Stroke = Brushes.Black,
                X1 = point1.X,
                Y1 = point1.Y,
                X2 = point2.X,
                Y2 = point2.Y,
            };
        }

        public Path GetPath(Point point1, Point point2)
        {
            var startPoint = new Point(point1.X + ((point2.X - point1.X) / 1.35), point1.Y + ((point2.Y - point1.Y) / 1.35));

            return new Path
            {
                Uid = "Arrow",
                StrokeThickness = 2,
                Fill = Brushes.DarkViolet,
                Stroke = Brushes.DarkViolet,
                Data = new GeometryGroup
                {
                    Children = new GeometryCollection
                    {
                        new PathGeometry
                        {
                            Figures = new PathFigureCollection
                            {
                                new PathFigure
                                {
                                    StartPoint = startPoint,
                                    Segments = new PathSegmentCollection
                                    {
                                        new LineSegment {Point = new Point(startPoint.X + 6, startPoint.Y + 15)},
                                        new LineSegment {Point = new Point(startPoint.X - 6, startPoint.Y + 15)},
                                        new LineSegment {Point = startPoint},
                                    },
                                },
                            },
                            Transform = new RotateTransform
                            {
                                Angle = Math.Atan2(point2.Y - point1.Y, point2.X - point1.X) * 180 / Math.PI + 90,
                                CenterX = startPoint.X,
                                CenterY = startPoint.Y,
                            },
                        },
                        new LineGeometry
                        {
                            StartPoint = point1,
                            EndPoint = point2,
                        },
                    }
                },
            };
        }
    }
}