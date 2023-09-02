using LP.Plot.Core.Layout;
using LP.Plot.Core.Primitives;
using LP.Plot.Core.Signal;
using LP.Plot.Core.Skia;
using SkiaSharp;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace LP.Plot.Core;

public partial class Plot : IRenderable
{
    private ISignalPlot signalPlot = null!;
    private Docker layout = null!;
    private int leftAxisWidth = 75;
    private int bottomAxisHeight = 75;
    private LPSize canvasSize;
    private RenderInfo renderInfo = new();
    private Stopwatch timer = new();

    public event EventHandler<EventArgs>? Changed;
    public void Invalidate() => Changed?.Invoke(this, EventArgs.Empty);

    protected Plot() { }

    public void Render(IRenderContext ctx)
    {
        using (var m = renderInfo.Measure())
        {
            canvasSize = ctx.Size;
            ctx.Canvas.Clear(SKColors.Black);
            layout.SetRect(LPRect.Create(ctx.Size));
            layout.Render(ctx);
            DrawZoomRect(ctx);
        }
        renderInfo.Render(ctx);
    }

    public static Plot CreateSignal(ISignal data)
        => CreateSignal(new List<ISignal>() { data });

    public static Plot CreateSignal(IEnumerable<ISignal> data)
    {
        var plot = new Plot();
        plot.signalPlot = new BufferedSignalPlot(data);
        plot.Sets.Add(new SignalSet()
        {
            Channels = data.Select(x => new SignalVM(x)).ToList()
        });
        plot.layout = Docker.CreateDefault(plot.signalPlot.YAxes.First(), plot.leftAxisWidth, plot.signalPlot.XAxis, plot.bottomAxisHeight, plot.signalPlot!);
        return plot;
    }

    public void PanRelative(double x, double y)
    {
        if (signalPlot is null || canvasSize.IsEmpty) return;
        // correcting for actual graph area
        x *= (float)canvasSize.Width / (canvasSize.Width - leftAxisWidth);
        y *= (float)canvasSize.Height / (canvasSize.Height - bottomAxisHeight);

        signalPlot.PanRelativeX(x);
        signalPlot.PanRelative(y);
    }

    public void ZoomAtRelative(double factor, double x, double y)
    {
        if (signalPlot is null || canvasSize.IsEmpty) return;
        var w = canvasSize.Width;
        x = (w * x - leftAxisWidth) / (w - leftAxisWidth);
        signalPlot.ZoomAtRelativeX(factor, x);
        var h = canvasSize.Height;
        y = (h * y - bottomAxisHeight) / (h - bottomAxisHeight);
        signalPlot.ZoomAtRelative(factor, y);
    }

    private Span? currentZoom = null;
    private Span pixelZoom = new();
    public void ZoomRect(double x, double y)
    {
        var tx = new LPTransform(signalPlot.XAxis.Range, leftAxisWidth, canvasSize.Width);
        var xx = tx.Inverse(x);

        if (currentZoom == null)
        {
            currentZoom = new(xx, xx);
            pixelZoom = new(x, x);
        }
        else
        {
            currentZoom = new(currentZoom.Value.Min, xx);
            pixelZoom = new(pixelZoom.Min, x);
        }
    }

    public void EndZoomRect()
    {
        if (currentZoom != null && Math.Abs(pixelZoom.Length) > 0.1)
        {
            var z = currentZoom.Value.Length > 0 ? currentZoom.Value : new Span(currentZoom.Value.Max, currentZoom.Value.Min);
            signalPlot.ZoomX(z);
        }
        currentZoom = null;
    }

    private void DrawZoomRect(IRenderContext ctx)
    {
        if (currentZoom == null || layout.Center == null)
            return;
        using (var paint = new SKPaint())
        {
            paint.Color = new SKColor(81, 170, 165, 128);
            paint.IsAntialias = true;
            SKRect rect = new SKRect(50, 50, 250, 250);
            ctx.Canvas.DrawRect((float)pixelZoom.Min, 0, (float)pixelZoom.Length, canvasSize.Height, paint);
        }
    }
}