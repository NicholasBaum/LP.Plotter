using LP.Plot.Core.Signal;

namespace LP.Plot.Data;

public class PlotVM
{
    public event EventHandler<EventArgs>? DataChanged;
    public Plot.Core.Plot Plot;

    public IReadOnlyList<SignalSetVM> Sets => sets;
    private List<SignalSetVM> sets = new List<SignalSetVM>();

    public PlotVM(string xAxisTitle)
    {
        this.Plot = new(xAxisTitle);
    }

    public void Add(IEnumerable<ISignal> signals, string setName)
    {
        this.Plot.Add(signals);
        this.sets.Add(new SignalSetVM(signals, setName));
        if (this.sets.Count == 1)
        {
            Plot.ResetAxes();
            Plot.SetDefaultYAxes();
        }
        DataChanged?.Invoke(this, EventArgs.Empty);
        Plot.Invalidate();
    }

    public void Remove(SignalSetVM set)
    {
        sets.Remove(set);
        foreach (var s in set.Channels)
            Plot.Remove(s.Source);
        Plot.Invalidate();
    }

    public void Invalidate() => Plot.Invalidate();

    public void ZoomOut() { }
    public void ZoomOutMap() { }

}