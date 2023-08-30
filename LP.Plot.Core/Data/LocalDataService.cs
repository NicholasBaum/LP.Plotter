﻿using LP.Plot.Core.Signal;
using static LP.Plot.Core.Data.Helper;

namespace LP.Plot.Core.Data;

public class LocalDataService
{
    public ISignal LoadSignal_S() => CreateSignals(LoadTestLap()).First();

    public List<ISignal> LoadSignal_M() => CreateSignals(LoadTestLap());

    public List<ISignal> LoadSignal_L() => CreateSignals(LoadTestRun());

    public List<ISignal> LoadSignal_XL() => CreateSignals(LoadTestSession());

    public ChannelDataSet LoadTestLap()
    {
        var folder = @"D:\work\LP.Plotter\LP.Plotter\wwwroot\csvdata\events\imola_2023\T2303_IMO_#29\D1PMRun1";
        return LoadFiles(Directory.GetFiles(folder));
    }

    public ChannelDataSet LoadTestRun()
    {
        var folder = @"D:\work\LP.Plotter\LP.Plotter\wwwroot\csvdata\events\imola_2023\T2303_IMO_#29\D1PMRun1";
        return LoadFiles(Directory.GetFiles(folder));
    }

    public ChannelDataSet LoadTestSession()
    {
        var folder = @"D:\work\LP.Plotter\LP.Plotter\wwwroot\csvdata\events\imola_2023\T2303_IMO_#29\";
        return LoadFiles(Directory.GetFiles(folder, "*.csv", searchOption: SearchOption.AllDirectories));
    }

    private ChannelDataSet LoadFiles(IEnumerable<string> files)
    {
        var set = files.Select(x =>
        {
            var text = File.ReadAllText(x);
            return ChannelDataSet.Create(new CsvInfo()
            {
                FileName = Path.GetFileName(x),
                Path = x,
                Url = x
            }, text);
        }).ToList();
        return ChannelDataSet.CreateMerged(set);
    }
}