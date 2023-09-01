using LP.Plot.Core.Primitives;

namespace LP.Plot.Core.Signal;

public interface IAxes
{
    Axis XAxis { get; }
    IEnumerable<Axis> YAxes { get; }
    //TODO: doesn't seem like a legit method could just create new ones so far
    void Reset(Axis xAxis, IEnumerable<Axis> yAxes);
    void PanRelativeX(double offset);
    void PanRelative(double offset);
    void ZoomAtRelativeX(double factor, double position);
    void ZoomAtRelative(double factor, double position);
    void ZoomX(Span value);
}
