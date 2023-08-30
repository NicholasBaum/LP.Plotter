using SkiaSharp;

namespace LP.Plot.Core.Skia;

public static class SKFonts
{
    public static readonly SKPaint White = new()
    {
        Color = SKColors.White,
        IsAntialias = true,
        Style = SKPaintStyle.Fill,
        TextAlign = SKTextAlign.Left,
        TextSize = 18,
    };
}
