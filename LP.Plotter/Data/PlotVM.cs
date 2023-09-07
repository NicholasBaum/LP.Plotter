using LP.Plot.Core;
using LP.Plot.Core.Signal;

namespace LP.Plotter.Data;

public class PlotVM
{
    public event EventHandler<EventArgs>? DataChanged;
    public Plot.Core.Plot Plot;
    private readonly string xAxisTitle;

    public IReadOnlyList<SignalSet> Sets => sets;
    private List<SignalSet> sets = new List<SignalSet>();

    public PlotVM(string xAxisTitle)
    {
        this.Plot = new(new ISignal[0], xAxisTitle);
        this.xAxisTitle = xAxisTitle;
    }

    public void Add(IEnumerable<ISignal> signals, string setName)
    {
        this.Plot.Add(signals);
        this.sets.Add(new SignalSet(signals, setName));
        if (this.sets.Count == 1)
            Plot.ResetAxes();
        DataChanged?.Invoke(this, EventArgs.Empty);
        Plot.Invalidate();
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
