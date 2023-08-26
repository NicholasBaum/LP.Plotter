using LP.Plot.Core.Primitives;

namespace LP.Plot.Core;

public class Axis
{
    public Axis() { }
    public Axis(Span range) => (Min, Max) = (range.Min, range.Max);
    public Span Range => new Span(Min, Max);
    public double Min { get; set; } = float.MaxValue;
    public double Max { get; set; } = float.MinValue;
    public double Length => Max - Min;

    public void Pan(double panx)
    {
        var offset = Length * panx;
        Min += offset;
        Max += offset;

    }

    public Axis ZoomAt(double factor, double relativeZoomCenter)
    {        
        var zoomCenter = Min + Length * relativeZoomCenter;
        var newLeftSide = (zoomCenter - Min)*factor;
        var newRightSide = (Max-zoomCenter ) * factor;
        Min = zoomCenter - newLeftSide;
        Max = zoomCenter + newRightSide;
        return this;
    }


    public Axis Scale(double factor)
    {
        var newRadius = (Length * factor) / 2;
        var mid = (Min + Max) / 2;
        Min = mid - newRadius;
        Max = mid + newRadius;
        return this;
    }
}