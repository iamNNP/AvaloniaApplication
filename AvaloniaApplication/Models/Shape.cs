using Avalonia.Media;

namespace AvaloniaApplication.Models;

public abstract class Shape
{
    protected int x, y;
    protected static readonly int r;
    protected Color color;

    public bool IsMoving { get; set; }

    public abstract bool IsInside(int nx, int ny);

    protected Shape(int x, int y, Color color)
    {
        this.x = x;
        this.y = y;
        this.color = color;
    }

    static Shape()
    {
        r = 35;
    }

    public int X
    {
        get { return this.x; }
        set { this.x = value; }
    }

    public int Y
    {
        get { return this.y; }
        set { this.y = value; }
    }

    public abstract void Draw(DrawingContext context);
}