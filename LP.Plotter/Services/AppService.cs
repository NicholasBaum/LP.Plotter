using LP.Plot.Core.Data;
using LP.Plot.Core.Signal;
using LP.Plotter.Core.Models;
using LP.Plotter.Data;
using OxyPlot;

namespace LP.Plotter.Services;

public class AppService
{
    public ChannelPlotModel CurrentPlot { get; } = new();
    public PlotVM CurrentModel { get; } = new("Time");

    public void Add(ChannelDataSet data)
    {
        var inUseAxes = CurrentModel.Sets.SelectMany(x => x.Channels.Select(x => x.YAxis))
            .Where(x => !string.IsNullOrEmpty(x.Key))
            .Distinct();
        var signals = Helper.CreateSignals(data, inUseAxes.ToList());
        this.CurrentModel.Add(signals, data.Name);
        AddToOxyModel(signals, data.Name, data.Info);
    }

    private void AddToOxyModel(List<StaticSignal> signals, string name, CsvInfo info)
    {
        var oxyInfo = new CsvInfo() { FileName = info.FileName, Path = info.Path, Url = info.Url };
        var channels = signals.Select(x => new VChannelVM(x.Name, x.YValues.Select((y, i) => new DataPoint(i * x.Period, y))));
        var oxy = new VChannelSet() { Name = name, Info = oxyInfo, Channels = channels.ToList() };
        CurrentPlot.Add(oxy);
    }
}
