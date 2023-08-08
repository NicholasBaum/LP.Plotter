namespace LP.Plotter.Data;

public class PlotData
{
    public event EventHandler<EventArgs>? Changed;

    public List<VChannelSet> Sets { get; } = new();

    public void Add(VChannelSet data)
    {
        Sets.Add(data);
        Changed?.Invoke(this, new());
    }
}
