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
        Axis speedAxis = new() { Title = "Speed" };
        StaticSignal speed = new(data.SpeedChannel.YValues, timeRange, speedAxis, SKPaints.NextPaint(), data.SpeedChannel.Name);
        signals.Add(speed);
        speedAxis.Range = speed.YRange;

        // temps
        Axis tempAxis = new() { Title = "Temp", Position = AxisPosition.Right };
        var tempChannels = data.Channels
            .Where(x => x.Name.Contains("TT"))
            .Select(x =>
            {
                var s = new StaticSignal(x.YValues, timeRange, tempAxis, SKPaints.NextPaint(), x.Name);
                tempAxis.Range = tempAxis.Range.GetBounding(s.YRange);
                return s;
            });
        tempAxis.ZoomAtCenter(1.1f);
        signals.AddRange(tempChannels);

        return signals;
    }
}
