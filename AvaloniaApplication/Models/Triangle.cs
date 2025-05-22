using System;
using Avalonia;
using Avalonia.Media;

namespace AvaloniaApplication.Models;

public class Triangle : Shape
{
    private Point A, B, C;
    private double Area;
    public Triangle(int x, int y, Color color, int r) : base(x, y, color, r)
    {
        Area = r * r * 0.25 * 3 * Math.Sqrt(3);
    }

    public override bool IsInside(int x0, int y0)
    {
        Point M = new Point(x0, y0);
        return Math.Abs(Area - TriangleSquare(A, B, M) - TriangleSquare(M, B, C) - TriangleSquare(A, M, C)) <= 0.1;
    }

    public override void Draw(DrawingContext context)
    {
        Brush lineBrush = new SolidColorBrush(color);
        Pen pen = new(lineBrush, 2, lineCap: PenLineCap.Square);
        A = new Point(x, y - r);
        B = new Point(x - r * (float)Math.Sqrt(3) / 2, y + (float)r / 2);
        C = new Point(x + r * (float)Math.Sqrt(3) / 2, y + (float)r / 2);
        context.DrawLine(pen, A, B);
        context.DrawLine(pen, A, C);
        context.DrawLine(pen, B, C);
    }

    private static double TriangleSquare(Point A, Point B, Point C)
    {
        double a = Point.Distance(A, B);
        double b = Point.Distance(B, C);
        double c = Point.Distance(C, A);
        double p = (a + b + c) / 2;
        return Math.Sqrt(p * (p - a) * (p - b) * (p - c));
    }
}