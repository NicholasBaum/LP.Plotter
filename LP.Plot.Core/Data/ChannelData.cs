using System.Globalization;
using System.Text.RegularExpressions;

namespace LP.Plot.Core.Data;

public class ChannelDataSet
{
    public required string Name { get; init; }
    public required CsvInfo Info { get; init; }
    public List<ChannelData> Channels = new();
    public ChannelData SpeedChannel => Channels.First(x => x.Name.Contains("Speed"));

    public static List<ChannelData> ParseCSV(string csvData)
    {
        var lines = csvData.Split('\n').Where(x => !string.IsNullOrEmpty(x)).ToArray();
        var headers = lines[0].Split(',');
        var channels = new List<(string Name, List<double> YValues)>();

        for (var j = 0; j < headers.Length - 1; j++)
            channels.Add(new(headers[j], new List<double>(lines.Length + 10)));

        for (var i = 1; i < lines.Length; i++)
        {
            var values = lines[i].Split(',');
            var splits = values[0].Split(":").Select(float.Parse).ToArray(); // e.g. 112:52
            var time = splits[0] + splits[1] / 100;
            channels.First().YValues.Add(time);

            for (var j = 1; j < values.Length - 1; j++)
            {
                channels[j].YValues.Add(float.Parse(values[j], NumberStyles.Float, CultureInfo.InvariantCulture));
            }
        }
        return channels.Select(x => new ChannelData(x.Name, x.YValues.ToArray())).ToList();
    }

    public static ChannelDataSet Create(CsvInfo info, string text)
    {
        var channels = ParseCSV(text);
        return new ChannelDataSet()
        {
            Name = GetSimpleLapName(info.Path),
            Info = info,
            Channels = channels,
        };
    }

    public static ChannelDataSet CreateMerged(IEnumerable<ChannelDataSet> set)
    {
        var channels = new List<(string Name, List<double> YValues)>();
        foreach (var c in set.First().Channels)
            channels.Add(new(c.Name, c.YValues.ToList()));
        var time = channels.First(x => x.Name.ToLowerInvariant().Contains("time"));
        foreach (var s in set.Skip(1))
        {
            foreach (var c in s.Channels)
            {
                if (c.Name.ToLowerInvariant().Contains("time"))
                {
                    var shiftedTime = c.YValues.Select(x => x + time.YValues.Last()).ToArray();
                    time.YValues.AddRange(shiftedTime);
                }
                else
                {
                    channels.First(x => x.Name == c.Name).YValues.AddRange(c.YValues);
                }
            }

        }

        var info = set.First().Info;

        return new ChannelDataSet()
        {
            Name = GetSimpleRunName(info.Path),
            Info = info,
            Channels = channels.Select(x => new ChannelData(x.Name, x.YValues.ToArray())).ToList(),
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

public class ChannelData
{
    public string Name { get; }
    public double[] YValues { get; }

    public ChannelData(string name, double[] yValues)
    {
        Name = name;
        YValues = yValues;
    }
}