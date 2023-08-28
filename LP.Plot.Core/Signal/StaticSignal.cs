using LP.Plot.Core.Primitives;
using SkiaSharp;

namespace LP.Plot.Core.Signal;

public class StaticSignal : ISignal
{
    public double[] YValues { get; }
    public double Period { get; }
    public Span YRange { get; }
    public Span XRange { get; }
    public Axis? YAxis { get; set; } = null;
    public SKPaint? Paint { get; set; } = null;

    public StaticSignal(double[] yValues, Span xRange)
    {
        this.YValues = yValues;
        XRange = xRange;
        YRange = new Span(yValues.Min(), yValues.Max());
        Period = XRange.Length / yValues.Length;
    }
}
