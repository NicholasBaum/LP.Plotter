using LP.Plot.Core.Primitives;
using SkiaSharp;
using System.Diagnostics;

namespace LP.Plot.Core.Signal;

public class BufferedSignalPlot : ISignalPlot, IRenderable
{
    public IReadOnlyList<Axis> YAxes => signals.Select(x => x.YAxis).Distinct().ToList();
    public Axis XAxis { get; }

    private List<ISignal> signals = new();
    private SignalsTracker signalsTracker;

    private ISignal Ref_Signal => signals.First();
    private Axis Ref_YAxis => Ref_Signal.YAxis!;
    private Span Ref_YRange_Max => Ref_Signal.YRange;
    private Span XRange_Max;

    private SKPath path = new SKPath();


    public BufferedSignalPlot(ISignal data) : this(new List<ISignal>() { data }) { }

    public BufferedSignalPlot(IEnumerable<ISignal> signals)
    {
        this.signals.AddRange(signals);
        XRange_Max = new(signals.Min(x => x.XRange.Min), signals.Max(x => x.XRange.Max));
        XAxis = new Axis(XRange_Max) { Position = AxisPosition.Bottom };
        signalsTracker = new SignalsTracker(signals, XAxis);
    }

    public void Remove(ISignal signal)
    {
        signals.Remove(signal);
        signalsTracker.Remove(signal);
    }

    public void Render(IRenderContext ctx)
    {
        ctx.Canvas.Save();
        ctx.Canvas.ClipRect(ctx.ClientRect.ToSkia());
        ctx.Canvas.Translate(ctx.ClientRect.Left, ctx.ClientRect.Top);
        ctx.Canvas.Clear(SKColors.Black);
        if (!signals.Any()) return;
        if (AlreadyBuffered(ctx.ClientRect.Size))
        {
            RenderFromBuffer(ctx);
        }
        else
        {
            RenderToBuffer(ctx);
            RenderFromBuffer(ctx);
            signalsTracker.Cache();
        }
        ctx.Canvas.Restore();
    }

    private Buffer buffer = null!;

    private bool AlreadyBuffered(LPSize newClientRectSize)
    {
        if (signalsTracker.HasChanged)
            Debug.WriteLine($"Rerender (signals or axes changed)");
        return buffer != null
            && !signalsTracker.HasChanged
            && buffer.IsSupported(newClientRectSize, XAxis.Range, Ref_YAxis.Range);
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
        var visibles = signals.Where(x => x.IsVisible).ToList();
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
        // iterating signals here instead of visibles because rendering inaccuracies can appear of yrange arent exactly the same
        foreach (var s in signals)
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

        foreach (var s in visibles)
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