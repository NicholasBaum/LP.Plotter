namespace LP.Plot.Core.Signal;

public class AxesTracker : IAxes
{
    public Axis XAxis => XAxisTracked.Source;
    public IEnumerable<Axis> YAxes => YAxesTracked.Select(x => x.Source).Distinct();

    private AxisWrapper XAxisTracked { get; }
    private List<AxisWrapper> YAxesTracked { get; } = new List<AxisWrapper>();

    public AxesTracker(Axis xAxis, IEnumerable<Axis> yAxes)
    {
        XAxisTracked = new AxisWrapper(xAxis);
        YAxesTracked = yAxes.Select(x => new AxisWrapper(x)).ToList();
    }

    public void PanRelativeX(double relativeOffset)
    {
        XAxis.PanRelative(relativeOffset);
    }

    public void PanRelative(double relativeOffset)
    {
        foreach (var axis in YAxes)
        {
            axis.PanRelative(relativeOffset);
        }
    }

    public void ZoomAtRelativeX(double factor, double relativePosition)
    {
        XAxis.ZoomAtRelative(factor, relativePosition);
        isDirty = true;
    }

    public void ZoomAtRelative(double factor, double relativePosition)
    {
        foreach (var axis in YAxes)
        {
            axis.ZoomAtRelative(factor, relativePosition);
        }
        isDirty = true;
    }

    private bool isDirty = false;
    public bool IsDirty()
    {
        return isDirty;
    }

    public void Reset()
    {
        isDirty = false;
    }

    private class AxisWrapper
    {
        public Axis Source { get; }
        public AxisWrapper(Axis axis)
        {
            Source = axis;
        }
    }
}
