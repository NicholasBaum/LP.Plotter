using OxyPlot;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LP.Plotter.Core.Models;

public class VChannelSet
{
    public required string Name { get; init; }
    public required CsvInfo Info { get; init; }
    public List<VChannelVM> Channels = new();
    public VChannelVM? SpeedChannel => Channels.FirstOrDefault(x => x.Name.Contains("Speed"));

    public static List<VChannelVM> ParseCSV(string csvData)
    {
        var lines = csvData.Split('\n').Where(x => !string.IsNullOrEmpty(x)).ToArray();
        var headers = lines[0].Split(',');
        var data = new List<VChannelVM>();

        for (var j = 0; j < headers.Count() - 1; j++)
        {
            data.Add(new VChannelVM(headers[j]));
        }

        for (var i = 1; i < lines.Count(); i++)
        {
            var values = lines[i].Split(',');
            var splits = values[0].Split(":").Select(float.Parse).ToArray(); // e.g. 112:52
            var time = splits[0] + splits[1] / 100;
            data.First().Points.Add(new DataPoint(time, time));

            for (var j = 1; j < values.Count() - 1; j++)
            {
                data[j].Points.Add(new DataPoint(time, float.Parse(values[j], NumberStyles.Float, CultureInfo.InvariantCulture)));
            }
        }
        return data;
    }

    public static VChannelSet Create(CsvInfo info, string csvDataString)
    {
        var channels = ParseCSV(csvDataString);
        return new VChannelSet()
        {
            Name = GetSimpleLapName(info.Path),
            Info = info,
            Channels = channels,
        };
    }

    public static VChannelSet CreateMerged(IEnumerable<VChannelSet> set)
    {
        var channels = new List<VChannelVM>();
        foreach (var s in set)
        {
            if (!channels.Any())
            {
                channels.AddRange(s.Channels.Select(c => new VChannelVM(c.Name, c.Points)));
                continue;
            }

            foreach (var c in s.Channels)
            {
                var pc = channels.First(x => x.Name == c.Name);
                var time = pc.Points.Last().X;
                List<DataPoint> shiftedPoints;
                if (pc.Name.ToLowerInvariant().Contains("time"))
                    shiftedPoints = c.Points.Select(x => new DataPoint(time + x.X, time + x.Y)).ToList();
                else
                    shiftedPoints = c.Points.Select(x => new DataPoint(time + x.X, x.Y)).ToList();
                pc.Points.AddRange(shiftedPoints);
            }

        }

        var info = set.First().Info;

        return new VChannelSet()
        {
            Name = GetSimpleRunName(info.Path),
            Info = info,
            Channels = channels,
        };
    }

    private static string GetSimpleRunName(string path)
    {
        var (carNumber, runNumber, lapNumber) = ParsePath(path);
        return $"{carNumber}_{runNumber}";
    }

    private static string GetSimpleLapName(string path)
    {
        var (carNumber, runNumber, lapNumber) = ParsePath(path);
        return $"{carNumber}_{runNumber}_{lapNumber}";
    }

    private static (string car, string run, string lap) ParsePath(string path)
    {
        //\events\imola_2023\T2303_IMO_#29\D1PMRun1\Tr491_Abs00006148_CAR 29_Lap0_cableData.csv to  "29_D1PMRun1_06148        
        Match match = Regex.Match(path, @"#(\d+)\\(.*)\\.*Abs\d{3}(\d{5})");
        if (!match.Success || match.Groups.Count < 4)
        {
            return ("car00", "run00", "lap00");
        }

        string carNumber = match.Groups[1].Value;
        string runNumber = match.Groups[2].Value;
        string lapNumber = match.Groups[3].Value;

        return (carNumber, runNumber, lapNumber);
    }
}
