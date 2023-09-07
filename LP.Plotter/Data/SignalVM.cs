using LP.Plot.Core.Signal;
using SkiaSharp;

namespace LP.Plot.Core;

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
