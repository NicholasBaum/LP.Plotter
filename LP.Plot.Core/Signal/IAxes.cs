namespace LP.Plot.Core.Signal;

public interface IAxes
{
    public Axis XAxis { get; }
    public IEnumerable<Axis> YAxes { get; }
    public void PanRelativeX(double offset);
    public void PanRelative(double offset);
    public void ZoomAtRelativeX(double factor, double position);
    public void ZoomAtRelative(double factor, double position);
}
