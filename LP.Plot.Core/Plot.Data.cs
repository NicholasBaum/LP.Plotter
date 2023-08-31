using SkiaSharp;

namespace LP.Plot.Core;

public interface IData
{
    public List<SignalSet> Sets { get; }
    public void Remove(SignalSet set);
}

public class SignalVM
{
    public string Name { get; set; } = "";
    public bool Selected { get; set; }
    public Axis YAxis { get; set; }
    public bool IsVisible { get; set; }
    public SKColor Color { get; set; }
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
