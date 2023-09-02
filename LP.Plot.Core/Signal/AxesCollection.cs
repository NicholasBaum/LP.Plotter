using LP.Plot.Core.Primitives;

namespace LP.Plot.Core.Signal;

public class AxesCollection : IAxes
{
    private readonly ISignalPlot plot;

    public Axis XAxis => plot.XAxis;
    private List<Axis> yAxes => plot.YAxes.Distinct().Where(x => x != XAxis).ToList();
    public IEnumerable<Axis> YAxes => yAxes;

    public AxesCollection(ISignalPlot plot)
    {
        this.plot = plot;
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
