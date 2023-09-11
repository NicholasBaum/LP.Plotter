using LP.Plot.Data;
using LP.Plot.Signal;
using LP.Plot.Models;
using OxyPlot;

namespace LP.Plot.Services;

public class AppService
{
    public ChannelPlotModel OxyModel { get; } = new();
    public PlotModelVM CurrentModel { get; } = new("Time");

    public void Add(ChannelDataSet data)
    {
        var signals = Helper.CreateSignals(data, CurrentModel.YAxes);
        this.CurrentModel.Add(signals, data.Name);
        OxyModel.Add(signals, data.Name, data.Info);
    }

    public void Add(IEnumerable<ISignal> signals, string name = "")
    {
        this.CurrentModel.Add(signals, name);
    }
}

public static class ChannelPlotmModelExtension
{
    public static void Add(this ChannelPlotModel model, List<StaticSignal> signals, string name, CsvInfo info)
    {
        var channels = signals.Select(x => new VChannelVM(x.Name, x.YValues.Select((y, i) => new DataPoint(i * x.Period, y))));
        var oxy = new VChannelSet() { Name = name, Path = info.Path, Channels = channels.ToList() };
        model.Add(oxy);
    }
}
