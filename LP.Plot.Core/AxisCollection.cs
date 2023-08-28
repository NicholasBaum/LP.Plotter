namespace LP.Plot.Core;

internal class AxisCollection
{
    private List<Axis> Axes { get; } = new List<Axis>();
    public Axis? RefAxis => Axes.First();

    public void Add(Axis yAxis)
    {
        Axes.Add(yAxis);
    }

    public void PanRelative(double relativOffset)
    {
        foreach (Axis axis in Axes)
        {
            axis.PanRelative(relativOffset);
        }
    }

    public void ZoomAtRelative(double factor, double relativePosition)
    {
        foreach (Axis axis in Axes)
        {
            axis.ZoomAtRelative(factor, relativePosition);
        }
    }
}
