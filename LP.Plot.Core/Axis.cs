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

    public void Pan(double offset)
    {
        Min += offset;
        Max += offset;
    }

    public void ZoomAt(double factor, double position)
    {
        // keep relative position of position
        var newLeftSide = (position - Min) * factor;
        var newRightSide = (Max - position) * factor;
        Min = position - newLeftSide;
        Max = position + newRightSide;
    }

    public void PanRelative(double relativOffset)
    {
        var offset = Length * relativOffset;
        Min += offset;
        Max += offset;
    }

    public void ZoomAtRelative(double factor, double relativePosition)
    {
        var zoomCenter = Min + Length * relativePosition;
        var newLeftSide = (zoomCenter - Min) * factor;
        var newRightSide = (Max - zoomCenter) * factor;
        Min = zoomCenter - newLeftSide;
        Max = zoomCenter + newRightSide;
    }

    public void ZoomAtCenter(double factor)
    {
        ZoomAtRelative(factor, 0.5);
    }
}