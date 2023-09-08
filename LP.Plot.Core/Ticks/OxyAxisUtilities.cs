namespace LP.Plot.Ticks;

using System;
using System.Collections.Generic;

/// <summary>
/// Static utility methods for the <see cref="Axis" /> classes.
/// </summary>
public static class AxisUtilities
{
    public static (double MajorStep, double MinorStep) GetIntervallSizes(double length, double dataRange, double tickCount = 60, double MinimumMinorStep = 0, double MinimumMajorStep = 0)
    {
        var majorStep = CalculateInterval(length, tickCount, dataRange);

        var minorStep = CalculateMinorInterval(majorStep);

        if (double.IsNaN(minorStep))
        {
            minorStep = 2;
        }

        if (double.IsNaN(majorStep))
        {
            majorStep = 10;
        }

        return (Math.Max(majorStep, MinimumMajorStep), Math.Max(minorStep, MinimumMinorStep));
    }

    public static double CalculateInterval(double availableSize, double maxIntervalSize, double range, double minIntervalCount = 2, double maxIntervalCount = double.MaxValue)
    {
        if (availableSize <= 0)
        {
            return maxIntervalSize;
        }

        if (Math.Abs(maxIntervalSize) <= 0)
        {
            throw new ArgumentException("Maximum interval size cannot be zero.", nameof(maxIntervalSize));
        }

        if (Math.Abs(range) <= 0)
        {
            throw new ArgumentException("Range cannot be zero.", nameof(range));
        }

        Func<double, double> exponent = x => Math.Ceiling(Math.Log(x, 10));
        Func<double, double> mantissa = x => x / Math.Pow(10, exponent(x) - 1);

        // bound min/max interval counts
        minIntervalCount = Math.Max(minIntervalCount, 0);
        maxIntervalCount = Math.Min(maxIntervalCount, availableSize / maxIntervalSize);

        range = Math.Abs(range);
        double interval = Math.Pow(10, exponent(range));
        double intervalCandidate = interval;

        // Function to remove 'double precision noise'
        // TODO: can this be improved
        Func<double, double> removeNoise = x => double.Parse(x.ToString("e14"));

        // decrease interval until interval count becomes less than maxIntervalCount
        while (true)
        {
            var m = (int)mantissa(intervalCandidate);
            if (m == 5)
            {
                // reduce 5 to 2
                intervalCandidate = removeNoise(intervalCandidate / 2.5);
            }
            else if (m == 2 || m == 1 || m == 10)
            {
                // reduce 2 to 1, 10 to 5, 1 to 0.5
                intervalCandidate = removeNoise(intervalCandidate / 2.0);
            }
            else
            {
                intervalCandidate = removeNoise(intervalCandidate / 2.0);
            }

            if (range / interval >= minIntervalCount && range / intervalCandidate > maxIntervalCount)
            {
                break;
            }

            if (double.IsNaN(intervalCandidate) || double.IsInfinity(intervalCandidate))
            {
                break;
            }

            interval = intervalCandidate;
        }

        return interval;
    }

    /// <summary>
    /// Calculates the minor interval.
    /// </summary>
    /// <param name="majorInterval">The major interval.</param>
    /// <returns>The minor interval.</returns>
    public static double CalculateMinorInterval(double majorInterval)
    {
        // check if majorInterval = 2*10^x
        // uses the mathematical identity log10(2 * 10^x) = x + log10(2)
        // -> we just have to check if the modulo of log10(2*10^x) = log10(2)
        if (Math.Abs(((Math.Log10(majorInterval) + 1000) % 1) - Math.Log10(2)) < 1e-10)
        {
            return majorInterval / 4;
        }

        return majorInterval / 5;
    }

    /// <summary>
    /// Creates tick values at the specified interval.
    /// </summary>
    /// <param name="from">The start value.</param>
    /// <param name="to">The end value.</param>
    /// <param name="step">The interval.</param>
    /// <param name="maxTicks">The maximum number of ticks (optional). The default value is 1000.</param>
    /// <returns>A sequence of values.</returns>
    /// <exception cref="System.ArgumentException">Step cannot be zero or negative.;step</exception>
    public static IList<double> CreateTickValues(double from, double to, double step, int maxTicks = 1000)
    {
        if (step <= 0)
        {
            throw new ArgumentOutOfRangeException("Step cannot be zero or negative.", nameof(step));
        }

        if (to < from)
        {
            step = -step;
        }

        var epsilon = step * 1e-3;
        from -= epsilon;
        to += epsilon;

        var startValue = Math.Ceiling(from / step) * step;
        if (startValue == -0d)
        {
            startValue = 0;
        }

        var values = new List<double>();
        var sign = Math.Sign(step);
        var i = 0;

        var currentValue = startValue;
        while ((to - currentValue) * sign >= 0 && i < maxTicks)
        {
            values.Add(currentValue);
            currentValue = startValue + (++i * step);
        }

        return values;
    }

    /// <summary>
    /// Analyses two lists of major and minor ticks and creates a new containing the subset of the minor ticks which are not too close to any of the major ticks.
    /// </summary>
    /// <param name="majorTicks">The major ticks. Must be monotonically ascending or descending.</param>
    /// <param name="minorTicks">The minor ticks. Must be monotonically ascending or descending (same direction as major ticks).</param>
    /// <returns>A new list containing a subset of the original minor ticks such that there are no minor ticks too close to a major tick.</returns>
    public static IList<double> FilterRedundantMinorTicks(IList<double> majorTicks, IList<double> minorTicks)
    {
        if (majorTicks.Count == 0 || minorTicks.Count == 0)
        {
            return minorTicks;
        }

        var ret = new List<double>();
        var previousMinorTick = 0d;
        var j = 1;

        var currentMajorTick = majorTicks.Count > 1 ? majorTicks[j] : majorTicks[0];

        static double GetEpsilon(double tick1, double tick2)
        {
            return Math.Abs(tick1 - tick2) * 1e-3;
        }

        // If there is only one minor tick, we can't determine a meaningful epsilon. 
        // But there also shouldn't be any precision loss, so we can require an exact match (epsilon = 0)
        var epsilon = minorTicks.Count > 1 ? GetEpsilon(minorTicks[0], minorTicks[1]) : 0;

        var sign = 1;
        if (majorTicks.Count > 1 && majorTicks[0] > majorTicks[1])
        {
            sign = -1;
        }

        for (var i = 0; i < minorTicks.Count; i++)
        {
            var currentMinorTick = minorTicks[i];
            if (i > 0)
            {
                epsilon = GetEpsilon(currentMinorTick, previousMinorTick);
            }

            while ((currentMajorTick - currentMinorTick) * sign < 0 && j < majorTicks.Count - 1)
            {
                currentMajorTick = majorTicks[++j];
            }

            var previousMajorTick = majorTicks[j - 1];
            if (Math.Abs(currentMinorTick - currentMajorTick) > epsilon && Math.Abs(currentMinorTick - previousMajorTick) > epsilon)
            {
                ret.Add(currentMinorTick);
            }

            previousMinorTick = currentMinorTick;
        }

        return ret;
    }
}
