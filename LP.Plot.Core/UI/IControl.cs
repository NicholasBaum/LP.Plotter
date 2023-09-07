using LP.Plot.Core.Primitives;

namespace LP.Plot.Core.UI;

public interface IControl : IRenderable
{
    public IControl? Parent { get; }
    public LPSize DesiredSize { get; }
    public LPRect Rect { get; }
    public void SetRect(LPRect rect);
    bool HasMouseCapture { get; }
    (double X, double Y) Transform(double x, double y);
    void OnMouseDown(LPMouseButtonEventArgs e);
    void OnMouseMove(LPMouseButtonEventArgs e);
    void OnMouseUp(LPMouseButtonEventArgs e);
    void OnMouseWheel(LPMouseWheelEventArgs e);
}