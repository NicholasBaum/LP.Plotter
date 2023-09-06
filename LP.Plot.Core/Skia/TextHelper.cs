using SkiaSharp;

namespace LP.Plot.Core.Skia;

public enum HorizontalAnchor
{
    Left, Center, Right
}

public enum VerticalAnchor
{
    Top, Center, Bottom
}

public static class TextHelper
{

    public static void DrawText(this SKCanvas canvas, string text, float x, float y, SKPaint paint, HorizontalAnchor horizontalAnchor, VerticalAnchor verticalAnchor)
    {
        SKRect rect = new();
        paint.MeasureText(text, ref rect);
        x = horizontalAnchor switch
        {
            HorizontalAnchor.Right => x - rect.Width,
            HorizontalAnchor.Left => x,
            HorizontalAnchor.Center => x - rect.Width / 2,
            _ => throw new NotImplementedException()
        };
        y = verticalAnchor switch
        {
            VerticalAnchor.Center => y + rect.Height / 2,
            VerticalAnchor.Top => y + rect.Height,
            VerticalAnchor.Bottom => y,
            _ => throw new NotImplementedException()
        };

        canvas.DrawText(text, x, y, paint);
    }

    public static void DrawTextCenterTop(this SKCanvas canvas, string text, float x, float y, SKPaint paint)
        => canvas.DrawText(text, x, y, paint, HorizontalAnchor.Center, VerticalAnchor.Top);

    public static void DrawTextCenterBottom(this SKCanvas canvas, string text, float x, float y, SKPaint paint)
        => canvas.DrawText(text, x, y, paint, HorizontalAnchor.Center, VerticalAnchor.Bottom);

    public static void DrawTextLeftCenter(this SKCanvas canvas, string text, float x, float y, SKPaint paint)
        => canvas.DrawText(text, x, y, paint, HorizontalAnchor.Left, VerticalAnchor.Center);

    public static void DrawTextRightCenter(this SKCanvas canvas, string text, float x, float y, SKPaint paint)
        => canvas.DrawText(text, x, y, paint, HorizontalAnchor.Right, VerticalAnchor.Center);
}
