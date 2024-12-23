using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Media;
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

    public override void Render(DrawingContext context)
    {
        foreach (var shape in _shapes)
        {
            shape.Draw(context);
        }

        Console.WriteLine("Drawing");
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
        foreach (var shape in _shapes.Where(shape => shape.IsInside(x0, y0)).ToList())
        {
            _shapes.Remove(shape);
        }

        InvalidateVisual();
    }
}