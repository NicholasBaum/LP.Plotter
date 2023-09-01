using LP.Plot.Core.Primitives;

namespace LP.Plot.Core.Signal;

public class AxesCollection : IAxes
{
    public Axis XAxis { get; private set; }
    private HashSet<Axis> yAxes { get; }
    public IEnumerable<Axis> YAxes => yAxes;

    public AxesCollection(Axis xAxis, IEnumerable<Axis> yAxes)
    {
        XAxis = xAxis;
        this.yAxes = new HashSet<Axis>(yAxes.Where(x => x != xAxis));
    }

    public void Reset(Axis xAxis, IEnumerable<Axis> yAxes)
    {
        XAxis = xAxis;
        this.yAxes.Clear();
        foreach (var axis in yAxes.Where(x => x != xAxis))
            this.yAxes.Add(axis);
    }

    public virtual void PanRelativeX(double offset)
    {
        XAxis.PanRelative(offset);
    }

    public virtual void PanRelative(double offset)
    {
        foreach (var axis in yAxes)
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
        foreach (var axis in yAxes)
        {
            axis.ZoomAtRelative(factor, position);
        }
    }

    public void ZoomX(Span newRange)
    {
        XAxis.Range = newRange;
    }
}
