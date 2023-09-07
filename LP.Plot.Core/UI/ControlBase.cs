using LP.Plot.Core.Primitives;

namespace LP.Plot.Core.UI;

public class ControlBase<T> : IControl, IInteraction where T : IRenderable
{
    public IControl? Parent { get; set; }
    public T Content { get; set; }

    public LPSize DesiredSize { get; set; }
    public LPRect Rect { get; private set; }
    public bool HasMouseCapture { get; set; }

    public ControlBase(T content)
    {
        Content = content;
    }

    public virtual void Render(IRenderContext ctx)
    {
        if (Content is null || Rect.IsEmpty) return;
        ctx.ClientRect = Rect;
        Content.Render(ctx);
    }

    public void SetRect(LPRect rect)
    {
        Rect = rect;
    }

    public (double X, double Y) Transform(double x, double y)
    {
        var xt = new LPTransform(Rect.Left, Rect.Right, 0, Rect.Width);
        var yt = new LPTransform(Rect.Top, Rect.Bottom, 0, Rect.Height);
        return (xt.Transform(x), yt.Transform(y));
    }

    public virtual void OnMouseDown(LPMouseButtonEventArgs e) { }
    public virtual void OnMouseMove(LPMouseButtonEventArgs e) { }
    public virtual void OnMouseUp(LPMouseButtonEventArgs e) { }
    public virtual void OnMouseWheel(LPMouseWheelEventArgs e) { }
}


