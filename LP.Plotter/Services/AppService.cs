using LP.Plotter.Core.Models;
using LP.Plotter.Data;

namespace LP.Plotter.Services;

public class AppService
{
    public ChannelPlotModel CurrentPlot { get; } = new();
    public PlotVM? CurrentModel { get; set; }
}
