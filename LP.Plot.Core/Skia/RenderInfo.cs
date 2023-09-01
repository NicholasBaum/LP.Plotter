using LP.Plot.Core.Primitives;
using SkiaSharp;
using System.Diagnostics;

namespace LP.Plot.Core.Skia;

public class RenderInfo : IRenderable, IDisposable
{
    Stopwatch timer = Stopwatch.StartNew();
    // working with ticks doesn't wrok in blazor wasm as far as i can tell
    List<TimeSpan> frameTimes = new(1000);
    TimeSpan lastTime;
    int windwoSize = 60;

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

    public IDisposable Measure()
    {
        lastTime = timer.Elapsed;
        return this;
    }

    public void Dispose()
    {
        frameTimes.Add(timer.Elapsed - lastTime);
        if (frameTimes.Count > frameTimes.Capacity)
            frameTimes.Clear();
    }

    public void Render(IRenderContext ctx)
    {
        var canvas = ctx.Canvas;
        var rect = new LPRect(ctx.Width - 140, 0, ctx.Width, 70);
        canvas.Save();
        canvas.ClipRect(rect.ToSkia());
        canvas.Translate(rect.Left, rect.Top);
        canvas.DrawRect(0, 0, 140, 70, black);

        var text = $"FTime {frameTimes.Last().TotalMilliseconds:0000}ms";
        SKRect bounds = new();
        white.MeasureText(text, ref bounds);
        canvas.DrawText(text, 0, 1.5f * bounds.Height, white);

        var time = frameTimes.TakeLast(windwoSize).Aggregate((s, x) => s + x);
        text = $"FAvg {(time / windwoSize).TotalMilliseconds:0000}ms";
        canvas.DrawText(text, 0, 3f * bounds.Height, white);

        var fps = time.TotalMilliseconds == 0 ? double.NaN : (windwoSize / time.TotalSeconds);
        text = $"Fps {fps:0.00}";
        canvas.DrawText(text, 0, 4.5f * bounds.Height, white);

        canvas.Restore();
    }
}
