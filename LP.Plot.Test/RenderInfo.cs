using SkiaSharp;
using System.Diagnostics;

namespace LP.Plot.Test;

public class RenderInfo
{
    Stopwatch sw = new Stopwatch();
    private int frameCount = 0;

    public RenderInfo()
    {

    }

    public void Restart()
    {
        sw.Restart();
        frameCount = 0;
    }

    public void PaintRenderInfo(SKCanvas canvas, SKImageInfo imageInfo)
    {
        using var black = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            TextAlign = SKTextAlign.Left,
        };
        using var white = new SKPaint
        {
            Color = SKColors.White,
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            TextAlign = SKTextAlign.Left,
            TextSize = 20,
        };
        using var font = new SKFont
        {
            Size = 24
        };

        var coord = new SKPoint(imageInfo.Width + 200, imageInfo.Height + 200);
        canvas.DrawRect(0, 0, 100, 100, black);

        var text = $"Frames {frameCount++}";
        SKRect bounds = new SKRect();
        white.MeasureText(text, ref bounds);
        canvas.DrawText(text, 0, 1.5f * bounds.Height, font, white);
        text = $"Fps {(frameCount / (double)sw.Elapsed.TotalSeconds):0.00}";

        white.MeasureText(text, ref bounds);
        canvas.DrawText(text, 0, bounds.Height * 2.5f, font, white);
    }
}
