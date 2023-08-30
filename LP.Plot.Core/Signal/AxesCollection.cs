namespace LP.Plot.Core.Signal;

public class AxesCollection : IAxes
{
    public Axis XAxis { get; }
    public IEnumerable<Axis> YAxes { get; }

    public AxesCollection(Axis xAxis, IEnumerable<Axis> yAxes)
    {
        XAxis = xAxis;
        YAxes = yAxes.ToList();
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
    }

    public void ZoomAtRelative(double factor, double relativePosition)
    {
        foreach (var axis in YAxes)
        {
            axis.ZoomAtRelative(factor, relativePosition);
        }
    }
}
