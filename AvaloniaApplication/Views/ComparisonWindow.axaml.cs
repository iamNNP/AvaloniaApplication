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
    private readonly List<(int Shapes, long Time)> _measurements;
    private readonly string _algorithmName;
    private const int GraphPadding = 30;
    private const int AxisThickness = 2;
    private const int PointRadius = 3;

    public ComparisonWindow(List<(int Shapes, long Time)> measurements, string algorithmName)
    {
        InitializeComponent();
        _measurements = measurements;
        _algorithmName = algorithmName;
        
        this.Opened += (s, e) => 
        {
            DrawGraph();
        };

        this.SizeChanged += (s, e) =>
        {
            DrawGraph();
        };
    }

    private void DrawGraph()
    {
        if (ComparisonCanvas == null || _measurements == null || _measurements.Count == 0)
            return;

        ComparisonCanvas.Children.Clear();

        var bounds = ComparisonCanvas.Bounds;
        if (bounds.Width <= 0 || bounds.Height <= 0)
            return;

        var maxShapes = _measurements.Max(m => m.Shapes);
        var maxTime = _measurements.Max(m => m.Time);
        if (maxTime == 0) maxTime = 1; // Prevent division by zero

        var scaleX = (bounds.Width - 2 * GraphPadding) / maxShapes;
        var scaleY = (bounds.Height - 2 * GraphPadding) / maxTime;

        // Draw Y axis
        var yAxisLine = new Line
        {
            StartPoint = new Point(GraphPadding, bounds.Height - GraphPadding),
            EndPoint = new Point(GraphPadding, GraphPadding),
            Stroke = Brushes.Black,
            StrokeThickness = AxisThickness
        };
        ComparisonCanvas.Children.Add(yAxisLine);

        // Draw X axis
        var xAxisLine = new Line
        {
            StartPoint = new Point(GraphPadding, bounds.Height - GraphPadding),
            EndPoint = new Point(bounds.Width - GraphPadding, bounds.Height - GraphPadding),
            Stroke = Brushes.Black,
            StrokeThickness = AxisThickness
        };
        ComparisonCanvas.Children.Add(xAxisLine);

        DrawGrid(ComparisonCanvas, bounds, maxShapes, maxTime, scaleX, scaleY);
        DrawDataLine(_measurements, ComparisonCanvas, scaleX, scaleY, bounds, Brushes.Blue);

        // Draw title
        var title = new TextBlock
        {
            Text = $"{_algorithmName} Algorithm Performance",
            FontSize = 16,
            FontWeight = FontWeight.Bold
        };
        Canvas.SetLeft(title, (bounds.Width - 200) / 2);
        Canvas.SetTop(title, GraphPadding / 2);
        ComparisonCanvas.Children.Add(title);

        // Draw axis labels
        var xAxisLabel = new TextBlock
        {
            Text = "Number of Shapes",
            FontSize = 12
        };
        Canvas.SetLeft(xAxisLabel, (bounds.Width - 100) / 2);
        Canvas.SetTop(xAxisLabel, bounds.Height - GraphPadding + 25);
        ComparisonCanvas.Children.Add(xAxisLabel);

        var yAxisLabel = new TextBlock
        {
            Text = "Time (ms)",
            FontSize = 12
        };
        var transform = new RotateTransform(-90);
        yAxisLabel.RenderTransform = transform;
        Canvas.SetLeft(yAxisLabel, GraphPadding - 25);
        Canvas.SetTop(yAxisLabel, bounds.Height / 2);
        ComparisonCanvas.Children.Add(yAxisLabel);
    }

    private void DrawGrid(Canvas canvas, Rect bounds, int maxShapes, long maxTime, double scaleX, double scaleY)
    {
        // X axis labels and grid lines
        int xStep = Math.Max(1, maxShapes / 10);
        for (int i = 0; i <= maxShapes; i += xStep)
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
                FontSize = 12
            };
            Canvas.SetLeft(label, x - 15);
            Canvas.SetTop(label, bounds.Height - GraphPadding + 5);
            canvas.Children.Add(label);
        }

        // Y axis labels and grid lines
        int yStep = Math.Max(1, (int)(maxTime / 10));
        for (int i = 0; i <= maxTime; i += yStep)
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
                Text = i.ToString() + "ms",
                FontSize = 12
            };
            Canvas.SetLeft(label, 5);
            Canvas.SetTop(label, y - 10);
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
} 