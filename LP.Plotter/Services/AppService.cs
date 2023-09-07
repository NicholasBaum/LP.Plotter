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
        var signals = Helper.CreateSignals(data);
        this.CurrentModel.Add(signals, data.Name);
        AddToOxyModel(signals, data.Name, data.Info);
    }

    private void AddToOxyModel(List<StaticSignal> signals, string name, LP.Plot.Core.Data.CsvInfo info)
    {
        var oxyInfo = new Core.Models.CsvInfo() { FileName = info.FileName, Path = info.Path, Url = info.Url };
        var channels = signals.Select(x => new VChannelVM(x.Name, x.YValues.Select((y, i) => new DataPoint(i * x.Period, y))));
        var oxy = new VChannelSet() { Name = name, Info = oxyInfo, Channels = channels.ToList() };
        CurrentPlot.Add(oxy);
    }
}
