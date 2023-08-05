namespace LP.Plotter.Data;

public class PlotData
{
    public event EventHandler<EventArgs>? Changed;

    public List<ChannelData> Channels { get; } = new();

    public void Add(CsvData csvData)
    {
        Channels.Add(csvData.Channels.First(x => x.Name.Contains("CarSpeed")));
        Changed?.Invoke(this, new());
    }
}
