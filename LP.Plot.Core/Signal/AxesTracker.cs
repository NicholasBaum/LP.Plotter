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
    public List<AxisWrapper> YWrappers { get; } = new List<AxisWrapper>();
    private IEnumerable<Axis> DistinctYAxes => YWrappers.Select(x => x.Source).Distinct();
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
        foreach (var axis in DistinctYAxes)
        {
            Debug.WriteLine(axis);
            axis.PanRelative(relativeOffset);
            Debug.WriteLine(axis);
        }
    }

    public void ZoomAtRelativeX(double factor, double relativePosition)
    {
        XAxis.Source.ZoomAtRelative(factor, relativePosition);
        isDirty = true;
    }

    public void ZoomAtRelative(double factor, double relativePosition)
    {
        foreach (var axis in DistinctYAxes)
        {
            axis.ZoomAtRelative(factor, relativePosition);
            Debug.WriteLine(axis);
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
