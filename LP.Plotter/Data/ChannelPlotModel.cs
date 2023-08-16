using OxyPlot;
using OxyPlot.Axes;

namespace LP.Plotter.Data;

public class ChannelPlotModel
{
    public event EventHandler<EventArgs>? Changed;
    public IReadOnlyList<VChannelSetVM> Sets => _setsView;


    private List<VChannelSetVM> sets { get; } = new();
    private IReadOnlyList<VChannelSetVM> _setsView;
    private List<Axis> Axes { get; } = new();
    private Axis? XAxis => Axes.FirstOrDefault(x => x.Position == AxisPosition.Bottom);


    public ChannelPlotModel()
    {
        _setsView = sets.AsReadOnlyList();
        CreateDefaultAxes();
    }

    public void ZoomOutMap()
    {
        this.Axes.ForEach(x =>
        {
            x.Minimum = double.NaN;
            x.Maximum = double.NaN;
            x.Reset();
        });
        Refresh();
    }

    public void ZoomOut()
    {
        if (XAxis is Axis ax)
        {
            ax.Zoom(ax.Scale / 2.0);
            Refresh();
        }
    }

    public void Draw(PlotModel oxyModel)
    {
        oxyModel.PlotAreaBorderColor = OxyColors.White;
        oxyModel.Background = OxyColors.Black;
        foreach (var c in this.sets.SelectMany(x => x.Channels))
        {
            if (c.YAxisKey == null)
                c.YAxisKey = GetOrCreateDefaultAxis(c).Key;
        }
        foreach (var ax in this.Axes)
        {
            if (oxyModel.Axes.Contains(ax))
            {
                // in case axis is already in use so it doesn't get reset
                if (double.IsNaN(ax.Minimum))
                    ax.Minimum = ax.ActualMinimum;
                if (double.IsNaN(ax.Maximum))
                    ax.Maximum = ax.ActualMaximum;
            }
            else
            {
                oxyModel.Axes.Add(ax);
            }
        }

        oxyModel.Series.Clear();
        foreach (var c in this.sets.SelectMany(x => x.Channels))
            oxyModel.Series.Add(c);
    }

    private Axis GetOrCreateDefaultAxis(VChannelVM channel)
    {
        return channel.Name switch
        {
            var n when n.Contains("TTyre") => GetOrCreateAxis("temp"),
            var n when n.Contains("Speed") => GetOrCreateAxis("speed"),
            var n when n.Contains("pTyre") => GetOrCreateAxis("tyre"),
            var n when n.Contains("pBrake") => GetOrCreateAxis("brake"),
            var n when n.Contains("rPedal") => GetOrCreateAxis("pedal"),
            var n when n.Contains("Gx_") => GetOrCreateAxis("Gx"),
            var n when n.Contains("Gy_") => GetOrCreateAxis("Gy"),
            _ => GetOrCreateAxis(Guid.NewGuid().ToString()),
        };
    }
    // TODO: this should probably return a separate Axis for every channel but sync them somehow if wanted e.g. by copying values
    //       could also create a axis per channel name...
    private Axis GetOrCreateAxis(string key)
    {
        if (this.Axes.FirstOrDefault(x => x.Key == key) is var axis && axis is not null)
        {
            return axis;
        }
        else
        {
            var newAxis = new LinearAxis() { Key = key, IsAxisVisible = false };
            this.Axes.Add(newAxis);
            return newAxis;
        }
    }

    private void CreateDefaultAxes()
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
        sets.Add(new(data));
        Changed?.Invoke(this, new());
    }

    public void Refresh() => Changed?.Invoke(this, new());
}
