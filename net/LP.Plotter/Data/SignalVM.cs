using LP.Plot.Signal;
using SkiaSharp;

namespace LP.Plot.Data;

public class SignalVM
{
    public readonly ISignal Source;
    private bool selected = true;

    public SignalVM(ISignal source)
    {
        this.Source = source;
    }

    public string Name
    {
        get => Source.Name;
        set => Source.Name = value;
    }
    public bool Selected
    {
        get => selected;
        set
        {
            selected = value;
            Source.IsVisible = value;
        }
    }
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
