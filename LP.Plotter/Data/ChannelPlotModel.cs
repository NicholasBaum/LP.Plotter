using OxyPlot;
using OxyPlot.Axes;

namespace LP.Plotter.Data;

public class ChannelPlotModel
{
    public event EventHandler<EventArgs>? Changed;
    public IReadOnlyList<VChannelSetVM> Sets => _setsView;
    private readonly IReadOnlyList<VChannelSetVM> _setsView;
    public IReadOnlyList<Axis> Axes => _axesView;
    private readonly IReadOnlyList<Axis> _axesView;

    private List<VChannelSetVM> sets { get; } = new();
    private List<Axis> axes { get; } = new();
    private Axis? XAxis => axes.FirstOrDefault(x => x.Position == AxisPosition.Bottom);
    private IEnumerable<VChannelVM> channels => sets.SelectMany(x => x.Channels);

    public ChannelPlotModel()
    {
        _setsView = sets.AsReadOnlyList();
        _axesView = axes.AsReadOnlyList();
        CreateDefaultAxes();
    }

    private void CreateDefaultAxes()
    {
        axes.Add(CreateDefaultLinearAxis("time", "time", AxisPosition.Bottom));
        axes.Add(CreateDefaultLinearAxis("temp", "temp", AxisPosition.Left));
        axes.Add(CreateDefaultLinearAxis("speed", "speed", AxisPosition.Right));
    }

    public void ZoomOutMap()
    {
        this.axes.ForEach(x =>
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
            var min = channels.Min(x => x.Points.First().X);
            var max = channels.Max(x => x.Points.Last().X);
            ax.Zoom(ax.Scale * 0.66);
            if (ax.ActualMinimum < min || ax.ActualMaximum > max)
                ax.Zoom(Math.Max(ax.ActualMinimum, min), Math.Min(ax.ActualMaximum, max));
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
        foreach (var ax in this.axes)
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
            var n when n.Contains("pTyre") => GetOrCreateAxis("pressure"),
            var n when n.Contains("pBrake") => GetOrCreateAxis("brake"),
            var n when n.Contains("rPedal") => GetOrCreateAxis("pedal"),
            var n when n.Contains("Steering") => GetOrCreateAxis("steer"),
            var n when n.Contains("Gx_") => GetOrCreateAxis("Gx"),
            var n when n.Contains("Gy_") => GetOrCreateAxis("Gy"),
            _ => GetOrCreateAxis(Guid.NewGuid().ToString()),
        };
    }
    // TODO: this should probably return a separate Axis for every channel but sync them somehow if wanted e.g. by copying values
    //       could also create a axis per channel name...
    private Axis GetOrCreateAxis(string key)
    {
        if (this.axes.FirstOrDefault(x => x.Key == key) is Axis axis)
        {
            return axis;
        }
        else
        {
            var newAxis = CreateDefaultLinearAxis(key, key, AxisPosition.Left, false);
            this.axes.Add(newAxis);
            return newAxis;
        }
    }

    private LinearAxis CreateDefaultLinearAxis(string key, string title, AxisPosition position = AxisPosition.Left, bool visible = true)
    {
        return new LinearAxis()
        {
            IsAxisVisible = visible,
            Position = position,
            FontSize = 16,
            TextColor = OxyColors.White,
            AxislineColor = OxyColors.White,
            TicklineColor = OxyColors.White,
            TitleColor = OxyColors.White,
            Title = title,
            Key = key,
        };
    }

    public void Add(VChannelSet data)
    {
        sets.Add(new(data));
        Changed?.Invoke(this, new());
    }

    public void AddRange(IEnumerable<VChannelSet> data)
    {
        sets.AddRange(data.Select(x => new VChannelSetVM(x)));
        Changed?.Invoke(this, new());
    }

    public void Remove(VChannelSetVM data)
    {
        sets.Remove(data);
        Changed?.Invoke(this, new());
    }

    public void Refresh() => Changed?.Invoke(this, new());
}
