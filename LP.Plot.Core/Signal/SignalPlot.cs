using LP.Plot.Core.Primitives;
using LP.Plot.Skia;
using SkiaSharp;

namespace LP.Plot.Core.Signal;

public class SignalPlot : IRenderable
{
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