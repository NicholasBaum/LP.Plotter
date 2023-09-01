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
    private ISignal source;

    public SignalVM(ISignal source)
    {
        this.source = source;
    }

    public string Name
    {
        get => source.Name;
        set => source.Name = value;
    }
    public bool Selected { get; set; }
    public Axis YAxis => source.YAxis;
    public bool IsVisible
    {
        get => source.IsVisible;
        set => source.IsVisible = value;
    }
    public SKColor Color
    {
        get
        {
            return source.Paint.Color;
        }
        set
        {
            if (source.Paint.Color != value)
            {
                Console.WriteLine(value);

                source.Paint.Color = value;
            }
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

    public void Remove(SignalSet set) => Sets.Remove(set);
}
