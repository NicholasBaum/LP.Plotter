using LP.Plot.Primitives;

namespace LP.Plot.UI;

public class DockerControl : ControlBase, IRenderable
{
    private IControl? left;
    private IControl? top;
    private IControl? right;
    private IControl? bottom;
    private IControl? center;

    public IControl? Left
    {
        get => left;
        set
        {
            left = value;
            Arrange();
        }
    }
    public IControl? Top
    {
        get => top;
        set
        {
            top = value;
            Arrange();
        }
    }
    public IControl? Right
    {
        get => right;
        set
        {
            right = value;
            Arrange();
        }
    }
    public IControl? Bottom
    {
        get => bottom;
        set
        {
            bottom = value;
            Arrange();
        }
    }
    public IControl? Center
    {
        get => center;
        set
        {
            center = value;
            Arrange();
        }
    }


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
        if (Rect.IsEmpty)
            return;
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