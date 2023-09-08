using LP.Plot.Core.Primitives;
using LP.Plot.Core.Signal;
using LP.Plot.Skia;

namespace LP.Plot.Core.Data;

public class Helper
{
    public static List<StaticSignal> CreateSignals(ChannelDataSet data)
    {
        return CreateSignals(data, new());
    }

    public static List<StaticSignal> CreateSignals(ChannelDataSet data, List<Axis> keyTagedAxes)
    {
        var signals = new List<StaticSignal>();
        var time = data.Channels.First(x => x.Name.Contains("Time"));
        var timeRange = new Span(time.YValues.First(), time.YValues.Last());

        // speed channel
        var speed_key = "speed";
        Axis speedAxis = new() { Title = "Speed", Position = AxisPosition.Left, Key = speed_key };
        StaticSignal speed = new(data.SpeedChannel.YValues, timeRange, speedAxis, SKPaints.NextPaint(), data.SpeedChannel.Name);
        speedAxis.Range = speed.YRange;
        if (keyTagedAxes.FirstOrDefault(x => x.Key == speed_key) is Axis sa)
            speed.YAxis = sa;
        signals.Add(speed);


        // temps
        var tyre_temp_key = "tyre_temp";
        Axis tempAxis = new() { Title = "Temp", Position = AxisPosition.Right, Key = tyre_temp_key };
        Axis? usedAxis = keyTagedAxes.FirstOrDefault(x => x.Key == tyre_temp_key);
        var tempChannels = data.Channels
            .Where(x => x.Name.Contains("TT"))
            .Select(x =>
            {
                var s = new StaticSignal(x.YValues, timeRange, usedAxis ?? tempAxis, SKPaints.NextPaint(), x.Name);
                tempAxis.Range = tempAxis.Range.GetBounding(s.YRange);
                return s;
            }).ToList();
        tempAxis.ZoomAtCenter(1.1f);
        signals.AddRange(tempChannels);

        return signals;
    }
}
