using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.IO;
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
    private string _shape = "circle";
    private Color _color;
    private Brush _lineBrush;
    private Pen _pen;
    private int _radius = 50;
    private string _algo = "Fast";

    public CustomControl()
    {
        _color = Colors.Yellow;
        _lineBrush = new SolidColorBrush(_color);
        _pen = new(_lineBrush, 3, lineCap: PenLineCap.Square);
    }

    public bool IsNotInConvexHullChain(Shape shape)
    {
        return !shape.InConvexHullChain;
    }
    
    public void DrawConvexHullSlow(DrawingContext? context)
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
    
    public void DrawConvexHullFast(DrawingContext? context)
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
        if (_algo == "Fast")
        {
            DrawConvexHullFast(context);
        }
        else
        {
            DrawConvexHullSlow(context);
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
            Shape lShape = new Circle(x0, y0, _color, _radius);
            if (_shape == "square")
            {
                lShape = new Square(x0, y0, _color, _radius);
            }

            if (_shape == "triangle")
            {
                lShape = new Triangle(x0, y0, _color, _radius);
            }

            _shapes.Add(lShape);
            if (_algo == "Fast")
            {
                DrawConvexHullFast(null);
            }
            else
            {
                DrawConvexHullSlow(null);
            }
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
            Shape lShape = new Circle(x0, y0, _color, _radius);
            _shapes.Add(lShape);
            if (_algo == "Fast")
            {
                DrawConvexHullFast(null);
            }
            else
            {
                DrawConvexHullSlow(null);
            }
            Console.WriteLine(lShape.InConvexHullChain);
            if (lShape.InConvexHullChain == false)
            {
                _shapes.Clear();
            }   
        }
        InvalidateVisual();
    }

    public void SetShapeType(string shape)
    {
        _shape = shape;
    }

    public void SetColor(Color color)
    {
        _color = color;
        _lineBrush = new SolidColorBrush(color);
        _pen = new(_lineBrush, 3, lineCap: PenLineCap.Square);
    }

    public void ShowComparison(int customControlWidth, int customControlHeight)
    {
        var measurements = new List<(int Shapes, long FastTime, long SlowTime)>();
        var random = new Random();

        for (int numShapes = 10; numShapes <= 300; numShapes += 10)
        {
            _shapes.Clear();
            for (int i = 0; i < numShapes; i++)
            {
                int x = random.Next(_radius, customControlWidth - _radius);
                int y = random.Next(_radius, customControlHeight - _radius);
                
                int shapeType = random.Next(3);
                Shape shape = shapeType switch
                {
                    0 => new Circle(x, y, _color, _radius),
                    1 => new Square(x, y, _color, _radius),
                    2 => new Triangle(x, y, _color, _radius),
                    _ => new Circle(x, y, _color, _radius)
                };
                _shapes.Add(shape);
            }

            var fastStopwatch = System.Diagnostics.Stopwatch.StartNew();
            DrawConvexHullFast(null);
            fastStopwatch.Stop();
            var fastTime = fastStopwatch.ElapsedMilliseconds;
            
            var slowStopwatch = System.Diagnostics.Stopwatch.StartNew();
            DrawConvexHullSlow(null);
            slowStopwatch.Stop();
            var slowTime = slowStopwatch.ElapsedMilliseconds;

            measurements.Add((numShapes, fastTime, slowTime));
        }

        var comparisonWindow = new Views.ComparisonWindow(measurements);
        comparisonWindow.Show();
        _shapes.Clear();
    }

    public void SaveToJson(string filePath)
    {
        var shapesData = _shapes.Select(shape => new
        {
            Type = shape.GetType().Name,
            X = shape.X,
            Y = shape.Y,
            Color = shape.Color.ToString(),
            Radius = shape.R
        }).ToList();

        var jsonString = JsonSerializer.Serialize(shapesData, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        File.WriteAllText(filePath, jsonString);
    }

    public void LoadFromJson(string filePath)
    {
        if (!File.Exists(filePath)) return;

        var jsonString = File.ReadAllText(filePath);
        var shapesData = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(jsonString);
        
        if (shapesData == null) return;
        
        _shapes.Clear();
        foreach (var shapeData in shapesData)
        {
            var type = shapeData["Type"].GetString();
            var x = shapeData["X"].GetInt32();
            var y = shapeData["Y"].GetInt32();
            var colorStr = shapeData["Color"].GetString();
            if (_shapes.Count == 0)
            {
                _radius = shapeData.ContainsKey("Radius") ? shapeData["Radius"].GetInt32() : _radius;
            }

            if (!Color.TryParse(colorStr, out var color))
            {
                color = Colors.Yellow;
            }

            Shape shape = type switch
            {
                "Circle" => new Circle(x, y, color, _radius),
                "Square" => new Square(x, y, color, _radius),
                "Triangle" => new Triangle(x, y, color, _radius),
                _ => new Circle(x, y, color, _radius)
            };
            _shapes.Add(shape);
        }
        InvalidateVisual();
    }

    public void SetRadius(int radius)
    {
        _radius = radius;
        foreach (var shape in _shapes)
        {
            shape.R = radius;
        }
        InvalidateVisual();
    }
    
    public void SetAlgo(string algo)
    {
        _algo = algo;
    }
}