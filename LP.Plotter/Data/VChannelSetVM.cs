namespace LP.Plotter.Data;

public class VChannelSetVM
{
    public string Name => source.Name;
    public List<VChannelVM> Channels => source.Channels.Where(x => x.Selected).ToList();
    public List<VChannelVM> SourceChannels => source.Channels;
    private readonly VChannelSet source;

    public VChannelSetVM(VChannelSet source)
    {
        this.source = source;

        source.SpeedChannel.Selected = true;
        source.SpeedChannel.YAxisKey = "speed";
        foreach (var x in source.Channels.Where(x => x.Name.Contains("TTyre")))
        {
            x.Selected = true;
            x.YAxisKey = "temp";
        }
    }
}
