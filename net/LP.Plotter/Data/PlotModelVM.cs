using LP.Plot.Signal;
using System.Threading.Channels;

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
            Plot.ResetXAxis();
        DataChanged?.Invoke(this, EventArgs.Empty);
        Plot.Invalidate();
    }

    public void AddSpecial(IEnumerable<ISignal> signals, string setName)
    {
        this.Plot.Add(signals);
        var vm = new SignalSetVM(signals, setName);
        foreach (var x in vm.Channels)
            x.Selected = x.Name.Contains("Speed") || x.Name.Contains("TT");
        this.sets.Add(vm);
        if (this.sets.Count == 1)
            Plot.ResetXAxis();
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

    public void ZoomOutX()
    {
        Plot.ZoomOutX();
        Invalidate();
    }

    public void ResetYAxes()
    {
        Plot.ResetYAxes();
        Invalidate();
    }
}