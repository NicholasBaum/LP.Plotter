
using LP.Plot.Core.Signal;
using SkiaSharp;

namespace LP.Plot.Core;

public class Plot : IRenderable
{
    private List<IRenderable> plotables { get; } = new();
    private SignalRenderer? signalRenderer = null;
    private Axis? XAxis => signalRenderer?.XAxis;


    public void Render(IRenderContext ctx)
    {
        foreach (var r in plotables)
            r.Render(ctx);
    }

    public SignalRenderer AddSignal(ISignalSource data)
    {
        this.signalRenderer = new SignalRenderer(data);
        plotables.Add(this.signalRenderer);
        return this.signalRenderer;
    }

    public static Plot CreateSignal(ISignalSource data)
    {
        var plot = new Plot();
        plot.AddSignal(data);
        return plot;
    }

    public void Pan(double x, double y)
    {
        if (signalRenderer is null) return;
        signalRenderer.XAxis.Pan(x);
        signalRenderer.YAxis.Pan(y);
    }

    public void ZoomAt(double factor, double x, double y)
    {
        if (signalRenderer is null) return;
        signalRenderer.XAxis.ZoomAt(factor, x);
        signalRenderer.YAxis.ZoomAt(factor, y);
    }
}

public interface IRenderable
{
    void Render(IRenderContext ctx);
}

public interface IRenderContext
{
    SKCanvas Canvas { get; }
    int Width { get; }
    int Height { get; }
}

public class SkiaRenderContext : IRenderContext
{
    public SKCanvas Canvas { get; }
    public int Width { get; }
    public int Height { get; }

    public SkiaRenderContext(SKCanvas canvas, int width, int height)
    {
        Canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
        Width = width;
        Height = height;
    }
}
