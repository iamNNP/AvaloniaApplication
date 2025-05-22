using System;
using Avalonia;
using Avalonia.Media;

namespace AvaloniaApplication.Models;

public class Square : Shape
{
    private Point A, B, C, D;
    private float r1;
    public Square(int x, int y, Color color, int r) : base(x, y, color, r)
    {
        r1 = r / (float)Math.Sqrt(2);
    }

    public override bool IsInside(int x0, int y0)
    {
        return x - r1 <= x0 && x0 <= x + r1 && y - r1 <= y0 && y0 <= y + r1;
    }

    public override void Draw(DrawingContext context)
    {
        Brush lineBrush = new SolidColorBrush(color);
        Pen pen = new(lineBrush, 2, lineCap: PenLineCap.Square);

        A = new Point(x - r1, y + r1);
        B = new Point(x + r1, y + r1);
        C = new Point(x + r1, y - r1);
        D = new Point(x - r1, y - r1);
        context.DrawLine(pen, A, B);
        context.DrawLine(pen, B, C);
        context.DrawLine(pen, C, D);
        context.DrawLine(pen, D, A);
    }
}