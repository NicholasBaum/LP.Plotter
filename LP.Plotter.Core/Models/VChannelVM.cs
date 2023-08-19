using OxyPlot;
using OxyPlot.Series;

namespace LP.Plotter.Core.Models;

public class VChannelVM : LineSeries
{
    private static int lastId = 0;
    public int Id { get; } = lastId++;
    public string Name { get; set; }
    public bool Selected { get; set; }

    public VChannelVM(string name)
    {
        Name = name;
        Title = name;
        TrackerFormatString = "{0}\n{1}: {2:0.00}\n{3}: {4:0.00}";
    }

    public VChannelVM(string name, IEnumerable<DataPoint> points) : this(name)
    {
        this.Points.AddRange(points);
    }
}

//{0} the title of the series
//{1} the title of the x-axis
//{2} the x-value
//{3} the title of the y-axis
//{4} the y-value