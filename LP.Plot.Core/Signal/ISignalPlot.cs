namespace LP.Plot.Core.Signal;

public interface ISignalPlot : IRenderable
{
    public IAxes Axes { get; }
}
