namespace LP.Plotter.Data;

public class CsvInfo
{
    public string Name => FileName;
    public required string FileName { get; init; }
    public required string Path { get; init; }
    public required string Url { get; init; }
    public List<VChannelVM>? Channels { get; set; }   
}
