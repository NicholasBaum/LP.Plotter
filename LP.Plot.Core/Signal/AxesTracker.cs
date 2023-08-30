using System.Diagnostics;

namespace LP.Plot.Core.Signal;

internal class AxisWrapper
{
    public Axis Source { get; }
    public AxisWrapper(Axis axis)
    {
        Source = axis;
    }
}

internal class AxesTracker
{
    private List<AxisWrapper> YWrappers { get; } = new List<AxisWrapper>();
    public IEnumerable<Axis> YAxes => YWrappers.Select(x => x.Source).Distinct();
    public AxisWrapper XAxis { get; }

    public AxesTracker(Axis xAxis)
    {
        XAxis = new AxisWrapper(xAxis);
    }

    public void AddY(Axis yAxis)
    {
        YWrappers.Add(new(yAxis));
    }

    public void PanRelativeX(double relativeOffset)
    {
        XAxis.Source.PanRelative(relativeOffset);
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
        XAxis.Source.ZoomAtRelative(factor, relativePosition);
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
}
