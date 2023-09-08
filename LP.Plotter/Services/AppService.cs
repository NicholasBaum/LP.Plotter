﻿using LP.Plot.Core.Data;
using LP.Plot.Core.Signal;
using LP.Plot.Data;
using LP.Plot.Models;
using OxyPlot;

namespace LP.Plot.Services;

public class AppService
{
    public ChannelPlotModel OxyModel { get; } = new();
    public PlotVM CurrentModel { get; } = new("Time");

    public void Add(ChannelDataSet data)
    {
        var inUseAxes = CurrentModel.Sets.SelectMany(x => x.Channels.Select(x => x.YAxis))
            .Where(x => !string.IsNullOrEmpty(x.Key))
            .Distinct();
        var signals = Helper.CreateSignals(data, inUseAxes.ToList());
        this.CurrentModel.Add(signals, data.Name);
        OxyModel.Add(signals, data.Name, data.Info);
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
