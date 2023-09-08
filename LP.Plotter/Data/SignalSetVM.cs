using LP.Plot.Core.Signal;

namespace LP.Plot.Data;

public class SignalSetVM
{
    public string Name { get; set; } = "";
    public List<SignalVM> Channels { get; } = new();

    public SignalSetVM(IEnumerable<ISignal> signals, string name)
    {
        Name = name;
        var channels = signals.Select(x => new SignalVM(x, false)).ToList();
        foreach (var channel in channels.Where(x => x.Name.Contains("Speed") || x.Name.Contains("TT")))
            channel.Selected = true;
        Channels.AddRange(channels);
    }
}
