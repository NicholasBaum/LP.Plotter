using LP.Plot.Signal;

namespace LP.Plot.Data;

public class SignalSetVM
{
    public string Name { get; set; } = "";
    public List<SignalVM> Channels { get; } = new();

    public SignalSetVM(IEnumerable<ISignal> signals, string name)
    {
        Name = name;
        var channels = signals.Select(x => new SignalVM(x)).ToList();
        Channels.AddRange(channels);
    }
}
