using Avalonia;
using Avalonia.Media;

namespace AvaloniaApplication.Models;

public class Circle : Shape
{
    public Circle(int x, int y, Color color, int r) : base(x, y, color, r)
    {
    }

    public override bool IsInside(int x0, int y0)
    {
        return (x - x0) * (x - x0) + (y - y0) * (y - y0) <= r * r;
    }

    public override void Draw(DrawingContext context)
    {
        Brush brush = new SolidColorBrush(Colors.White);
        Brush lineBrush = new SolidColorBrush(color);
        Pen pen = new(lineBrush, 2, lineCap: PenLineCap.Square);
        context.DrawEllipse(brush, pen, new Point(x, y), r, r);
    }
    
}