using LP.Plot.Core.Primitives;
using LP.Plot.Core.Skia;
using LP.Plot.Core.Ticks;
using LP.Plot.Skia;
using SkiaSharp;

namespace LP.Plot.Core;

public enum AxisPosition
{
    Left, Top, Right, Bottom
}

public class Axis : IRenderable
{
    public Axis() { }
    public Axis(double min, double max) => (Min, Max) = (min, max);
    public Axis(Span range) => (Min, Max) = (range.Min, range.Max);
    public Span Range
    {
        get => new Span(Min, Max);
        set => (min, max) = (value.Min, value.Max);
    }
    public double Min
    {
        get => min;
        set => min = value;
    }
    private double min = float.MaxValue;
    public double Max
    {
        get => max;
        set => max = value;
    }
    private double max = float.MinValue;
    public double Length => Max - Min;
    public AxisPosition Position { get; set; } = AxisPosition.Left;
    public string Title { get; set; } = string.Empty;
    public SKPaint Font { get; set; } = SKFonts.White;
    public bool IsHorizontal => Position == AxisPosition.Top || Position == AxisPosition.Bottom;

    private float majorH = 10;
    private float minorH = 5;
    private float labelOffset = 17;
    private string labelFormat = "g6";

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

        switch (Position)
        {
            case AxisPosition.Left:
                DrawLeftAxis(ctx);
                break;
            case AxisPosition.Bottom:
                DrawBottomAxis(ctx);
                break;
            case AxisPosition.Right:
                DrawRightAxis(ctx);
                break;
            case AxisPosition.Top:
                DrawTopAxis(ctx);
                break;
        }

        ctx.Canvas.Restore();
    }

    public void DrawLeftAxis(IRenderContext ctx)
    {
        var rect = ctx.ClientRect;
        var canvas = ctx.Canvas;
        canvas.Clear(SKColors.Black);
        canvas.DrawLine(rect.Width - 1.0f, 0f, rect.Width - 1.0f, rect.Height, SKPaints.White);

        canvas.DrawTextRotated270LeftCenter(Title, 5, rect.Height / 2, Font);

        var ticks = GetTickValues(ctx.ClientRect.Height);
        var t = new LPTransform(Min, Max, rect.Height, 0);

        foreach (var tick in ticks.MajorTicks)
        {
            var ptick = (float)t.Transform(tick);
            canvas.DrawLine(rect.Width - majorH, ptick, rect.Width, ptick, SKPaints.White);
            canvas.DrawTextRightCenter(tick.ToString(labelFormat), rect.Width - labelOffset, ptick, Font);
        }

        foreach (var tick in ticks.MinorTicks)
        {
            var ptick = (float)t.Transform(tick);
            canvas.DrawLine(rect.Width - minorH, ptick, rect.Width, ptick, SKPaints.White);
        }
    }

    public void DrawTopAxis(IRenderContext ctx)
    {
        var rect = ctx.ClientRect;
        var canvas = ctx.Canvas;
        ctx.Canvas.Clear(SKColors.Black);
        ctx.Canvas.DrawLine(0, rect.Height - 1.0f, rect.Width, rect.Height - 1.0f, SKPaints.White);

        canvas.DrawTextCenterTop(Title, rect.Width / 2, 5, Font);

        var ticks = GetTickValues(ctx.ClientRect.Width);
        var t = new LPTransform(Min, Max, 0, rect.Width);
        foreach (var tick in ticks.MajorTicks)
        {
            var ptick = (float)t.Transform(tick);
            canvas.DrawLine(ptick, rect.Height - majorH, ptick, rect.Height, SKPaints.White);
            canvas.DrawTextCenterBottom(tick.ToString(labelFormat), ptick, rect.Height - labelOffset, Font);
        }

        foreach (var tick in ticks.MinorTicks)
        {
            var ptick = (float)t.Transform(tick);
            canvas.DrawLine(ptick, rect.Height - minorH, ptick, rect.Height, SKPaints.White);
        }
    }

    public void DrawRightAxis(IRenderContext ctx)
    {
        var rect = ctx.ClientRect;
        var canvas = ctx.Canvas;
        canvas.Clear(SKColors.Black);
        canvas.DrawLine(1.0f, 0f, 1.0f, rect.Height, SKPaints.White);

        canvas.DrawTextRotated270RightCenter(Title, rect.Width, rect.Height / 2, Font);

        var ticks = GetTickValues(ctx.ClientRect.Height);
        var t = new LPTransform(Min, Max, rect.Height, 0);

        foreach (var tick in ticks.MajorTicks)
        {
            var ptick = (float)t.Transform(tick);
            canvas.DrawLine(0, ptick, majorH, ptick, SKPaints.White);
            canvas.DrawTextLeftCenter(tick.ToString(labelFormat), labelOffset, ptick, Font);
        }

        foreach (var tick in ticks.MinorTicks)
        {
            var ptick = (float)t.Transform(tick);
            canvas.DrawLine(0, ptick, minorH, ptick, SKPaints.White);
        }
    }


    public void DrawBottomAxis(IRenderContext ctx)
    {
        var rect = ctx.ClientRect;
        var canvas = ctx.Canvas;
        ctx.Canvas.Clear(SKColors.Black);
        ctx.Canvas.DrawLine(0, 0.5f, rect.Width, 0.5f, SKPaints.White);

        canvas.DrawTextCenterBottom(Title, rect.Width / 2, rect.Height - 5, Font);

        var ticks = GetTickValues(ctx.ClientRect.Width);
        var t = new LPTransform(Min, Max, 0, rect.Width);
        foreach (var tick in ticks.MajorTicks)
        {
            var ptick = (float)t.Transform(tick);
            canvas.DrawLine(ptick, 0, ptick, majorH, SKPaints.White);
            canvas.DrawTextCenterTop(tick.ToString(labelFormat), ptick, labelOffset, Font);
        }

        foreach (var tick in ticks.MinorTicks)
        {
            var ptick = (float)t.Transform(tick);
            canvas.DrawLine(ptick, 0, ptick, minorH, SKPaints.White);
        }
    }

    private (double[] MajorTicks, double[] MinorTicks) GetTickValues(int screenLength)
    {
        var step = AxisUtilities.GetIntervallSizes(screenLength, this.Range.Length);
        var majorTicks = AxisUtilities.CreateTickValues(Min, Max, step.MajorStep);
        var minorTicks = AxisUtilities.CreateTickValues(Min, Max, step.MinorStep);
        minorTicks = AxisUtilities.FilterRedundantMinorTicks(majorTicks, minorTicks);

        return (majorTicks.ToArray(), minorTicks.ToArray());
    }

    public override string ToString()
    {
        return $"[Min: {Min}, Max: {Max}, Length: {Length}]";
    }
}