namespace LP.Plot.Core.Signal;

public interface ISignalPlot : IRenderable
{
    public IAxes Axes { get; }
    public Axis XAxis { get; }
    public IReadOnlyList<Axis> YAxes { get; }
    public void Remove(ISignal signal);
}