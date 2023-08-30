namespace LP.Plot.Core.Signal;

public interface IAxes
{
    public Axis XAxis { get; }
    public IEnumerable<Axis> YAxes { get; }
    public void PanRelativeX(double relativeOffset);
    public void PanRelative(double relativeOffset);
    public void ZoomAtRelativeX(double factor, double relativePosition);
    public void ZoomAtRelative(double factor, double relativePosition);
}
