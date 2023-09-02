using LP.Plot.Core.Primitives;
using SkiaSharp;

namespace LP.Plot.Core.Signal;

internal class SignalsTracker : IAxes
{
    public Axis XAxis => XAxisTracked.Source;
    public IEnumerable<Axis> YAxes => YAxesTracked.Select(x => x.Source);

    private AxisTracker XAxisTracked;
    private List<AxisTracker> YAxesTracked = new List<AxisTracker>();
    private bool wasZoomed = false;
    private bool wasReset = false;
    private List<SignalTracker> signals;

    public SignalsTracker(IEnumerable<ISignal> signals, Axis xAxis)
    {
        this.signals = signals.Select(x => new SignalTracker(x)).ToList();
        XAxisTracked = new AxisTracker(xAxis);
        YAxesTracked = signals
            .Select(x => x.YAxis)
            .Distinct()
            .Where(x => x != XAxis)
            .Select(x => new AxisTracker(x))
            .ToList();
    }

    public void Remove(ISignal signal) => signals.RemoveAll(x => x.signal == signal);
    public bool HasChanged
    {
        get
        {
            return wasReset || wasZoomed || signals.Any(x => x.HasChanged);
        }
    }

    public void Cache()
    {
        signals.ForEach(x => x.Cache());
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
        public AxisTracker(Axis axis)
        {
            Source = axis;
        }
    }
}