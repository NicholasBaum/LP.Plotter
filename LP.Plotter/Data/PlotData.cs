namespace LP.Plotter.Data;

public class PlotData
{
    public event EventHandler<EventArgs>? Changed;

    public List<CsvData> Lines { get; } = new();

    public void Add(CsvData csvData)
    {
        Lines.Add(csvData);
        Changed?.Invoke(this, new());
    }
}
