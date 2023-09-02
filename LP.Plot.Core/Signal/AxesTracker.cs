using LP.Plot.Core.Primitives;
using SkiaSharp;

namespace LP.Plot.Core.Signal;

internal class SignalsTracker : IAxes
{
    public Axis XAxis { get; }
    public IEnumerable<Axis> YAxes => trackedAxes.Select(x => x.Source).Where(x => x != XAxis);

    private readonly List<AxisTracker> trackedAxes;
    private bool wasZoomed = false;
    private bool wasReset = false;
    private readonly List<SignalTracker> trackedSignals;

    public SignalsTracker(IEnumerable<ISignal> signals, Axis xAxis)
    {
        this.trackedSignals = signals.Select(x => new SignalTracker(x)).ToList();
        this.XAxis = xAxis;
        trackedAxes = signals
            .Select(x => x.YAxis)
            .Append(xAxis)
            .Distinct()
            .Select(x => new AxisTracker(x))
            .ToList();
    }

    public void Remove(ISignal signal) => trackedSignals.RemoveAll(x => x.signal == signal);
    public bool HasChanged
    {
        get
        {
            return wasReset
                || wasZoomed
                || trackedSignals.Any(x => x.HasChanged)
                || trackedAxes.Any(x => x.HasChanged);
        }
    }

    public void Cache()
    {
        trackedSignals.ForEach(x => x.Cache());
        wasZoomed = wasReset = false;
    }

    public void PanRelativeX(double offset)
    {
        XAxis.PanRelative(offset);
    }

    public void PanRelative(double offset)
    {
        foreach (var axis in YAxes)
        {
            axis.PanRelative(offset);
        }
    }

    public void ZoomAtRelativeX(double factor, double position)
    {
        XAxis.ZoomAtRelative(factor, position);
        wasZoomed = true;
    }

    public void ZoomAtRelative(double factor, double position)
    {
        foreach (var axis in YAxes)
        {
            axis.ZoomAtRelative(factor, position);
        }
        wasZoomed = true;
    }

    public void ZoomX(Span newRange)
    {
        XAxis.Range = newRange;
        wasZoomed = true;
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

        public bool HasChanged
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
        public bool HasChanged { get; private set; }
        public AxisTracker(Axis axis)
        {
            Source = axis;
            axis.AxisChanged += (_, e) => { HasChanged = !e.WasPanned; };
        }

        public void Cache()
        {
            HasChanged = false;
        }
    }
}