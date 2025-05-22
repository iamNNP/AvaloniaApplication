using System;

namespace AvaloniaApplication;

public delegate void RadiusDelegate(object sender, RadiusEventArgs e);

public class RadiusEventArgs : EventArgs
{
    public int Radius { get; }

    public RadiusEventArgs(int radius)
    {
        Radius = radius;
    }
}