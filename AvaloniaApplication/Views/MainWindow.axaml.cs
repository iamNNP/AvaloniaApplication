using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Interactivity;

namespace AvaloniaApplication.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    
    private void OnPointerPressed(object sender, PointerPressedEventArgs e)
    {
        CustomControl? customControl = this.Find<CustomControl>("MyCustomControl");
        var point = e.GetCurrentPoint(this);
        if (point.Properties.IsLeftButtonPressed)
        {
            customControl?.Click((int)e.GetPosition(customControl).X, (int)e.GetPosition(customControl).Y);
        }
        else
        {
            customControl?.Delete((int)e.GetPosition(customControl).X, (int)e.GetPosition(customControl).Y);
        }
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        CustomControl? customControl = this.Find<CustomControl>("MyCustomControl");
        customControl?.Move((int)e.GetPosition(customControl).X, (int)e.GetPosition(customControl).Y);
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        CustomControl? customControl = this.Find<CustomControl>("MyCustomControl");
        customControl?.Release((int)e.GetPosition(customControl).X, (int)e.GetPosition(customControl).Y);
    }
    
    private void OnSelectShape(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var customControl = this.Find<CustomControl>("MyCustomControl");
        if (sender is MenuItem menuItem)
        {
            string shape = menuItem.Tag as string;
            customControl?.SetCurrentShapeType(shape);
        }
    }
    
    // private void OnColorChanged(object? sender, ColorChangedEventArgs  e)
    // {
    //     var colorPicker = sender as ColorPicker;
    //     if (colorPicker != null)
    //     {
    //         var selectedColor = colorPicker.Color;
    //         var customControl = this.Find<CustomControl>("MyCustomControl");
    //         customControl?.SetCurrentColor(selectedColor);
    //     }
    // }
}