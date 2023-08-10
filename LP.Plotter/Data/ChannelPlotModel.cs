namespace LP.Plotter.Data;

public class ChannelPlotModel
{
    public event EventHandler<EventArgs>? Changed;

    public List<VChannelSetVM> Sets { get; } = new();

    public void Add(VChannelSet data)
    {
        Sets.Add(new(data));
        Changed?.Invoke(this, new());
    }

    public void Refresh() => Changed?.Invoke(this, new());
}
