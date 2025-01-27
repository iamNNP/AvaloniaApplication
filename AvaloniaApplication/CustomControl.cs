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
        new Circle(100, 100),
        new Square(200, 100),
        new Triangle(300, 100),
    ];
    private int _pX, _pY;
    private static Brush _lineBrush = new SolidColorBrush(Colors.Green);
    private static Pen _pen = new(_lineBrush, 2, lineCap: PenLineCap.Square);

    public override void Render(DrawingContext context)
    {
        foreach (var shape in _shapes)
        {
            shape.Draw(context);
        }
        Console.WriteLine("Drawing");
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
                Console.WriteLine($"i: {x1} {y1} j: {x2} {y2}");
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
                    context.DrawLine(_pen, new Point(x1, y1), new Point(x2, y2));
                }
            }
        }
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
            _shapes.Add(new Circle(x0, y0));
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

        _pX = x0;
        _pY = y0;
        InvalidateVisual();
    }
    
    public void Delete(int x0, int y0)
    {
        for (int i = _shapes.Count() - 1; i >= 0; i--)
        {
            if (_shapes[i].IsInside(x0, y0))
            {
                _shapes.Remove(_shapes[i]);
                break;
            }
        }
        
        InvalidateVisual();
    }
}