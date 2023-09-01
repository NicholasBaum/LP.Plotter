using LP.Plot.Core.Primitives;

namespace LP.Plot.Core.Signal;

public class AxesTracker : IAxes
{
    public Axis XAxis => XAxisTracked.Source;
    public IEnumerable<Axis> YAxes => YAxesTracked.Select(x => x.Source);

    private AxisTracker XAxisTracked;
    private List<AxisTracker> YAxesTracked = new List<AxisTracker>();
    private bool wasZoomed = false;
    private bool wasReset = false;

    public AxesTracker(Axis xAxis, IEnumerable<Axis> yAxes)
    {
        XAxisTracked = new AxisTracker(xAxis);
        YAxesTracked = yAxes
            .Distinct()
            .Where(x => x != XAxis)
            .Select(x => new AxisTracker(x))
            .ToList();
    }

    public void Reset(Axis xAxis, IEnumerable<Axis> yAxes)
    {
        XAxisTracked = new AxisTracker(xAxis);
        YAxesTracked = yAxes
            .Distinct()
            .Where(x => x != XAxis)
            .Select(x => new AxisTracker(x))
            .ToList();
        wasReset = true;
    }

    public bool ShouldRerender() => wasZoomed || wasReset;

    public void Reset() => wasZoomed = wasReset = false;

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
        wasZoomed = true;
    }

    public void ZoomAtRelative(double factor, double position)
    {
        foreach (var axis in YAxes)
        {
            axis.ZoomAtRelative(factor, position);
        }
        wasZoomed = true;
    }

    public void ZoomX(Span newRange)
    {
        XAxis.Range = newRange;
        wasZoomed = true;
    }

    private class AxisTracker
    {
        public Axis Source { get; }
        public AxisTracker(Axis axis)
        {
            Source = axis;
        }
    }
}
