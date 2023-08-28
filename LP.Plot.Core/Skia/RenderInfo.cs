using SkiaSharp;
using System.Diagnostics;

namespace LP.Plot.Core.Skia;

public class RenderInfo
{
    Stopwatch sw = new Stopwatch();
    private int frameCount = 0;

    private readonly SKPaint black = new()
    {
        Color = SKColors.Black,
        IsAntialias = true,
        Style = SKPaintStyle.Fill,
        TextAlign = SKTextAlign.Left,
    };

    private readonly SKPaint white = new()
    {
        Color = SKColors.White,
        IsAntialias = true,
        Style = SKPaintStyle.Fill,
        TextAlign = SKTextAlign.Left,
        TextSize = 18,
    };

    public void RestartMeasuring()
    {
        sw.Restart();
        frameCount = 0;
    }

    public void PaintRenderInfo(SKCanvas canvas, SKImageInfo imageInfo)
    {
        canvas.DrawRect(0, 0, 140, 70, black);

        var text = $"Frames {frameCount++}";
        SKRect bounds = new();
        white.MeasureText(text, ref bounds);
        canvas.DrawText(text, 0, 1.5f * bounds.Height, white);

        text = $"Fps {(frameCount / (double)sw.Elapsed.TotalSeconds):0.00}";
        canvas.DrawText(text, 0, 3f * bounds.Height, white);

        text = $"FTime {((double)sw.Elapsed.TotalSeconds / frameCount):0.00000}";
        canvas.DrawText(text, 0, 4.5f * bounds.Height, white);
    }
}
