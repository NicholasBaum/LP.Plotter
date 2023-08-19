using LP.Plotter.Core.Models;

namespace LP.Plotter.Services;

public class AppService
{
    public ChannelPlotModel CurrentPlot { get; } = new();
}
