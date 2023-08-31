﻿using LP.Plot.Core.Primitives;
using SkiaSharp;
using System.Diagnostics;

namespace LP.Plot.Core.Skia;

public class RenderInfo : IRenderable
{
    Stopwatch sw = new Stopwatch();
    private int frameCount = 0;

    private readonly SKPaint black = new()
    {
        Color = new SKColor(50, 50, 50, 128),
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

    public void Render(IRenderContext ctx)
    {
        var canvas = ctx.Canvas;
        var rect = new LPRect(ctx.Width - 140, 0, ctx.Width, 70);
        canvas.Save();
        canvas.ClipRect(rect.ToSkia());
        canvas.Translate(rect.Left, rect.Top);
        canvas.DrawRect(0, 0, 140, 70, black);

        var text = $"Frames {frameCount++}";
        SKRect bounds = new();
        white.MeasureText(text, ref bounds);
        canvas.DrawText(text, 0, 1.5f * bounds.Height, white);

        text = $"Fps {(frameCount / (double)sw.Elapsed.TotalSeconds):0.00}";
        canvas.DrawText(text, 0, 3f * bounds.Height, white);

        text = $"FTime {((double)sw.Elapsed.TotalMilliseconds / frameCount):00000}ms";
        canvas.DrawText(text, 0, 4.5f * bounds.Height, white);
        canvas.Restore();
    }
}
