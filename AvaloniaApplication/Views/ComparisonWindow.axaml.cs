using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Controls.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Threading;
using System.Reactive.Linq;
using ReactiveUI;

namespace AvaloniaApplication.Views;

public partial class ComparisonWindow : Window
{
    private readonly List<(int Shapes, long FastTime, long SlowTime)> _measurements;
    private const int GraphPadding = 30;
    private const int AxisThickness = 2;
    private const int PointRadius = 3;

    public ComparisonWindow(List<(int Shapes, long FastTime, long SlowTime)> measurements)
    {
        InitializeComponent();
        _measurements = measurements;
        
        this.WhenAnyValue(x => x.Bounds).Subscribe(_ => 
        {
            if (ComparisonCanvas != null)
            {
                Dispatcher.UIThread.Post(DrawGraph);
            }
        });
    }

    private void DrawGraph()
    {
        if (ComparisonCanvas == null || ComparisonCanvas.Bounds.Width <= 0 || ComparisonCanvas.Bounds.Height <= 0 || _measurements.Count == 0)
            return;

        var canvas = ComparisonCanvas;
        canvas.Children.Clear();

        var bounds = canvas.Bounds;
        var maxShapes = _measurements.Max(m => m.Shapes);
        var maxTime = Math.Max(_measurements.Max(m => m.FastTime), _measurements.Max(m => m.SlowTime));
        
        var scaleX = (bounds.Width - 2 * GraphPadding) / maxShapes;
        var scaleY = (bounds.Height - 2 * GraphPadding) / maxTime;

        // Draw axes
        var axisLine = new Line
        {
            StartPoint = new Point(GraphPadding, bounds.Height - GraphPadding),
            EndPoint = new Point(GraphPadding, GraphPadding),
            Stroke = Brushes.Black,
            StrokeThickness = AxisThickness
        };
        canvas.Children.Add(axisLine);

        axisLine = new Line
        {
            StartPoint = new Point(GraphPadding, bounds.Height - GraphPadding),
            EndPoint = new Point(bounds.Width - GraphPadding, bounds.Height - GraphPadding),
            Stroke = Brushes.Black,
            StrokeThickness = AxisThickness
        };
        canvas.Children.Add(axisLine);

        // Draw grid and labels
        DrawGrid(canvas, bounds, maxShapes, maxTime, scaleX, scaleY);

        // Draw data lines
        DrawDataLine(_measurements.Select(m => (m.Shapes, m.FastTime)).ToList(), 
            canvas, scaleX, scaleY, bounds, Brushes.Blue);
        DrawDataLine(_measurements.Select(m => (m.Shapes, m.SlowTime)).ToList(), 
            canvas, scaleX, scaleY, bounds, Brushes.Red);

        // Draw legend
        DrawLegend(canvas, bounds);
    }

    private void DrawGrid(Canvas canvas, Rect bounds, int maxShapes, long maxTime, double scaleX, double scaleY)
    {
        // X-axis grid and labels
        for (int i = 0; i <= maxShapes; i += maxShapes / 10)
        {
            var x = GraphPadding + i * scaleX;
            
            var gridLine = new Line
            {
                StartPoint = new Point(x, bounds.Height - GraphPadding),
                EndPoint = new Point(x, GraphPadding),
                Stroke = Brushes.LightGray,
                StrokeThickness = 1
            };
            canvas.Children.Add(gridLine);

            var label = new TextBlock
            {
                Text = i.ToString(),
                FontSize = 12,
                TextAlignment = TextAlignment.Center
            };
            Canvas.SetLeft(label, x - label.Bounds.Width / 2);
            Canvas.SetTop(label, bounds.Height - GraphPadding + 5);
            canvas.Children.Add(label);
        }

        // Y-axis grid and labels
        for (int i = 0; i <= maxTime; i += (int)(maxTime / 10))
        {
            var y = bounds.Height - (GraphPadding + i * scaleY);
            
            var gridLine = new Line
            {
                StartPoint = new Point(GraphPadding, y),
                EndPoint = new Point(bounds.Width - GraphPadding, y),
                Stroke = Brushes.LightGray,
                StrokeThickness = 1
            };
            canvas.Children.Add(gridLine);

            var label = new TextBlock
            {
                Text = i.ToString(),
                FontSize = 12,
                TextAlignment = TextAlignment.Right
            };
            Canvas.SetLeft(label, GraphPadding - label.Bounds.Width - 5);
            Canvas.SetTop(label, y - label.Bounds.Height / 2);
            canvas.Children.Add(label);
        }
    }

    private void DrawDataLine(List<(int Shapes, long Time)> data, Canvas canvas,
        double scaleX, double scaleY, Rect bounds, IBrush color)
    {
        Point? previousPoint = null;

        foreach (var (shapes, time) in data)
        {
            var x = GraphPadding + shapes * scaleX;
            var y = bounds.Height - (GraphPadding + time * scaleY);
            var currentPoint = new Point(x, y);

            if (previousPoint.HasValue)
            {
                var line = new Line
                {
                    StartPoint = previousPoint.Value,
                    EndPoint = currentPoint,
                    Stroke = color,
                    StrokeThickness = 2
                };
                canvas.Children.Add(line);
            }

            var point = new Ellipse
            {
                Width = PointRadius * 2,
                Height = PointRadius * 2,
                Fill = color
            };
            Canvas.SetLeft(point, x - PointRadius);
            Canvas.SetTop(point, y - PointRadius);
            canvas.Children.Add(point);

            previousPoint = currentPoint;
        }
    }

    private void DrawLegend(Canvas canvas, Rect bounds)
    {
        var legendY = GraphPadding + 10;
        var legendX = bounds.Width - GraphPadding - 150;

        // Fast algorithm legend
        var fastLine = new Line
        {
            StartPoint = new Point(legendX, legendY),
            EndPoint = new Point(legendX + 30, legendY),
            Stroke = Brushes.Blue,
            StrokeThickness = 2
        };
        canvas.Children.Add(fastLine);

        var fastLabel = new TextBlock
        {
            Text = "Fast Algorithm",
            FontSize = 12,
            Foreground = Brushes.Black
        };
        Canvas.SetLeft(fastLabel, legendX + 40);
        Canvas.SetTop(fastLabel, legendY - fastLabel.Bounds.Height / 2);
        canvas.Children.Add(fastLabel);

        // Slow algorithm legend
        legendY += 20;
        var slowLine = new Line
        {
            StartPoint = new Point(legendX, legendY),
            EndPoint = new Point(legendX + 30, legendY),
            Stroke = Brushes.Red,
            StrokeThickness = 2
        };
        canvas.Children.Add(slowLine);

        var slowLabel = new TextBlock
        {
            Text = "Slow Algorithm",
            FontSize = 12,
            Foreground = Brushes.Black
        };
        Canvas.SetLeft(slowLabel, legendX + 40);
        Canvas.SetTop(slowLabel, legendY - slowLabel.Bounds.Height / 2);
        canvas.Children.Add(slowLabel);
    }
} 