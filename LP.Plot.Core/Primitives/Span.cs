namespace LP.Plot.Core.Primitives;

public readonly struct Span
{
    public double Min { get; }
    public double Max { get; }

    public double Length => Max - Min;

    public Span(double min, double max)
    {
        Min = min;
        Max = max;
    }
}
