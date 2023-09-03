using SkiaSharp;

namespace LP.Plot.Core.Signal;

internal class SignalsTracker
{
    public Axis XAxis { get; }
    public IEnumerable<Axis> YAxes => trackedYAxes.Select(x => x.Source).Where(x => x != XAxis);

    private readonly AxisTracker trackedXAxis;
    private readonly List<AxisTracker> trackedYAxes;
    private readonly List<SignalTracker> trackedSignals;

    public SignalsTracker(IEnumerable<ISignal> signals, Axis xAxis)
    {
        this.trackedSignals = signals.Select(x => new SignalTracker(x)).ToList();
        this.XAxis = xAxis;
        this.trackedXAxis = new(xAxis);
        trackedYAxes = signals
            .Select(x => x.YAxis)
            .Distinct()
            .Select(x => new AxisTracker(x))
            .ToList();
    }

    public void Remove(ISignal signal)
        => trackedSignals.RemoveAll(x => x.signal == signal);

    public bool NeedsRerender()
        => trackedSignals.Any(x => x.HasChanged()) || trackedXAxis.HasZoomed()
        || trackedYAxes.Any(x => x.HasZoomed()) || HasUnproportionalPan();

    private bool HasUnproportionalPan()
    {
        if (trackedYAxes.Count == 0) return false;
        var tmp = trackedYAxes.First().RelativePanAmountIfNotZoomed();
        return trackedYAxes.Skip(1).Any(x => !FloatEquals(x.RelativePanAmountIfNotZoomed(), tmp, 10e-10));
    }

    public void Cache()
    {
        trackedSignals.ForEach(x => x.Cache());
        trackedXAxis.Cache();
        trackedYAxes.ForEach(x => x.Cache());
    }


    /// <summary>
    /// Signal Wrapper
    /// </summary>
    private class SignalTracker
    {
        public readonly ISignal signal;
        private SKPaint lastPaint;
        private bool lastVisibility;

        public SignalTracker(ISignal signal)
        {
            this.signal = signal;
            this.lastPaint = signal.Paint;
            this.lastVisibility = signal.IsVisible;
        }

        public bool HasChanged()
            => lastPaint != signal.Paint || lastVisibility != signal.IsVisible;

        public void Cache()
        {
            this.lastPaint = signal.Paint;
            this.lastVisibility = signal.IsVisible;
        }
    }

    /// <summary>
    /// Axis Wrapper
    /// </summary>
    private class AxisTracker
    {
        public Axis Source { get; }
        private double lastLength = 0;
        private double lastMin = 0;

        public AxisTracker(Axis axis)
        {
            Source = axis;
        }

        public bool HasZoomed()
            => !FloatEquals(Source.Length, lastLength, 10e-6);

        // if the axis was actually zoomed this doesn't reuturn the correct pan amount
        public double RelativePanAmountIfNotZoomed()
            => (lastMin - Source.Min) / lastLength;

        public void Cache()
        {
            lastLength = Source.Length;
            lastMin = Source.Min;
        }
    }

    private static bool FloatEquals(double x, double y, double tolerance)
    {
        var diff = Math.Abs(x - y);
        return diff <= tolerance ||
               diff <= Math.Max(Math.Abs(x), Math.Abs(y)) * tolerance;
    }
}