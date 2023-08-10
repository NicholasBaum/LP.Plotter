namespace LP.Plotter.Data;

public class VChannelSetVM
{
    public string Name => source.Name;
    public List<VChannel> Channels { get; set; } = new();
    public List<VChannel> SourceChannels => source.Channels;
    private readonly VChannelSet source;

    public VChannelSetVM(VChannelSet source)
    {
        this.source = source;
        Channels.Add(source.SpeedChannel);
        Channels.AddRange(source.Channels.Where(x => x.Name.Contains("TTyre")));
    }
}
