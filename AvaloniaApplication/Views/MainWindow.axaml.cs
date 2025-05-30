using System;
using System.Threading.Tasks;
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
    private RadiusWindow? _radiusWindow;
    private string? _currentFilePath;

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

    private void OnNewClick(object sender, RoutedEventArgs e)
    {
        var customControl = this.Find<CustomControl>("MyCustomControl");
        if (customControl != null)
        {
            customControl.Clear();
            _currentFilePath = null;
        }
    }

    private async void OnSaveClick(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(_currentFilePath))
        {
            await SaveAsFile();
        }
        else
        {
            MyCustomControl.SaveToJson(_currentFilePath);
        }
    }

    private async void OnSaveAsClick(object sender, RoutedEventArgs e)
    {
        await SaveAsFile();
    }

    private async Task SaveAsFile()
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
            _currentFilePath = file.Path.LocalPath;
            MyCustomControl.SaveToJson(_currentFilePath);
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

        if (files != null && files.Count > 0)
        {
            _currentFilePath = files[0].Path.LocalPath;
            MyCustomControl.LoadFromJson(_currentFilePath);
        }
    }

    private void OnRadiusControlClick(object? sender, RoutedEventArgs e)
    {
        if (_radiusWindow == null || !_radiusWindow.IsVisible)
        {
            _radiusWindow?.Close();
            _radiusWindow = new RadiusWindow();
            _radiusWindow.Closed += (s, e) => _radiusWindow = null;
            _radiusWindow.RadiusChanged += OnRadiusChanged;
            _radiusWindow.Show();
        }
        else
        {
            _radiusWindow.Activate();
            _radiusWindow.Focus();
        }
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