using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Avalonia.Platform.Storage;
using static Avalonia.Visual;
using ColorPicker.Models;
using AvaloniaApplication.ViewModels;

namespace AvaloniaApplication.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    
    private void OnPointerPressed(object sender, PointerPressedEventArgs e)
    {
        var customControl = this.Find<CustomControl>("MyCustomControl");
        if (customControl == null) return;

        var point = e.GetCurrentPoint(this);
        
        var controlPoint = e.GetPosition(customControl);
        
        if (controlPoint.X >= 0 && controlPoint.Y >= 0 && 
            controlPoint.X <= customControl.Bounds.Width && 
            controlPoint.Y <= customControl.Bounds.Height)
        {
            if (point.Properties.IsLeftButtonPressed)
            {
                customControl.Click((int)controlPoint.X, (int)controlPoint.Y);
            }
            else
            {
                customControl.Delete((int)controlPoint.X, (int)controlPoint.Y);
            }
            e.Handled = true;
        }
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        var customControl = this.Find<CustomControl>("MyCustomControl");
        if (customControl == null) return;

        var controlPoint = e.GetPosition(customControl);
        
        if (controlPoint.X >= 0 && controlPoint.Y >= 0 && 
            controlPoint.X <= customControl.Bounds.Width && 
            controlPoint.Y <= customControl.Bounds.Height)
        {
            customControl.Move((int)controlPoint.X, (int)controlPoint.Y);
            e.Handled = true;
        }
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        var customControl = this.Find<CustomControl>("MyCustomControl");
        if (customControl == null) return;

        var controlPoint = e.GetPosition(customControl);
        
        if (controlPoint.X >= 0 && controlPoint.Y >= 0 && 
            controlPoint.X <= customControl.Bounds.Width && 
            controlPoint.Y <= customControl.Bounds.Height)
        {
            customControl.Release((int)controlPoint.X, (int)controlPoint.Y);
            e.Handled = true;
        }
    }
    
    private void OnSelectShape(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var customControl = this.Find<CustomControl>("MyCustomControl");
        if (sender is MenuItem menuItem && customControl != null && menuItem.Tag is string shape)
        {
            customControl.SetShapeType(shape);
        }
    }
    
    private void OnColorChanged(object? sender, ColorChangedEventArgs e)
    {
        var customControl = this.Find<CustomControl>("MyCustomControl");
        if (customControl != null)
        {
            customControl.SetColor(e.NewColor);
        }
    }

    private void OnShowComparison(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var customControl = this.Find<CustomControl>("MyCustomControl");
        if (sender is Button && customControl != null)
        {
            customControl.ShowComparison((int)customControl.Bounds.Width, (int)customControl.Bounds.Height);
        }
    }

    private async void OnSaveClick(object sender, RoutedEventArgs e)
    {
        var storageProvider = StorageProvider;
        var options = new FilePickerSaveOptions
        {
            DefaultExtension = "json",
            SuggestedFileName = "shapes.json",
            FileTypeChoices = new[]
            {
                new FilePickerFileType("JSON files")
                {
                    Patterns = new[] { "*.json" }
                }
            }
        };

        var file = await storageProvider.SaveFilePickerAsync(options);

        if (file != null)
        {
            MyCustomControl.SaveToJson(file.Path.LocalPath);
        }
    }

    private async void OnLoadClick(object sender, RoutedEventArgs e)
    {
        var storageProvider = StorageProvider;
        var options = new FilePickerOpenOptions
        {
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("JSON files")
                {
                    Patterns = new[] { "*.json" }
                }
            }
        };

        var files = await storageProvider.OpenFilePickerAsync(options);

        if (files != null)
        {
            MyCustomControl.LoadFromJson(files[0].Path.LocalPath);
        }
    }

    private void OnRadiusControlClick(object? sender, RoutedEventArgs e)
    {
        var radiusWindow = new RadiusWindow();
        radiusWindow.RadiusChanged += OnRadiusChanged;
        radiusWindow.Show();
    }

    private void OnRadiusChanged(object sender, RadiusEventArgs e)
    {
        var customControl = this.Find<CustomControl>("MyCustomControl");
        if (customControl != null)
        {
            customControl.SetRadius(e.Radius);
        }
    }
    
    private void OnSelectAlgo(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var customControl = this.Find<CustomControl>("MyCustomControl");
        if (sender is MenuItem menuItem && customControl != null && menuItem.Tag is string algo)
        {
            customControl.SetAlgo(algo);
        }
    }

}