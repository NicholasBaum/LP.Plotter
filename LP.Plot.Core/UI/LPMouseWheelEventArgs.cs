namespace LP.Plot.Core.UI;

public class LPMouseWheelEventArgs
{
    public LPMouseWheelEventArgs(double x, double y, double delta)
    {
        Delta = delta;
        X = x;
        Y = y;
    }

    public double Delta { get; }
    public double X { get; }
    public double Y { get; }
}
