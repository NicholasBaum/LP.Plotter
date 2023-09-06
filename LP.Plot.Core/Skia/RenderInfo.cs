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

    private readonly SKPaint bgPaint = new()
    {
        Color = new SKColor(50, 50, 50, 128),
        IsAntialias = true,
        Style = SKPaintStyle.Fill,
        TextAlign = SKTextAlign.Left,
        TextSize = 18,
    };
    private readonly SKPaint white = new()
    {
        Color = SKColors.White,
        IsAntialias = true,
        Style = SKPaintStyle.Fill,
        TextAlign = SKTextAlign.Left,
        TextSize = 18,
    };

    private readonly SKPaint whiteRA = new()
    {
        Color = SKColors.White,
        IsAntialias = true,
        Style = SKPaintStyle.Fill,
        TextAlign = SKTextAlign.Right,
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

    private int width = 120;
    public void Render(IRenderContext ctx)
    {
        var canvas = ctx.Canvas;
        var rect = new LPRect(ctx.Width - width, 0, ctx.Width, 70);
        canvas.Save();
        canvas.ClipRect(rect.ToSkia());
        canvas.Translate(rect.Left, rect.Top);
        canvas.DrawRect(0, 0, width, 70, bgPaint);
        //canvas.Clear(bgColor);


        var text = $"FTime";
        SKRect bounds = new();
        white.MeasureText(text, ref bounds);
        canvas.DrawText("FTime", 0, 1.5f * bounds.Height, white);
        canvas.DrawText($"{frameTimes.Last().TotalMilliseconds:####}ms", width, 1.5f * bounds.Height, whiteRA);

        var time = frameTimes.TakeLast(windwoSize).Aggregate((s, x) => s + x);
        canvas.DrawText("FAvg", 0, 3f * bounds.Height, white);
        canvas.DrawText($"{(time / windwoSize).TotalMilliseconds:####}ms", width, 3f * bounds.Height, whiteRA);

        var fps = time.TotalMilliseconds == 0 ? double.NaN : (windwoSize / time.TotalSeconds);
        canvas.DrawText("Fps", 0, 4.5f * bounds.Height, white);
        canvas.DrawText($"{fps:0}", width, 4.5f * bounds.Height, whiteRA);

        canvas.Restore();
    }
}
