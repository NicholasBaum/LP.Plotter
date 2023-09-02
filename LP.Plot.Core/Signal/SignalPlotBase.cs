using LP.Plot.Core.Primitives;

namespace LP.Plot.Core.Signal;

public abstract class SignalPlotBase : ISignalPlot
{
    public abstract Axis XAxis { get; }

    public abstract IReadOnlyList<Axis> YAxes { get; }

    public virtual void PanRelativeX(double offset)
    {
        XAxis.PanRelative(offset);
    }

    public virtual void PanRelative(double offset)
    {
        foreach (var axis in YAxes)
        {
            axis.PanRelative(offset);
        }
    }

    public virtual void ZoomAtRelativeX(double factor, double position)
    {
        XAxis.ZoomAtRelative(factor, position);
    }

    public virtual void ZoomAtRelative(double factor, double position)
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

    public abstract void Remove(ISignal signal);
    public abstract void Render(IRenderContext ctx);
}
