using LP.Plot.Signal;

namespace LP.Plot.Data;

public class PlotModelVM
{
    public event EventHandler<EventArgs>? DataChanged;
    public PlotModel Plot;

    public IReadOnlyList<SignalSetVM> Sets => sets;
    private List<SignalSetVM> sets = new List<SignalSetVM>();

    public List<Axis> YAxes => Sets.SelectMany(x => x.Channels.Select(x => x.YAxis)).Distinct().ToList();

    public PlotModelVM(string xAxisTitle)
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