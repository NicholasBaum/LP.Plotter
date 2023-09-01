using LP.Plot.Core.Signal;
using SkiaSharp;

namespace LP.Plot.Core;

public interface IData
{
    public List<SignalSet> Sets { get; }
    public void Remove(SignalSet set);
}

public class SignalVM
{
    public readonly ISignal Source;

    public SignalVM(ISignal source)
    {
        this.Source = source;
    }

    public string Name
    {
        get => Source.Name;
        set => Source.Name = value;
    }
    public bool Selected { get; set; }
    public Axis YAxis => Source.YAxis;
    public bool IsVisible
    {
        get => Source.IsVisible;
        set => Source.IsVisible = value;
    }
    public SKColor Color
    {
        get => Source.Paint.Color;
        set
        {
            Source.Paint = new SKPaint()
            {
                Color = value,
                StrokeWidth = Source.Paint.StrokeWidth,
                IsAntialias = Source.Paint.IsAntialias,
                Style = Source.Paint.Style
            };
        }
    }
}

public class SignalSet
{
    public string Name { get; set; } = "";
    public List<SignalVM> Channels { get; set; } = new();
    public List<SignalVM> SourceChannels { get; set; } = new();
}

public partial class Plot : IData
{
    public List<SignalSet> Sets { get; set; } = new();

    public void Remove(SignalSet set)
    {
        Sets.Remove(set);
        foreach (var s in set.Channels)
            signalPlot.Remove(s.Source);
        (this.signalPlot as BufferedSignalPlot)?.RerenderOnNextFrame();
        this.Invalidate();
    }
}
