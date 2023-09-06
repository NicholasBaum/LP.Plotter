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


    public static void DrawTextRotated270LeftCenter(this SKCanvas canvas, string text, float x, float y, SKPaint paint)
    {
        SKRect textRect = new();
        paint.MeasureText(text, ref textRect);
        var rotCenter = new SKPoint(textRect.Height / 2, y - textRect.Width / 2);
        var textOrigin = rotCenter - new SKPoint(textRect.Width, -x / 2 - textRect.Height / 2);
        canvas.Save();
        canvas.RotateDegrees(-90, rotCenter.X, rotCenter.Y);
        canvas.DrawText(text, textOrigin, paint);
        canvas.Restore();
    }

    public static void DrawTextRotated270RightCenter(this SKCanvas canvas, string text, float x, float y, SKPaint paint)
    {
        SKRect textRect = new();
        paint.MeasureText(text, ref textRect);
        var rotCenter = new SKPoint(textRect.Height / 2, y - textRect.Width / 2);
        var textOrigin = rotCenter - new SKPoint(textRect.Width, -x / 2 - textRect.Height);
        canvas.Save();
        canvas.RotateDegrees(-90, rotCenter.X, rotCenter.Y);
        canvas.DrawText(text, textOrigin, paint);
        canvas.Restore();
    }
}
