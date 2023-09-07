using LP.Plot.Core.Primitives;

namespace LP.Plot.Core.Layout;

public class Cell : IControl
{
    public IControl? Parent { get; set; }
    public IRenderable? Content { get; set; }
    public LPSize DesiredSize { get; set; }
    public LPRect Rect { get; private set; }

    public void Render(IRenderContext ctx)
    {
        if (Content is null || Rect.IsEmpty) return;
        ctx.ClientRect = Rect;
        this.Content.Render(ctx);
    }

    public void SetRect(LPRect rect)
    {
        this.Rect = rect;
    }
}


