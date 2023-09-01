namespace LP.Plot.Core.Signal;

public interface ISignalPlot : IRenderable
{
    public IAxes Axes { get; }
    public void Remove(ISignal signal);
}