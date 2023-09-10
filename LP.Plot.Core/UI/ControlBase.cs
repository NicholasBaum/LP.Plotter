using LP.Plot.Primitives;

namespace LP.Plot.UI;

public abstract class ControlBase : IControl
{
    public IControl? Parent { get; set; }

    public LPSize DesiredSize { get; set; }
    public LPRect Rect { get; protected set; }
    public bool HasMouseCapture { get; set; }

    public abstract void Render(IRenderContext ctx);

    public virtual void SetRect(LPRect rect)
    {
        Rect = rect;
    }

    public DPoint Transform(double x, double y)
        => Transform(new(x, y));

    public DPoint Transform(DPoint p)
    {
        var xt = new LinarTransform(Rect.Left, Rect.Right, 0, Rect.Width);
        var yt = new LinarTransform(Rect.Top, Rect.Bottom, 0, Rect.Height);
        return new(xt.Transform(p.X), yt.Transform(p.Y));
    }

    public virtual void OnMouseDown(LPMouseButtonEventArgs e) { }
    public virtual void OnMouseMove(LPMouseButtonEventArgs e) { }
    public virtual void OnMouseUp(LPMouseButtonEventArgs e) { }
    public virtual void OnMouseWheel(LPMouseWheelEventArgs e) { }
}

public class ControlBase<T> : ControlBase where T : IRenderable
{
    public T? Content { get; set; }

    public ControlBase() { }

    public ControlBase(T content)
    {
        Content = content;
    }

    public override void Render(IRenderContext ctx)
    {
        if (Content is null || Rect.IsEmpty) return;
        ctx.ClientRect = Rect;
        Content.Render(ctx);
    }
}


