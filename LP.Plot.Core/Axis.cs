using LP.Plot.Core.Primitives;
using LP.Plot.Skia;
using SkiaSharp;

namespace LP.Plot.Core;

public class Axis : IRenderable
{
    public Axis() { }
    public Axis(double min, double max) => (Min, Max) = (min, max);
    public Axis(Span range) => (Min, Max) = (range.Min, range.Max);
    public Span Range => new Span(Min, Max);
    public double Min { get; set; } = float.MaxValue;
    public double Max { get; set; } = float.MinValue;
    public double Length => Max - Min;

    public void Pan(double offset)
    {
        Min += offset;
        Max += offset;
    }

    public void ZoomAt(double factor, double position)
    {
        // keep relative position of position
        var newLeftSide = (position - Min) * factor;
        var newRightSide = (Max - position) * factor;
        Min = position - newLeftSide;
        Max = position + newRightSide;
    }

    public void PanRelative(double relativOffset)
    {
        var offset = Length * relativOffset;
        Min += offset;
        Max += offset;
    }

    public void ZoomAtRelative(double factor, double relativePosition)
    {
        var zoomCenter = Min + Length * relativePosition;
        var newLeftSide = (zoomCenter - Min) * factor;
        var newRightSide = (Max - zoomCenter) * factor;
        Min = zoomCenter - newLeftSide;
        Max = zoomCenter + newRightSide;
    }

    public void ZoomAtCenter(double factor)
    {
        ZoomAtRelative(factor, 0.5);
    }

    public void Render(IRenderContext ctx)
    {
        ctx.Canvas.Save();
        ctx.Canvas.ClipRect(ctx.ClientRect.ToSkia());
        ctx.Canvas.Translate(ctx.ClientRect.Left, ctx.ClientRect.Top);
        ctx.Canvas.Clear(SKColors.Red);
        ctx.Canvas.DrawLine(0, 0.5f, ctx.ClientRect.Width, 0.5f, SKPaints.Red);
        ctx.Canvas.Restore();
    }

    public override string ToString()
    {
        return $"[Min: {Min}, Max: {Max}, Length: {Length}]";
    }
}