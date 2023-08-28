using LP.Plot.Core.Primitives;
using SkiaSharp;

namespace LP.Plot.Core.Signal;

public interface ISignal
{
    /// <summary>
    /// x distance between two samples
    /// </summary>
    double Period { get; }
    Span YRange { get; }
    Span XRange { get; }
    Axis? YAxis { get; set; }
    SKPaint? Paint { get; set; }
    double[] YValues { get; }
}