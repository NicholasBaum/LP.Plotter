using LP.Plot.Core.Primitives;
using LP.Plot.Skia;
using SkiaSharp;

namespace LP.Plot.Core.Layout;

internal class BorderControl : IControl
{
    public IControl? Parent { get; set; }
    public LPSize DesiredSize { get; set; }
    public LPRect Rect { get; private set; }

    public SKPaint Paint { get; set; } = SKPaints.White;
    public bool ShowLeft { get; set; } = true;
    public bool ShowTop { get; set; } = true;
    public bool ShowRight { get; set; } = true;
    public bool ShowBottom { get; set; } = true;

    public void Render(IRenderContext ctx)
    {
        if (Rect.IsEmpty) return;
        var t = (int)Math.Round(Paint.StrokeWidth);
        var t2 = (int)Math.Round(Paint.StrokeWidth / 2);
        if (ShowTop)
            ctx.Canvas.DrawLine(Rect.Left - t, t2, Rect.Right + t, t2, Paint);
        if (ShowBottom)
            ctx.Canvas.DrawLine(Rect.Left - t, Rect.Bottom - t2, Rect.Right + t, Rect.Bottom - t2, Paint);
        if (ShowLeft)
            ctx.Canvas.DrawLine(Rect.Left - t2, t2, Rect.Left - t2, Rect.Bottom - t2, Paint);
        if (ShowRight)
            ctx.Canvas.DrawLine(Rect.Right + t2, t2, Rect.Right + t2, Rect.Bottom - t2, Paint);
    }

    public void SetRect(LPRect rect)
        => Rect = rect;
}
