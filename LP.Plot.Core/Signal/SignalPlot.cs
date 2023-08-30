using LP.Plot.Core.Primitives;
using LP.Plot.Skia;
using SkiaSharp;

namespace LP.Plot.Core.Signal;

public interface IAxes
{
    public Axis XAxis { get; }
    public IEnumerable<Axis> YAxes { get; }
    public void PanRelativeX(double relativeOffset);
    public void PanRelative(double relativeOffset);
    public void ZoomAtRelativeX(double factor, double relativePosition);
    public void ZoomAtRelative(double factor, double relativePosition);
}

public interface ISignalPlot : IRenderable
{
    public IAxes Axes { get; }
}

public class AxesCollection : IAxes
{
    public Axis XAxis { get; }
    public IEnumerable<Axis> YAxes { get; }

    public AxesCollection(Axis xAxis, IEnumerable<Axis> yAxes)
    {
        XAxis = xAxis;
        YAxes = yAxes.ToList();
    }

    public void PanRelativeX(double relativeOffset)
    {
        XAxis.PanRelative(relativeOffset);
    }

    public void PanRelative(double relativeOffset)
    {
        foreach (var axis in YAxes)
        {
            axis.PanRelative(relativeOffset);
        }
    }

    public void ZoomAtRelativeX(double factor, double relativePosition)
    {
        XAxis.ZoomAtRelative(factor, relativePosition);
    }

    public void ZoomAtRelative(double factor, double relativePosition)
    {
        foreach (var axis in YAxes)
        {
            axis.ZoomAtRelative(factor, relativePosition);
        }
    }
}

public class SignalPlot : IRenderable, ISignalPlot
{
    public IAxes Axes { get; }
    private List<ISignal> data = new();
    public Axis XAxis = new();
    public Axis DefaultYAxis = new();
    private SKPath path = new SKPath();
    private SKPaint DefaultPaint = SKPaints.White;
    public IEnumerable<Axis> YAxes => data.Where(x => x.YAxis is not null).Select(x => x.YAxis!);

    public SignalPlot(ISignal data)
    {
        this.data.Add(data);
        this.XAxis = new Axis(data.XRange);
        this.DefaultYAxis = new Axis(data.YRange);
        this.DefaultYAxis.ZoomAtCenter(1.1);
    }

    public SignalPlot(IEnumerable<ISignal> signals)
    {
        this.data.AddRange(signals);
        XAxis = new Axis(signals.First().XRange);
        DefaultYAxis = new Axis(signals.First().YRange);
        foreach (var s in signals)
        {
            s.Paint ??= SKPaints.NextPaint();
        }
    }

    public void Render(IRenderContext ctx)
    {
        ctx.Canvas.Save();
        ctx.Canvas.ClipRect(ctx.ClientRect.ToSkia());
        ctx.Canvas.Translate(ctx.ClientRect.Left, ctx.ClientRect.Top);
        ctx.Canvas.Clear(SKColors.Black);
        if (AlreadyBuffered())
        {
            RenderFromBuffer(ctx);
        }
        else
        {
            RenderToBuffer(ctx);
        }
        ctx.Canvas.Restore();
    }

    private bool AlreadyBuffered()
    {
        return false;
    }

    private void RenderFromBuffer(IRenderContext ctx)
    {

    }

    private void RenderToBuffer(IRenderContext ctx)
    {
        foreach (ISignal signal in data)
            RenderSignal(signal, signal.YAxis ?? DefaultYAxis, signal.Paint ?? DefaultPaint, ctx.Canvas, ctx.ClientRect.Size);
    }

    private void RenderAllSignal(IRenderContext ctx)
    {
        foreach (ISignal signal in data)
            RenderSignal(signal, signal.YAxis ?? DefaultYAxis, signal.Paint ?? DefaultPaint, ctx.Canvas, ctx.ClientRect.Size);
    }

    private void RenderSignal(ISignal data, Axis yAxis, SKPaint paint, SKCanvas canvas, LPSize imageSize)
    {
        SignalRenderer.FillDecimatedPath(data.YValues, data.XRange, XAxis.Range, yAxis.Range, imageSize, path);
        //SignalRenderer.FillFullPath(data.YValues, data.XRange, XAxis.Range, yAxis.Range, imageSize, path);
        canvas.DrawPath(path, paint);
        //SignalRenderer.DrawVerticalLines(data.YValues, data.XRange, XAxis.Range, yAxis.Range, canvas, paint, imageSize);
    }
}