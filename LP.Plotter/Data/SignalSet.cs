using LP.Plot.Core.Signal;

namespace LP.Plot.Core;

public class SignalSet
{
    public SignalSet(IEnumerable<ISignal> signals, string name)
    {
        Name = name;
        var channels = signals.Select(x => new SignalVM(x));
        Channels.AddRange(channels);
        SourceChannels.AddRange(channels);
    }

    public string Name { get; set; } = "";
    public List<SignalVM> Channels { get; set; } = new();
    public List<SignalVM> SourceChannels { get; set; } = new();
}
