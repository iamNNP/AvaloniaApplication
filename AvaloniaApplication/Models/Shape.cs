using Avalonia.Media;

namespace AvaloniaApplication.Models;

public abstract class Shape
{
    protected int x, y;
    protected static int r;
    protected static Color color;

    public bool IsMoving { get; set; }

    public abstract bool IsInside(int nx, int ny);

    protected Shape(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    static Shape()
    {
        color = Colors.Green;
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