using OxyPlot;

namespace LP.Plotter.Data;

public class VChannelSet
{
    public string? Name => Info?.Name;
    public CsvInfo? Info { get; init; }
    public List<ChannelData> Channels = new();
    public ChannelData SpeedChannel => Channels.First(x => x.Name.Contains("Speed"));

    public static List<ChannelData> ParseCSV(string csvData)
    {
        var lines = csvData.Split('\n').Where(x => !string.IsNullOrEmpty(x)).ToArray();
        var headers = lines[0].Split(',');
        var data = new List<ChannelData>();

        for (var j = 0; j < headers.Count() - 1; j++)
        {
            data.Add(new ChannelData(headers[j]));
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
}
