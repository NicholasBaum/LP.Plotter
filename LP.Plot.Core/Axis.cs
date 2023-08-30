using LP.Plot.Core.Primitives;
using LP.Plot.Core.Skia;
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
    public Span Range => new Span(Min, Max);
    public double Min { get; set; } = float.MaxValue;
    public double Max { get; set; } = float.MinValue;
    public double Length => Max - Min;
    public AxisPosition Position { get; set; } = AxisPosition.Left;
    public string Title { get; set; } = string.Empty;
    public SKPaint Font { get; set; } = SKFonts.White;

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
        }

        ctx.Canvas.Restore();
    }

    public void DrawLeftAxis(IRenderContext ctx)
    {
        var rect = ctx.ClientRect;
        var canvas = ctx.Canvas;
        canvas.Clear(SKColors.Black);
        canvas.DrawLine(rect.Width - 1.0f, 0f, rect.Width - 1.0f, rect.Height, SKPaints.White);

        Title = "Speed";
        SKRect textRect = new();
        Font.MeasureText(Title, ref textRect);
        var rotCenter = new SKPoint(textRect.Height / 2 + 5, (rect.Height - textRect.Width) / 2);
        var textOrigin = rotCenter - new SKPoint(textRect.Width, 0);
        canvas.Save();
        canvas.RotateDegrees(-90, rotCenter.X, rotCenter.Y);
        canvas.DrawText(Title, textOrigin, Font);
        canvas.Restore();

        var ticks = GetTickValues();
        var t = new LPTransform(Min, Max, rect.Height, 0);
        var h = 10;
        foreach (var tick in ticks)
        {
            var ptick = (float)t.Transform(tick);
            canvas.DrawLine(rect.Width - h, ptick, rect.Width, ptick, SKPaints.White);
            var label = $"{tick:0000}";
            Font.MeasureText(label, ref textRect);
            canvas.DrawText(label, rect.Width - h - 5 - textRect.Width, ptick + textRect.Height/2, Font);
        }
    }

    public void DrawBottomAxis(IRenderContext ctx)
    {
        var rect = ctx.ClientRect;
        var canvas = ctx.Canvas;
        ctx.Canvas.Clear(SKColors.Black);
        ctx.Canvas.DrawLine(0, 0.5f, rect.Width, 0.5f, SKPaints.White);

        Title = "Time";
        SKRect textRect = new();
        Font.MeasureText(Title, ref textRect);
        canvas.DrawText(Title, (rect.Width - textRect.Width) / 2, rect.Height - textRect.Height - 5, Font);

        var ticks = GetTickValues();
        var t = new LPTransform(Min, Max, 0, rect.Width);
        var h = 10;
        foreach (var tick in ticks)
        {
            var ptick = (float)t.Transform(tick);
            canvas.DrawLine(ptick, 0, ptick, h, SKPaints.White);
            var label = $"{tick:0000}";
            Font.MeasureText(label, ref textRect);
            canvas.DrawText(label, ptick - textRect.Width / 2, h + 5 + textRect.Height, Font);
        }
    }

    public double[] GetTickValues()
    {
        var tickCount = 10;
        var gap = Length / tickCount;
        gap = RoundToNearestPowerOfTen(gap);
        var i = (int)(Min / gap);
        var first = i * gap;
        return Enumerable.Range(0, (int)(Length / gap) + 1).Select(x => first + x * gap).ToArray();
    }

    static double RoundToNearestPowerOfTen(double number)
    {
        int exponent = (int)Math.Floor(Math.Log10(number));
        double baseValue = Math.Pow(10, exponent);
        double roundedValue = Math.Round(number / baseValue) * baseValue;
        return roundedValue;
    }

    public override string ToString()
    {
        return $"[Min: {Min}, Max: {Max}, Length: {Length}]";
    }
}