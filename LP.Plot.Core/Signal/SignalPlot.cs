using LP.Plot.Core.Primitives;
using LP.Plot.Skia;
using SkiaSharp;
using System.Drawing;

namespace LP.Plot.Core.Signal;

public class SignalPlot : IRenderable
{
    private List<ISignal> data = new();
    public Axis XAxis = new();
    internal AxisCollection YAxes = new();
    private SKPath path = new SKPath();
    private SKPaint DefaultPaint = SKPaints.White;

    public SignalPlot(ISignal data) : this(new List<ISignal>() { data }) { }

    public SignalPlot(IEnumerable<ISignal> signals)
    {
        this.data.AddRange(signals);
        XAxis = new Axis(signals.First().XRange);
        foreach (var s in signals)
        {
            s.Paint ??= SKPaints.NextPaint();
            if (s.YAxis == null)
            {
                s.YAxis = new Axis(s.YRange);
                s.YAxis.ZoomAtCenter(1.1);
            }
            YAxes.Add(s.YAxis);
        }
    }

    public void Render(IRenderContext ctx)
    {
        ctx.Canvas.Save();
        ctx.Canvas.ClipRect(ctx.ClientRect.ToSkia());
        ctx.Canvas.Translate(ctx.ClientRect.Left, ctx.ClientRect.Top);
        ctx.Canvas.Clear(SKColors.Black);
        if (AlreadyBuffered(ctx.ClientRect.Size))
        {
            RenderFromBuffer(ctx);
        }
        else
        {
            RenderToBuffer(ctx);
        }
        ctx.Canvas.Restore();
    }

    private Buffer buffer = null!;
    private bool AlreadyBuffered(LPSize size)
    {
        if (buffer == null
            || buffer.CanvasSize != size
            || !buffer.VirtualXRange.Contains(XAxis.Range)
            || YAxes.IsDirty)
            return false;
        else
            return true;
    }

    private void RenderFromBuffer(IRenderContext ctx)
    {
        var dx = XAxis.Length / ctx.ClientRect.Width;
        var dy = YAxes.RefAxis.Length / ctx.ClientRect.Height;
        ctx.Canvas.DrawSurface(buffer.Surface, (int)((buffer.XRange.Min - XAxis.Min) / dx), (int)((YAxes.RefAxis.Min - buffer.YRange.Min) / dy));
    }

    private void RenderToBuffer(IRenderContext ctx)
    {
        System.Diagnostics.Debug.WriteLine("ReRendering");

        //var grContext = GRContext.CreateGl();        
        //var surface = SKSurface.Create(grContext, true, new SKImageInfo(100, 100));        
        //var canvas = surface.Canvas;

        buffer?.Dispose();
        var xAxis_B = new Span(0, data.Max(x => x.XRange.Max));
        var dpx = XAxis.Length / ctx.ClientRect.Size.Width;
        // yvalues might be of the canvas as well
        // have to calc the yaxis taking the max yrange and adjust canvas height
        LPSize size_B = new((int)(xAxis_B.Length / dpx), ctx.ClientRect.Size.Height);
        Span xDataRange_B = new Span(double.MinValue, double.MaxValue);
        var surface_B = SKSurface.Create(new SKImageInfo(size_B.Width, size_B.Height));
        var canvas_B = surface_B.Canvas;
        foreach (ISignal s in data)
        {
            var renderedXRange = new Span
                (
                xAxis_B.Min <= s.XRange.Min ? double.MinValue : xAxis_B.Min,
                s.XRange.Max <= xAxis_B.Max ? double.MaxValue : xAxis_B.Max
                );
            //xDataRange_B = new Span(Math.Max(xDataRange_B.Min, renderedXRange.Min), Math.Min(xDataRange_B.Max, renderedXRange.Max));
            RenderSignal(s, s.YAxis ?? YAxes.RefAxis, s.Paint ?? DefaultPaint, canvas_B, size_B);
        }

        buffer = new Buffer()
        {
            CanvasSize = ctx.ClientRect.Size,
            VirtualCanvasSize = size_B,
            Surface = surface_B,
            XRange = xAxis_B,
            VirtualXRange = xDataRange_B,
            YRange = YAxes.RefAxis.Range
        };
        RenderFromBuffer(ctx);
        YAxes.IsDirty = false;
    }

    private void RenderAllSignal(IRenderContext ctx)
    {
        foreach (ISignal signal in data)
            RenderSignal(signal, signal.YAxis ?? YAxes.RefAxis, signal.Paint ?? DefaultPaint, ctx.Canvas, ctx.ClientRect.Size);
    }

    private void RenderSignal(ISignal data, Axis yAxis, SKPaint paint, SKCanvas canvas, LPSize imageSize)
    {
        SignalRenderer.FillDecimatedPath(data.YValues, data.XRange, XAxis.Range, yAxis.Range, imageSize, path);
        //SignalRenderer.FillFullPath(data.YValues, data.XRange, XAxis.Range, yAxis.Range, imageSize, path);
        canvas.DrawPath(path, paint);
        //SignalRenderer.DrawVerticalLines(data.YValues, data.XRange, XAxis.Range, yAxis.Range, canvas, paint, imageSize);
    }

    private class Buffer : IDisposable
    {
        public LPSize CanvasSize { get; set; }
        public LPSize VirtualCanvasSize { get; set; }
        public required SKSurface Surface { get; init; }
        public Span XRange { get; set; }
        public Span VirtualXRange { get; set; }
        public Span YRange { get; set; }

        public bool Contains(Span range)
            => VirtualXRange.Contains(range);

        public void Dispose()
        {
            Surface?.Dispose();
        }
    }
}