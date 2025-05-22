using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Controls.Primitives;

namespace AvaloniaApplication.Views;

public partial class RadiusWindow : Window
{
    public event RadiusDelegate? RadiusChanged;

    public RadiusWindow()
    {
        InitializeComponent();
    }

    private void OnRadiusChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        RadiusChanged?.Invoke(this, new RadiusEventArgs((int)e.NewValue));
    }
} 