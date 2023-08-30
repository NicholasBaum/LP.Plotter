using LP.Plot.Core.Layout;
using LP.Plot.Core.Primitives;
using LP.Plot.Core.Signal;
using SkiaSharp;

namespace LP.Plot.Core;

public class Plot : IRenderable
{
    private BufferedSignalPlot signalRenderer = null!;
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

    public BufferedSignalPlot AddSignal(ISignal data)
        => AddSignal(new List<ISignal>() { data });

    public BufferedSignalPlot AddSignal(IEnumerable<ISignal> data)
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
        plot.layout = Docker.CreateDefault(plot.signalRenderer.Axes.YAxes.First(), plot.leftAxisWidth, plot.signalRenderer.XAxis, plot.bottomAxisHeight, plot.signalRenderer!);
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
