using LP.Plot.Core.Layout;
using LP.Plot.Core.Primitives;
using LP.Plot.Core.Signal;
using SkiaSharp;

namespace LP.Plot.Core;

public class Plot : IRenderable
{
    private ISignalPlot signalRenderer = null!;
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
        this.signalRenderer = new BufferedSignalPlot(data);
        return this.signalRenderer;
    }

    public static Plot CreateSignal(ISignal data)
        => CreateSignal(new List<ISignal>() { data });

    public static Plot CreateSignal(IEnumerable<ISignal> data)
    {
        var plot = new Plot();
        plot.AddSignal(data);
        plot.layout = Docker.CreateDefault(plot.signalRenderer.Axes.YAxes.First(), plot.leftAxisWidth, plot.signalRenderer.Axes.XAxis, plot.bottomAxisHeight, plot.signalRenderer!);
        return plot;
    }

    public void PanRelative(double x, double y)
    {
        if (signalRenderer is null || canvasSize.IsEmpty) return;
        // correcting for actual graph area
        x *= (float)canvasSize.Width / (canvasSize.Width - leftAxisWidth);
        y *= (float)canvasSize.Height / (canvasSize.Height - bottomAxisHeight);

        signalRenderer.Axes.PanRelativeX(x);
        signalRenderer.Axes.PanRelative(y);
    }

    public void ZoomAtRelative(double factor, double x, double y)
    {
        if (signalRenderer is null || canvasSize.IsEmpty) return;
        var w = canvasSize.Width;
        x = (w * x - leftAxisWidth) / (w - leftAxisWidth);
        signalRenderer.Axes.ZoomAtRelativeX(factor, x);
        var h = canvasSize.Height;
        y = (h * y - bottomAxisHeight) / (h - bottomAxisHeight);
        signalRenderer.Axes.ZoomAtRelative(factor, y);
    }
}