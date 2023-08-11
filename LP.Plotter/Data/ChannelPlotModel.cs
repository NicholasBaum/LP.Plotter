using OxyPlot;
using OxyPlot.Axes;

namespace LP.Plotter.Data;

public class ChannelPlotModel
{
    public event EventHandler<EventArgs>? Changed;

    public List<VChannelSetVM> Sets { get; } = new();
    public List<Axis> Axes { get; } = new();

    public ChannelPlotModel()
    {
        Axes.Add(new LinearAxis
        {
            Position = AxisPosition.Bottom,
            FontSize = 16,
            TextColor = OxyColors.White,
            AxislineColor = OxyColors.White,
            TicklineColor = OxyColors.White,
            TitleColor = OxyColors.White,
            Title = "time",
            Key = "time",
        });
        Axes.Add(new LinearAxis
        {
            Position = AxisPosition.Left,
            FontSize = 16,
            TextColor = OxyColors.White,
            AxislineColor = OxyColors.White,
            TicklineColor = OxyColors.White,
            TitleColor = OxyColors.White,
            Title = "temp",
            Key = "temp",
        }); ;
        Axes.Add(new LinearAxis
        {
            Position = AxisPosition.Right,
            FontSize = 16,
            TextColor = OxyColors.White,
            AxislineColor = OxyColors.White,
            TicklineColor = OxyColors.White,
            TitleColor = OxyColors.White,
            Title = "speed",
            Key = "speed",
        });
    }

    public void Add(VChannelSet data)
    {
        Sets.Add(new(data));
        Changed?.Invoke(this, new());
    }

    public void Refresh() => Changed?.Invoke(this, new());
}
