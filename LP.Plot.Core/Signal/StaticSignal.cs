using LP.Plot.Core.Primitives;
using LP.Plot.Skia;
using SkiaSharp;

namespace LP.Plot.Core.Signal;

public class StaticSignal : ISignal
{
    public double[] YValues { get; }
    public double Period { get; }
    public Span YRange { get; }
    public Span XRange { get; }
    public Axis YAxis { get; set; }
    public SKPaint Paint { get; set; }

    public StaticSignal(double[] yValues, Span xRange, Axis yAxis, SKPaint paint)
    {
        YValues = yValues;
        XRange = xRange;
        YRange = new Span(yValues.Min(), yValues.Max());
        Period = XRange.Length / yValues.Length;
        Paint = paint;
        YAxis = yAxis;
    }

    public static ISignal Create(double[] yValues, Span xRange)
    {
        return new StaticSignal(yValues, xRange, new Axis(new Span(yValues.Min(), yValues.Max()).ScaleAtCenter(1.1)), SKPaints.NextPaint());
    }
}
