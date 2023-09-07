namespace LP.Plot.Core.UI;

public class LPMouseButtonEventArgs
{
    public LPMouseButtonEventArgs(double x, double y, LPButton pressedButton)
    {
        PressedButton = pressedButton;
        X = x;
        Y = y;
    }

    public LPButton PressedButton { get; }
    public double X { get; }
    public double Y { get; }
}
