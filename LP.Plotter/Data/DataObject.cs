namespace LP.Plotter.Data;

public class DataObject
{
    public string Name => FileName;
    public required string FileName { get; init; }
    public required string Path { get; init; }
    public required string Url { get; init; }
    public object? Channels { get; set; }
}
