using LP.Plot.Core.Primitives;
using LP.Plot.Core.Signal;

namespace LP.Plot.Core.Data;

public class Helper
{
    public static List<ISignal> CreateSignals(ChannelDataSet data)
    {
        List<ISignal> signals = new List<ISignal>();
        Axis TempAxis = new();
        var time = data.Channels.First(x => x.Name.Contains("Time"));
        foreach (var c in data.Channels.Where(x => x.Name.Contains("Speed") || x.Name.Contains("TT")))
        {
            var signal = new StaticSignal(c.YValues, new Span(time.YValues.First(), time.YValues.Last()));
            signal.YAxis = new Axis() { Min = c.YValues.Min(), Max = c.YValues.Max() };
            signals.Add(signal);

            if (c.Name.Contains("TT"))
            {
                TempAxis.Min = Math.Min(TempAxis.Min, signal.YAxis.Min);
                TempAxis.Max = Math.Max(TempAxis.Max, signal.YAxis.Max);
                signal.YAxis = TempAxis;
            }
            else
            {
                signal.YAxis.ZoomAtCenter(1.1f);
            }
        }
        TempAxis.ZoomAtCenter(1.1f);
        return signals;
    }
}
