using LP.Plot.Core.Primitives;
using LP.Plot.Core.Skia;
using LP.Plot.Skia;
using SkiaSharp;
using System.Diagnostics;

namespace LP.Plot.Core.Signal;

public class SignalPlot : IRenderable
{
    private List<ISignal> data = new();
    public readonly Axis XAxis = new();
    internal AxesTracker Axes;
    private Span MaxXRange;
    private SKPath path = new SKPath();
    private readonly SKPaint DefaultPaint = SKPaints.White;

    public SignalPlot(ISignal data) : this(new List<ISignal>() { data }) { }

    public SignalPlot(IEnumerable<ISignal> signals)
    {
        this.data.AddRange(signals);
        MaxXRange = new(signals.Min(x => x.XRange.Min), signals.Max(x => x.XRange.Max));
        XAxis = new Axis(MaxXRange);
        Axes = new AxesTracker(XAxis);
        foreach (var s in signals)
        {
            s.Paint ??= SKPaints.NextPaint();
            if (s.YAxis == null)
            {
                s.YAxis = new Axis(s.YRange);
                s.YAxis.ZoomAtCenter(1.1);
            }
            Axes.AddY(s.YAxis);
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
            RenderFromBuffer(ctx);
            Axes.Reset();
        }
        ctx.Canvas.Restore();
    }

    private Buffer buffer = null!;
    private bool AlreadyBuffered(LPSize newClientRectSize)
    {
        if (Axes.IsDirty())
        {
            Debug.WriteLine($"IsDirty");
        }

        return buffer != null && !Axes.IsDirty() && buffer.IsSupported(newClientRectSize, XAxis);
    }

    private int CalcRenderLength(Span renderedAxisRange, Span visibleAxisRange, int clientLength)
    {
        var pixelsPerUnit = clientLength / visibleAxisRange.Length;
        var length = (int)(renderedAxisRange.Length * pixelsPerUnit);
        return length;
    }

    private void RenderFromBuffer(IRenderContext ctx)
    {
        ctx.Canvas.DrawSurface(buffer.Surface, (float)(-buffer.SurfaceSize.Width * Axes.RelativeOffsetX), (float)(buffer.SurfaceSize.Height * Axes.RelativeOffsetY));
    }

    private void RenderToBuffer(IRenderContext ctx)
    {
        System.Diagnostics.Debug.WriteLine("ReRendering");

        buffer?.Dispose();

        var rect = ctx.ClientRect;
        // pixels per unit
        var xDensity = rect.Width / XAxis.Length;
        var xRange_new = MaxXRange;
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
        var height_new = Math.Ceiling(rect.Height * max_scale);
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

        //var vXRange = new Span(xRange_new.Min <= MaxXRange.Min ? double.MinValue : MaxXRange.Min, MaxXRange.Max <= xRange_new.Max ? double.MaxValue : MaxXRange.Max);
        var vXRange = new Span(double.MinValue, double.MaxValue);
        buffer = new Buffer(ctx.ClientRect.Size, surface_B, xRange_new, vXRange, size_new);
    }

    private class Buffer : IDisposable
    {
        public Buffer(LPSize clientRectSize, SKSurface surface, Span actualXRange, Span supportedXRange, LPSize surfaceSize)
        {
            ClientRectSize = clientRectSize;
            Surface = surface;
            SurfaceSize = surfaceSize;
            ActualXRange = actualXRange;
            SupportedXRange = supportedXRange;
        }

        private LPSize ClientRectSize { get; }
        public SKSurface Surface { get; }
        public LPSize SurfaceSize { get; }

        /// <summary>
        /// The range of data that was rendered.
        /// </summary>
        public Span ActualXRange { get; }
        /// <summary>
        /// The range that can be rendered from the buffer e.g. returns an infinite span if all data was rendered to the buffer.
        /// </summary>
        private Span SupportedXRange { get; }

        public void Dispose()
        {
            Surface?.Dispose();
        }

        internal bool IsSupported(LPSize newClientRectSize, Axis xAxis)
        {
            if (newClientRectSize != ClientRectSize)
            {
                Debug.WriteLine("Client Missmatch");
                return false;
            }
            //var newDpx = xAxis.Length / newClientRectSize.Width;
            //if (!FloatEquals(Dpx, newDpx, 1e-10))
            //{
            //    Debug.WriteLine("Dpx Missmatch");
            //    return false;
            //}
            if (!SupportedXRange.Contains(xAxis.Range))
            {
                Debug.WriteLine("Range not supported");
                return false;
            }
            return true;
        }

        bool FloatEquals(double x, double y, double tolerance)
        {
            var diff = Math.Abs(x - y);
            return diff <= tolerance ||
                   diff <= Math.Max(Math.Abs(x), Math.Abs(y)) * tolerance;
        }
    }
}