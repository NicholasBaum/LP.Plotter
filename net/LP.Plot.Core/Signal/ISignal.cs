using LP.Plot.Primitives;
using SkiaSharp;

namespace LP.Plot.Signal;

public interface ISignal
{
    /// <summary>
    /// x distance between two samples
    /// </summary>
    double Period { get; }
    Span YRange { get; }
    Span XRange { get; }
    Axis YAxis { get; set; }
    SKPaint Paint { get; set; }
    double[] YValues { get; }
    bool IsVisible { get; set; }
    string Name { get; set; }
}