using OxyPlot;
using OxyPlot.Series;

namespace LP.Plotter.Data;

public class VChannelVM : LineSeries
{
    private static int lastId = 0;
    public int Id { get; } = lastId++;
    public string Name { get; set; }
    public bool Selected { get; set; }
    
    public VChannelVM(string name)
    {
        Name = name;
    }

    public VChannelVM(string name, IEnumerable<DataPoint> points)
    {
        Name = name;
        this.Points.AddRange(points);
    }
}
