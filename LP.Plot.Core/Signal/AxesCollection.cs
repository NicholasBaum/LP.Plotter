using LP.Plot.Core.Primitives;

namespace LP.Plot.Core.Signal;

public class AxesCollection : IAxes
{
    public Axis XAxis { get; }
    public HashSet<Axis> YAxes { get; }

    IEnumerable<Axis> IAxes.YAxes => YAxes;

    public AxesCollection(Axis xAxis, IEnumerable<Axis> yAxes)
    {
        XAxis = xAxis;
        YAxes = new HashSet<Axis>(yAxes.Where(x => x != xAxis));
    }

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
}
