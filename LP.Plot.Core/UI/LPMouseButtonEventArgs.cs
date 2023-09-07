using LP.Plot.Core.Primitives;

namespace LP.Plot.Core.UI;

public class LPMouseButtonEventArgs
{
    public LPMouseButtonEventArgs(double x, double y, LPButton pressedButton) : this(new(x, y), pressedButton) { }
    public LPMouseButtonEventArgs(DPoint point, LPButton pressedButton)
    {
        PressedButton = pressedButton;
        Point = point;
    }

    public LPButton PressedButton { get; }
    public DPoint Point { get; }
    public double X => Point.X;
    public double Y => Point.Y;
}
