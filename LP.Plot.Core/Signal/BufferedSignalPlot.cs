using LP.Plot.Core.Primitives;
using LP.Plot.Skia;
using SkiaSharp;
using System.Diagnostics;

namespace LP.Plot.Core.Signal;

public class BufferedSignalPlot : IRenderable, ISignalPlot
{
    IAxes ISignalPlot.Axes => Axes;

    private List<ISignal> data = new();
    private Axis XAxis = new();
    private AxesTracker Axes { get; }
    private ISignal Ref_Signal => data.First();
    private Axis Ref_YAxis => Ref_Signal.YAxis!;
    private Span Ref_YRange_Max => Ref_Signal.YRange;
    private Span XRange_Max;

    private SKPath path = new SKPath();


    public BufferedSignalPlot(ISignal data) : this(new List<ISignal>() { data }) { }

    public BufferedSignalPlot(IEnumerable<ISignal> signals)
    {
        this.data.AddRange(signals);
        XRange_Max = new(signals.Min(x => x.XRange.Min), signals.Max(x => x.XRange.Max));
        XAxis = new Axis(XRange_Max) { Position = AxisPosition.Bottom };
        List<Axis> yAxes = new();
        foreach (var s in signals)
        {
            s.Paint ??= SKPaints.NextPaint();
            if (s.YAxis == null)
            {
                s.YAxis = new Axis(s.YRange);
                s.YAxis.ZoomAtCenter(1.1);
            }
            yAxes.Add(s.YAxis);
        }
        Axes = new(XAxis, yAxes);
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
            RenderFromBuffer(ctx);
            Axes.Reset();
        }
        ctx.Canvas.Restore();
    }

    private Buffer buffer = null!;

    private bool AlreadyBuffered(LPSize newClientRectSize)
    {
        if (Axes.ShouldRerender())
            Debug.WriteLine($"IsDirty");
        return buffer != null && !Axes.ShouldRerender() && buffer.IsSupported(newClientRectSize, XAxis.Range, Ref_YAxis.Range);
    }

    private void RenderFromBuffer(IRenderContext ctx)
    {
        var xOffset = buffer.XD2p.Transform(XAxis.Min);
        var yOffset = buffer.YD2p.Transform(Ref_YAxis.Max);
        ctx.Canvas.DrawSurface(buffer.Surface, (float)-xOffset, (float)-yOffset);
    }

    private void RenderToBuffer(IRenderContext ctx, int widthLimit = 3000, int heightLimit = 3000)
    {
        System.Diagnostics.Debug.WriteLine("ReRendering");

        buffer?.Dispose();

        var rect = ctx.ClientRect;
        // pixels per unit
        var xDensity = rect.Width / XAxis.Length;
        var xRange_new = XRange_Max;
        var limit = Math.Max(widthLimit, rect.Width) / xDensity;
        if (xRange_new.Length > limit)
            xRange_new = XAxis.Range.ScaleAtCenter(limit / XAxis.Length);
        var width_new = (int)(xRange_new.Length * xDensity);
        // the necessary height to completly draw all signals with the original density/zoom factor
        double max_scale = 1;
        foreach (var s in data)
        {
            var range_new = s.YRange;
            var yrange = s.YAxis!.Range;
            var b = Math.Max(range_new.Max - yrange.Mid, yrange.Mid - range_new.Min);
            var scale = b / (yrange.Length / 2);
            if (scale < 1)
                continue;
            else
                max_scale = Math.Max(max_scale, scale);
        }
        var yLimit = Math.Max(heightLimit, rect.Height);
        var height_new = Math.Min(yLimit, Math.Ceiling(rect.Height * max_scale));
        // correcting scale again
        max_scale = height_new / rect.Height;
        LPSize size_new = new(width_new, (int)height_new);

        var surface_B = SKSurface.Create(new SKImageInfo(size_new.Width, size_new.Height));
        var canvas_B = surface_B.Canvas;

        foreach (var s in data)
        {
            Span b_YAxisRange = s.YAxis!.Range.ScaleAtCenter(max_scale);
            SignalRenderer.FillDecimatedPath(s.YValues, s.XRange, xRange_new, b_YAxisRange, size_new, path);
            canvas_B.DrawPath(path, s.Paint);
        }

        var xRange_virtual = CalcSupport(xRange_new, XRange_Max);
        var xD2p = new LPTransform(xRange_new, new Span(0, size_new.Width));
        var yRange = Ref_YAxis.Range.ScaleAtCenter(max_scale);
        var yRange_virtual = CalcSupport(yRange, Ref_YRange_Max);
        var yD2p = new LPTransform(yRange.Min, yRange.Max, size_new.Height, 0);
        buffer = new Buffer(rect.Size, surface_B, xRange_virtual, yRange_virtual, xD2p, yD2p);
        Debug.WriteLine(size_new);
    }

    private static Span CalcSupport(Span range, Span dataRange)
        => new Span(range.Min <= dataRange.Min ? double.MinValue : range.Min, dataRange.Max <= range.Max ? double.MaxValue : range.Max);







    private class Buffer : IDisposable
    {
        public SKSurface Surface { get; }
        public LPTransform XD2p { get; }
        public LPTransform YD2p { get; }

        private LPSize ClientRectSize { get; }
        private readonly Span supportedXRange;
        private readonly Span supportedYRange;

        public Buffer(LPSize clientRectSize, SKSurface surface, Span supportedXRange, Span supportedYRange, LPTransform xD2p, LPTransform yD2p)
        {
            ClientRectSize = clientRectSize;
            Surface = surface;
            this.supportedXRange = supportedXRange;
            this.supportedYRange = supportedYRange;
            XD2p = xD2p;
            YD2p = yD2p;
        }

        public void Dispose()
        {
            Surface?.Dispose();
        }

        public bool IsSupported(LPSize newClientRectSize, Span xRange, Span yRange)
        {
            if (newClientRectSize != ClientRectSize)
            {
                Debug.WriteLine("Client Missmatch");
                return false;
            }
            if (!supportedXRange.Contains(xRange))
            {
                Debug.WriteLine("X-Range not supported");
                return false;
            }
            if (!supportedYRange.Contains(yRange))
            {
                Debug.WriteLine("Y-Range not supported");
                return false;
            }
            return true;
        }
    }
}