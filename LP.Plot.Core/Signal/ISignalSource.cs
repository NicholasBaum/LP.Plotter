using LP.Plot.Core.Primitives;

namespace LP.Plot.Core.Signal;

public interface ISignalSource
{
    /// <summary>
    /// x distance between two samples
    /// </summary>
    double Period { get; }
    Span YRange { get; }
    Span XRange { get; }
    double[] YValues { get; }
}
