namespace LP.Plot.Core.Signal;

public class AxesTracker : IAxes
{
    public Axis XAxis => XAxisTracked.Source;
    public IEnumerable<Axis> YAxes => YAxesTracked.Select(x => x.Source);

    private AxisTracker XAxisTracked { get; }
    private List<AxisTracker> YAxesTracked { get; } = new List<AxisTracker>();
    private bool wasZoomed = false;

    public AxesTracker(Axis xAxis, IEnumerable<Axis> yAxes)
    {
        XAxisTracked = new AxisTracker(xAxis);
        YAxesTracked = yAxes
            .Distinct()
            .Where(x => x != XAxis)
            .Select(x => new AxisTracker(x))
            .ToList();
    }

    public bool ShouldRerender() => wasZoomed;

    public void Reset() => wasZoomed = false;

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

    private class AxisTracker
    {
        public Axis Source { get; }
        public AxisTracker(Axis axis)
        {
            Source = axis;
        }
    }
}
