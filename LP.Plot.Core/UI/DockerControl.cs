using LP.Plot.Core.Primitives;

namespace LP.Plot.Core.UI;

public class DockerControl : ControlBase, IRenderable
{
    public IControl? Left { get; set; }
    public IControl? Top { get; set; }
    public IControl? Right { get; set; }
    public IControl? Bottom { get; set; }
    public IControl? Center { get; set; }


    public override void Render(IRenderContext ctx)
    {
        Left?.Render(ctx);
        Top?.Render(ctx);
        Right?.Render(ctx);
        Bottom?.Render(ctx);
        Center?.Render(ctx);
    }

    public override void SetRect(LPRect rect)
    {
        if (rect == Rect) return;
        Rect = rect;
        Arrange();
    }

    private void Arrange()
    {
        var w = Rect.Width;
        // desired widths
        var lwd = Left == null ? 0 : Math.Max(0, Left.DesiredSize.Width);
        var rwd = Right == null ? 0 : Math.Max(0, Right.DesiredSize.Width);

        // assigned widths
        int lw = 0, rw = 0, cw = 0;
        if (lwd > w)
        {
            lw = w;
        }
        else
        {
            lw = lwd;
            if (w - lw < rwd)
            {
                rw = w - lw;
            }
            else
            {
                rw = rwd;
                cw = w - lw - rw;
            }
        }

        var h = Rect.Height;

        // desired heights
        var thd = Top == null ? 0 : Math.Max(0, Top.DesiredSize.Height);
        var bhd = Bottom == null ? 0 : Math.Max(0, Bottom.DesiredSize.Height);

        // assigned heights
        int th = 0, bh = 0, ch = 0;

        if (cw > 0)
        {
            if (thd > h)
            {
                th = h;
            }
            else
            {
                th = thd;
                if (h - th < bhd)
                {
                    bh = h - th;
                }
                else
                {
                    bh = bhd;
                    ch = h - th - bh;
                }
            }
        }
        Left?.SetRect(LPRect.Create(0, th, lw, ch));
        Right?.SetRect(LPRect.Create(lw + cw, th, rw, ch));
        Top?.SetRect(LPRect.Create(lw, 0, cw, th));
        Bottom?.SetRect(LPRect.Create(lw, th + ch, cw, bh));
        Center?.SetRect(LPRect.Create(lw, th, cw, ch));
    }
}