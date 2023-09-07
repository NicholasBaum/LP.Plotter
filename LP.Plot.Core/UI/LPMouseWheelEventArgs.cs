using LP.Plot.Core.Primitives;

namespace LP.Plot.Core.UI;

public class LPMouseWheelEventArgs
{
    public LPMouseWheelEventArgs(double x, double y, double delta) : this(new(x, y), delta) { }
    public LPMouseWheelEventArgs(DPoint point, double delta)
    {
        Delta = delta;
        Point = point;
    }

    public double Delta { get; }
    public DPoint Point { get; }
    public double X => Point.X;
    public double Y => Point.Y;
}
