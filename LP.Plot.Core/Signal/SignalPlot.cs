using LP.Plot.Core.Primitives;
using SkiaSharp;

namespace LP.Plot.Core.Signal;

public class SignalPlot : IRenderable, ISignalPlot
{
    public IAxes Axes { get; }

    private List<ISignal> signals = new();
    private Axis XAxis = new();
    private SKPath path = new SKPath();

    public SignalPlot(ISignal data) : this(new[] { data }) { }

    public SignalPlot(IEnumerable<ISignal> signals)
    {
        this.signals.AddRange(signals);
        Span XRange_Max = new(signals.Min(x => x.XRange.Min), signals.Max(x => x.XRange.Max));
        XAxis = new Axis(XRange_Max) { Position = AxisPosition.Bottom };
        Axes = new AxesCollection(XAxis, this.signals.Select(x => x.YAxis));
    }

    public void Remove(ISignal signal)
    {
        signals.Remove(signal);
        Axes.Reset(XAxis, signals.Select(x => x.YAxis));
    }

    public void Render(IRenderContext ctx)
    {
        ctx.Canvas.Save();
        ctx.Canvas.ClipRect(ctx.ClientRect.ToSkia());
        ctx.Canvas.Translate(ctx.ClientRect.Left, ctx.ClientRect.Top);
        ctx.Canvas.Clear(SKColors.Black);
        foreach (ISignal signal in signals.Where(x => x.IsVisible))
            RenderSignal(signal, signal.YAxis, signal.Paint, ctx.Canvas, ctx.ClientRect.Size);
        ctx.Canvas.Restore();
    }

    private void RenderSignal(ISignal data, Axis yAxis, SKPaint paint, SKCanvas canvas, LPSize imageSize)
    {
        SignalRenderer.FillDecimatedPath(data.YValues, data.XRange, XAxis.Range, yAxis.Range, imageSize, path);
        //SignalRenderer.FillFullPath(data.YValues, data.XRange, XAxis.Range, yAxis.Range, imageSize, path);
        canvas.DrawPath(path, paint);
        //SignalRenderer.DrawVerticalLines(data.YValues, data.XRange, XAxis.Range, yAxis.Range, canvas, paint, imageSize);
    }
}