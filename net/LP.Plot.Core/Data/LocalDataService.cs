﻿using LP.Plot.Signal;
using static LP.Plot.Data.Helper;

namespace LP.Plot.Data;

public class LocalDataService
{
    public StaticSignal LoadSignal_S() => CreateEssentialSignals(LoadTestLap()).First();

    public List<StaticSignal> LoadSignal_M() => CreateEssentialSignals(LoadTestLap());

    public List<StaticSignal> LoadSignal_L() => CreateEssentialSignals(LoadTestRun());

    public List<StaticSignal> LoadSignal_XL() => CreateEssentialSignals(LoadTestSession());

    public ChannelDataSet LoadTestLap()
    {
        var folder = @"D:\work\LP.Plotter\net\LP.Plotter\wwwroot\csvdata\events\imola_2023\T2303_IMO_#29\D1PMRun1";
        return LoadFile(Directory.GetFiles(folder).First());
    }

    public ChannelDataSet LoadTestRun()
    {
        var folder = @"D:\work\LP.Plotter\net\LP.Plotter\wwwroot\csvdata\events\imola_2023\T2303_IMO_#29\D1PMRun1";
        return LoadFiles(Directory.GetFiles(folder));
    }

    public ChannelDataSet LoadTestSession()
    {
        var folder = @"D:\work\LP.Plotter\net\LP.Plotter\wwwroot\csvdata\events\imola_2023\T2303_IMO_#29\";
        return LoadFiles(Directory.GetFiles(folder, "*.csv", searchOption: SearchOption.AllDirectories));
    }

    private ChannelDataSet LoadFile(string file)
    {
        var text = File.ReadAllText(file);
        return ChannelDataSet.Create(new CsvInfo()
        {
            FileName = Path.GetFileName(file),
            Path = file,
            Url = file
        }, text);
    }

    private ChannelDataSet LoadFiles(IEnumerable<string> files)
    {
        var set = files.Select(LoadFile).ToList();
        return ChannelDataSet.CreateMerged(set);
    }
}
