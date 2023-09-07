using LP.Plot.Core;
using LP.Plot.Core.Signal;

namespace LP.Plotter.Data;

public class PlotVM
{
    public readonly Plot.Core.Plot Plot;
    public IReadOnlyList<SignalSet> Sets => sets;
    private List<SignalSet> sets = new List<SignalSet>();

    public PlotVM(IEnumerable<ISignal> signals, string xAxisTitle)
    {
        this.Plot = new(signals, xAxisTitle);
        this.sets.Add(new SignalSet(signals, "Default Set"));
    }

    public void Remove(SignalSet set)
    {
        sets.Remove(set);
        foreach (var s in set.Channels)
            Plot.Remove(s.Source);
        Plot.Invalidate();
    }

    public void Invalidate() => Plot.Invalidate();
}
