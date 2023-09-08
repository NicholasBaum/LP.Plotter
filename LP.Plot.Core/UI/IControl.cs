using LP.Plot.Primitives;

namespace LP.Plot.UI;

public interface IControl : IRenderable
{
    public IControl? Parent { get; }
    public LPSize DesiredSize { get; }
    public LPRect Rect { get; }
    public void SetRect(LPRect rect);
    bool HasMouseCapture { get; }
    DPoint Transform(double x, double y);
    DPoint Transform(DPoint p);
    void OnMouseDown(LPMouseButtonEventArgs e);
    void OnMouseMove(LPMouseButtonEventArgs e);
    void OnMouseUp(LPMouseButtonEventArgs e);
    void OnMouseWheel(LPMouseWheelEventArgs e);
}