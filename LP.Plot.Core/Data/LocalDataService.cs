using LP.Plot.Core.Signal;
using static LP.Plot.Core.Data.Helper;

namespace LP.Plot.Core.Data;

public class LocalDataService
{
    public StaticSignal LoadSignal_S() => CreateSignals(LoadTestLap()).First();

    public List<StaticSignal> LoadSignal_M() => CreateSignals(LoadTestLap());

    public List<StaticSignal> LoadSignal_L() => CreateSignals(LoadTestRun());

    public List<StaticSignal> LoadSignal_XL() => CreateSignals(LoadTestSession());

    public ChannelDataSet LoadTestLap()
    {
        var folder = @"D:\work\LP.Plotter\LP.Plotter\wwwroot\csvdata\events\imola_2023\T2303_IMO_#29\D1PMRun1";
        return LoadFile(Directory.GetFiles(folder).First());
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
