using SkiaSharp;
using System.Diagnostics;

namespace LP.Plot.Signal;

internal class SignalsTracker
{
    public Axis XAxis { get; }
    public IEnumerable<Axis> YAxes => uniqueTrackedYAxes.Select(x => x.Source).Where(x => x != XAxis);

    private readonly AxisTracker trackedXAxis;
    private readonly List<AxisTracker> uniqueTrackedYAxes;
    private readonly List<SignalTracker> trackedSignals;
    private bool signalsAddedOrRemoved;

    public SignalsTracker(IEnumerable<ISignal> signals, Axis xAxis)
    {
        this.trackedSignals = signals.Select(x => new SignalTracker(x)).ToList();
        this.XAxis = xAxis;
        this.trackedXAxis = new(xAxis);
        uniqueTrackedYAxes = signals
            .Select(x => x.YAxis)
            .Distinct()
            .Select(x => new AxisTracker(x))
            .ToList();
    }

    public void Add(ISignal signal)
    {
        this.trackedSignals.Add(new SignalTracker(signal));
        AddUniqueAxis(signal);
        signalsAddedOrRemoved = true;
    }

    private void AddUniqueAxis(ISignal signal)
    {
        var axis = signal.YAxis;
        if (!uniqueTrackedYAxes.Any(x => x.Source == axis))
            uniqueTrackedYAxes.Add(new AxisTracker(axis));
    }

    public void Remove(ISignal signal)
    {
        trackedSignals.RemoveAll(x => x.Signal == signal);
        if (!trackedSignals.Any(x => x.Signal.YAxis == signal.YAxis))
            uniqueTrackedYAxes.RemoveAll(x => x.Source == signal.YAxis);
        signalsAddedOrRemoved = true;
    }

    public bool NeedsRerender()
    {
        var signalsChanged = trackedSignals.Any(x => x.HasChanged()) || signalsAddedOrRemoved;
        var xAxisWasZoomed = trackedXAxis.HasZoomed();
        var yAxisWasZoomed = uniqueTrackedYAxes.Any(x => x.HasZoomed());
        var singleSignalWasPanned = HasUnproportionalPan();
        var result = signalsChanged || xAxisWasZoomed || yAxisWasZoomed || singleSignalWasPanned;
        if (result)
        {
            Debug.WriteLine($"Signal/XZoom/YZoom/Pan {signalsChanged}/{xAxisWasZoomed}/{yAxisWasZoomed}/{singleSignalWasPanned}");
            Console.WriteLine($"Signal/XZoom/YZoom/Pan {signalsChanged}/{xAxisWasZoomed}/{yAxisWasZoomed}/{singleSignalWasPanned}");
        }
        return result;
    }

    private bool HasUnproportionalPan()
    {
        if (uniqueTrackedYAxes.Count == 0) return false;
        var tmp = uniqueTrackedYAxes.First().RelativePanAmountIfNotZoomed();
        return uniqueTrackedYAxes.Skip(1).Any(x => !FloatEquals(x.RelativePanAmountIfNotZoomed(), tmp, 10e-10));
    }

    public void Cache()
    {
        trackedSignals.ForEach(x => x.Cache());
        trackedXAxis.Cache();
        uniqueTrackedYAxes.ForEach(x => x.Cache());
        signalsAddedOrRemoved = false;
    }


    /// <summary>
    /// Signal Wrapper
    /// </summary>
    private class SignalTracker
    {
        public readonly ISignal Signal;
        private SKPaint lastPaint;
        private bool lastVisibility;

        public SignalTracker(ISignal signal)
        {
            this.Signal = signal;
            this.lastPaint = signal.Paint;
            this.lastVisibility = signal.IsVisible;
        }

        public bool HasChanged()
            => lastPaint != Signal.Paint || lastVisibility != Signal.IsVisible;

        public void Cache()
        {
            this.lastPaint = Signal.Paint;
            this.lastVisibility = Signal.IsVisible;
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