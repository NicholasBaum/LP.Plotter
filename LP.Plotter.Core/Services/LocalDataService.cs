
using LP.Plotter.Core.Models;
using LP.Plot.Core.Signal;
using LP.Plot.Core.Primitives;

namespace LP.Plotter.Core.Services;

public class LocalDataService
{
    public ISignal LoadSignal_M()
    {
        var data = LoadTest();
        var signal = new StaticSignal(data.SpeedChannel.Points.Select(x => x.Y).ToArray(), new Span(data.SpeedChannel.Points.First().X, data.SpeedChannel.Points.Last().X));
        return signal;
        //foreach (var c in data.Channels.Where(x => x.Name.Contains("Speed") || x.Name.Contains("TT")))
        //{
        //    var yValues = c.Points.Select(x => (float)x.Y).ToArray();
        //    var yaxis = new Axis() { Min = (float)yValues.Min(), Max = (float)yValues.Max() }.Scale(1.1f);
        //    if (c.Name.Contains("TT"))
        //    {
        //        TempAxis.Min = Math.Min(TempAxis.Min, yaxis.Min);
        //        TempAxis.Max = Math.Max(TempAxis.Max, yaxis.Max);
        //        yaxis = TempAxis;
        //    }
        //    channels.Add((yValues, yaxis, SKPaints.NextPaint()));
        //}
        //var min = data.SpeedChannel.Points.First().X;
        //var max = data.SpeedChannel.Points.Last().X;
        //XAxis = new Axis() { Min = (float)min, Max = (float)max };
        //XDataRange = new Axis() { Min = (float)min, Max = (float)max };
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
