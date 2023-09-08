using LP.Plot.Primitives;
using LP.Plot.Signal;
using LP.Plot.Skia;

namespace LP.Plot.Data;

public class Helper
{
    public static List<StaticSignal> CreateEssentialSignals(ChannelDataSet data)
    {
        return CreateEssentialSignals(data, new());
    }

    public static List<StaticSignal> CreateEssentialSignals(ChannelDataSet data, List<Axis> keyTagedAxes)
    {
        return CreateSignals(data, keyTagedAxes, true);
    }

    public static List<StaticSignal> CreateSignals(ChannelDataSet data, List<Axis> keyTagedAxes, bool essentialsOnly = false)
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
        CreateChannels("Temp", "TT", data, keyTagedAxes, signals, timeRange, "tyre_temp");

        if (essentialsOnly)
            return signals;

        // brakes 
        CreateChannels("Pressure", "pBrake", data, keyTagedAxes, signals, timeRange, "p_brake");
        CreateChannels("Pressure", "pTyre", data, keyTagedAxes, signals, timeRange, "p_tyre");
        CreateChannels("Force", "Gx", data, keyTagedAxes, signals, timeRange, "f_gx");
        CreateChannels("Force", "Gy", data, keyTagedAxes, signals, timeRange, "f_gy");
        CreateChannels("Angle", "steering", data, keyTagedAxes, signals, timeRange, "a_steer");
        CreateChannels("Pressure", "Pedal", data, keyTagedAxes, signals, timeRange, "p_pedal");
        //var pbrakes = data.Channels.Where(x => x.Name.Contains("pBrake")).ToList();
        //var pTyre = data.Channels.Where(x => x.Name.Contains("pTyre")).ToList();
        //var gX = data.Channels.Where(x => x.Name.Contains("Gx")).ToList();
        //var gY = data.Channels.Where(x => x.Name.Contains("Gy")).ToList();
        //var steering = data.Channels.Where(x => x.Name.Contains("steering")).ToList();
        //var pedal = data.Channels.Where(x => x.Name.Contains("Pedal")).ToList();


        return signals;
    }

    private static void CreateChannels(string title, string nameFragment, ChannelDataSet data, List<Axis> keyTagedAxes, List<StaticSignal> signals, Span timeRange, string key)
    {
        Axis axis = new() { Title = title, Position = AxisPosition.Right, Key = key };
        Axis? usedAxis = keyTagedAxes.FirstOrDefault(x => x.Key == key);
        var channels = data.Channels
            .Where(x => x.Name.Contains(nameFragment))
            .Select(x =>
            {
                var s = new StaticSignal(x.YValues, timeRange, usedAxis ?? axis, SKPaints.NextPaint(), x.Name);
                axis.Range = axis.Range.GetBounding(s.YRange);
                return s;
            }).ToList();
        axis.ZoomAtCenter(1.1f);
        signals.AddRange(channels);
    }
}
