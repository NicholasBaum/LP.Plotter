namespace LP.Plot.Core.UI;

public interface IInteraction
{
    bool HasMouseCapture { get; }
    (double X, double Y) Transform(double x, double y);
    void OnMouseDown(LPMouseButtonEventArgs e);
    void OnMouseMove(LPMouseButtonEventArgs e);
    void OnMouseUp(LPMouseButtonEventArgs e);
    void OnMouseWheel(LPMouseWheelEventArgs e);
}
