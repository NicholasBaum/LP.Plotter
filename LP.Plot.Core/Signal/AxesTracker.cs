namespace LP.Plot.Core.Signal;

internal class AxesTracker
{
    private List<Axis> YAxes { get; } = new List<Axis>();
    internal Axis XAxis { get; }

    public AxesTracker(Axis xAxis)
    {
        XAxis = xAxis;
    }

    public void AddY(Axis yAxis)
    {
        YAxes.Add(yAxis);
    }

    public void PanRelativeX(double relativeOffset)
    {
        XAxis.PanRelative(relativeOffset);
        this.RelativeOffsetX += relativeOffset;

    }

    public void PanRelative(double relativeOffset)
    {
        foreach (Axis axis in YAxes)
        {
            axis.PanRelative(relativeOffset);
        }
        this.RelativeOffsetY += relativeOffset;
    }

    public void ZoomAtRelativeX(double factor, double relativePosition)
    {
        XAxis.ZoomAtRelative(factor, relativePosition);
        isDirty = true;
    }

    public void ZoomAtRelative(double factor, double relativePosition)
    {
        foreach (Axis axis in YAxes)
        {
            axis.ZoomAtRelative(factor, relativePosition);
        }
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
