using Avalonia.Media;

namespace AvaloniaApplication.Models;

public abstract class Shape
{
    protected int x, y;
    protected int r;
    protected Color color;

    public bool IsMoving { get; set; }
    public bool InConvexHullChain { get; set; }

    public abstract bool IsInside(int nx, int ny);

    protected Shape(int x, int y, Color color, int r)
    {
        this.x = x;
        this.y = y;
        this.color = color;
        this.r = r;
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
    
    public Color Color
    {
        get { return this.color; }
        set { this.color = value; }
    }

    public virtual int R
    {
        get { return this.r; }
        set { this.r = value; }
    }
    public abstract void Draw(DrawingContext context);
}