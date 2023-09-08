using LP.Plot.Core.Primitives;
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

    internal static BorderControl CreateLeft(DockerControl layout, int width)
    {
        return new BorderControl() { Parent = layout, DesiredSize = new LPSize(width, 0), ShowTop = false, ShowRight = false, ShowBottom = false };
    }

    internal static BorderControl CreateTop(DockerControl layout, int height)
    {
        return new BorderControl() { Parent = layout, DesiredSize = new LPSize(0, height), ShowLeft = false, ShowRight = false, ShowBottom = false }; ;
    }

    internal static BorderControl CreateRight(DockerControl layout, int width)
    {
        return new BorderControl() { Parent = layout, DesiredSize = new LPSize(width, 0), ShowLeft = false, ShowTop = false, ShowBottom = false };
    }

    internal static BorderControl CreateBottom(DockerControl layout, int height)
    {
        return new BorderControl() { Parent = layout, DesiredSize = new LPSize(0, height), ShowLeft = false, ShowTop = false, ShowRight = false };
    }

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
