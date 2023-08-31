using LP.Plotter.Core.Models;

namespace LP.Plotter.Services;

public class AppService
{
    public ChannelPlotModel CurrentPlot { get; } = new();
    public LP.Plot.Core.Plot? Plot { get; set; }
}
