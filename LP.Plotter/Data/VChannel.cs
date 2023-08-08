using OxyPlot;
using OxyPlot.Series;

namespace LP.Plotter.Data;

public class VChannel : LineSeries
{
    public string Name { get; set; }

    public VChannel(string name)
    {
        Name = name;
    }

    public VChannel(string name, IEnumerable<DataPoint> points)
    {
        Name = name;
        this.Points.AddRange(points);
    }
}
