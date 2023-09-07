namespace LP.Plot.Core.UI;

public interface IInteraction
{
    public bool HasMouseCapture { get; }
    public (double X, double Y) Transform(double x, double y);
    void OnMouseWheel(LPMouseWheelEventArgs e);
}
