using OxyPlot;
using System.Text.RegularExpressions;

namespace LP.Plotter.Data;

public class VChannelSet
{
    public required string Name { get; init; }
    public required CsvInfo Info { get; init; }
    public List<VChannelVM> Channels = new();
    public VChannelVM SpeedChannel => Channels.First(x => x.Name.Contains("Speed"));

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
                data[j].Points.Add(new DataPoint(time, float.Parse(values[j])));
            }
        }
        return data;
    }

    public static VChannelSet Create(CsvInfo info, string csvDataString)
    {
        var channels = ParseCSV(csvDataString);
        return new VChannelSet()
        {
            Name = GetSimpleName(info.Path),
            Info = info,
            Channels = channels,
        };
    }

    private static string GetSimpleName(string input)
    {
        //\events\imola_2023\T2303_IMO_#29\D1PMRun1\Tr491_Abs00006148_CAR 29_Lap0_cableData.csv to  "29_D1PMRun1_06148        
        Match match = Regex.Match(input, @"#(\d+)\\(.*)\\.*Abs\d{3}(\d{5})");
        if (!match.Success || match.Groups.Count < 4)
        {
            return "Invalid input format";
        }

        string carNumber = match.Groups[1].Value;
        string runNumber = match.Groups[2].Value;
        string lapNumber = match.Groups[3].Value;

        string output = $"{carNumber}_{runNumber}_{lapNumber}";

        return output;
    }
}
