using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using AvaloniaApplication.Models;

namespace AvaloniaApplication;

public class CustomControl : UserControl
{
    private readonly List<Shape> _shapes =
    [
        // new Circle(100, 100),
        // new Square(200, 100),
        // new Triangle(300, 100),
    ];
    private int _pX, _pY;
    private static Brush _lineBrush = new SolidColorBrush(Colors.Green);
    private static Pen _pen = new(_lineBrush, 2, lineCap: PenLineCap.Square);

    public bool IsNotInConvexHullChain(Shape shape)
    {
        return !shape.InConvexHullChain;
    }
    public void DrawConvexHullSlow(DrawingContext context)
    {
        foreach (var shape in _shapes)
        {
            shape.InConvexHullChain = false;
        }
        int x1, y1, x2, y2;
        double k, b;
        bool upper, lower;
        for (int i = 0; i < _shapes.Count() - 1; i++)
        {
            for (int j = i + 1; j < _shapes.Count(); j++)
            {
                upper = true;
                lower = true;
                x1 = _shapes[i].X;
                y1 = _shapes[i].Y;
                x2 = _shapes[j].X;
                y2 = _shapes[j].Y;
                if (x1 == x2)
                {
                    if (!(_shapes.All(shape => shape.X >= x1) || _shapes.All(shape => shape.X <= x1)))
                    {
                        continue;
                    }
                }
                k = (double)(y2 - y1) / (x2 - x1);
                b = y1 - k * x1;
                for (int t = 0; t < _shapes.Count(); t++)
                {
                    if (t != i && t != j)
                    {
                        if (_shapes[t].Y > k * _shapes[t].X + b)
                        {
                            lower = false;
                        }
                        else if (_shapes[t].Y < k * _shapes[t].X + b)
                        {
                            upper = false;
                        }
                    }
                }

                if (!(lower == false && upper == false))
                {
                    _shapes[i].InConvexHullChain = true;
                    _shapes[j].InConvexHullChain = true;
                    if (context != null)
                    {
                        context.DrawLine(_pen, new Point(x1, y1), new Point(x2, y2));
                    }
                }
            }
        }
    }
    public static int Orientation(Shape p, Shape q, Shape r)
    {
        int val = (q.Y - p.Y) * (r.X - q.X) -
                (q.X - p.X) * (r.Y - q.Y);
     
        if (val == 0) return 0;
        return (val > 0)? 1: 2;
    }
    public void DrawConvexHullFast(DrawingContext context)
    {
        foreach (var shape in _shapes)
        {
            shape.InConvexHullChain = false;
        }
        
        if (_shapes.Count != 0)
        {
            int leftMost = 0;
            for (int i = 1; i < _shapes.Count; i++)
                if (_shapes[i].X < _shapes[leftMost].X)
                    leftMost = i;

            int prev = leftMost, next;
            int lastToDraw = leftMost;
            
            do
            {
                _shapes[prev].InConvexHullChain = true;
                if (context != null)
                {
                    context.DrawLine(_pen, new Point(_shapes[lastToDraw].X, _shapes[lastToDraw].Y), new Point(_shapes[prev].X, _shapes[prev].Y));
                    lastToDraw = prev;
                }
                next = (prev + 1) % _shapes.Count;
            
                for (int i = 0; i < _shapes.Count; i++)
                {
                    if (Orientation(_shapes[prev], _shapes[i], _shapes[next]) == 2)
                    {
                        next = i;   
                    }
                }
                prev = next;
            } while (prev != leftMost);
            if (context != null)
            {
                context.DrawLine(_pen, new Point(_shapes[leftMost].X, _shapes[leftMost].Y), new Point(_shapes[lastToDraw].X, _shapes[lastToDraw].Y));
            }
        }
    }
    public override void Render(DrawingContext context)
    {
        foreach (var shape in _shapes)
        {
            shape.Draw(context);
        }
        Console.WriteLine("Drawing");
        DrawConvexHullFast(context);
    }

    public void Click(int x0, int y0)
    {
        foreach (var shape in _shapes.Where(shape => shape.IsInside(x0, y0)))
        {
            Console.WriteLine("Click");
            shape.IsMoving = true;
        }
        
        if (_shapes.All(shape => !shape.IsInside(x0, y0)))
        {
            Console.WriteLine("Drawing new shape");
            Shape lShape = new Circle(x0, y0);
            _shapes.Add(lShape);
            DrawConvexHullFast(null);
            // DrawConvexHullSlow(null);
            if (lShape.InConvexHullChain == false && _shapes.Count != 1)
            {
                _shapes.Remove(lShape);
                foreach (var shape in _shapes)
                {
                    shape.IsMoving = true;
                }
            }
        }
        _pX = x0;
        _pY = y0;
        InvalidateVisual();
    }

    public void Move(int x0, int y0)
    {
        foreach (var shape in _shapes.Where(shape => shape.IsMoving))
        {
            Console.WriteLine("Move");
            shape.X += x0 - _pX;
            shape.Y += y0 - _pY;
        }

        _pX = x0;
        _pY = y0;
        InvalidateVisual();
    }

    public void Release(int x0, int y0)
    {
        foreach (var shape in _shapes.Where(shape => shape.IsMoving))
        {
            Console.WriteLine("Release");
            shape.X += x0 - _pX;
            shape.Y += y0 - _pY;
            shape.IsMoving = false;
        }

        if (_shapes.Count != 1)
        {
            _shapes.RemoveAll(IsNotInConvexHullChain);
        }
        _pX = x0;
        _pY = y0;
        InvalidateVisual();
    }
    
    public void Delete(int x0, int y0)
    {
        bool isInsideShape = false;
        for (int i = _shapes.Count() - 1; i >= 0; i--)
        {
            if (_shapes[i].IsInside(x0, y0))
            {
                isInsideShape = true;
                _shapes.Remove(_shapes[i]);
                break;
            }
        }

        if (!isInsideShape)
        {
            Console.WriteLine("if1");
            Console.WriteLine("Drawing new shape");
            Shape lShape = new Circle(x0, y0);
            _shapes.Add(lShape);
            DrawConvexHullFast(null);
            // DrawConvexHullSlow(null);
            Console.WriteLine(lShape.InConvexHullChain);
            if (lShape.InConvexHullChain == false)
            {
                // Console.WriteLine("lol");
                _shapes.Clear();
            }   
        }
        InvalidateVisual();
    }
}