
using LP.Plotter.Core.Models;

namespace LP.Plotter.Core.Services;

public class LocalDataService
{
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
