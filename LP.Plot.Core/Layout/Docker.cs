using LP.Plot.Core.Primitives;

namespace LP.Plot.Core.Layout;

public interface IControl : IRenderable
{
    public IControl? Parent { get; }
    public LPSize DesiredSize { get; }
    public LPRect Rect { get; }
    public void SetRect(LPRect rect);
}

public class Docker : IControl, IRenderable
{
    public IControl? Parent { get; set; }
    public IControl? Left { get; set; }
    public IControl? Top { get; set; }
    public IControl? Right { get; set; }
    public IControl? Bottom { get; set; }
    public IControl? Center { get; set; }
    public LPSize DesiredSize { get; set; }
    public LPRect Rect { get; private set; }

    private Docker() { }

    public void Render(IRenderContext ctx)
    {
        Left?.Render(ctx);
        Top?.Render(ctx);
        Right?.Render(ctx);
        Bottom?.Render(ctx);
        Center?.Render(ctx);
    }

    public void SetRect(LPRect rect)
    {
        if (rect == this.Rect) return;
        this.Rect = rect;
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

    public static Docker CreateDefault(IRenderable left, IRenderable bottom, IRenderable center)
    {
        var grid = new Docker();
        grid.Left = new Cell() { Parent = grid, Content = left, DesiredSize = new LPSize(50, 0) };
        grid.Bottom = new Cell() { Parent = grid, Content = bottom, DesiredSize = new LPSize(0, 150) };
        grid.Center = new Cell() { Parent = grid, Content = center };
        return grid;
    }
}

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