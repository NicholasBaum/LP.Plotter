using LP.Plotter.Core.Models;
using LP.Plot.Core.Signal;
using LP.Plot.Core.Primitives;
using LP.Plot.Core;

namespace LP.Plotter.Core.Services;

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

    private List<ISignal> CreateSignals(ChannelDataSet data)
    {
        List<ISignal> signals = new List<ISignal>();
        Axis TempAxis = new();
        var time = data.Channels.First(x => x.Name.Contains("Time"));
        foreach (var c in data.Channels.Where(x => x.Name.Contains("Speed") || x.Name.Contains("TT")))
        {
            var signal = new StaticSignal(c.YValues, new Span(time.YValues.First(), time.YValues.Last()));
            signal.YAxis = new Axis() { Min = c.YValues.Min(), Max = c.YValues.Max() };
            signals.Add(signal);

            if (c.Name.Contains("TT"))
            {
                TempAxis.Min = Math.Min(TempAxis.Min, signal.YAxis.Min);
                TempAxis.Max = Math.Max(TempAxis.Max, signal.YAxis.Max);
                signal.YAxis = TempAxis;
            }
            else
            {
                signal.YAxis.ZoomAtCenter(1.1f);
            }
        }
        TempAxis.ZoomAtCenter(1.1f);
        return signals;
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
