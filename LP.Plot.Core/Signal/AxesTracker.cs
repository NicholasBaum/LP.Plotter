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
    private List<AxisWrapper> YAxes { get; } = new List<AxisWrapper>();
    internal AxisWrapper XAxis { get; }

    public AxesTracker(Axis xAxis)
    {
        XAxis = new AxisWrapper(xAxis);
    }

    public void AddY(Axis yAxis)
    {
        YAxes.Add(new(yAxis));
    }

    public void PanRelativeX(double relativeOffset)
    {
        XAxis.Source.PanRelative(relativeOffset);
        this.RelativeOffsetX += relativeOffset;

    }

    public void PanRelative(double relativeOffset)
    {
        foreach (var axis in YAxes)
        {
            axis.Source.PanRelative(relativeOffset);
        }
        this.RelativeOffsetY += relativeOffset;
    }

    public void ZoomAtRelativeX(double factor, double relativePosition)
    {
        XAxis.Source.ZoomAtRelative(factor, relativePosition);
        RelativeOffsetX *= factor;
        isDirty = true;
    }

    public void ZoomAtRelative(double factor, double relativePosition)
    {
        foreach (var axis in YAxes)
        {
            axis.Source.ZoomAtRelative(factor, relativePosition);            
        }
        RelativeOffsetY *= factor;
        isDirty = true;
    }

    public double RelativeOffsetY { get; private set; }
    public double RelativeOffsetX { get; private set; }

    private bool isDirty = false;
    public bool IsDirty()
    {
        return isDirty;
    }

    public void Reset()
    {
        RelativeOffsetY = 0.0;
        isDirty = false;
    }
}
