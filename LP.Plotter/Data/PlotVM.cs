using LP.Plot.Core;
using LP.Plot.Core.Signal;

namespace LP.Plotter.Data;

public class PlotVM
{
    public readonly Plot.Core.Plot Plot;

    public PlotVM(IEnumerable<ISignal> signals, string xAxisTitle)
    {
        this.Plot = new(signals, xAxisTitle);
        this.Sets.Add(new SignalSet(signals, "Default Set"));
    }

    public List<SignalSet> Sets { get; set; } = new();

    public void Remove(SignalSet set)
    {
        Sets.Remove(set);
        foreach (var s in set.Channels)
            Plot.Remove(s.Source);
        Plot.Invalidate();
    }

    public void Invalidate() => Plot.Invalidate();
}
