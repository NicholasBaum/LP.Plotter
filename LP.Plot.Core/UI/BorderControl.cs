using LP.Plot.Skia;
using SkiaSharp;

namespace LP.Plot.Core.UI;

internal class BorderControl : ControlBase
{
    public SKPaint Paint { get; set; } = SKPaints.White;
    public bool ShowLeft { get; set; } = true;
    public bool ShowTop { get; set; } = true;
    public bool ShowRight { get; set; } = true;
    public bool ShowBottom { get; set; } = true;

    public override void Render(IRenderContext ctx)
    {
        if (Rect.IsEmpty) return;
        var t = (int)Math.Round(Paint.StrokeWidth);
        var t2 = (int)Math.Round(Paint.StrokeWidth / 2);

        if (ShowTop)
            ctx.Canvas.DrawLine(Rect.Left - t, Rect.Top + t2, Rect.Right + t, Rect.Top + t2, Paint);
        if (ShowBottom)
            ctx.Canvas.DrawLine(Rect.Left - t, Rect.Bottom - t2, Rect.Right + t, Rect.Bottom - t2, Paint);
        if (ShowLeft)
            ctx.Canvas.DrawLine(Rect.Left + t2, Rect.Top, Rect.Left + t2, Rect.Bottom, Paint);
        if (ShowRight)
            ctx.Canvas.DrawLine(Rect.Right - t2, Rect.Top, Rect.Right - t2, Rect.Bottom, Paint);
    }
}
