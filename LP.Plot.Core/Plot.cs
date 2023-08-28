
using LP.Plot.Core.Layout;
using LP.Plot.Core.Primitives;
using LP.Plot.Core.Signal;
using SkiaSharp;

namespace LP.Plot.Core;

public class Plot : IRenderable
{
    private List<IRenderable> plotables { get; } = new();
    private Signal.SignalPlot? signalRenderer = null;
    private Docker layout = null!;

    protected Plot() { }

    public void Render(IRenderContext ctx)
    {
        //foreach (var r in plotables)
        //    r.Render(ctx);
        ctx.Canvas.Clear(SKColors.Teal);
        layout.SetRect(LPRect.Create(ctx.Size));
        layout.Render(ctx);
    }

    public SignalPlot AddSignal(ISignal data)
        => AddSignal(new List<ISignal>() { data });

    public SignalPlot AddSignal(IEnumerable<ISignal> data)

    {
        this.signalRenderer = new SignalPlot(data);
        plotables.Add(this.signalRenderer);
        return this.signalRenderer;
    }

    public static Plot CreateSignal(ISignal data)
        => CreateSignal(new List<ISignal>() { data });

    public static Plot CreateSignal(IEnumerable<ISignal> data)
    {
        var plot = new Plot();
        plot.AddSignal(data);
        plot.layout = Docker.CreateDefault(plot.signalRenderer!.YAxes.RefAxis!, plot.signalRenderer!.XAxis, plot.signalRenderer!);
        return plot;
    }

    public void Pan(double x, double y)
    {
        if (signalRenderer is null) return;
        signalRenderer.XAxis.PanRelative(x);
        //signalRenderer.YAxes.PanRelative(y);
    }

    public void ZoomAt(double factor, double x, double y)
    {
        //if (signalRenderer is null) return;
        //signalRenderer.XAxis.ZoomAtRelative(factor, x);
        //signalRenderer.YAxes.ZoomAtRelative(factor, y);
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
    LPSize Size { get; }
    LPRect ClientRect { get; set; }
}

public class SkiaRenderContext : IRenderContext
{
    public SKCanvas Canvas { get; }
    public int Width { get; }
    public int Height { get; }
    public LPSize Size => new LPSize(Width, Height);
    public LPRect ClientRect { get; set; }

    public SkiaRenderContext(SKCanvas canvas, int width, int height)
    {
        Canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
        Width = width;
        Height = height;
    }
}
