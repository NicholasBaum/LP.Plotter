namespace LP.Plot.Models;

public class VChannelSet
{
    public required string Name { get; init; }
    public required string Path { get; init; }
    public List<VChannelVM> Channels = new();
    public VChannelVM? SpeedChannel => Channels.FirstOrDefault(x => x.Name.Contains("Speed"));
}
