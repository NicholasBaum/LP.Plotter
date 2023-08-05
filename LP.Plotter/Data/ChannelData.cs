using OxyPlot;
using OxyPlot.Series;

namespace LP.Plotter.Data;

public class ChannelData : LineSeries
{
    public string Name { get; set; }

    public ChannelData(string name)
    {
        Name = name;
    }

    public ChannelData(string name, IEnumerable<DataPoint> points)
    {
        Name = name;
        this.Points.AddRange(points);
    }
}
