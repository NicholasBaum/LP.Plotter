using LP.Plot.Core.Layout;
using LP.Plot.Core.Primitives;
using LP.Plot.Core.Signal;
using SkiaSharp;
using System.Diagnostics;

namespace LP.Plot.Core;

public class Plot : IRenderable
{
    private ISignalPlot signalPlot = null!;
    private Docker layout = null!;
    private int leftAxisWidth = 75;
    private int bottomAxisHeight = 75;
    private LPSize canvasSize;

    protected Plot() { }

    public void Render(IRenderContext ctx)
    {
        canvasSize = ctx.Size;
        ctx.Canvas.Clear(SKColors.Black);
        layout.SetRect(LPRect.Create(ctx.Size));
        layout.Render(ctx);
    }

    public ISignalPlot AddSignal(ISignal data)
        => AddSignal(new List<ISignal>() { data });

    public ISignalPlot AddSignal(IEnumerable<ISignal> data)
    {
        this.signalPlot = new BufferedSignalPlot(data);
        return this.signalPlot;
    }

    public static Plot CreateSignal(ISignal data)
        => CreateSignal(new List<ISignal>() { data });

    public static Plot CreateSignal(IEnumerable<ISignal> data)
    {
        var plot = new Plot();
        plot.AddSignal(data);
        plot.layout = Docker.CreateDefault(plot.signalPlot.Axes.YAxes.First(), plot.leftAxisWidth, plot.signalPlot.Axes.XAxis, plot.bottomAxisHeight, plot.signalPlot!);
        return plot;
    }

    public void PanRelative(double x, double y)
    {
        if (signalPlot is null || canvasSize.IsEmpty) return;
        // correcting for actual graph area
        x *= (float)canvasSize.Width / (canvasSize.Width - leftAxisWidth);
        y *= (float)canvasSize.Height / (canvasSize.Height - bottomAxisHeight);

        signalPlot.Axes.PanRelativeX(x);
        signalPlot.Axes.PanRelative(y);
    }

    public void ZoomAtRelative(double factor, double x, double y)
    {
        if (signalPlot is null || canvasSize.IsEmpty) return;
        var w = canvasSize.Width;
        x = (w * x - leftAxisWidth) / (w - leftAxisWidth);
        signalPlot.Axes.ZoomAtRelativeX(factor, x);
        var h = canvasSize.Height;
        y = (h * y - bottomAxisHeight) / (h - bottomAxisHeight);
        signalPlot.Axes.ZoomAtRelative(factor, y);
    }

    private Span? currentXZoomSpan = null;

    public void ZoomRect(double x, double y)
    {
        var tx = new LPTransform(signalPlot.Axes.XAxis.Range, leftAxisWidth, canvasSize.Width);
        x = tx.Inverse(x);

        if (currentXZoomSpan == null)
        {
            currentXZoomSpan = new(x, x);
        }
        else
        {
            currentXZoomSpan = new(currentXZoomSpan.Value.Min, x);
            Debug.WriteLine(currentXZoomSpan);
        }
    }

    public void EndZoomRect()
    {
        Debug.WriteLine(currentXZoomSpan);
        currentXZoomSpan = null;
    }
}