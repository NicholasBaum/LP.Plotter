
using LP.Plotter.Core.Models;
using LP.Plot.Core.Signal;
using LP.Plot.Core.Primitives;
using LP.Plot.Core;

namespace LP.Plotter.Core.Services;

public class LocalDataService
{
    public ISignal LoadSignal_M() => LoadSignals_M().First();

    public List<ISignal> LoadSignals_M()
    {
        var data = LoadTest();
        List<ISignal> signals = new List<ISignal>();
        Axis TempAxis = new();
        foreach (var c in data.Channels.Where(x => x.Name.Contains("Speed") || x.Name.Contains("TttT")))
        {
            var first = c.Points.First();
            var last = c.Points.Last();
            var yValues = c.Points.Select(x => x.Y).ToArray();

            var signal = new StaticSignal(yValues, new Span(first.X, last.X));
            signal.YAxis = new Axis() { Min = yValues.Min(), Max = yValues.Max() };
            signals.Add(signal);

            if (c.Name.Contains("TT"))
            {
                TempAxis.Min = Math.Min(TempAxis.Min, signal.YAxis.Min);
                TempAxis.Max = Math.Max(TempAxis.Max, signal.YAxis.Max);
                signal.YAxis = TempAxis;
            }
        }
        TempAxis.ZoomAtCenter(1.1f);
        return signals;
    }

    public VChannelSet LoadTest()
    {
        var folder = @"D:\work\LP.Plotter\LP.Plotter\wwwroot\csvdata\events\imola_2023\T2303_IMO_#29\D1PMRun1";
        var files = Directory.GetFiles(folder);
        var set = files.Select(x =>
        {
            var text = File.ReadAllText(x);
            return VChannelSet.Create(new CsvInfo()
            {
                FileName = Path.GetFileName(x),
                Path = x,
                Url = x
            }, text);
        }).ToList();
        return VChannelSet.CreateMerged(set);
    }

    public VChannelSet LoadTest2()
    {
        var folder = @"D:\work\LP.Plotter\LP.Plotter\wwwroot\csvdata\events\imola_2023\T2303_IMO_#29\";
        var files = Directory.GetFiles(folder, "*.csv", searchOption: SearchOption.AllDirectories);
        var set = files.Select(x =>
        {
            var text = File.ReadAllText(x);
            return VChannelSet.Create(new CsvInfo()
            {
                FileName = Path.GetFileName(x),
                Path = x,
                Url = x
            }, text);
        }).ToList();
        return VChannelSet.CreateMerged(set);
    }

    public VChannelSet LoadTest3()
    {
        var folder = @"D:\work\LP.Plotter\LP.Plotter\wwwroot\csvdata\events\imola_2023\T2303_IMO_#29\D1PMRun1";
        var files = Directory.GetFiles(folder);
        var set = files.Select(x =>
        {
            var text = File.ReadAllText(x);
            return VChannelSet.Create(new CsvInfo()
            {
                FileName = Path.GetFileName(x),
                Path = x,
                Url = x
            }, text);
        }).ToList();
        return set.First();
    }
}
