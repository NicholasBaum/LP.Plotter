using LP.Plot.Core.Primitives;
using LP.Plot.Core.Signal;
using LP.Plot.Skia;

namespace LP.Plot.Core.Data;

public class Helper
{
    public static List<StaticSignal> CreateSignals(ChannelDataSet data)
    {
        var signals = new List<StaticSignal>();
        var time = data.Channels.First(x => x.Name.Contains("Time"));
        var timeRange = new Span(time.YValues.First(), time.YValues.Last());

        // speed channel
        signals.Add(StaticSignal.Create(data.SpeedChannel.YValues, timeRange, data.Name));

        // temps
        Axis TempAxis = new();
        var tempChannels = data.Channels
            .Where(x => x.Name.Contains("TT"))
            .Select(x =>
            {
                var s = new StaticSignal(x.YValues, timeRange, TempAxis, SKPaints.NextPaint(), data.Name);
                TempAxis.Range = TempAxis.Range.GetBounding(s.YRange);
                return s;
            });
        TempAxis.ZoomAtCenter(1.1f);
        signals.AddRange(tempChannels);

        return signals;
    }
}
