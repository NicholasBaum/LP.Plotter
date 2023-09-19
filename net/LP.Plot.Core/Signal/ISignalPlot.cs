using LP.Plot.Primitives;

namespace LP.Plot.Signal;

public interface ISignalPlot : IRenderable
{
    public Axis XAxis { get; }
    public IReadOnlyList<Axis> YAxes { get; }
    public IReadOnlyList<ISignal> Signals { get; }

    public void Add(ISignal signal);
    public void Remove(ISignal signal);

    public void PanRelativeX(double offset)
    {
        XAxis.PanRelative(offset);
    }

    public void PanRelative(double offset)
    {
        foreach (var axis in YAxes)
        {
            axis.PanRelative(offset);
        }
    }

    public void ZoomAtRelativeX(double factor, double position)
    {
        XAxis.ZoomAtRelative(factor, position);
    }

    public void ZoomAtRelative(double factor, double position)
    {
        foreach (var axis in YAxes)
        {
            axis.ZoomAtRelative(factor, position);
        }
    }

    public void ZoomX(Span newRange)
    {
        XAxis.Range = newRange;
    }
}