using LP.Plot.Core.Primitives;

namespace LP.Plot.Core.Signal;

public interface ISignalPlot : IRenderable
{
    public Axis XAxis { get; }
    public IReadOnlyList<Axis> YAxes { get; }
    public void Remove(ISignal signal);

    void PanRelativeX(double offset);
    void PanRelative(double offset);
    void ZoomAtRelativeX(double factor, double position);
    void ZoomAtRelative(double factor, double position);
    void ZoomX(Span value);
}