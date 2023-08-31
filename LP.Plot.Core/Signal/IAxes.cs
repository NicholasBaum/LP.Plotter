using LP.Plot.Core.Primitives;

namespace LP.Plot.Core.Signal;

public interface IAxes
{
    Axis XAxis { get; }
    IEnumerable<Axis> YAxes { get; }
    void PanRelativeX(double offset);
    void PanRelative(double offset);
    void ZoomAtRelativeX(double factor, double position);
    void ZoomAtRelative(double factor, double position);
    void ZoomX(Span value);
}
